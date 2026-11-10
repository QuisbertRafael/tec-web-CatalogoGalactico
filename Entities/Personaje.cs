namespace CatalogoGalactico.Entities;

/// <summary>Personaje del universo. Tiene como máximo una carta (1:1) y varias participaciones en eventos.</summary>
public class Personaje
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Especie { get; set; } = "";
    public Faccion Faccion { get; set; }
    public string Afiliacion { get; set; } = "";
    public EstadoPersonaje Estado { get; set; }
    public bool FuerzaSensitivo { get; set; }
    public string Image { get; set; } = "";

    public CardPersonaje? Card { get; set; }
    public List<EventoParticipante> Participaciones { get; set; } = new();
}
