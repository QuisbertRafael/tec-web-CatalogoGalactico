using CatalogoGalactico.Entities;

namespace CatalogoGalactico.Dtos;

/// <summary>Experiencia ganada en un evento concreto.</summary>
public record XpEvento(int EventoId, string Evento, string Fecha, int Xp, string Motivo);

/// <summary>Progresión completa de una carta.</summary>
public record ProgresionCarta(
    int CartaId,
    int PersonajeId,
    string Personaje,
    Faccion Faccion,
    int XpTotal,
    int Nivel,
    string Rango,
    int PoderBase,
    int PoderEfectivo,
    int PeligrosidadEfectiva,
    int? XpParaSiguienteNivel,
    List<XpEvento> Eventos);

/// <summary>Poder de una carta en una fecha de la historia.</summary>
public record PoderEnFecha(
    int CartaId,
    int PersonajeId,
    string Personaje,
    string Fecha,
    int Anio,
    int XpAcumulada,
    int Nivel,
    string Rango,
    int PoderBase,
    int PoderEfectivo);
