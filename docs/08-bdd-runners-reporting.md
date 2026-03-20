# BDD, runners y reporting

El PDF menciona `BDD: SpecFlow` y `Test Runners: MSTest, NUnit, XUnit`. Aquí eso se traduce a una capa de ejecución coherente.

## Runner elegido

Se eligió `NUnit` porque:

- es expresivo,
- tiene atributos útiles para automation,
- convive bien con suites de integración,
- y es una elección común en equipos de testing .NET.

## Dónde entra SpecFlow

SpecFlow aporta una capa de lenguaje ubicua:

- `Features/*.feature`
- `Steps/*.cs`

Eso sirve para:

- alinear negocio y automatización,
- expresar comportamiento,
- mantener escenarios legibles.

## Cuándo usar BDD

Úsalo cuando:

- el lenguaje del negocio importe,
- quieras trazabilidad funcional,
- haya colaboración con analistas o QA manual.

No lo uses para todo. Un smoke técnico simple puede ser mejor como prueba directa en código.

## Reporting

Hoy el repo guarda artefactos de texto. Eso es intencional: primero se construye una base clara, luego se sofisticará.

Evoluciones naturales:

- screenshots,
- logs estructurados,
- request/response payloads,
- reportes HTML,
- publicación de artefactos en CI.
