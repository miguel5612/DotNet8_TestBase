# Arquitectura del framework

El PDF menciona `Test Automation Framework with Layered Architecture`, `layers`, `approaches`, `reporting`, `logging`, `design patterns` y `design principles`. Esta es la traducción práctica.

## Capas

### Core

Define contratos, settings, excepciones y extensiones reutilizables.

### Infrastructure

Implementa cómo se cargan settings y cómo se escriben artefactos.

### Web / Api / Desktop

Cada capa contiene la lógica específica del canal que automatiza.

### Tests

Orquesta la ejecución real, aserciones, BDD y categorías.

## Patrones usados

- Factory: creación de drivers y sesiones.
- Page Object: modelado de páginas web.
- Screen Object: modelado de aplicaciones desktop.
- Client Object: encapsulación de APIs.
- Flow Object: composición de pasos técnicos en intención funcional.

## Principios aplicados

- Responsabilidad única.
- Separación de preocupaciones.
- Bajo acoplamiento.
- Alta cohesión.
- Configuración externa.

## Qué evita esta arquitectura

- Selectores mezclados con asserts.
- URLs hardcodeadas en tests.
- Código repetido por canal.
- Configuración dispersa.
- Dependencia directa de herramientas en cada test.

## Reporting y logging

El PDF lo menciona como tema obligatorio. Aquí se aterriza con `ArtifactLogger`, que deja:

- resumen del resultado,
- carpeta por ejecución,
- base para evolucionar a screenshots, payloads o traces.

Si luego quieres crecer, el siguiente paso natural es integrar:

- screenshots web,
- request/response API,
- capturas desktop,
- reportes HTML agregados.
