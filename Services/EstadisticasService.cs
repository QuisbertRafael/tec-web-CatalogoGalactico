using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

/// <summary>Rankings, MVP y simulación de batallas.</summary>
public class EstadisticasService(AlmacenMemoria db)
{
    private const double FactorMin = 0.90;
    private const double FactorMax = 1.10;
    private const int MultiplicadorPeligrosidad = 10;
    private const int BonusFuerzaSensitivo = 15;

    public Resultado<List<RankingItem>> Ranking(string por)
    {
        var campo = por.Trim().ToLowerInvariant();
        if (campo is not ("poder" or "peligrosidad"))
            return Resultado<List<RankingItem>>.Falla("Valor de 'por' inválido. Use 'poder' o 'peligrosidad'.");

        var ranking = db.Cartas
            .Join(db.Personajes, c => c.PersonajeId, p => p.Id, (c, p) => (P: p, C: c))
            .Select(x => (x.P, Valor: campo == "poder" ? x.C.Poder : x.C.NivelPeligrosidad))
            .OrderByDescending(x => x.Valor).ThenBy(x => x.P.Nombre)
            .Select((x, i) => new RankingItem(i + 1, x.P.Id, x.P.Nombre, x.P.Faccion, x.Valor))
            .ToList();

        return Resultado<List<RankingItem>>.Ok(ranking);
    }

    public Resultado<MvpRespuesta> Mvp(int eventoId)
    {
        var ev = db.Eventos.FirstOrDefault(e => e.Id == eventoId);
        if (ev is null) return Resultado<MvpRespuesta>.Falla($"No existe el evento con id {eventoId}.", 404);

        var mejor = ev.Participantes
            .Select(id => (P: db.Personajes.FirstOrDefault(p => p.Id == id), C: db.Cartas.FirstOrDefault(c => c.PersonajeId == id)))
            .Where(x => x.P is not null && x.C is not null)
            .OrderByDescending(x => x.C!.Poder).ThenByDescending(x => x.C!.NivelPeligrosidad).ThenBy(x => x.P!.Id)
            .FirstOrDefault();

        if (mejor.P is null || mejor.C is null)
            return Resultado<MvpRespuesta>.Falla("Ningún participante del evento tiene carta, no se puede calcular el MVP.");

        return Resultado<MvpRespuesta>.Ok(new MvpRespuesta(ev.Id, ev.Nombre, mejor.P, mejor.C));
    }

    public Resultado<SimulacionRespuesta> Simular(int eventoId, int? semilla)
    {
        var idx = db.Eventos.FindIndex(e => e.Id == eventoId);
        if (idx < 0) return Resultado<SimulacionRespuesta>.Falla($"No existe el evento con id {eventoId}.", 404);
        var ev = db.Eventos[idx];



        // 1) Validar todo antes de modificar nada: sin resultados parciales.
        if (ev.Participantes.Count < 2)
            return Resultado<SimulacionRespuesta>.Falla("El evento necesita al menos 2 participantes para simular una batalla.");

        var sinCarta = ev.Participantes.Where(id => db.Cartas.All(c => c.PersonajeId != id)).ToList();
        if (sinCarta.Count > 0)
        {
            var nombres = sinCarta.Select(id => db.Personajes.First(p => p.Id == id).Nombre);
            return Resultado<SimulacionRespuesta>.Falla($"Faltan cartas para: {string.Join(", ", nombres)}.");
        }

        var filas = ev.Participantes
            .Select(id => (P: db.Personajes.First(p => p.Id == id), C: db.Cartas.First(c => c.PersonajeId == id)))
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

        // 3) Guardar el resultado solo cuando todo salió bien.
        db.Eventos[idx] = ev with { Ganador = ganador };

        var bandoGanador = ganador == "Rebelde" ? bandoRebelde
                        : ganador == "Imperio" ? bandoImperio
                        : null;
        var estrella = bandoGanador?.Participantes.OrderByDescending(p => p.Fuerza).First();

        var mensaje = estrella is null
            ? $"Empate: {bandoRebelde.FuerzaFinal} vs {bandoImperio.FuerzaFinal}"
            : $"Ganó el bando {ganador} ({bandoRebelde.FuerzaFinal} vs {bandoImperio.FuerzaFinal}). Destacado: {estrella.Nombre}";

        return Resultado<SimulacionRespuesta>.Ok(new SimulacionRespuesta(
            ev.Id, ev.Nombre, semillaUsada, bandoRebelde, bandoImperio, neutrales, ganador, criterio,
            estrella?.PersonajeId, mensaje));
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
