using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Entities;
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

/// <summary>Rankings, MVP y simulación de batallas.</summary>
public class EstadisticasService(CatalogoDbContext db) : IEstadisticasService
{
    private const double FactorMin = 0.90;
    private const double FactorMax = 1.10;
    private const int MultiplicadorPeligrosidad = 10;
    private const int BonusFuerzaSensitivo = 15;

    public async Task<Resultado<List<RankingItem>>> RankingAsync(string por)
    {
        var campo = por.Trim().ToLowerInvariant();
        if (campo is not ("poder" or "peligrosidad"))
            return Resultado<List<RankingItem>>.Falla("Valor de 'por' inválido. Use 'poder' o 'peligrosidad'.");

        var datos = await db.Cartas.AsNoTracking()
            .Select(c => new { c.Personaje.Id, c.Personaje.Nombre, c.Personaje.Faccion, c.Poder, c.NivelPeligrosidad })
            .ToListAsync();

        var ranking = datos
            .Select(x => new { x.Id, x.Nombre, x.Faccion, Valor = campo == "poder" ? x.Poder : x.NivelPeligrosidad })
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

        var mejor = cartas
            .OrderByDescending(c => c.Poder).ThenByDescending(c => c.NivelPeligrosidad).ThenBy(c => c.PersonajeId)
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

        // 2) Calcular.
        var semillaUsada = semilla ?? Random.Shared.Next();
        var rng = new Random(semillaUsada);
        var bandoRebelde = ConstruirBando("Rebelde", rebeldes, rng);
        var bandoImperio = ConstruirBando("Imperio", imperiales, rng);

        var ganador = bandoRebelde.FuerzaFinal > bandoImperio.FuerzaFinal ? "Rebelde"
                    : bandoRebelde.FuerzaFinal < bandoImperio.FuerzaFinal ? "Imperio"
                    : "Empate";

        var criterio = $"Fuerza individual = poder + peligrosidad x {MultiplicadorPeligrosidad} + {BonusFuerzaSensitivo} si es sensible a la Fuerza. " +
                       $"Cada bando suma sus fuerzas y las multiplica por un factor aleatorio entre {FactorMin:0.00} y {FactorMax:0.00} (semilla {semillaUsada}). " +
                       "Gana el bando con mayor fuerza final; si coinciden, es Empate. Los neutrales no suman a ningún bando.";

        var bandoGanador = ganador == "Rebelde" ? bandoRebelde : ganador == "Imperio" ? bandoImperio : null;
        var destacado = bandoGanador?.Participantes.OrderByDescending(p => p.Fuerza).First();
        var resultado = destacado is null
            ? $"Empate: {bandoRebelde.FuerzaFinal} vs {bandoImperio.FuerzaFinal}"
            : $"Ganó el bando {ganador} ({bandoRebelde.FuerzaFinal} vs {bandoImperio.FuerzaFinal}). Destacado: {destacado.Nombre}";

        // 3) Guardar el resultado solo cuando todo salió bien.
        ev.Ganador = ganador;
        await db.SaveChangesAsync();

        return Resultado<SimulacionRespuesta>.Ok(new SimulacionRespuesta(
            ev.Id, ev.Nombre, semillaUsada, bandoRebelde, bandoImperio, neutrales, ganador, criterio,
            destacado?.PersonajeId, resultado));
    }

    private static BandoResultado ConstruirBando(string nombre, List<(Personaje P, CardPersonaje C)> filas, Random rng)
    {
        var detalle = filas
            .Select(f => new ParticipanteFuerza(f.P.Id, f.P.Nombre,
                f.C.Poder + f.C.NivelPeligrosidad * MultiplicadorPeligrosidad + (f.P.FuerzaSensitivo ? BonusFuerzaSensitivo : 0)))
            .ToList();

        var fuerzaBase = detalle.Sum(d => d.Fuerza);
        var factor = Math.Round(FactorMin + rng.NextDouble() * (FactorMax - FactorMin), 3);
        return new BandoResultado(nombre, detalle, fuerzaBase, factor, Math.Round(fuerzaBase * factor, 2));
    }
}
