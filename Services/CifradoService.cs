using System.Security.Cryptography;
using System.Text;

namespace CatalogoGalactico.Services;

/// <summary>
/// Cifrado AES-256-GCM. La clave (32 bytes en Base64) viene de Secret Manager: Cifrado:Clave.
/// Formato guardado (Base64): nonce (12 bytes) + tag (16 bytes) + texto cifrado.
/// Cada valor usa un nonce aleatorio, así el mismo texto cifrado dos veces da resultados distintos.
/// Se registra como Singleton: no tiene estado que cambie, solo la clave leída al arrancar.
/// </summary>
public class CifradoService : ICifradoService
{
    private const int TamanoNonce = 12;
    private const int TamanoTag = 16;
    private readonly byte[] _clave;

    public CifradoService(IConfiguration configuration)
    {
        var claveBase64 = configuration["Cifrado:Clave"]
            ?? throw new InvalidOperationException(
                "Falta el secreto Cifrado:Clave. Genérala (32 bytes en Base64) y guárdala con dotnet user-secrets (ver README).");

        _clave = Convert.FromBase64String(claveBase64);
        if (_clave.Length != 32)
            throw new InvalidOperationException("La clave debe tener 32 bytes (256 bits) en Base64.");
    }

    public string Cifrar(string textoPlano)
    {
        var datos = Encoding.UTF8.GetBytes(textoPlano);
        var nonce = RandomNumberGenerator.GetBytes(TamanoNonce);
        var cifrado = new byte[datos.Length];
        var tag = new byte[TamanoTag];

        using var aes = new AesGcm(_clave, TamanoTag);
        aes.Encrypt(nonce, datos, cifrado, tag);

        var resultado = new byte[TamanoNonce + TamanoTag + cifrado.Length];
        Buffer.BlockCopy(nonce, 0, resultado, 0, TamanoNonce);
        Buffer.BlockCopy(tag, 0, resultado, TamanoNonce, TamanoTag);
        Buffer.BlockCopy(cifrado, 0, resultado, TamanoNonce + TamanoTag, cifrado.Length);
        return Convert.ToBase64String(resultado);
    }

    public string Descifrar(string textoCifrado)
    {
        var datos = Convert.FromBase64String(textoCifrado);
        var nonce = datos[..TamanoNonce];
        var tag = datos[TamanoNonce..(TamanoNonce + TamanoTag)];
        var cifrado = datos[(TamanoNonce + TamanoTag)..];
        var plano = new byte[cifrado.Length];

        using var aes = new AesGcm(_clave, TamanoTag);
        aes.Decrypt(nonce, cifrado, tag, plano);
        return Encoding.UTF8.GetString(plano);
    }
}
