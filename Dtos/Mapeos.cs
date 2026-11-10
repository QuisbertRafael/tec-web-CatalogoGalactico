using CatalogoGalactico.Entities;

namespace CatalogoGalactico.Dtos;

/// <summary>Convierte entidades (lo que vive en la base) en DTOs (lo que sale por la API).</summary>
public static class Mapeos
{
    public static PersonajeRespuesta ADto(this Personaje p) =>
        new(p.Id, p.Nombre, p.Especie, p.Faccion, p.Afiliacion, p.Estado, p.FuerzaSensitivo, p.Image);

    public static CardRespuesta ADto(this CardPersonaje c) =>
        new(c.Id, c.PersonajeId, c.Poder, c.HabilidadEspecial, c.Arma, c.NivelPeligrosidad, c.ImagenUrl);

    /// <summary>Requiere que Participantes esté cargado (Include).</summary>
    public static EventoRespuesta ADto(this Evento e) =>
        new(e.Id, e.Nombre, e.Fecha, e.Anio, e.Ubicacion, e.Descripcion,
            e.Participantes.Select(x => x.PersonajeId).OrderBy(x => x).ToList(),
            e.Participantes.Where(x => x.Murio).Select(x => x.PersonajeId).OrderBy(x => x).ToList(),
            e.Ganador);
}
