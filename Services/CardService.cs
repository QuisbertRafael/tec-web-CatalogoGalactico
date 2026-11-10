using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Entities;
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

public class CardService(CatalogoDbContext db) : ICardService
{
    private record Problema(string Mensaje, int Codigo);

    public async Task<List<CardRespuesta>> ListarAsync(int? personajeId, int? poderMin, int? peligrosidad)
    {
        var q = db.Cartas.AsNoTracking().AsQueryable();
        if (personajeId is not null) q = q.Where(c => c.PersonajeId == personajeId.Value);
        if (poderMin is not null) q = q.Where(c => c.Poder >= poderMin.Value);
        if (peligrosidad is not null) q = q.Where(c => c.NivelPeligrosidad == peligrosidad.Value);

        var lista = await q.OrderBy(c => c.Id).ToListAsync();
        return lista.Select(c => c.ADto()).ToList();
    }

    public async Task<Resultado<CardRespuesta>> ObtenerAsync(int id)
    {
        var c = await db.Cartas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return c is null
            ? Resultado<CardRespuesta>.Falla($"No existe la carta con id {id}.", 404)
            : Resultado<CardRespuesta>.Ok(c.ADto());
    }

    public async Task<Resultado<CardRespuesta>> CrearAsync(CardRequest r)
    {
        var problema = await ValidarAsync(r, null);
        if (problema is not null) return Resultado<CardRespuesta>.Falla(problema.Mensaje, problema.Codigo);

        var c = new CardPersonaje
        {
            PersonajeId = r.PersonajeId,
            Poder = r.Poder,
            HabilidadEspecial = r.HabilidadEspecial.Trim(),
            Arma = r.Arma.Trim(),
            NivelPeligrosidad = r.NivelPeligrosidad,
            ImagenUrl = r.ImagenUrl.Trim()
        };
        db.Cartas.Add(c);
        await db.SaveChangesAsync();
        return Resultado<CardRespuesta>.Ok(c.ADto(), 201);
    }

    public async Task<Resultado<CardRespuesta>> ActualizarAsync(int id, CardRequest r)
    {
        var c = await db.Cartas.FirstOrDefaultAsync(x => x.Id == id);
        if (c is null) return Resultado<CardRespuesta>.Falla($"No existe la carta con id {id}.", 404);

        var problema = await ValidarAsync(r, id);
        if (problema is not null) return Resultado<CardRespuesta>.Falla(problema.Mensaje, problema.Codigo);

        c.PersonajeId = r.PersonajeId;
        c.Poder = r.Poder;
        c.HabilidadEspecial = r.HabilidadEspecial.Trim();
        c.Arma = r.Arma.Trim();
        c.NivelPeligrosidad = r.NivelPeligrosidad;
        c.ImagenUrl = r.ImagenUrl.Trim();
        await db.SaveChangesAsync();
        return Resultado<CardRespuesta>.Ok(c.ADto());
    }

    public async Task<Resultado<bool>> EliminarAsync(int id)
    {
        var c = await db.Cartas.FirstOrDefaultAsync(x => x.Id == id);
        if (c is null) return Resultado<bool>.Falla($"No existe la carta con id {id}.", 404);

        db.Cartas.Remove(c);
        await db.SaveChangesAsync();
        return Resultado<bool>.Ok(true);
    }

    private async Task<Problema?> ValidarAsync(CardRequest r, int? idActual)
    {
        var errores = new List<string>();
        if (r.Poder is < 1 or > 100) errores.Add("El poder debe estar entre 1 y 100.");
        if (r.NivelPeligrosidad is < 1 or > 5) errores.Add("El nivel de peligrosidad debe estar entre 1 y 5.");
        if (string.IsNullOrWhiteSpace(r.HabilidadEspecial)) errores.Add("La habilidad especial es obligatoria.");
        if (string.IsNullOrWhiteSpace(r.Arma)) errores.Add("El arma es obligatoria.");
        if (!Uri.TryCreate(r.ImagenUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            errores.Add("imagenUrl debe ser una URL http/https válida.");

        if (errores.Count > 0) return new Problema(string.Join(" ", errores), 400);

        if (!await db.Personajes.AnyAsync(p => p.Id == r.PersonajeId))
            return new Problema($"No existe el personaje con id {r.PersonajeId}.", 400);

        // Relación 1:1: un personaje no puede tener dos cartas.
        var excluir = idActual ?? 0;
        if (await db.Cartas.AnyAsync(c => c.PersonajeId == r.PersonajeId && c.Id != excluir))
            return new Problema("El personaje ya tiene una carta (relación 1:1).", 409);

        return null;
    }
}
