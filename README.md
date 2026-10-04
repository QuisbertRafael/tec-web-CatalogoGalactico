# Catálogo Galáctico API

API REST con Minimal API (.NET 10) para administrar personajes, cartas coleccionables y eventos. Datos en memoria.

## Ejecutar
```bash
dotnet run
```
Swagger UI: `/swagger`

## Endpoints
| Recurso | Rutas |
|---|---|
| Personajes | `GET/POST /personajes`, `GET/PUT/DELETE /personajes/{id}`, `GET /personajes/ranking?por=poder`, `GET /personajes/{id}/eventos` |
| Cartas | `GET/POST /cartas`, `GET/PUT /cartas/{id}` |
| Eventos | `GET/POST /eventos`, `GET/PUT /eventos/{id}`, `GET /eventos/{id}/mvp`, `POST /eventos/{id}/simular?semilla=42` |

## Ejemplos
```bash
curl "http://localhost:5000/personajes?faccion=Imperio&fuerzaSensitivo=true"
curl -X POST "http://localhost:5000/eventos/5/simular?semilla=42"
```

## Reglas destacadas
- Un personaje tiene una sola carta (1:1).
- Un evento con muertes marca a esos personajes como `Muerto`.
- No se permiten participaciones posteriores a una muerte consignada.
- La simulación no guarda resultado si falla alguna validación.
