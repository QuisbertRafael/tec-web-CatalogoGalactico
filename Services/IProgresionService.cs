using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public interface IProgresionService
{
    Task<Dictionary<int, int>> XpTotalAsync();
    Task<Dictionary<int, int>> XpPreviaAsync(IEnumerable<int> personajeIds, int anio);
    Task<Resultado<ProgresionCarta>> ProgresionAsync(int cartaId);
    Task<Resultado<PoderEnFecha>> PoderEnFechaAsync(int cartaId, string fecha);
}
