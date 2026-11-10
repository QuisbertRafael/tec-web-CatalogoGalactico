using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Entities;
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

public class EventoService(CatalogoDbContext db) : IEventoService
{
    private static readonly string[] Ganadores = ["Rebelde", "Imperio", "Empate"];

    /// <summary>Una participación con los datos del evento, para validar la línea de tiempo.</summary>
    private record Registro(int PersonajeId, bool Murio, string Evento, string Fecha, int Anio);

    public async Task<List<EventoRespuesta>> ListarAsync(string? nombre, string? ubicacion, int? personajeId)
    {
        var q = db.Eventos.AsNoTracking().Include(e => e.Participantes).AsQueryable();
        if (!string.IsNullOrWhiteSpace(nombre)) q = q.Where(e => e.Nombre.Contains(nombre));
        if (!string.IsNullOrWhiteSpace(ubicacion)) q = q.Where(e => e.Ubicacion.Contains(ubicacion));
        if (personajeId is not null) q = q.Where(e => e.Participantes.Any(p => p.PersonajeId == personajeId.Value));

        var eventos = await q.OrderBy(e => e.Anio).ThenBy(e => e.Id).ToListAsync();
        return eventos.Select(e => e.ADto()).ToList();
    }

    public async Task<Resultado<EventoRespuesta>> ObtenerAsync(int id)
    {
        var e = await db.Eventos.AsNoTracking().Include(x => x.Participantes).FirstOrDefaultAsync(x => x.Id == id);
        return e is null
            ? Resultado<EventoRespuesta>.Falla($"No existe el evento con id {id}.", 404)
            : Resultado<EventoRespuesta>.Ok(e.ADto());
    }

    /// <summary>Eventos en los que participa un personaje, en orden cronológico.</summary>
    public async Task<Resultado<List<EventoRespuesta>>> PorPersonajeAsync(int personajeId)
    {
        if (!await db.Personajes.AnyAsync(p => p.Id == personajeId))
            return Resultado<List<EventoRespuesta>>.Falla($"No existe el personaje con id {personajeId}.", 404);

        var eventos = await db.Eventos.AsNoTracking()
            .Include(e => e.Participantes)
            .Where(e => e.Participantes.Any(p => p.PersonajeId == personajeId))
            .OrderBy(e => e.Anio).ThenBy(e => e.Id)
            .ToListAsync();
        return Resultado<List<EventoRespuesta>>.Ok(eventos.Select(e => e.ADto()).ToList());
    }

    public Task<Resultado<EventoRespuesta>> CrearAsync(EventoRequest r) => GuardarAsync(null, r);

    public Task<Resultado<EventoRespuesta>> ActualizarAsync(int id, EventoRequest r) => GuardarAsync(id, r);

