using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Entities;
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

/// <summary>
/// Rankings, MVP y simulación de batallas. Con el sistema de experiencia, todo usa el
/// poder efectivo (poder base + bonus por nivel) y la simulación reparte XP.
/// </summary>
public class EstadisticasService(CatalogoDbContext db, IProgresionService progresion) : IEstadisticasService
{
    private const double FactorMin = 0.90;
    private const double FactorMax = 1.10;
    private const int MultiplicadorPeligrosidad = 10;
    private const int BonusFuerzaSensitivo = 15;

    // XP del sistema de experiencia
    private const int XpParticipar = 10;
    private const int XpGanar = 20;
    private const int XpPerder = 5;
    private const int XpEmpate = 10;
    private const int XpMvp = 15;
    private const double MultiplicadorMasDebil = 1.5;

    public async Task<Resultado<List<RankingItem>>> RankingAsync(string por)
    {
        var campo = por.Trim().ToLowerInvariant();
        if (campo is not ("poder" or "peligrosidad" or "experiencia"))
            return Resultado<List<RankingItem>>.Falla("Valor de 'por' inválido. Use 'poder', 'peligrosidad' o 'experiencia'.");

        var datos = await db.Cartas.AsNoTracking()
            .Select(c => new { c.Personaje.Id, c.Personaje.Nombre, c.Personaje.Faccion, c.Poder, c.NivelPeligrosidad })
            .ToListAsync();
        var xpTotal = await progresion.XpTotalAsync();

        var ranking = datos
            .Select(x =>
            {
                var xp = xpTotal.GetValueOrDefault(x.Id);
                var valor = campo switch
                {
                    "poder" => ProgresionService.PoderEfectivo(x.Poder, xp),
                    "peligrosidad" => ProgresionService.PeligrosidadEfectiva(x.NivelPeligrosidad, xp),
                    _ => xp
                };
                return new { x.Id, x.Nombre, x.Faccion, Valor = valor };
            })
            .OrderByDescending(x => x.Valor).ThenBy(x => x.Nombre)
            .Select((x, i) => new RankingItem(i + 1, x.Id, x.Nombre, x.Faccion, x.Valor))
            .ToList();

        return Resultado<List<RankingItem>>.Ok(ranking);
    }

    public async Task<Resultado<MvpRespuesta>> MvpAsync(int eventoId)
    {
        var ev = await db.Eventos.AsNoTracking().Include(e => e.Participantes).FirstOrDefaultAsync(e => e.Id == eventoId);
        if (ev is null) return Resultado<MvpRespuesta>.Falla($"No existe el evento con id {eventoId}.", 404);

        var ids = ev.Participantes.Select(p => p.PersonajeId).ToList();
        var cartas = await db.Cartas.AsNoTracking().Include(c => c.Personaje).Where(c => ids.Contains(c.PersonajeId)).ToListAsync();
        var xpPrevia = await progresion.XpPreviaAsync(ids, ev.Anio);

        var mejor = cartas
            .OrderByDescending(c => ProgresionService.PoderEfectivo(c.Poder, xpPrevia.GetValueOrDefault(c.PersonajeId)))
            .ThenByDescending(c => ProgresionService.PeligrosidadEfectiva(c.NivelPeligrosidad, xpPrevia.GetValueOrDefault(c.PersonajeId)))
            .ThenBy(c => c.PersonajeId)
            .FirstOrDefault();
        if (mejor is null)
            return Resultado<MvpRespuesta>.Falla("Ningún participante del evento tiene carta, no se puede calcular el MVP.");

        return Resultado<MvpRespuesta>.Ok(new MvpRespuesta(ev.Id, ev.Nombre, mejor.Personaje.ADto(), mejor.ADto()));
    }

