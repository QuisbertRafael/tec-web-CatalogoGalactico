using System.ComponentModel.DataAnnotations;

namespace CatalogoGalactico.Dtos;

/// <summary>Datos para crear o reemplazar la carta de un personaje.</summary>
public record CardRequest(
    int PersonajeId,
    [property: Range(1, 100)] int Poder,
    [property: Required, StringLength(100)] string HabilidadEspecial,
    [property: Required, StringLength(100)] string Arma,
    [property: Range(1, 5)] int NivelPeligrosidad,
    [property: Required, StringLength(500)] string ImagenUrl);

public record CardRespuesta(
    int Id,
    int PersonajeId,
    int Poder,
    string HabilidadEspecial,
    string Arma,
    int NivelPeligrosidad,
    string ImagenUrl);
