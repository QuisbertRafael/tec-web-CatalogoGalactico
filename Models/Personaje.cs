namespace CatalogoGalactico.Models;

public enum Faccion { Rebelde, Imperio, Neutral }

public enum EstadoPersonaje { Vivo, Muerto, Desconocido }

public record Personaje(
    int Id,
    string Nombre,
    string Especie,
    Faccion Faccion,
    string Afiliacion,
    EstadoPersonaje Estado,
    bool FuerzaSensitivo,
    string Image);
