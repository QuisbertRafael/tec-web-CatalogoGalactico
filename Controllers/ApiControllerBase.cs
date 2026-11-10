using CatalogoGalactico.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalactico.Controllers;

/// <summary>
/// Base de todos los controladores. Traduce el Resultado que devuelven los servicios
/// a una respuesta HTTP: 200, 201 (con Location), 400, 404 o 409 (ProblemDetails).
/// </summary>
[ApiController]
[Produces("application/json")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult Responder<T>(Resultado<T> resultado, Func<T, string>? ubicacion = null)
    {
        if (!resultado.EsOk)
        {
            var titulo = resultado.Codigo switch
            {
                404 => "Recurso no encontrado",
                409 => "Conflicto con el estado actual",
                _ => "Solicitud inválida"
            };
            return Problem(title: titulo, detail: resultado.Error, statusCode: resultado.Codigo);
        }

        if (resultado.Codigo == 201 && ubicacion is not null)
            return Created(ubicacion(resultado.Valor!), resultado.Valor);

        return Ok(resultado.Valor);
    }

    /// <summary>Para DELETE: 204 sin cuerpo si salió bien.</summary>
    protected IActionResult ResponderSinContenido(Resultado<bool> resultado) =>
        resultado.EsOk ? NoContent() : Responder(resultado);
}
