using CatalogoGalactico.Entities;

namespace CatalogoGalactico.Dtos;

public record RankingItem(int Posicion, int PersonajeId, string Nombre, Faccion Faccion, int Valor);

public record MvpRespuesta(int EventoId, string Evento, PersonajeRespuesta Personaje, CardRespuesta Carta);

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
