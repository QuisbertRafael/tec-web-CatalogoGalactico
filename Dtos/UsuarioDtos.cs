
using System.ComponentModel.DataAnnotations;

namespace CatalogoGalactico.Dtos;

/// <summary>
/// Datos para registrar un usuario.
/// La contraseña se guarda como hash (PasswordHasher de ASP.NET Core Identity).
/// </summary>
public record UsuarioRequest(
    [param: Required, StringLength(50, MinimumLength = 3)]
    string NombreUsuario,

    [param: Required, EmailAddress, StringLength(200)]
    string Email,

    [param: Required, StringLength(100, MinimumLength = 8)]
    string Password
);

/// <summary>
/// Respuesta pública: nunca incluye la contraseña ni su hash.
/// </summary>
public record UsuarioRespuesta(
    int Id,
    string NombreUsuario,
    string Email
);
