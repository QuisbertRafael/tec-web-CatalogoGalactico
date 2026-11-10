namespace CatalogoGalactico.Entities;

/// Hecho narrativo. <c>Anio</c> es el año uniforme (BBY negativo, ABY positivo, Yavin = 0);
/// <c>Fecha</c> es su texto ("10 BBY", "3 ABY", "Batalla de Yavin").

public class Evento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Fecha { get; set; } = "";
    public int Anio { get; set; }
    public string Ubicacion { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string? Ganador { get; set; }

    public List<EventoParticipante> Participantes { get; set; } = new();
}
