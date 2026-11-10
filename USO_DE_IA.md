# Declaración de uso de IA

> Completa las partes marcadas con **[COMPLETAR]** con lo que realmente hiciste, y borra esta nota antes de entregar.

## Herramientas usadas
- **Claude** (Anthropic), en su interfaz de chat web.

## Qué le pedí en los momentos importantes
1. **Primera versión de la API (Minimal API en memoria).** Le pasé el enunciado en PDF y le pedí crear los archivos del
   proyecto, el orden de ramas y pull requests, y un Swagger detallado.
2. **Defensa de Minimal API.** Le pedí ayuda para agregar el campo `image` a los personajes, crear
   `GET /personajes/{id}/con-card`, habilitar CORS e integrar la API con el frontend en React del docente.
3. **Componente propio.** Le pedí ideas de funcionalidades que usaran las tres tablas y que tuvieran relación con la
   historia de Star Wars; elegí el sistema de experiencia de cartas y le pedí desarrollarlo y explicar qué implicaba.
4. **Trabajo final.** Le pasé el enunciado del trabajo, mi proyecto y el repositorio de ejemplo del docente
   (BackofficeApi), y le pedí convertir mi proyecto a Web API con controladores, EF Core con MySQL, Secret Manager,
   hash de contraseñas y cifrado de un campo, y preparar el código por fases/ramas con prefijo EC3.

## Qué acepté
- La estructura en capas (Entities, Dtos, Data, Services, Controllers) y el uso de `Resultado<T>` en los servicios para
  separar la lógica de negocio de HTTP.
- La tabla intermedia `EventoParticipante` para la relación N:N y para guardar la XP por participación.
- Servicios con interfaz y ciclo de vida Scoped (usan el DbContext) y `CifradoService` como Singleton.
- Usar EF Core 9.0.0 con Pomelo 9.0.0 sobre net10.0, porque Pomelo todavía no tiene versión para EF Core 10.

## Qué corregí o cuestioné
- La IA me indicó que el comando `dotnet add Swashbuckle.AspNetCore.Swagger` estaba mal y que el paquete correcto es
  `Swashbuckle.AspNetCore`.
- Los enlaces de imágenes de ejemplo no me convencieron: los reemplacé por enlaces reales de Wookieepedia, y agregué por
  mi cuenta a los personajes Cal Kestis y Trilla Suduri con sus cartas.
- Al integrar el frontend, el panel no mostraba el resultado de la simulación. Leí `App.jsx` y vi que esperaba el campo
  `resultado` y no `mensaje`, así que se renombró en la respuesta.
- Errores míos que corregí durante el proceso: pegué el contenido de `Program.cs` dentro de otro archivo y puse
  `app.UseCors()` antes de `builder.Build()`.
- La primera propuesta de la IA para el trabajo final usaba BCrypt, un cifrado dentro de EF Core y otros nombres de
  secretos. Al compartirle el repositorio de ejemplo del docente, lo cambiamos para seguir su enfoque:
  `PasswordHasher` de ASP.NET Core Identity, un `CifradoService` y los secretos `ConnectionStrings:DefaultConnection`
  y `Cifrado:Clave`.
- **[COMPLETAR]** Qué revisé o cambié del código del trabajo final (por ejemplo: qué probé en Swagger, qué verifiqué en
  MySQL, qué ajusté o rechacé de lo que propuso la IA y por qué).

## Qué comprobé yo
- **[COMPLETAR]** Por ejemplo: que `PasswordHash` en la tabla `Usuarios` es un hash y no la contraseña, que `Email`
  aparece cifrado en MySQL y descifrado en `GET /usuarios/{id}`, que cada migración se aplicó en mi base de datos y que
  mis secretos no aparecen en el historial de Git.
