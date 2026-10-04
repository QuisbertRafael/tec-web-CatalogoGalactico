using CatalogoGalactico.Models;
using CatalogoGalactico.Services;

namespace CatalogoGalactico.Endpoints;

public static class EventoEndpoints
{
    public static IEndpointRouteBuilder MapEventoEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/eventos").WithTags("Eventos");

        g.MapGet("/", (string? nombre, string? ubicacion, int? personajeId, EventoService s) =>
                Results.Ok(s.Listar(nombre, ubicacion, personajeId)))
            .WithName("ListarEventos")
            .WithSummary("Lista eventos en orden cronológico")
            .WithDescription("""
                Filtros opcionales combinables: `nombre`, `ubicacion` (texto contenido) y `personajeId` (eventos donde participa).
                Se ordenan por `anio` (BBY negativo, Yavin = 0, ABY positivo).
                """)
            .Produces<List<Evento>>();

        g.MapGet("/{id:int}", (int id, EventoService s) => s.Obtener(id).ToHttp())
            .WithName("ObtenerEvento")
            .WithSummary("Obtiene un evento por id")
            .Produces<Evento>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        g.MapGet("/{id:int}/mvp", (int id, EstadisticasService s) => s.Mvp(id).ToHttp())
            .WithName("MvpDeEvento")
            .WithSummary("Participante con mayor poder del evento")
            .WithDescription("Compara el `poder` de las cartas de los participantes. Desempata por peligrosidad y luego por id. Falla con 400 si ningún participante tiene carta.")
            .Produces<MvpRespuesta>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        g.MapPost("/", (EventoRequest r, EventoService s) => s.Crear(r).ToHttp(e => $"/eventos/{e.Id}"))
            .WithName("CrearEvento")
            .WithSummary("Crea un evento")
            .WithDescription("""
                `fecha` acepta `"10 BBY"`, `"3 ABY"` o `"Batalla de Yavin"` y se guarda como año numérico (`-10`, `3`, `0`).

                **Reglas de consistencia:**
                - Todos los participantes deben existir.
                - `muertes` debe ser subconjunto de `participantes`; esos personajes pasan a estado **Muerto** automáticamente.
                - Un personaje muere una sola vez y no puede participar en eventos posteriores a su muerte.

                ```json
                {
                  "nombre": "Batalla de Jakku",
                  "fecha": "29 ABY",
                  "ubicacion": "Jakku",
                  "descripcion": "Último enfrentamiento entre la Nueva República y los restos del Imperio.",
                  "participantes": [2, 3, 8],
                  "muertes": [],
                  "ganador": null
                }
                ```
                """)
            .Produces<Evento>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        g.MapPut("/{id:int}", (int id, EventoRequest r, EventoService s) => s.Actualizar(id, r).ToHttp())
            .WithName("ActualizarEvento")
            .WithSummary("Reemplaza los datos de un evento")
            .WithDescription("Aplica las mismas reglas que la creación. Si se quita una muerte de `muertes` y el personaje no muere en otro evento, vuelve a estado **Vivo**.")
            .Produces<Evento>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        g.MapPost("/{id:int}/simular", (int id, int? semilla, EstadisticasService s) => s.Simular(id, semilla).ToHttp())
            .WithName("SimularEvento")
            .WithSummary("Simula la batalla del evento")
            .WithDescription("""
                Suma la fuerza de cada bando (Rebelde vs Imperio) usando las cartas de los participantes y aplica un
                factor aleatorio acotado entre 0.90 y 1.10. Los neutrales no suman.

                - `fuerza individual = poder + peligrosidad x 10 + 15 si es sensible a la Fuerza`
                - `semilla` (opcional): misma semilla = mismo resultado. La respuesta siempre informa la semilla usada.

                El ganador se guarda en el evento **solo si la simulación fue válida**. Devuelve 400 si hay menos de
                2 participantes, faltan cartas o no hay al menos un Rebelde y un Imperial.

                Ejemplo: `POST /eventos/5/simular?semilla=42`
                """)
            .Produces<SimulacionRespuesta>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
