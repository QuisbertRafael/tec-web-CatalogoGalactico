using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Entities;
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

public class PersonajeService(CatalogoDbContext db) : IPersonajeService
{
    public async Task<List<PersonajeRespuesta>> ListarAsync(
        Faccion? faccion, bool? fuerzaSensitivo, EstadoPersonaje? estado, string? nombre)
    {
        var q = db.Personajes.AsNoTracking().AsQueryable();
        if (faccion is not null) q = q.Where(p => p.Faccion == faccion.Value);
        if (fuerzaSensitivo is not null) q = q.Where(p => p.FuerzaSensitivo == fuerzaSensitivo.Value);
        if (estado is not null) q = q.Where(p => p.Estado == estado.Value);
        if (!string.IsNullOrWhiteSpace(nombre)) q = q.Where(p => p.Nombre.Contains(nombre));

        var lista = await q.OrderBy(p => p.Id).ToListAsync();
        return lista.Select(p => p.ADto()).ToList();
    }

    public async Task<Resultado<PersonajeRespuesta>> ObtenerAsync(int id)
    {
        var p = await db.Personajes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return p is null
            ? Resultado<PersonajeRespuesta>.Falla($"No existe el personaje con id {id}.", 404)
            : Resultado<PersonajeRespuesta>.Ok(p.ADto());
    }

    /// <summary>Ficha de combate: combina dos tablas (Personajes y Cartas).</summary>
    public async Task<Resultado<PersonajeConCard>> ConCardAsync(int id)
    {
        var p = await db.Personajes.AsNoTracking().Include(x => x.Card).FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return Resultado<PersonajeConCard>.Falla($"No existe el personaje con id {id}.", 404);

        return Resultado<PersonajeConCard>.Ok(new PersonajeConCard(
            p.Id, p.Nombre, p.Especie, p.Faccion, p.Afiliacion, p.Estado, p.FuerzaSensitivo, p.Image,
            p.Card?.ADto()));
    }

    public async Task<Resultado<PersonajeRespuesta>> CrearAsync(PersonajeRequest r)
    {
        var error = Validar(r);
        if (error is not null) return Resultado<PersonajeRespuesta>.Falla(error);

        var p = new Personaje
        {
            Nombre = r.Nombre.Trim(),
            Especie = r.Especie.Trim(),
            Faccion = r.Faccion,
            Afiliacion = r.Afiliacion.Trim(),
            Estado = r.Estado,
            FuerzaSensitivo = r.FuerzaSensitivo,
            Image = r.Image?.Trim() ?? ""
        };
        db.Personajes.Add(p);
        await db.SaveChangesAsync();
        return Resultado<PersonajeRespuesta>.Ok(p.ADto(), 201);
    }

    public async Task<Resultado<PersonajeRespuesta>> ActualizarAsync(int id, PersonajeRequest r)
    {
        var p = await db.Personajes.FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return Resultado<PersonajeRespuesta>.Falla($"No existe el personaje con id {id}.", 404);

        var error = Validar(r);
        if (error is not null) return Resultado<PersonajeRespuesta>.Falla(error);

        // Coherencia: si un evento consigna su muerte, el estado no puede cambiarse a mano.
        if (r.Estado != EstadoPersonaje.Muerto)
        {
            var eventoMuerte = await db.EventoParticipantes
                .Where(x => x.PersonajeId == id && x.Murio)
                .Select(x => x.Evento.Nombre)
                .FirstOrDefaultAsync();
            if (eventoMuerte is not null)
                return Resultado<PersonajeRespuesta>.Falla(
                    $"La muerte del personaje está consignada en el evento '{eventoMuerte}'. Modifique ese evento para cambiar su estado.", 409);
        }

        p.Nombre = r.Nombre.Trim();
        p.Especie = r.Especie.Trim();
        p.Faccion = r.Faccion;
        p.Afiliacion = r.Afiliacion.Trim();
        p.Estado = r.Estado;
        p.FuerzaSensitivo = r.FuerzaSensitivo;
        p.Image = r.Image?.Trim() ?? "";
        await db.SaveChangesAsync();
        return Resultado<PersonajeRespuesta>.Ok(p.ADto());
    }

    public async Task<Resultado<bool>> EliminarAsync(int id)
    {
        var p = await db.Personajes.FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return Resultado<bool>.Falla($"No existe el personaje con id {id}.", 404);

        var evento = await db.EventoParticipantes
            .Where(x => x.PersonajeId == id)
            .Select(x => x.Evento.Nombre)
            .FirstOrDefaultAsync();
        if (evento is not null)
            return Resultado<bool>.Falla($"No se puede eliminar a '{p.Nombre}': participa en el evento '{evento}'.", 409);

        db.Personajes.Remove(p); // su carta (1:1) se elimina en cascada
        await db.SaveChangesAsync();
        return Resultado<bool>.Ok(true);
    }

    private static string? Validar(PersonajeRequest r)
    {
        var errores = new List<string>();
        if (string.IsNullOrWhiteSpace(r.Nombre)) errores.Add("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(r.Especie)) errores.Add("La especie es obligatoria.");
        if (string.IsNullOrWhiteSpace(r.Afiliacion)) errores.Add("La afiliación es obligatoria.");
        if (!Enum.IsDefined(r.Faccion)) errores.Add("La facción debe ser Rebelde, Imperio o Neutral.");
        if (!Enum.IsDefined(r.Estado)) errores.Add("El estado debe ser Vivo, Muerto o Desconocido.");
        return errores.Count == 0 ? null : string.Join(" ", errores);
    }
}
