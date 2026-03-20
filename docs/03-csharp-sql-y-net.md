# C#, SQL y .NET para automation

El PDF pide base técnica sólida en `.NET`. Eso no es relleno. Un automation engineer fuerte no solo “usa Selenium”; sabe programar bien.

## OOP

En este repo:

- `WebDriverFactory` encapsula creación de drivers.
- `SeleniumWebFormPage` representa la página.
- `WebFormFlow` orquesta una intención de negocio.
- `JsonPlaceholderHttpClient` y `JsonPlaceholderRestSharpClient` encapsulan la capa API.
- `CalculatorScreen` abstrae la UI desktop.

Eso es orientación a objetos aplicada, no teoría aislada.

## Interfaces

Se usan para desacoplar contrato de implementación:

- `IAutomationSettingsProvider`
- `IArtifactLogger`

Esto permite cambiar la fuente de configuración o el mecanismo de logging sin reescribir consumidores.

## Excepciones

La excepción `AutomationConfigurationException` muestra un criterio importante:

- falla temprano,
- falla con mensaje claro,
- falla donde realmente está el problema.

Silenciar errores de configuración es una mala práctica frecuente en frameworks junior.

## Colecciones, genéricos y LINQ

Las colecciones tipadas aparecen en settings y utilidades.
LINQ se usa para transformar datos sin ruido accidental.
La clave no es “saber sintaxis”, sino elegir la colección correcta y no sobrecomplicar.

## Extension methods

`StringExtensions.NormalizeWhitespace()` es un ejemplo útil:

- encapsula una transformación repetible,
- mejora legibilidad,
- evita lógica duplicada.

Una extension method buena simplifica; una mala esconde comportamiento crítico.

## SQL

El PDF menciona `CRUD`, `query` y `subquery`. Aunque este repo no agrega una base de datos real, debes dominar al menos:

- `SELECT`, `INSERT`, `UPDATE`, `DELETE`
- `JOIN`
- `GROUP BY`
- subconsultas
- filtros por fechas y estados

En automatización esto te sirve para:

- preparar datos,
- verificar persistencia,
- limpiar evidencia,
- validar reglas fuera de la UI.

## .NET testing

Este framework usa `net8.0` y `NUnit`, pero debes entender el panorama:

- `MSTest`: simple, corporativo, integración nativa con Microsoft.
- `NUnit`: flexible, expresivo, cómodo para suites de automatización.
- `xUnit`: muy usado en desarrollo, minimalista, fuerte en unit testing.

Aquí se eligió `NUnit` por equilibrio entre expresividad y familiaridad para automation.
