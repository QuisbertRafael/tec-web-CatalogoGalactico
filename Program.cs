
using System.Reflection;
using System.Text.Json.Serialization;
using CatalogoGalactico.Data;
using CatalogoGalactico.Entities;
using CatalogoGalactico.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Falta ConnectionStrings:DefaultConnection. Configúrala con dotnet user-secrets.");

builder.Services.AddDbContext<CatalogoDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36))
    ));



builder.Services.AddScoped<IPersonajeService, PersonajeService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IEstadisticasService, EstadisticasService>();



// Gestión de usuarios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Hash y verificación de contraseñas
builder.Services.AddScoped<
    IPasswordHasher<Usuario>,
    PasswordHasher<Usuario>>();

// Cifrado y descifrado de información
builder.Services.AddScoped<ICifradoService, CifradoService>();



builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        ));

builder.Services.AddEndpointsApiExplorer();



builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catálogo Galáctico API",
        Version = "v2",
        Description = """
            Web API (ASP.NET Core con controladores, EF Core y MySQL)
            para consultar y administrar personajes, cartas
            coleccionables y eventos.

            ### Modelo
            - Personaje 1:1 Carta
            - Evento N:N Personaje mediante EventoParticipante
            - Usuario: contraseña guardada como hash (Identity)
              y email cifrado (AES-GCM)

            ### Convención de fechas
            "10 BBY" → -10 · "Batalla de Yavin" → 0 · "3 ABY" → 3

            ### Códigos de respuesta
            200 OK · 201 creado · 204 sin contenido
            400 datos inválidos · 404 no existe · 409 conflicto.
            Los errores usan ProblemDetails.
            """,
        Contact = new OpenApiContact
        {
            Name = "Tecnologías Web 1"
        },
        License = new OpenApiLicense
        {
            Name = "Uso académico"
        }
    });

    o.IncludeXmlComments(
        Path.Combine(
            AppContext.BaseDirectory,
            $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"
        )
    );
});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});



var app = builder.Build();

app.UseCors();



app.UseSwagger();

app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Catálogo Galáctico API v2"
    );

    o.DocumentTitle = "Catálogo Galáctico API";
    o.DocExpansion(DocExpansion.List);
    o.DefaultModelsExpandDepth(1);
    o.DisplayRequestDuration();
    o.EnableFilter();
});


app.MapGet("/", () =>
    Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapControllers();


app.Run();
