using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Entities;
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

/// <summary>
/// Sistema de experiencia. El poder base de la carta nunca cambia: el poder efectivo se calcula
/// con la XP que el personaje ganó en eventos anteriores a la fecha consultada.
/// </summary>
public class ProgresionService(CatalogoDbContext db) : IProgresionService
{
    // XP mínima para cada nivel: nivel 1 = 0, nivel 2 = 100, nivel 3 = 250, nivel 4 = 500, nivel 5 = 900.
    private static readonly int[] UmbralesXp = [0, 100, 250, 500, 900];
    private const int BonusPoderPorNivel = 4;
    private const int PoderMaximo = 100;
    private const int PeligrosidadMaxima = 5;

    private static readonly string[] RangosRebelde = ["Recluta", "Soldado", "Comandante", "Héroe de la Alianza", "Leyenda"];
    private static readonly string[] RangosImperio = ["Cadete", "Oficial", "Comandante", "Almirante", "Gran Almirante"];
    private static readonly string[] RangosNeutral = ["Novato", "Mercenario", "Veterano", "Experto", "Maestro"];

    public static int NivelPorXp(int xp)
    {
        var nivel = 1;
        for (var i = 1; i < UmbralesXp.Length; i++)
            if (xp >= UmbralesXp[i]) nivel = i + 1;
        return nivel;
    }

    public static int PoderEfectivo(int poderBase, int xp) =>
        Math.Min(PoderMaximo, poderBase + (NivelPorXp(xp) - 1) * BonusPoderPorNivel);

    /// <summary>Los niveles 3 y 5 suben 1 punto de peligrosidad (máximo 5).</summary>
    public static int PeligrosidadEfectiva(int peligrosidadBase, int xp)
    {
        var nivel = NivelPorXp(xp);
        return Math.Min(PeligrosidadMaxima, peligrosidadBase + (nivel >= 3 ? 1 : 0) + (nivel >= 5 ? 1 : 0));
    }

    public static string Rango(Faccion faccion, int nivel)
    {
        var rangos = faccion switch
        {
            Faccion.Rebelde => RangosRebelde,
            Faccion.Imperio => RangosImperio,
            _ => RangosNeutral
        };
        return rangos[nivel - 1];
    }

    /// <summary>XP que falta para el siguiente nivel; null si ya está en el nivel máximo.</summary>
    public static int? XpParaSiguienteNivel(int xp, int nivel) =>
        nivel >= UmbralesXp.Length ? null : UmbralesXp[nivel] - xp;

    /// <summary>XP total de cada personaje (todos los eventos).</summary>
    public async Task<Dictionary<int, int>> XpTotalAsync()
    {
        var filas = await db.EventoParticipantes.AsNoTracking()
            .Select(x => new { x.PersonajeId, x.XpOtorgada })
            .ToListAsync();
        return filas.GroupBy(x => x.PersonajeId).ToDictionary(g => g.Key, g => g.Sum(x => x.XpOtorgada));
    }

    /// <summary>XP acumulada solo con eventos de año ANTERIOR al indicado (los del mismo año no cuentan).</summary>
    public async Task<Dictionary<int, int>> XpPreviaAsync(IEnumerable<int> personajeIds, int anio)
    {
        var ids = personajeIds.ToList();
        var filas = await db.EventoParticipantes.AsNoTracking()
            .Where(x => ids.Contains(x.PersonajeId) && x.Evento.Anio < anio)
            .Select(x => new { x.PersonajeId, x.XpOtorgada })
            .ToListAsync();
        return filas.GroupBy(x => x.PersonajeId).ToDictionary(g => g.Key, g => g.Sum(x => x.XpOtorgada));
    }

    public async Task<Resultado<ProgresionCarta>> ProgresionAsync(int cartaId)
    {
        var carta = await db.Cartas.AsNoTracking().Include(c => c.Personaje).FirstOrDefaultAsync(c => c.Id == cartaId);
        if (carta is null) return Resultado<ProgresionCarta>.Falla($"No existe la carta con id {cartaId}.", 404);

        var participaciones = await db.EventoParticipantes.AsNoTracking()
            .Include(x => x.Evento)
            .Where(x => x.PersonajeId == carta.PersonajeId && x.XpOtorgada > 0)
            .OrderBy(x => x.Evento.Anio).ThenBy(x => x.EventoId)
            .ToListAsync();

        var xp = participaciones.Sum(x => x.XpOtorgada);
        var nivel = NivelPorXp(xp);

        return Resultado<ProgresionCarta>.Ok(new ProgresionCarta(
            carta.Id, carta.PersonajeId, carta.Personaje.Nombre, carta.Personaje.Faccion,
            xp, nivel, Rango(carta.Personaje.Faccion, nivel),
            carta.Poder, PoderEfectivo(carta.Poder, xp), PeligrosidadEfectiva(carta.NivelPeligrosidad, xp),
            XpParaSiguienteNivel(xp, nivel),
            participaciones.Select(x => new XpEvento(x.EventoId, x.Evento.Nombre, x.Evento.Fecha, x.XpOtorgada, x.XpMotivo ?? "")).ToList()));
    }

    public async Task<Resultado<PoderEnFecha>> PoderEnFechaAsync(int cartaId, string fecha)
    {
        if (!FechaGalactica.TryParse(fecha, out var anio))
            return Resultado<PoderEnFecha>.Falla("La fecha debe tener el formato '10 BBY', '3 ABY' o 'Batalla de Yavin'.");

        var carta = await db.Cartas.AsNoTracking().Include(c => c.Personaje).FirstOrDefaultAsync(c => c.Id == cartaId);
        if (carta is null) return Resultado<PoderEnFecha>.Falla($"No existe la carta con id {cartaId}.", 404);

        var xpPorPersonaje = await XpPreviaAsync([carta.PersonajeId], anio);
        var xp = xpPorPersonaje.GetValueOrDefault(carta.PersonajeId);
        var nivel = NivelPorXp(xp);

        return Resultado<PoderEnFecha>.Ok(new PoderEnFecha(
            carta.Id, carta.PersonajeId, carta.Personaje.Nombre, FechaGalactica.Formatear(anio), anio,
            xp, nivel, Rango(carta.Personaje.Faccion, nivel), carta.Poder, PoderEfectivo(carta.Poder, xp)));
    }
}
