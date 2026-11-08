namespace CatalogoGalactico.Models;


public record Evento(
    int Id,
    string Nombre,
    string Fecha,
    int Anio,
    string Ubicacion,
    string Descripcion,
    List<int> Participantes,
    List<int> Muertes,
    string? Ganador
    )
    
    {
            public List<int> PersonajesParticipantesIds => Participantes;

    };
