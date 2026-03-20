# Trazabilidad del PDF a la implementación

El PDF extraído es una guía corta de entrevista con lista de temas. Este documento los aterriza a archivos concretos del repo.

## Matriz de trazabilidad

| Punto del PDF | Aplicación en este repositorio | Documento principal |
| --- | --- | --- |
| Testing process | Estructura de diseño, ejecución, evidencia y pipeline | `02-fundamentos-testing.md` |
| Types of Testing | Separación en `web`, `api` y `desktop` | `02-fundamentos-testing.md` |
| Test Design Techniques | Casos smoke, validación de respuesta, flujo feliz y estructura BDD | `02-fundamentos-testing.md` |
| Defect Management | Evidencias en `artifacts/`, categorización y trazabilidad | `02-fundamentos-testing.md` |
| C# OOP Principles | Clases por responsabilidad, encapsulación, composición | `03-csharp-sql-y-net.md` |
| Collections & Generics | Uso de colecciones tipadas en settings y utilidades | `03-csharp-sql-y-net.md` |
| Extension Methods | `StringExtensions.NormalizeWhitespace()` | `03-csharp-sql-y-net.md` |
| LINQ | Uso en transformación de datos y sanitización | `03-csharp-sql-y-net.md` |
| Interfaces | `IAutomationSettingsProvider`, `IArtifactLogger` | `03-csharp-sql-y-net.md` |
| Exceptions | `AutomationConfigurationException`, validación de respuestas API | `03-csharp-sql-y-net.md` |
| SQL CRUD / Query / Subquery | Base teórica documentada para apoyar escenarios end to end | `03-csharp-sql-y-net.md` |
| Testing in .NET | Solución `net8.0`, NUnit, paquetes, convenciones | `03-csharp-sql-y-net.md` |
| Framework with layered architecture | `Core`, `Infrastructure`, `Web`, `Api`, `Desktop`, `Tests` | `04-arquitectura-framework.md` |
| Layers, approaches, reporting, logging | Logger de artefactos, flows, pages, clients y assets CI | `04-arquitectura-framework.md` |
| Design patterns / principles | Factory, Page Object, Screen Object, Client Object, separación por capas | `04-arquitectura-framework.md` |
| Selenium | `WebDriverFactory`, `SeleniumWebFormPage`, `WebFormFlow` | `05-automatizacion-web.md` |
| API Testing | Clientes `HttpClient` y `RestSharp` | `06-automatizacion-api.md` |
| Authorization | `BearerToken` en `automation.settings.json` y clientes API | `06-automatizacion-api.md` |
| HTTP Methods | `GET` y `POST` implementados | `06-automatizacion-api.md` |
| Response validations | Aserciones de contrato y contenido | `06-automatizacion-api.md` |
| Business API mocking / advanced response validation | Mock local de banca y retail con validaciÃ³n de status, body y regla de negocio | `11-api-mocking-test-patterns.md` |
| BDD: SpecFlow | Features y Steps en cada suite de pruebas | `08-bdd-runners-reporting.md` |
| Test Runners: MSTest, NUnit, xUnit | Implementación activa con `NUnit`, comparación documentada con otros runners | `08-bdd-runners-reporting.md` |
| CI/CD | `GitHub Actions`, `Azure DevOps`, `Jenkinsfile` | `09-ci-cd-quality-gates.md` |
| Quality Gates | Restore, build, test por categoría y criterios mínimos | `09-ci-cd-quality-gates.md` |

## Punto extendido: desktop

El PDF extraído no enumera explícitamente una librería desktop, pero el usuario pidió cubrir `web`, `api` y `desktop`. Para respetar el espíritu del PDF se mantuvo:

- Ecosistema `.NET`
- Arquitectura por capas
- Automatización basada en drivers
- Objetos de interacción desacoplados

La implementación elegida fue `Appium/WinAppDriver` sobre Windows Calculator.

## Cómo usar esta trazabilidad

Si en una entrevista o ejercicio te preguntan por un tema del PDF, no respondas solo con definición. Responde con tres niveles:

1. Concepto.
2. Ejemplo en código.
3. Riesgo o buena práctica asociada.
