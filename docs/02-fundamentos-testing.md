# Fundamentos de testing

El PDF lista `testing process`, `types of testing`, `test design techniques` y `defect management`. Eso es la base. Sin esto, automatizar solo produce scripts frágiles.

## Proceso de pruebas

Un proceso de pruebas sano sigue este ciclo:

1. Entender el requisito.
2. Identificar riesgos.
3. Diseñar casos.
4. Preparar datos y ambiente.
5. Ejecutar.
6. Validar resultados.
7. Reportar defectos.
8. Mantener evidencia.

En este repo ese flujo se refleja así:

- Configuración central en `automation.settings.json`.
- Casos de ejemplo en `tests/`.
- Evidencias en `artifacts/`.
- Ejecución local y en pipeline.

## Tipos de testing

### Web

Valida comportamiento visible desde el navegador. Tiene más costo y más flakiness que API.

### API

Valida contrato, status codes, payloads, headers y reglas de negocio sin UI. Suele ser más estable y rápida.

### Desktop

Valida aplicaciones nativas o híbridas en Windows. Tiene dependencias de ambiente más fuertes, por eso conviene aislarla.

## Técnicas de diseño de pruebas

Aunque el repo trae ejemplos smoke, tu diseño debe poder crecer hacia:

- Partición de equivalencia.
- Análisis de valores límite.
- Tablas de decisión.
- Transición de estados.
- Casos basados en riesgo.

Una buena señal profesional es explicar por qué un caso existe, no solo mostrar el script.

## Gestión de defectos

Un defecto bien reportado debe incluir:

- Título claro.
- Ambiente.
- Pasos de reproducción.
- Resultado esperado.
- Resultado actual.
- Evidencia.
- Severidad y prioridad.

El logger de artefactos del repo existe para dejar rastros mínimos de ejecución y conectar la falla con evidencia concreta.

## Regla práctica

Automatiza primero lo que sea:

- repetible,
- crítico,
- estable,
- medible,
- y costoso de validar manualmente.

No automatices UI solo porque “se ve más completo”. Eso suele ser una mala decisión técnica.
