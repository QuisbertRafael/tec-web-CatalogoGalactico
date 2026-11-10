using CatalogoGalactico.Dtos;
using CatalogoGalactico.Entities;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public interface IPersonajeService
{
    Task<List<PersonajeRespuesta>> ListarAsync(Faccion? faccion, bool? fuerzaSensitivo, EstadoPersonaje? estado, string? nombre);
    Task<Resultado<PersonajeRespuesta>> ObtenerAsync(int id);
    Task<Resultado<PersonajeConCard>> ConCardAsync(int id);
    Task<Resultado<PersonajeRespuesta>> CrearAsync(PersonajeRequest r);
    Task<Resultado<PersonajeRespuesta>> ActualizarAsync(int id, PersonajeRequest r);
    Task<Resultado<bool>> EliminarAsync(int id);
}
