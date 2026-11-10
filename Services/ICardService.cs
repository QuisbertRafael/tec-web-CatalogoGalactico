using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public interface ICardService
{
    Task<List<CardRespuesta>> ListarAsync(int? personajeId, int? poderMin, int? peligrosidad);
    Task<Resultado<CardRespuesta>> ObtenerAsync(int id);
    Task<Resultado<CardRespuesta>> CrearAsync(CardRequest r);
    Task<Resultado<CardRespuesta>> ActualizarAsync(int id, CardRequest r);
    Task<Resultado<bool>> EliminarAsync(int id);
}
