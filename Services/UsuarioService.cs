using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Entities;
using CatalogoGalactico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

public class UsuarioService(CatalogoDbContext db, IPasswordHasher<Usuario> hasher, ICifradoService cifrado)
    : IUsuarioService
{
    public async Task<List<UsuarioRespuesta>> ListarAsync()
    {
        var usuarios = await db.Usuarios.AsNoTracking().OrderBy(u => u.Id).ToListAsync();
        return usuarios.Select(ADto).ToList();
    }

    public async Task<Resultado<UsuarioRespuesta>> ObtenerAsync(int id)
    {
        var u = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return u is null
            ? Resultado<UsuarioRespuesta>.Falla($"No existe el usuario con id {id}.", 404)
            : Resultado<UsuarioRespuesta>.Ok(ADto(u));
    }

    public async Task<Resultado<UsuarioRespuesta>> CrearAsync(UsuarioRequest r)
    {
        var nombre = r.NombreUsuario.Trim();
        if (await db.Usuarios.AnyAsync(u => u.NombreUsuario == nombre))
            return Resultado<UsuarioRespuesta>.Falla("Ya existe un usuario con ese nombre de usuario.", 409);

        var usuario = new Usuario
        {
            NombreUsuario = nombre,
            Email = cifrado.Cifrar(r.Email.Trim()) // campo sensible: se guarda cifrado (AES-GCM)
        };
        // La contraseña nunca se guarda: solo su hash con sal (PBKDF2 de ASP.NET Core Identity).
        usuario.PasswordHash = hasher.HashPassword(usuario, r.Password);

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        return Resultado<UsuarioRespuesta>.Ok(ADto(usuario), 201);
    }

    public async Task<Resultado<bool>> EliminarAsync(int id)
    {
        var u = await db.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
        if (u is null) return Resultado<bool>.Falla($"No existe el usuario con id {id}.", 404);

        db.Usuarios.Remove(u);
        await db.SaveChangesAsync();
        return Resultado<bool>.Ok(true);
    }

    /// <summary>La API entrega el email descifrado y nunca el hash.</summary>
    private UsuarioRespuesta ADto(Usuario u) => new(u.Id, u.NombreUsuario, cifrado.Descifrar(u.Email));
}
