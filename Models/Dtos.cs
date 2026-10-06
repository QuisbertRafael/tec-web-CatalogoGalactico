using System.ComponentModel.DataAnnotations;

namespace CatalogoGalactico.Models;

// Solicitudes

public record PersonajeRequest(
    [property: Required, StringLength(80, MinimumLength = 2)] string Nombre,
    [property: Required, StringLength(50, MinimumLength = 2)] string Especie,
    Faccion Faccion,
    [property: Required, StringLength(80, MinimumLength = 2)] string Afiliacion,
    EstadoPersonaje Estado,
    bool FuerzaSensitivo,
    //Tarea 1, se agregó el image
    string? Image = null);

public record CardRequest(
    int PersonajeId,
    [property: Range(1, 100)] int Poder,
    [property: Required, StringLength(100)] string HabilidadEspecial,
    [property: Required, StringLength(100)] string Arma,
    [property: Range(1, 5)] int NivelPeligrosidad,
    [property: Required] string ImagenUrl);

public record EventoRequest(
    [property: Required, StringLength(120, MinimumLength = 2)] string Nombre,
    [property: Required] string Fecha,
    [property: Required, StringLength(120)] string Ubicacion,
    [property: Required, StringLength(500)] string Descripcion,
    List<int> Participantes,
    List<int>? Muertes,
    string? Ganador);

// Respuestas

public record RankingItem(int Posicion, int PersonajeId, string Nombre, Faccion Faccion, int Valor);

public record MvpRespuesta(int EventoId, string Evento, Personaje Personaje, CardPersonaje Carta);

public record ParticipanteFuerza(int PersonajeId, string Nombre, int Fuerza);

public record BandoResultado(
    string Bando,
    List<ParticipanteFuerza> Participantes,
    int FuerzaBase,
    double FactorAleatorio,
    double FuerzaFinal);

public record SimulacionRespuesta(
    int EventoId,
    string Evento,
    int Semilla,
    BandoResultado Rebelde,
    BandoResultado Imperio,
    List<string> NeutralesIgnorados,
    string Ganador,
    string Criterio,
    int? GanadorId,
    string Resultado);

//tarea 2
public record PersonajeConCard(
    int Id,
    string Nombre,
    string Especie,
    Faccion Faccion,
    string Afiliacion,
    EstadoPersonaje Estado,
    bool FuerzaSensitivo,
    string image,
    CardPersonaje? Card
);