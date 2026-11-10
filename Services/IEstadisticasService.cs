using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public interface IEstadisticasService
{
    Task<Resultado<List<RankingItem>>> RankingAsync(string por);
    Task<Resultado<MvpRespuesta>> MvpAsync(int eventoId);
    Task<Resultado<SimulacionRespuesta>> SimularAsync(int eventoId, int? semilla);
}
