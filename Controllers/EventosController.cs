using CatalogoGalactico.Dtos;
using CatalogoGalactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalactico.Controllers;

/// <summary>Eventos del universo, sus participantes y la simulación de batallas.</summary>
[Route("eventos")]
public class EventosController(IEventoService eventos, IEstadisticasService estadisticas) : ApiControllerBase
{
    /// <summary>Lista eventos en orden cronológico (BBY a ABY).</summary>
    /// <param name="nombre">Texto contenido en el nombre.</param>
    /// <param name="ubicacion">Texto contenido en la ubicación.</param>
    /// <param name="personajeId">Solo eventos donde participa ese personaje.</param>
    [HttpGet]
    [ProducesResponseType<List<EventoRespuesta>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] string? nombre, [FromQuery] string? ubicacion, [FromQuery] int? personajeId) =>
        Ok(await eventos.ListarAsync(nombre, ubicacion, personajeId));

    /// <summary>Obtiene un evento por id.</summary>
    /// <response code="404">No existe el evento.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<EventoRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obtener(int id) => Responder(await eventos.ObtenerAsync(id));

    /// <summary>Participante con mayor poder del evento.</summary>
    /// <remarks>Desempata por peligrosidad y luego por id. Falla con 400 si ningún participante tiene carta.</remarks>
    /// <response code="404">No existe el evento.</response>
    [HttpGet("{id:int}/mvp")]
    [ProducesResponseType<MvpRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Mvp(int id) => Responder(await estadisticas.MvpAsync(id));

    /// <summary>Crea un evento.</summary>
    /// <remarks>
    /// "fecha" acepta "10 BBY", "3 ABY" o "Batalla de Yavin" y se guarda como año numérico (-10, 3, 0).
    ///
    /// Reglas: todos los participantes deben existir; "muertes" es un subconjunto de "participantes" y esos personajes
    /// pasan a estado Muerto; un personaje muere una sola vez y no puede participar en eventos posteriores a su muerte (409).
    ///
    ///     {
    ///       "nombre": "Batalla de Jakku",
    ///       "fecha": "29 ABY",
    ///       "ubicacion": "Jakku",
    ///       "descripcion": "Último enfrentamiento entre la Nueva República y los restos del Imperio.",
    ///       "participantes": [2, 3, 8],
    ///       "muertes": [],
    ///       "ganador": null
    ///     }
    /// </remarks>
    /// <response code="201">Evento creado.</response>
    /// <response code="409">Un personaje participaría después de su muerte.</response>
    [HttpPost]
    [ProducesResponseType<EventoRespuesta>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Crear([FromBody] EventoRequest request) =>
        Responder(await eventos.CrearAsync(request), e => $"/eventos/{e.Id}");

    /// <summary>Reemplaza los datos de un evento.</summary>
    /// <remarks>Mismas reglas que la creación. Si se quita una muerte y el personaje no muere en otro evento, vuelve a Vivo.</remarks>
    /// <response code="404">No existe el evento.</response>
    /// <response code="409">Un personaje participaría después de su muerte.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType<EventoRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] EventoRequest request) =>
        Responder(await eventos.ActualizarAsync(id, request));

    /// <summary>Simula la batalla del evento (Rebelde contra Imperio).</summary>
    /// <param name="semilla">Opcional. La misma semilla da el mismo resultado; la respuesta siempre informa la usada.</param>
    /// <remarks>
    /// Suma la fuerza de cada bando con las cartas de los participantes y aplica un factor aleatorio entre 0.90 y 1.10.
    /// Los neutrales no suman. El ganador se guarda solo si la simulación fue válida: devuelve 400 si hay menos de 2
    /// participantes, faltan cartas o no hay al menos un Rebelde y un Imperial.
    ///
    /// Ejemplo: POST /eventos/5/simular?semilla=42
    /// </remarks>
    /// <response code="404">No existe el evento.</response>
    [HttpPost("{id:int}/simular")]
    [ProducesResponseType<SimulacionRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Simular(int id, [FromQuery] int? semilla) =>
        Responder(await estadisticas.SimularAsync(id, semilla));
}
