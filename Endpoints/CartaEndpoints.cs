using CatalogoGalactico.Models;
using CatalogoGalactico.Services;

namespace CatalogoGalactico.Endpoints;

public static class CartaEndpoints
{
    public static IEndpointRouteBuilder MapCartaEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/cartas").WithTags("Cartas");

        g.MapGet("/", (int? personajeId, int? poderMin, int? peligrosidad, CardService s) =>
                Results.Ok(s.Listar(personajeId, poderMin, peligrosidad)))
            .WithName("ListarCartas")
            .WithSummary("Lista cartas con filtros opcionales")
            .WithDescription("""
                Filtros opcionales combinables:

                - `personajeId`: carta de un personaje concreto
                - `poderMin`: poder mayor o igual al valor indicado
                - `peligrosidad`: nivel de peligrosidad exacto (1 a 5)
                """)
            .Produces<List<CardPersonaje>>();

        g.MapGet("/{id:int}", (int id, CardService s) => s.Obtener(id).ToHttp())
            .WithName("ObtenerCarta")
            .WithSummary("Obtiene una carta por id")
            .Produces<CardPersonaje>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        g.MapPost("/", (CardRequest r, CardService s) => s.Crear(r).ToHttp(c => $"/cartas/{c.Id}"))
            .WithName("CrearCarta")
            .WithSummary("Crea la carta de un personaje")
            .WithDescription("""
                **Reglas:** el personaje debe existir y no tener ya una carta (relación 1:1);
                `poder` entre 1 y 100; `nivelPeligrosidad` entre 1 y 5; `imagenUrl` http/https válida.

                ```json
                {
                  "personajeId": 10,
                  "poder": 72,
                  "habilidadEspecial": "Combate con dos sables",
                  "arma": "Sables de luz blancos",
                  "nivelPeligrosidad": 4,
                  "imagenUrl": "https://example.com/cartas/ahsoka.jpg"
                }
                ```
                """)
            .Produces<CardPersonaje>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        g.MapPut("/{id:int}", (int id, CardRequest r, CardService s) => s.Actualizar(id, r).ToHttp())
            .WithName("ActualizarCarta")
            .WithSummary("Reemplaza los datos de una carta")
            .WithDescription("Aplica las mismas reglas que la creación. No se puede asignar la carta a un personaje que ya tiene otra.")
            .Produces<CardPersonaje>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
