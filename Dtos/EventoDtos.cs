using System.ComponentModel.DataAnnotations;

namespace CatalogoGalactico.Dtos;

/// <summary>
/// Datos para crear o reemplazar un evento. "Fecha" acepta "10 BBY", "3 ABY" o "Batalla de Yavin".
/// "Muertes" debe ser un subconjunto de "Participantes".
/// </summary>
public record EventoRequest(
    [property: Required, StringLength(120, MinimumLength = 2)] string Nombre,
    [property: Required] string Fecha,
    [property: Required, StringLength(120)] string Ubicacion,
    [property: Required, StringLength(500)] string Descripcion,
    List<int> Participantes,
    List<int>? Muertes,
    string? Ganador);

public record EventoRespuesta(
    int Id,
    string Nombre,
    string Fecha,
    int Anio,
    string Ubicacion,
    string Descripcion,
    List<int> Participantes,
    List<int> Muertes,
    string? Ganador)
{
    /// <summary>Mismo contenido que Participantes, con el nombre que lee el frontend.</summary>
    public List<int> PersonajesParticipantesIds => Participantes;
}
