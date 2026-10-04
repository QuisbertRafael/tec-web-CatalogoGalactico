using CatalogoGalactico.Models;
using CatalogoGalactico.Services;

namespace CatalogoGalactico.Endpoints;

public static class PersonajeEndpoints
{
    public static IEndpointRouteBuilder MapPersonajeEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/personajes").WithTags("Personajes");

        g.MapGet("/", (Faccion? faccion, bool? fuerzaSensitivo, EstadoPersonaje? estado, string? nombre, PersonajeService s) =>
                Results.Ok(s.Listar(faccion, fuerzaSensitivo, estado, nombre)))
            .WithName("ListarPersonajes")
            .WithSummary("Lista personajes con filtros opcionales")
            .WithDescription("""
                Todos los filtros son opcionales y **se combinan con AND**.

                - `faccion`: `Rebelde`, `Imperio` o `Neutral`
                - `fuerzaSensitivo`: `true` o `false`
                - `estado`: `Vivo`, `Muerto` o `Desconocido`
                - `nombre`: texto contenido en el nombre (sin distinguir mayúsculas)

                Ejemplo: `GET /personajes?faccion=Imperio&fuerzaSensitivo=true`
                """)
            .Produces<List<Personaje>>();

        // Rutas literales (/ranking) antes que /{id:int} para que quede claro el orden.
        g.MapGet("/ranking", (string? por, EstadisticasService s) => s.Ranking(por ?? "poder").ToHttp())
            .WithName("RankingPersonajes")
            .WithSummary("Ranking de personajes según su carta")
            .WithDescription("""
                Ordena de mayor a menor a los personajes que tienen carta.

                - `por=poder` (por defecto): usa el campo `poder` de la carta
                - `por=peligrosidad`: usa `nivelPeligrosidad`

                Ejemplo: `GET /personajes/ranking?por=poder`
                """)
            .Produces<List<RankingItem>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        g.MapGet("/{id:int}", (int id, PersonajeService s) => s.Obtener(id).ToHttp())
            .WithName("ObtenerPersonaje")
            .WithSummary("Obtiene un personaje por id")
            .Produces<Personaje>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        g.MapGet("/{id:int}/eventos", (int id, EventoService s) => s.PorPersonaje(id).ToHttp())
            .WithName("EventosDePersonaje")
            .WithSummary("Eventos en los que participa un personaje")
            .WithDescription("Devuelve los eventos donde el personaje figura como participante, en orden cronológico (BBY → ABY).")
            .Produces<List<Evento>>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        g.MapPost("/", (PersonajeRequest r, PersonajeService s) => s.Crear(r).ToHttp(p => $"/personajes/{p.Id}"))
            .WithName("CrearPersonaje")
            .WithSummary("Crea un personaje")
            .WithDescription("""
                Devuelve **201** con el personaje creado y el header `Location`.

                ```json
                {
                  "nombre": "Ahsoka Tano",
                  "especie": "Togruta",
                  "faccion": "Rebelde",
                  "afiliacion": "Rebeldes de Lothal",
                  "estado": "Vivo",
                  "fuerzaSensitivo": true
                }
                ```
                """)
            .Produces<Personaje>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        g.MapPut("/{id:int}", (int id, PersonajeRequest r, PersonajeService s) => s.Actualizar(id, r).ToHttp())
            .WithName("ActualizarPersonaje")
            .WithSummary("Reemplaza los datos de un personaje")
            .WithDescription("""
                Reemplaza todos los campos. **Regla:** si la muerte del personaje está consignada en un evento,
                su estado no puede cambiarse a mano (se modifica desde el evento).
                """)
            .Produces<Personaje>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        g.MapDelete("/{id:int}", (int id, PersonajeService s) =>
            {
                var r = s.Eliminar(id);
                return r.EsOk ? Results.NoContent() : r.ToHttp();
            })
            .WithName("EliminarPersonaje")
            .WithSummary("Elimina un personaje")
            .WithDescription("Elimina también su carta (relación 1:1). Falla con 400 si el personaje participa en algún evento.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
