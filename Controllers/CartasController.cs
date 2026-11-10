using CatalogoGalactico.Dtos;
using CatalogoGalactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalactico.Controllers;

/// <summary>Cartas coleccionables de los personajes (relación 1:1).</summary>
[Route("cartas")]
public class CartasController(ICardService cartas, IProgresionService progresion) : ApiControllerBase
{
    /// <summary>Lista cartas con filtros opcionales combinables.</summary>
    /// <param name="personajeId">Carta de un personaje concreto.</param>
    /// <param name="poderMin">Poder mayor o igual al valor.</param>
    /// <param name="peligrosidad">Nivel de peligrosidad exacto (1 a 5).</param>
    [HttpGet]
    [ProducesResponseType<List<CardRespuesta>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] int? personajeId, [FromQuery] int? poderMin, [FromQuery] int? peligrosidad) =>
        Ok(await cartas.ListarAsync(personajeId, poderMin, peligrosidad));

    /// <summary>Obtiene una carta por id.</summary>
    /// <response code="404">No existe la carta.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<CardRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obtener(int id) => Responder(await cartas.ObtenerAsync(id));

    /// <summary>Crea la carta de un personaje.</summary>
    /// <remarks>
    /// Reglas: el personaje debe existir y no tener ya una carta (409); poder entre 1 y 100;
    /// nivelPeligrosidad entre 1 y 5; imagenUrl http/https válida.
    ///
    ///     {
    ///       "personajeId": 12,
    ///       "poder": 72,
    ///       "habilidadEspecial": "Combate con dos sables",
    ///       "arma": "Sables de luz blancos",
    ///       "nivelPeligrosidad": 4,
    ///       "imagenUrl": "https://example.com/cartas/ahsoka.jpg"
    ///     }
    /// </remarks>
    /// <response code="201">Carta creada.</response>
    /// <response code="409">El personaje ya tiene una carta.</response>
    [HttpPost]
    [ProducesResponseType<CardRespuesta>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Crear([FromBody] CardRequest request) =>
        Responder(await cartas.CrearAsync(request), c => $"/cartas/{c.Id}");

    /// <summary>Reemplaza los datos de una carta.</summary>
    /// <response code="404">No existe la carta.</response>
    /// <response code="409">El personaje destino ya tiene otra carta.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType<CardRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CardRequest request) =>
        Responder(await cartas.ActualizarAsync(id, request));

    /// <summary>Elimina una carta.</summary>
    /// <response code="204">Eliminada.</response>
    /// <response code="404">No existe la carta.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id) => ResponderSinContenido(await cartas.EliminarAsync(id));

    /// <summary>Progresión de la carta: XP total, nivel, rango, poder base y efectivo, y desglose por evento.</summary>
    /// <remarks>El poder base nunca cambia; el poder efectivo suma un bonus por nivel según la XP ganada en las simulaciones.</remarks>
    /// <response code="404">No existe la carta.</response>
    [HttpGet("{id:int}/progresion")]
    [ProducesResponseType<ProgresionCarta>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Progresion(int id) => Responder(await progresion.ProgresionAsync(id));

    /// <summary>Poder efectivo de la carta en una fecha de la historia.</summary>
    /// <param name="fecha">"10 BBY", "3 ABY" o "Batalla de Yavin". Solo cuenta la XP de eventos de años anteriores.</param>
    /// <remarks>Ejemplo: GET /cartas/1/poder?fecha=3 ABY</remarks>
    /// <response code="404">No existe la carta.</response>
    [HttpGet("{id:int}/poder")]
    [ProducesResponseType<PoderEnFecha>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PoderEnFecha(int id, [FromQuery] string fecha) =>
        Responder(await progresion.PoderEnFechaAsync(id, fecha));
}
