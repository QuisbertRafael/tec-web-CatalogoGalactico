using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public class CardService(AlmacenMemoria db)
{
    public List<CardPersonaje> Listar(int? personajeId, int? poderMin, int? peligrosidad)
    {
        IEnumerable<CardPersonaje> q = db.Cartas;
        if (personajeId is not null) q = q.Where(c => c.PersonajeId == personajeId);
        if (poderMin is not null) q = q.Where(c => c.Poder >= poderMin);
        if (peligrosidad is not null) q = q.Where(c => c.NivelPeligrosidad == peligrosidad);
        return q.OrderBy(c => c.Id).ToList();
    }

    public Resultado<CardPersonaje> Obtener(int id)
    {
        var c = db.Cartas.FirstOrDefault(x => x.Id == id);
        return c is null
            ? Resultado<CardPersonaje>.Falla($"No existe la carta con id {id}.", 404)
            : Resultado<CardPersonaje>.Ok(c);
    }

    public Resultado<CardPersonaje> Crear(CardRequest r)
    {
        var error = Validar(r, null);
        if (error is not null) return Resultado<CardPersonaje>.Falla(error);

        var c = new CardPersonaje(db.SiguienteCartaId(), r.PersonajeId, r.Poder,
            r.HabilidadEspecial.Trim(), r.Arma.Trim(), r.NivelPeligrosidad, r.ImagenUrl.Trim());
        db.Cartas.Add(c);
        return Resultado<CardPersonaje>.Ok(c, 201);
    }

    public Resultado<CardPersonaje> Actualizar(int id, CardRequest r)
    {
        var idx = db.Cartas.FindIndex(x => x.Id == id);
        if (idx < 0) return Resultado<CardPersonaje>.Falla($"No existe la carta con id {id}.", 404);

        var error = Validar(r, id);
        if (error is not null) return Resultado<CardPersonaje>.Falla(error);

        var c = new CardPersonaje(id, r.PersonajeId, r.Poder,
            r.HabilidadEspecial.Trim(), r.Arma.Trim(), r.NivelPeligrosidad, r.ImagenUrl.Trim());
        db.Cartas[idx] = c;
        return Resultado<CardPersonaje>.Ok(c);
    }

    private string? Validar(CardRequest r, int? idActual)
    {
        var errores = new List<string>();

        if (!db.Personajes.Any(p => p.Id == r.PersonajeId))
            errores.Add($"No existe el personaje con id {r.PersonajeId}.");
        else if (db.Cartas.Any(c => c.PersonajeId == r.PersonajeId && c.Id != idActual))
            errores.Add("El personaje ya tiene una carta (relación 1:1).");

        if (r.Poder is < 1 or > 100) errores.Add("El poder debe estar entre 1 y 100.");
        if (r.NivelPeligrosidad is < 1 or > 5) errores.Add("El nivel de peligrosidad debe estar entre 1 y 5.");
        if (string.IsNullOrWhiteSpace(r.HabilidadEspecial)) errores.Add("La habilidad especial es obligatoria.");
        if (string.IsNullOrWhiteSpace(r.Arma)) errores.Add("El arma es obligatoria.");
        if (!Uri.TryCreate(r.ImagenUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            errores.Add("imagenUrl debe ser una URL http/https válida.");

        return errores.Count == 0 ? null : string.Join(" ", errores);
    }
}
