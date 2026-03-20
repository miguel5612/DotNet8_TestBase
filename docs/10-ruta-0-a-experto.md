# Ruta de 0 a experto

Este documento convierte el PDF y el repo en un plan de crecimiento real.

## Nivel 0: Fundamentos

Objetivo:
entender testing, HTTP, HTML básico, C# básico y control de versiones.

Debes poder explicar:

- qué es un caso de prueba,
- qué diferencia hay entre UI y API,
- qué es un `GET` y un `POST`,
- qué es una clase,
- qué es una interfaz,
- qué es una excepción.

## Nivel 1: Junior sólido

Objetivo:
ejecutar y modificar las pruebas existentes.

Debes poder:

- cambiar `automation.settings.json`,
- agregar un nuevo campo a una página,
- validar un nuevo atributo de una respuesta,
- entender una feature de SpecFlow,
- lanzar pruebas por categoría.

## Nivel 2: Semi senior

Objetivo:
diseñar nuevas piezas del framework sin romper lo existente.

Debes poder:

- crear una nueva page o screen object,
- extraer utilidades comunes,
- decidir cuándo usar UI y cuándo API,
- agregar logging útil,
- preparar ejecución en CI.

## Nivel 3: Senior

Objetivo:
tomar decisiones de arquitectura y calidad.

Debes poder:

- defender una arquitectura por capas,
- identificar deuda técnica,
- reducir flakiness,
- definir quality gates,
- proponer estrategia de automatización por riesgo,
- explicar tradeoffs entre herramientas.

## Nivel 4: Experto

Objetivo:
construir soluciones escalables y enseñar a otros.

Debes poder:

- estandarizar prácticas en varios equipos,
- diseñar reutilización multi-canal,
- integrar observabilidad y métricas,
- conectar testing con entrega continua,
- evaluar herramientas por costo, valor y mantenibilidad.

## Ejercicios recomendados

1. Cambia la suite web para soportar `Chrome`, `Edge` y `Firefox`.
2. Agrega autenticación real a la capa API.
3. Integra un proveedor de datos desde SQL.
4. Captura screenshots y adjúntalos como artefactos.
5. Agrega tags por criticidad y por canal.
6. Crea un stage rápido y otro nocturno en CI.

## Cómo estudiar este repo

No memorices nombres de librerías. Estudia en este orden:

1. concepto,
2. código,
3. riesgo,
4. decisión técnica,
5. mejora posible.

Si puedes explicar esos cinco niveles por cada tema del PDF, ya no estás preparando una entrevista: estás operando como automation engineer serio.
