using CatalogoGalactico.Dtos;
using CatalogoGalactico.Entities;
using CatalogoGalactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalactico.Controllers;

/// <summary>Administración de personajes y consultas que los relacionan con cartas y eventos.</summary>
[Route("personajes")]
public class PersonajesController(
    IPersonajeService personajes,
    IEstadisticasService estadisticas,
    IEventoService eventos) : ApiControllerBase
{
    /// <summary>Lista personajes con filtros opcionales combinables.</summary>
    /// <param name="faccion">Rebelde, Imperio o Neutral.</param>
    /// <param name="fuerzaSensitivo">true o false.</param>
    /// <param name="estado">Vivo, Muerto o Desconocido.</param>
    /// <param name="nombre">Texto contenido en el nombre.</param>
    /// <remarks>Ejemplo: GET /personajes?faccion=Imperio&amp;fuerzaSensitivo=true</remarks>
    [HttpGet]
    [ProducesResponseType<List<PersonajeRespuesta>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] Faccion? faccion,
        [FromQuery] bool? fuerzaSensitivo,
        [FromQuery] EstadoPersonaje? estado,
        [FromQuery] string? nombre) =>
        Ok(await personajes.ListarAsync(faccion, fuerzaSensitivo, estado, nombre));

    /// <summary>Ranking de personajes según su carta.</summary>
    /// <param name="por">poder (por defecto), peligrosidad o experiencia.</param>
    /// <remarks>Ejemplo: GET /personajes/ranking?por=poder</remarks>
    [HttpGet("ranking")]
    [ProducesResponseType<List<RankingItem>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Ranking([FromQuery] string? por) =>
        Responder(await estadisticas.RankingAsync(por ?? "poder"));

    /// <summary>Obtiene un personaje por id.</summary>
    /// <response code="404">No existe el personaje.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<PersonajeRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obtener(int id) => Responder(await personajes.ObtenerAsync(id));

    /// <summary>Ficha de combate: el personaje junto con su carta.</summary>
    /// <remarks>Combina dos tablas. El campo card es null si el personaje no tiene carta.</remarks>
    /// <response code="404">No existe el personaje.</response>
    [HttpGet("{id:int}/con-card")]
    [ProducesResponseType<PersonajeConCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConCard(int id) => Responder(await personajes.ConCardAsync(id));

    /// <summary>Eventos en los que participa un personaje, en orden cronológico.</summary>
    /// <response code="404">No existe el personaje.</response>
    [HttpGet("{id:int}/eventos")]
    [ProducesResponseType<List<EventoRespuesta>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EventosDe(int id) => Responder(await eventos.PorPersonajeAsync(id));

    /// <summary>Crea un personaje.</summary>
    /// <remarks>
    /// Ejemplo:
    ///
    ///     {
    ///       "nombre": "Ahsoka Tano",
    ///       "especie": "Togruta",
    ///       "faccion": "Rebelde",
    ///       "afiliacion": "Rebeldes de Lothal",
    ///       "estado": "Vivo",
    ///       "fuerzaSensitivo": true,
    ///       "image": "https://example.com/ahsoka.png"
    ///     }
    /// </remarks>
    /// <response code="201">Personaje creado (header Location con su ruta).</response>
    [HttpPost]
    [ProducesResponseType<PersonajeRespuesta>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear([FromBody] PersonajeRequest request) =>
        Responder(await personajes.CrearAsync(request), p => $"/personajes/{p.Id}");

    /// <summary>Reemplaza los datos de un personaje.</summary>
    /// <remarks>Si la muerte del personaje está consignada en un evento, su estado no puede cambiarse a mano (409).</remarks>
    /// <response code="404">No existe el personaje.</response>
    /// <response code="409">El estado contradice un evento donde murió.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType<PersonajeRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] PersonajeRequest request) =>
        Responder(await personajes.ActualizarAsync(id, request));

    /// <summary>Elimina un personaje (y su carta).</summary>
    /// <response code="204">Eliminado.</response>
    /// <response code="404">No existe el personaje.</response>
    /// <response code="409">El personaje participa en algún evento.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id) => ResponderSinContenido(await personajes.EliminarAsync(id));
}
