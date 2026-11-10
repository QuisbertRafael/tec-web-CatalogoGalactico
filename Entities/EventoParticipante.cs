namespace CatalogoGalactico.Entities;

/// <summary>
/// Tabla intermedia de la relación N:N entre Evento y Personaje.
/// Evento 1:N EventoParticipante y Personaje 1:N EventoParticipante.
/// </summary>
public class EventoParticipante
{
    public int EventoId { get; set; }
    public int PersonajeId { get; set; }

    /// <summary>true si el personaje muere en este evento.</summary>
    public bool Murio { get; set; }

    /// <summary>Experiencia que ganó el personaje en este evento (la fija la simulación).</summary>
    public int XpOtorgada { get; set; }

    /// <summary>Desglose de por qué ganó esa experiencia.</summary>
    public string? XpMotivo { get; set; }

    public Evento Evento { get; set; } = null!;
    public Personaje Personaje { get; set; } = null!;
}