    public async Task<Resultado<SimulacionRespuesta>> SimularAsync(int eventoId, int? semilla)
    {
        var ev = await db.Eventos.Include(e => e.Participantes).FirstOrDefaultAsync(e => e.Id == eventoId);
        if (ev is null) return Resultado<SimulacionRespuesta>.Falla($"No existe el evento con id {eventoId}.", 404);

        // 1) Validar todo antes de modificar nada: sin resultados parciales.
        var ids = ev.Participantes.Select(p => p.PersonajeId).ToList();
        if (ids.Count < 2)
            return Resultado<SimulacionRespuesta>.Falla("El evento necesita al menos 2 participantes para simular una batalla.");

        var personajes = await db.Personajes.AsNoTracking().Where(p => ids.Contains(p.Id)).ToListAsync();
        var cartas = await db.Cartas.AsNoTracking().Where(c => ids.Contains(c.PersonajeId)).ToListAsync();

        var sinCarta = ids.Where(id => cartas.All(c => c.PersonajeId != id)).ToList();
        if (sinCarta.Count > 0)
        {
            var nombres = personajes.Where(p => sinCarta.Contains(p.Id)).Select(p => p.Nombre);
            return Resultado<SimulacionRespuesta>.Falla($"Faltan cartas para: {string.Join(", ", nombres)}.");
        }

        var filas = ids
            .Select(id => (P: personajes.First(p => p.Id == id), C: cartas.First(c => c.PersonajeId == id)))
            .ToList();
        var rebeldes = filas.Where(f => f.P.Faccion == Faccion.Rebelde).ToList();
        var imperiales = filas.Where(f => f.P.Faccion == Faccion.Imperio).ToList();
        var neutrales = filas.Where(f => f.P.Faccion == Faccion.Neutral).Select(f => f.P.Nombre).ToList();

        if (rebeldes.Count == 0 || imperiales.Count == 0)
            return Resultado<SimulacionRespuesta>.Falla("Se necesita al menos un participante Rebelde y uno del Imperio para simular.");

        // 2) Calcular con el poder efectivo: solo cuenta la XP de eventos de años anteriores.
        var xpPrevia = await progresion.XpPreviaAsync(ids, ev.Anio);
        int Xp(int personajeId) => xpPrevia.GetValueOrDefault(personajeId);

        var semillaUsada = semilla ?? Random.Shared.Next();
        var rng = new Random(semillaUsada);
        var bandoRebelde = ConstruirBando("Rebelde", rebeldes, Xp, rng);
        var bandoImperio = ConstruirBando("Imperio", imperiales, Xp, rng);

        var ganador = bandoRebelde.FuerzaFinal > bandoImperio.FuerzaFinal ? "Rebelde"
                    : bandoRebelde.FuerzaFinal < bandoImperio.FuerzaFinal ? "Imperio"
                    : "Empate";

        var criterio = $"Fuerza individual = poder efectivo (poder base + bonus por nivel) + peligrosidad efectiva x {MultiplicadorPeligrosidad} + {BonusFuerzaSensitivo} si es sensible a la Fuerza. " +
                       $"Cada bando suma sus fuerzas y las multiplica por un factor aleatorio entre {FactorMin:0.00} y {FactorMax:0.00} (semilla {semillaUsada}). " +
                       "Gana el bando con mayor fuerza final; si coinciden, es Empate. Los neutrales no suman a ningún bando.";

        var bandoGanador = ganador == "Rebelde" ? bandoRebelde : ganador == "Imperio" ? bandoImperio : null;
        var destacado = bandoGanador?.Participantes.OrderByDescending(p => p.Fuerza).First();
        var resultado = destacado is null
            ? $"Empate: {bandoRebelde.FuerzaFinal} vs {bandoImperio.FuerzaFinal}"
            : $"Ganó el bando {ganador} ({bandoRebelde.FuerzaFinal} vs {bandoImperio.FuerzaFinal}). Destacado: {destacado.Nombre}";

        // MVP del evento: mayor poder efectivo entre todos los participantes.
        var mvpId = filas
            .OrderByDescending(f => ProgresionService.PoderEfectivo(f.C.Poder, Xp(f.P.Id)))
            .ThenByDescending(f => ProgresionService.PeligrosidadEfectiva(f.C.NivelPeligrosidad, Xp(f.P.Id)))
            .ThenBy(f => f.P.Id)
            .First().P.Id;

        var ganadorEraMasDebil = ganador == "Rebelde" ? bandoRebelde.FuerzaBase < bandoImperio.FuerzaBase
                               : ganador == "Imperio" ? bandoImperio.FuerzaBase < bandoRebelde.FuerzaBase
                               : false;

        // 3) Repartir XP y guardar. Re-simular REEMPLAZA la XP del evento, no la acumula.
        var experiencia = new List<XpParticipante>();
        foreach (var participacion in ev.Participantes)
        {
            var p = personajes.First(x => x.Id == participacion.PersonajeId);
            var bando = p.Faccion switch { Faccion.Rebelde => "Rebelde", Faccion.Imperio => "Imperio", _ => null };

            var motivos = new List<string> { $"participó +{XpParticipar}" };
            var xp = XpParticipar;

            if (bando is not null)
            {
                if (ganador == "Empate") { xp += XpEmpate; motivos.Add($"empate +{XpEmpate}"); }
                else if (bando == ganador) { xp += XpGanar; motivos.Add($"bando ganador +{XpGanar}"); }
                else { xp += XpPerder; motivos.Add($"bando perdedor +{XpPerder}"); }
            }

            if (p.Id == mvpId) { xp += XpMvp; motivos.Add($"MVP +{XpMvp}"); }

            if (bando is not null && bando == ganador && ganadorEraMasDebil)
            {
                xp = (int)Math.Round(xp * MultiplicadorMasDebil);
                motivos.Add($"x{MultiplicadorMasDebil} por ganar siendo el bando más débil");
            }

            participacion.XpOtorgada = xp;
            participacion.XpMotivo = string.Join(", ", motivos);

            var nivelAntes = ProgresionService.NivelPorXp(Xp(p.Id));
            var nivelDespues = ProgresionService.NivelPorXp(Xp(p.Id) + xp);
            experiencia.Add(new XpParticipante(p.Id, p.Nombre, xp, participacion.XpMotivo, nivelAntes, nivelDespues, nivelDespues > nivelAntes));
        }

        ev.Ganador = ganador;
        await db.SaveChangesAsync();

        return Resultado<SimulacionRespuesta>.Ok(new SimulacionRespuesta(
            ev.Id, ev.Nombre, semillaUsada, bandoRebelde, bandoImperio, neutrales, ganador, criterio,
            destacado?.PersonajeId, resultado, experiencia));
    }

    private static BandoResultado ConstruirBando(
        string nombre, List<(Personaje P, CardPersonaje C)> filas, Func<int, int> xpPrevia, Random rng)
    {
        var detalle = filas
            .Select(f =>
            {
                var xp = xpPrevia(f.P.Id);
                var fuerza = ProgresionService.PoderEfectivo(f.C.Poder, xp)
                             + ProgresionService.PeligrosidadEfectiva(f.C.NivelPeligrosidad, xp) * MultiplicadorPeligrosidad
                             + (f.P.FuerzaSensitivo ? BonusFuerzaSensitivo : 0);
                return new ParticipanteFuerza(f.P.Id, f.P.Nombre, fuerza);
            })
            .ToList();

        var fuerzaBase = detalle.Sum(d => d.Fuerza);
        var factor = Math.Round(FactorMin + rng.NextDouble() * (FactorMax - FactorMin), 3);
        return new BandoResultado(nombre, detalle, fuerzaBase, factor, Math.Round(fuerzaBase * factor, 2));
    }
}
