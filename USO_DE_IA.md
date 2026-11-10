# Declaración de uso de inteligencia artificial

## Herramientas utilizadas

Durante el desarrollo de **Catálogo Galáctico** utilicé **Claude** y **ChatGPT** como herramientas de apoyo técnico. Las consulté para aclarar dudas, revisar alternativas de implementación, detectar errores y orientar las pruebas. También tomé como referencia propuestas y fragmentos de código generados por IA en distintas etapas, que fui integrando y ajustando según los requisitos del proyecto.

## ¿En qué me ayudaron?

- **Organización del proyecto:** apoyo para estructurar la primera versión de la API, documentar los endpoints en Swagger e integrar el backend con el frontend en React.
- **Adaptación a la versión final:** orientación para trabajar con controladores, Entity Framework Core, MySQL, migraciones, Secret Manager y servicios separados por responsabilidades.
- **Funcionalidad adicional:** exploración e implementación del sistema de experiencia y progresión de las cartas a partir de la participación en eventos.
- **Depuración:** revisión de errores de compilación y ejecución, registros de dependencias en `Program.cs` y validaciones de los DTO.
- **Pruebas:** apoyo para interpretar las respuestas de Swagger y comprobar el comportamiento de la simulación y la progresión de personajes.

## Cambios y decisiones durante el desarrollo

Reemplacé enlaces de imágenes de ejemplo por referencias de Wookieepedia e incorporé a **Cal Kestis** y **Trilla Suduri** con sus respectivas cartas. También revisé la integración con el frontend y ajusté el campo de respuesta de la simulación de `mensaje` a `resultado`, de acuerdo con lo que esperaba `App.jsx`.

Para el manejo de usuarios, contrasté las propuestas iniciales con el proyecto de referencia del docente y seguí su enfoque de `PasswordHasher` de ASP.NET Core Identity, un servicio de cifrado y configuración mediante Secret Manager.

## Comprobaciones realizadas

Probé los endpoints desde Swagger y revisé sus respuestas. Por ejemplo:

- `POST /eventos/5/simular?semilla=42`: la simulación de Endor devolvió como ganador al bando Rebelde y asignó **30 XP** a Luke Skywalker.
- `GET /cartas/1/progresion`: mostró **30 XP**, nivel 1 y la participación de Luke en Endor.
- `GET /cartas/1/poder?fecha=3 ABY`: devolvió **0 XP acumulada** para esa fecha y un poder de **85**, sin aplicar anticipadamente la experiencia del evento posterior.

En general, utilicé la IA como apoyo durante el desarrollo y, especialmente, para **resolver problemas y contrastar resultados**. Las respuestas y sugerencias se revisaron en función del funcionamiento del proyecto y de los requisitos de la materia.