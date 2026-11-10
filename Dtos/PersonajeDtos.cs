using System.ComponentModel.DataAnnotations;
using CatalogoGalactico.Entities;

namespace CatalogoGalactico.Dtos;

/// <summary>Datos para crear o reemplazar un personaje.</summary>
public record PersonajeRequest(
    [property: Required, StringLength(80, MinimumLength = 2)] string Nombre,
    [property: Required, StringLength(50, MinimumLength = 2)] string Especie,
    Faccion Faccion,
    [property: Required, StringLength(80, MinimumLength = 2)] string Afiliacion,
    EstadoPersonaje Estado,
    bool FuerzaSensitivo,
    [property: StringLength(500)] string? Image = null);

public record PersonajeRespuesta(
    int Id,
    string Nombre,
    string Especie,
    Faccion Faccion,
    string Afiliacion,
    EstadoPersonaje Estado,
    bool FuerzaSensitivo,
    string Image);

/// <summary>Ficha de combate: el personaje junto con su carta (null si no tiene).</summary>
public record PersonajeConCard(
    int Id,
    string Nombre,
    string Especie,
    Faccion Faccion,
    string Afiliacion,
    EstadoPersonaje Estado,
    bool FuerzaSensitivo,
    string Image,
    CardRespuesta? Card);
