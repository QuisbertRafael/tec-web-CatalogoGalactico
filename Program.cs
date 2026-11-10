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

// La cadena de conexión NO está en el código ni en appsettings.json:
// viene de Secret Manager (dotnet user-secrets). Ver README.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Falta ConnectionStrings:DefaultConnection. Configúrala con 'dotnet user-secrets set' (ver README).");

// Se fija la versión de MySQL para que EF no tenga que conectarse al arrancar ni al generar migraciones.
// Si usas MariaDB, cambia por: new MariaDbServerVersion(new Version(10, 11, 0)).
builder.Services.AddDbContext<CatalogoDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

// Protección de datos: hash de contraseñas (Identity) y cifrado AES-GCM de campos sensibles.
// La clave Cifrado:Clave también viene de Secret Manager.
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddSingleton<ICifradoService, CifradoService>();

// Servicios Scoped: dependen del DbContext, que es Scoped (una instancia por petición HTTP).
// Servicios del catálogo
builder.Services.AddScoped<IPersonajeService, PersonajeService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IEstadisticasService, EstadisticasService>();

// Servicio de progresión
builder.Services.AddScoped<IProgresionService, ProgresionService>();

// Servicios de usuarios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<ICifradoService, CifradoService>();

// Los enums viajan como texto ("Rebelde", "Vivo") en JSON y en Swagger.
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catálogo Galáctico API",
        Version = "v2",
        Description = """
            Web API (ASP.NET Core con controladores, EF Core y MySQL) para consultar y administrar **personajes**,
            sus **cartas coleccionables** y los **eventos** en los que participan.

            ### Modelo
            - `Personaje` 1:1 `Carta`
            - `Evento` N:N `Personaje` mediante la tabla intermedia `EventoParticipante`
            - `Usuario`: contraseña guardada como hash (Identity) y email cifrado (AES-GCM)

            ### Convención de fechas
            `"10 BBY"` → -10 · `"Batalla de Yavin"` → 0 · `"3 ABY"` → 3

            ### Códigos de respuesta
            `200` OK · `201` creado · `204` sin contenido · `400` datos inválidos ·
            `404` no existe · `409` conflicto con el estado actual (por ejemplo, participar después de morir).
            Los errores usan el formato estándar *ProblemDetails*.
            """,
        Contact = new OpenApiContact { Name = "Tecnologías Web 1" },
        License = new OpenApiLicense { Name = "Uso académico" }
    });

    // Muestra en Swagger los comentarios /// de los controladores.
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

// CORS para el frontend en React (Vite).
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
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Catálogo Galáctico API v2");
    o.DocumentTitle = "Catálogo Galáctico API";
    o.DocExpansion(DocExpansion.List);
    o.DefaultModelsExpandDepth(1);
    o.DisplayRequestDuration();
    o.EnableFilter();
});

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

app.Run();
