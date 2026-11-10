# Catálogo Galáctico API

Web API (ASP.NET Core con controladores, .NET 10) que administra **personajes**, sus **cartas coleccionables** y los
**eventos** en los que participan. Los datos se guardan en **MySQL** con **Entity Framework Core**, las contraseñas se
guardan como **hash** y el email de los usuarios se guarda **cifrado**.

## Qué hace
- CRUD de personajes, cartas, eventos y usuarios, con DTOs, servicios con inyección de dependencias y códigos HTTP
  200, 201, 204, 400, 404 y 409.
- Relaciones: Personaje 1:1 Carta, Evento N:N Personaje (tabla intermedia `EventoParticipante`).
- Reglas de negocio: un personaje muere una sola vez y no puede participar en eventos posteriores a su muerte;
  la muerte registrada en un evento actualiza el estado del personaje.
- Consultas: filtros combinables, ranking, MVP de un evento, ficha de combate (`con-card`) y simulación de batalla
  con factor aleatorio reproducible (`?semilla=42`).
- **Componente propio: sistema de experiencia de cartas.** El poder base de una carta no cambia; el poder efectivo
  crece con la XP ganada en simulaciones de eventos anteriores a cada fecha.
- Swagger con documentación de cada endpoint en `/swagger`.

## Requisitos
- .NET SDK 10
- MySQL 8 en ejecución
- Herramienta de migraciones: `dotnet tool install --global dotnet-ef --version 9.0.0`
  (EF Core 9 porque Pomelo, el proveedor de MySQL, todavía no publica versión para EF Core 10; funciona sobre net10.0)

## 1. Crear la base de datos y el usuario en MySQL
Con el usuario root:
```sql
CREATE DATABASE catalogo_galactico CHARACTER SET utf8mb4;
CREATE USER 'catalogo_user'@'localhost' IDENTIFIED BY 'ElijaUnaClaveSegura';
GRANT ALL PRIVILEGES ON catalogo_galactico.* TO 'catalogo_user'@'localhost';
FLUSH PRIVILEGES;
```

## 2. Configurar los secretos
Ningún secreto está en el código ni en `appsettings.json`. Se guardan con Secret Manager (los valores son ficticios):

```bash
dotnet restore
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=catalogo_galactico;User=catalogo_user;Password=SU_CLAVE"
dotnet user-secrets set "Cifrado:Clave" "PEGA_AQUI_LA_CLAVE_32_BYTES_EN_BASE64"
dotnet user-secrets list
```

La clave de cifrado debe ser de 32 bytes (256 bits), aleatoria, en Base64:
- Windows (PowerShell): `$b = New-Object byte[] 32; [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b); [Convert]::ToBase64String($b)`
- macOS / Linux / Git Bash: `openssl rand -base64 32`

## 3. Aplicar las migraciones
```bash
dotnet ef database update
```
Crea las tablas e inserta los datos iniciales. Migraciones incluidas: `Inicial`, `AgregarUsuarios` y `AgregarExperiencia`.
Si falla la conexión con los secretos, prueba:
```bash
dotnet ef database update --connection "Server=127.0.0.1;Port=3306;Database=catalogo_galactico;Uid=catalogo_user;Password=SU_CLAVE;SslMode=none"
```
Para revisar la base: `SHOW TABLES;`, `DESCRIBE EventoParticipantes;`, `SHOW CREATE TABLE Personajes;`

## 4. Ejecutar
```bash
dotnet run
```
Abre `http://localhost:5156/swagger`.

## Protección de datos: cómo comprobarla
1. Crea un usuario con `POST /usuarios` (nombreUsuario, email, password).
2. En MySQL:
   ```sql
   SELECT Id, NombreUsuario, Email, PasswordHash FROM Usuarios;
   ```
   - `PasswordHash` es el hash de ASP.NET Core Identity (PBKDF2 con sal), nunca la contraseña.
   - `Email` es texto cifrado con AES-256-GCM (Base64 ilegible).
3. `GET /usuarios/{id}` devuelve el email descifrado y nunca devuelve la contraseña ni su hash.

La clave de cifrado vive en Secret Manager (`Cifrado:Clave`), no en el repositorio.

## Endpoints principales
| Recurso | Rutas |
|---|---|
| Personajes | `GET/POST /personajes`, `GET/PUT/DELETE /personajes/{id}`, `GET /personajes/{id}/con-card`, `GET /personajes/{id}/eventos`, `GET /personajes/ranking?por=poder` |
| Cartas | `GET/POST /cartas`, `GET/PUT/DELETE /cartas/{id}`, `GET /cartas/{id}/progresion`, `GET /cartas/{id}/poder?fecha=3 ABY` |
| Eventos | `GET/POST /eventos`, `GET/PUT /eventos/{id}`, `GET /eventos/{id}/mvp`, `POST /eventos/{id}/simular?semilla=42` |
| Usuarios | `GET/POST /usuarios`, `GET/DELETE /usuarios/{id}` |

## Sistema de experiencia
| Situación | XP |
|---|---|
| Participar en el evento | +10 |
| Bando ganador / perdedor / empate | +20 / +5 / +10 |
| MVP del evento | +15 |
| Ganar siendo el bando con menor fuerza base | x1.5 |

Niveles por XP: 1 (0), 2 (100), 3 (250), 4 (500), 5 (900). Cada nivel suma +4 al poder (tope 100) y los niveles
3 y 5 suben 1 punto de peligrosidad (máximo 5). Solo cuenta la XP de eventos de años **anteriores** a la fecha
consultada, y re-simular un evento reemplaza su XP en lugar de acumularla.

## Estructura
| Carpeta | Contenido |
|---|---|
| `Entities/` | Entidades de la base de datos y enums |
| `Dtos/` | Solicitudes y respuestas de la API, y `Mapeos` entre entidades y DTOs |
| `Data/` | `CatalogoDbContext` y datos iniciales |
| `Services/` | Interfaces y servicios con las reglas de negocio (Scoped) y `CifradoService` (Singleton) |
| `Controllers/` | Controladores y traducción de `Resultado<T>` a HTTP |
| `Migrations/` | Migraciones de EF Core |
