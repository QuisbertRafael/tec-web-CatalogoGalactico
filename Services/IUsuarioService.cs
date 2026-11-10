using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public interface IUsuarioService
{
    Task<List<UsuarioRespuesta>> ListarAsync();
    Task<Resultado<UsuarioRespuesta>> ObtenerAsync(int id);
    Task<Resultado<UsuarioRespuesta>> CrearAsync(UsuarioRequest r);
    Task<Resultado<bool>> EliminarAsync(int id);
}
