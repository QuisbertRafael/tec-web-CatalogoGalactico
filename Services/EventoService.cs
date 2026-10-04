using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public class EventoService(AlmacenMemoria db)
{
    private static readonly string[] Ganadores = ["Rebelde", "Imperio", "Empate"];

    public List<Evento> Listar(string? nombre, string? ubicacion, int? personajeId)
    {
        IEnumerable<Evento> q = db.Eventos;
        if (!string.IsNullOrWhiteSpace(nombre))
            q = q.Where(e => e.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(ubicacion))
            q = q.Where(e => e.Ubicacion.Contains(ubicacion, StringComparison.OrdinalIgnoreCase));
        if (personajeId is not null) q = q.Where(e => e.Participantes.Contains(personajeId.Value));
        return q.OrderBy(e => e.Anio).ThenBy(e => e.Id).ToList();
    }

    public Resultado<Evento> Obtener(int id)
    {
        var e = db.Eventos.FirstOrDefault(x => x.Id == id);
        return e is null
            ? Resultado<Evento>.Falla($"No existe el evento con id {id}.", 404)
            : Resultado<Evento>.Ok(e);
    }

    public Resultado<List<Evento>> PorPersonaje(int personajeId)
    {
        if (!db.Personajes.Any(p => p.Id == personajeId))
            return Resultado<List<Evento>>.Falla($"No existe el personaje con id {personajeId}.", 404);

        var eventos = db.Eventos
            .Where(e => e.Participantes.Contains(personajeId))
            .OrderBy(e => e.Anio).ThenBy(e => e.Id)
            .ToList();
        return Resultado<List<Evento>>.Ok(eventos);
    }

    public Resultado<Evento> Crear(EventoRequest r) => Guardar(null, r);

    public Resultado<Evento> Actualizar(int id, EventoRequest r) =>
        db.Eventos.Any(e => e.Id == id)
            ? Guardar(id, r)
            : Resultado<Evento>.Falla($"No existe el evento con id {id}.", 404);

    private Resultado<Evento> Guardar(int? id, EventoRequest r)
    {
        var errores = new List<string>();
        if (string.IsNullOrWhiteSpace(r.Nombre)) errores.Add("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(r.Ubicacion)) errores.Add("La ubicación es obligatoria.");
        if (string.IsNullOrWhiteSpace(r.Descripcion)) errores.Add("La descripción es obligatoria.");
        if (!FechaGalactica.TryParse(r.Fecha, out var anio))
            errores.Add("La fecha debe tener el formato '10 BBY', '3 ABY' o 'Batalla de Yavin'.");

        var participantes = (r.Participantes ?? []).Distinct().ToList();
        var muertes = (r.Muertes ?? []).Distinct().ToList();

        var inexistentes = participantes.Where(p => db.Personajes.All(x => x.Id != p)).ToList();
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

        if (errores.Count > 0) return Resultado<Evento>.Falla(string.Join(" ", errores));

        var candidato = new Evento(id ?? 0, r.Nombre.Trim(), FechaGalactica.Formatear(anio), anio,
            r.Ubicacion.Trim(), r.Descripcion.Trim(), participantes, muertes, ganador);

        var conflicto = ValidarCoherenciaTemporal(candidato);
        if (conflicto is not null) return Resultado<Evento>.Falla(conflicto);

        var muertesPrevias = id is null
            ? new List<int>()
            : db.Eventos.First(e => e.Id == id).Muertes;

        Evento guardado;
        if (id is null)
        {
            guardado = candidato with { Id = db.SiguienteEventoId() };
            db.Eventos.Add(guardado);
        }
        else
        {
            guardado = candidato;
            db.Eventos[db.Eventos.FindIndex(e => e.Id == id)] = guardado;
        }

        SincronizarEstados(muertesPrevias, guardado);
        return Resultado<Evento>.Ok(guardado, id is null ? 201 : 200);
    }


    private string? ValidarCoherenciaTemporal(Evento candidato)
    {
        var todos = db.Eventos.Where(e => e.Id != candidato.Id).Append(candidato).ToList();

        foreach (var pid in candidato.Participantes)
        {
            var nombre = db.Personajes.First(p => p.Id == pid).Nombre;
            var muertes = todos.Where(e => e.Muertes.Contains(pid)).ToList();

            if (muertes.Count > 1)
                return $"'{nombre}' ya tiene su muerte consignada en otro evento ('{muertes.First(m => m.Id != candidato.Id).Nombre}').";

            if (muertes.Count == 1)
            {
                var muerte = muertes[0];
                var posterior = todos.FirstOrDefault(e => e.Participantes.Contains(pid) && e.Anio > muerte.Anio);
                if (posterior is not null)
                    return $"'{nombre}' murió en '{muerte.Nombre}' ({muerte.Fecha}) y no puede participar en '{posterior.Nombre}' ({posterior.Fecha}).";
            }
        }
        return null;
    }

    private void SincronizarEstados(List<int> muertesPrevias, Evento evento)
    {
        foreach (var pid in evento.Muertes)
            CambiarEstado(pid, EstadoPersonaje.Muerto);

        foreach (var pid in muertesPrevias.Except(evento.Muertes))
            if (!db.Eventos.Any(e => e.Muertes.Contains(pid)))
                CambiarEstado(pid, EstadoPersonaje.Vivo);
    }

    private void CambiarEstado(int personajeId, EstadoPersonaje estado)
    {
        var idx = db.Personajes.FindIndex(p => p.Id == personajeId);
        if (idx >= 0) db.Personajes[idx] = db.Personajes[idx] with { Estado = estado };
    }
}
