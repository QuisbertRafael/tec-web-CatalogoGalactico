using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public interface IEventoService
{
    Task<List<EventoRespuesta>> ListarAsync(string? nombre, string? ubicacion, int? personajeId);
    Task<Resultado<EventoRespuesta>> ObtenerAsync(int id);
    Task<Resultado<List<EventoRespuesta>>> PorPersonajeAsync(int personajeId);
    Task<Resultado<EventoRespuesta>> CrearAsync(EventoRequest r);
    Task<Resultado<EventoRespuesta>> ActualizarAsync(int id, EventoRequest r);
}
