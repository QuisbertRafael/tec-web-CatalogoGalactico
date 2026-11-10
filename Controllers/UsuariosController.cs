using CatalogoGalactico.Dtos;
using CatalogoGalactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalactico.Controllers;

/// <summary>
/// Usuarios del catálogo. La contraseña se guarda como hash (PasswordHasher de ASP.NET Core Identity) y el email cifrado con AES-GCM;
/// la API nunca devuelve la contraseña ni su hash, y entrega el email descifrado.
/// </summary>
[Route("usuarios")]
public class UsuariosController(IUsuarioService usuarios) : ApiControllerBase
{
    /// <summary>Lista los usuarios (sin contraseña).</summary>
    [HttpGet]
    [ProducesResponseType<List<UsuarioRespuesta>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar() => Ok(await usuarios.ListarAsync());

    /// <summary>Obtiene un usuario por id.</summary>
    /// <response code="404">No existe el usuario.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<UsuarioRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obtener(int id) => Responder(await usuarios.ObtenerAsync(id));

    /// <summary>Registra un usuario.</summary>
    /// <remarks>
    /// La contraseña (mínimo 8 caracteres) se guarda como hash (PasswordHasher de ASP.NET Core Identity). El email se guarda cifrado.
    ///
    ///     {
    ///       "nombreUsuario": "rafa",
    ///       "email": "rafa@example.com",
    ///       "password": "ClaveSegura123"
    ///     }
    /// </remarks>
    /// <response code="201">Usuario creado.</response>
    /// <response code="409">El nombre de usuario ya existe.</response>
    [HttpPost]
    [ProducesResponseType<UsuarioRespuesta>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Crear([FromBody] UsuarioRequest request) =>
        Responder(await usuarios.CrearAsync(request), u => $"/usuarios/{u.Id}");

    /// <summary>Elimina un usuario.</summary>
    /// <response code="204">Eliminado.</response>
    /// <response code="404">No existe el usuario.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id) => ResponderSinContenido(await usuarios.EliminarAsync(id));
}
