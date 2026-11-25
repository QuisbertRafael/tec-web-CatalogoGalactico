using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CatalogoGalactico.Middleware;

/// <summary>Registra cada petición a los controladores del catálogo.</summary>
public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>() is null)
        {
            await next(context);
            return;
        }

        var cid = Guid.NewGuid().ToString("N");
        context.Response.Headers["X-Correlation-Id"] = cid;
        using var scope = logger.BeginScope("cid={CorrelationId}", cid);
        var reloj = Stopwatch.StartNew();
        var metodo = context.Request.Method;
        // Usamos la plantilla de ruta, no el cuerpo ni los parámetros de consulta.
        var ruta = (context.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText ?? "(controlador)";
        var estado = StatusCodes.Status500InternalServerError;
        try
        {
            logger.LogInformation("Inicio {Metodo} {Ruta}", metodo, ruta);
            await next(context);
            estado = context.Response.StatusCode;
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            estado = 499; // marcador de log: cliente canceló; no es una respuesta enviada.
            logger.LogWarning("El cliente canceló la petición");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError("Excepción no controlada: {Tipo}", ex.GetType().Name);
            throw;
        }
        finally
        {
            reloj.Stop();
            var nivel = estado >= 500 ? LogLevel.Error : estado >= 400 ? LogLevel.Warning : LogLevel.Information;
            logger.Log(nivel, "Fin {Metodo} {Ruta} | estado={Estado} | duracion={DuracionMs} ms",
                metodo, ruta, estado, reloj.ElapsedMilliseconds);
        }
    }
}
