namespace CatalogoGalactico.Entities;

/// <summary>
/// Usuario del catálogo. PasswordHash guarda el hash de ASP.NET Core Identity (nunca la contraseña).
/// Email se guarda cifrado en la base de datos (AES-GCM) y la API lo entrega descifrado.
/// </summary>
public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}
