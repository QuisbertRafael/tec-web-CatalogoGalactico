using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public class PersonajeService(AlmacenMemoria db)
{
    public List<Personaje> Listar(Faccion? faccion, bool? fuerzaSensitivo, EstadoPersonaje? estado, string? nombre)
    {
        IEnumerable<Personaje> q = db.Personajes;
        if (faccion is not null) q = q.Where(p => p.Faccion == faccion);
        if (fuerzaSensitivo is not null) q = q.Where(p => p.FuerzaSensitivo == fuerzaSensitivo);
        if (estado is not null) q = q.Where(p => p.Estado == estado);
        if (!string.IsNullOrWhiteSpace(nombre))
            q = q.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
        return q.OrderBy(p => p.Id).ToList();
    }

    public Resultado<Personaje> Obtener(int id)
    {
        var p = db.Personajes.FirstOrDefault(x => x.Id == id);
        return p is null
            ? Resultado<Personaje>.Falla($"No existe el personaje con id {id}.", 404)
            : Resultado<Personaje>.Ok(p);
    }

    public Resultado<PersonajeConCard> ConCard(int id)
    {
        var p = db.Personajes.FirstOrDefault(x => x.Id == id);
        if (p is null)
            return Resultado<PersonajeConCard>.Falla($"No existe el personaje con el id ¨{id}.", 404);
        
        var card = db.Cartas.FirstOrDefault(c => c.PersonajeId == id);
        return Resultado<PersonajeConCard>.Ok(new PersonajeConCard(
            p.Id, p.Nombre, p.Especie, p.Faccion, p.Afiliacion, p.Estado, p.FuerzaSensitivo, p.Image, card
        ));
    }

    public Resultado<Personaje> Crear(PersonajeRequest r)
    {
        var error = Validar(r);
        if (error is not null) return Resultado<Personaje>.Falla(error);

        var p = new Personaje(db.SiguientePersonajeId(), r.Nombre.Trim(), r.Especie.Trim(),
            r.Faccion, r.Afiliacion.Trim(), r.Estado, r.FuerzaSensitivo, r.Image?.Trim());
        db.Personajes.Add(p);
        return Resultado<Personaje>.Ok(p, 201);
    }

    public Resultado<Personaje> Actualizar(int id, PersonajeRequest r)
    {
        var idx = db.Personajes.FindIndex(x => x.Id == id);
        if (idx < 0) return Resultado<Personaje>.Falla($"No existe el personaje con id {id}.", 404);

        var error = Validar(r);
        if (error is not null) return Resultado<Personaje>.Falla(error);

        // Coherencia: si un evento consigna su muerte, el estado no puede cambiarse a mano.
        if (r.Estado != EstadoPersonaje.Muerto)
        {
            var ev = db.Eventos.FirstOrDefault(e => e.Muertes.Contains(id));
            if (ev is not null)
                return Resultado<Personaje>.Falla(
                    $"La muerte del personaje está consignada en el evento '{ev.Nombre}'. Modifique ese evento para cambiar su estado.");
        }

        var actualizado = new Personaje(id, r.Nombre.Trim(), r.Especie.Trim(),
            r.Faccion, r.Afiliacion.Trim(), r.Estado, r.FuerzaSensitivo, r.Image?.Trim());
        db.Personajes[idx] = actualizado;
        return Resultado<Personaje>.Ok(actualizado);
    }

    public Resultado<bool> Eliminar(int id)
    {
        var p = db.Personajes.FirstOrDefault(x => x.Id == id);
        if (p is null) return Resultado<bool>.Falla($"No existe el personaje con id {id}.", 404);

        var ev = db.Eventos.FirstOrDefault(e => e.Participantes.Contains(id));
        if (ev is not null)
            return Resultado<bool>.Falla(
                $"No se puede eliminar a '{p.Nombre}': participa en el evento '{ev.Nombre}'.");

        db.Cartas.RemoveAll(c => c.PersonajeId == id); // su carta (1:1) se elimina con él
        db.Personajes.Remove(p);
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
