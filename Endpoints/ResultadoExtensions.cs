using CatalogoGalactico.Models;

namespace CatalogoGalactico.Endpoints;

public static class ResultadoExtensions
{
    /// <summary>Traduce un Resultado a HTTP: 200/201 con el valor o ProblemDetails con el código de error.</summary>
    public static IResult ToHttp<T>(this Resultado<T> r, Func<T, string>? ubicacion = null)
    {
        if (!r.EsOk)
        {
            return Results.Problem(
                title: r.Codigo == 404 ? "Recurso no encontrado" : "Solicitud inválida",
                detail: r.Error,
                statusCode: r.Codigo);
        }

        return r.Codigo == 201 && ubicacion is not null
            ? Results.Created(ubicacion(r.Valor!), r.Valor)
            : Results.Ok(r.Valor);
    }
}
