namespace CatalogoGalactico.Services;

/// <summary>Cifra y descifra campos sensibles antes de guardarlos en la base de datos.</summary>
public interface ICifradoService
{
    string Cifrar(string textoPlano);
    string Descifrar(string textoCifrado);
}
