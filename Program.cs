using System.Text.Json.Serialization;
using CatalogoGalactico.Data;
using CatalogoGalactico.Endpoints;
using CatalogoGalactico.Services;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Los enums viajan como texto ("Rebelde", "Vivo") en JSON y en Swagger.
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Almacén en memoria y servicios (singleton: los datos viven mientras corre la app).
builder.Services.AddSingleton<AlmacenMemoria>();
builder.Services.AddSingleton<PersonajeService>();
builder.Services.AddSingleton<CardService>();
builder.Services.AddSingleton<EventoService>();
builder.Services.AddSingleton<EstadisticasService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catálogo Galáctico API",
        Version = "v1",
        Description = """
            API REST (Minimal API) para consultar y administrar **personajes**, sus **cartas coleccionables**
            y los **eventos** en los que participan.

            ### Modelo
            - `Personaje` 1:1 `Carta`
            - `Evento` N:N `Personaje` (el evento guarda la lista de ids de participantes)

            ### Convención de fechas
            `"10 BBY"` → -10 · `"Batalla de Yavin"` → 0 · `"3 ABY"` → 3

            ### Errores
            `404` recurso inexistente · `400` datos o reglas inválidas · `201` recurso creado.
            Los errores usan el formato estándar *ProblemDetails*.

            > Prototipo académico: los datos viven en memoria y se reinician al reiniciar la aplicación.
            """,
        Contact = new OpenApiContact { Name = "Tecnologías Web 1" },
        License = new OpenApiLicense { Name = "Uso académico" }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Catálogo Galáctico API v1");
    o.DocumentTitle = "Catálogo Galáctico API";
    o.DocExpansion(DocExpansion.List);
    o.DefaultModelsExpandDepth(1);
    o.DisplayRequestDuration();
    o.EnableFilter();
});

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapPersonajeEndpoints();
app.MapCartaEndpoints();
app.MapEventoEndpoints();

app.Run();

