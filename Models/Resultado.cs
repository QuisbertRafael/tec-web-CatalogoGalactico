namespace CatalogoGalactico.Models;

public record Resultado<T>(T? Valor, int Codigo, string? Error)
{
    public bool EsOk => Error is null;

    public static Resultado<T> Ok(T valor, int codigo = 200) => new(valor, codigo, null);

    public static Resultado<T> Falla(string error, int codigo = 400) => new(default, codigo, error);
}