    private async Task<Resultado<EventoRespuesta>> GuardarAsync(int? id, EventoRequest r)
    {
        Evento? existente = null;
        if (id is not null)
        {
            existente = await db.Eventos.Include(e => e.Participantes).FirstOrDefaultAsync(e => e.Id == id);
            if (existente is null) return Resultado<EventoRespuesta>.Falla($"No existe el evento con id {id}.", 404);
        }

        // 1) Validar datos (400).
        var errores = new List<string>();
        if (string.IsNullOrWhiteSpace(r.Nombre)) errores.Add("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(r.Ubicacion)) errores.Add("La ubicación es obligatoria.");
        if (string.IsNullOrWhiteSpace(r.Descripcion)) errores.Add("La descripción es obligatoria.");
        if (!FechaGalactica.TryParse(r.Fecha, out var anio))
            errores.Add("La fecha debe tener el formato '10 BBY', '3 ABY' o 'Batalla de Yavin'.");

        var participantes = (r.Participantes ?? new List<int>()).Distinct().ToList();
        var muertes = (r.Muertes ?? new List<int>()).Distinct().ToList();

        var idsExistentes = await db.Personajes
            .Where(p => participantes.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();
        var inexistentes = participantes.Except(idsExistentes).ToList();
        if (inexistentes.Count > 0)
            errores.Add($"No existen personajes con id: {string.Join(", ", inexistentes)}.");

        var muertesAjenas = muertes.Where(m => !participantes.Contains(m)).ToList();
        if (muertesAjenas.Count > 0)
            errores.Add($"Los personajes que mueren deben figurar como participantes (ids: {string.Join(", ", muertesAjenas)}).");

        string? ganador = null;
        if (!string.IsNullOrWhiteSpace(r.Ganador))
        {
            ganador = Ganadores.FirstOrDefault(g => g.Equals(r.Ganador.Trim(), StringComparison.OrdinalIgnoreCase));
            if (ganador is null) errores.Add("El ganador debe ser Rebelde, Imperio o Empate.");
        }

        if (errores.Count > 0) return Resultado<EventoRespuesta>.Falla(string.Join(" ", errores));

        // 2) Validar la línea de tiempo (409).
        var conflicto = await ValidarCoherenciaTemporalAsync(id, r.Nombre.Trim(), FechaGalactica.Formatear(anio), anio, participantes, muertes);
        if (conflicto is not null) return Resultado<EventoRespuesta>.Falla(conflicto, 409);

        // 3) Guardar evento y participaciones.
        var muertesPrevias = existente?.Participantes.Where(p => p.Murio).Select(p => p.PersonajeId).ToList()
                             ?? new List<int>();

        Evento evento;
        if (existente is null)
        {
            evento = new Evento();
            db.Eventos.Add(evento);
        }
        else
        {
            evento = existente;
        }

        evento.Nombre = r.Nombre.Trim();
        evento.Fecha = FechaGalactica.Formatear(anio);
        evento.Anio = anio;
        evento.Ubicacion = r.Ubicacion.Trim();
        evento.Descripcion = r.Descripcion.Trim();
        evento.Ganador = ganador;

        foreach (var actual in evento.Participantes.ToList())
        {
            if (!participantes.Contains(actual.PersonajeId))
            {
                db.EventoParticipantes.Remove(actual);
            }
            else
            {
                actual.Murio = muertes.Contains(actual.PersonajeId);
                // El evento cambió: el resultado y la experiencia anteriores ya no son válidos.
                actual.XpOtorgada = 0;
                actual.XpMotivo = null;
            }
        }

        foreach (var pid in participantes.Where(pid => evento.Participantes.All(x => x.PersonajeId != pid)))
            evento.Participantes.Add(new EventoParticipante { PersonajeId = pid, Murio = muertes.Contains(pid) });

        // 4) Efecto entre colecciones: actualizar el estado de los personajes afectados.
        var afectados = muertes.Union(muertesPrevias).ToList();
        if (afectados.Count > 0)
        {
            var personajes = await db.Personajes.Where(p => afectados.Contains(p.Id)).ToListAsync();
            foreach (var p in personajes)
            {
                if (muertes.Contains(p.Id))
                    p.Estado = EstadoPersonaje.Muerto;
                else if (!await db.EventoParticipantes.AnyAsync(x => x.PersonajeId == p.Id && x.Murio && x.EventoId != evento.Id))
                    p.Estado = EstadoPersonaje.Vivo; // dejó de morir en este evento y no muere en otro
            }
        }

        await db.SaveChangesAsync();

        var guardado = await db.Eventos.AsNoTracking().Include(e => e.Participantes).FirstAsync(e => e.Id == evento.Id);
        return Resultado<EventoRespuesta>.Ok(guardado.ADto(), existente is null ? 201 : 200);
    }

    /// <summary>
    /// Regla: un personaje muere una sola vez y no puede participar en eventos posteriores a su muerte.
    /// Se evalúa con el evento candidato incluido. Devuelve null si todo está bien.
    /// </summary>
    private async Task<string?> ValidarCoherenciaTemporalAsync(
        int? id, string nombreEvento, string fecha, int anio, List<int> participantes, List<int> muertes)
    {
        var idActual = id ?? 0;

        var otras = (await db.EventoParticipantes.AsNoTracking()
                .Where(x => x.EventoId != idActual && participantes.Contains(x.PersonajeId))
                .Select(x => new { x.PersonajeId, x.Murio, Evento = x.Evento.Nombre, x.Evento.Fecha, x.Evento.Anio })
                .ToListAsync())
            .Select(x => new Registro(x.PersonajeId, x.Murio, x.Evento, x.Fecha, x.Anio))
            .ToList();

        var nombres = await db.Personajes.AsNoTracking()
            .Where(p => participantes.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Nombre);

        foreach (var pid in participantes)
        {
            var nombre = nombres[pid];

            // Todas las participaciones del personaje, incluida la del evento candidato.
            var registros = otras.Where(o => o.PersonajeId == pid).ToList();
            var candidato = new Registro(pid, muertes.Contains(pid), nombreEvento, fecha, anio);
            registros.Add(candidato);

            var muertesDelPersonaje = registros.Where(x => x.Murio).ToList();
            if (muertesDelPersonaje.Count > 1)
            {
                var otraMuerte = muertesDelPersonaje.First(x => x != candidato);
                return $"'{nombre}' ya tiene su muerte consignada en otro evento ('{otraMuerte.Evento}').";
            }

            if (muertesDelPersonaje.Count == 1)
            {
                var muerte = muertesDelPersonaje[0];
                var posterior = registros.FirstOrDefault(x => x.Anio > muerte.Anio);
                if (posterior is not null)
                    return $"'{nombre}' murió en '{muerte.Evento}' ({muerte.Fecha}) y no puede participar en '{posterior.Evento}' ({posterior.Fecha}).";
            }
        }

        return null;
    }
}
