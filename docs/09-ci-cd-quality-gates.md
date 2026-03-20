# CI/CD y quality gates

El PDF menciona `What is CI/CD?`, `Jenkins`, `GitHub actions`, `Azure DevOps` y `Quality Gates`. Aquí no se dejó como teoría: hay ejemplos concretos.

## Archivos incluidos

- `.github/workflows/dotnet-tests.yml`
- `azure-pipelines/automation-tests.yml`
- `Jenkinsfile`

## Flujo mínimo recomendado

1. Restore.
2. Build.
3. Ejecutar API.
4. Ejecutar Web.
5. Ejecutar Desktop si el agente lo soporta.
6. Publicar artefactos.

## Qué es un quality gate

Es una condición objetiva que define si el cambio puede avanzar. Ejemplos:

- build exitoso,
- pruebas smoke verdes,
- cero defectos bloqueantes,
- cobertura mínima,
- análisis estático aceptable.

## Quality gates recomendados para este framework

- El proyecto debe restaurar y compilar.
- API tests deben pasar siempre.
- Web tests deben pasar en agentes con browser preparado.
- Desktop tests deben correr solo en agentes especializados.
- Las suites deben ejecutarse por categoría.

## Error común

Poner todas las pruebas en un solo gate. Eso vuelve el pipeline lento y frágil.

Lo correcto es construir capas:

- gate rápido,
- gate medio,
- gate profundo.
