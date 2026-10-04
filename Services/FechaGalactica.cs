namespace CatalogoGalactico.Services;

public static class FechaGalactica
{
    public const string Yavin = "Batalla de Yavin";

    public static bool TryParse(string? texto, out int anio)
    {
        anio = 0;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        var t = texto.Trim();
        if (t.Equals(Yavin, StringComparison.OrdinalIgnoreCase)) return true;

        var partes = t.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length != 2 || !int.TryParse(partes[0], out var n) || n < 0) return false;

        if (partes[1].Equals("BBY", StringComparison.OrdinalIgnoreCase)) { anio = -n; return true; }
        if (partes[1].Equals("ABY", StringComparison.OrdinalIgnoreCase)) { anio = n; return true; }
        return false;
    }

    public static string Formatear(int anio) => anio switch
    {
        0 => Yavin,
        < 0 => $"{-anio} BBY",
        _ => $"{anio} ABY"
    };
}
