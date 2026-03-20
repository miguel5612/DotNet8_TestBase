# AutomatizaciÃƒÂ³n API

El PDF menciona `API Testing`, `RestSharp`, `HttpClient`, `Authorization`, `HTTP Methods` y `Response validations`. Todo eso quedÃƒÂ³ reflejado.

## ImplementaciÃƒÂ³n del repo

- `JsonPlaceholderHttpClient`
  Cliente basado en `HttpClient`.
- `JsonPlaceholderRestSharpClient`
  Cliente basado en `RestSharp`.
- `CreatePostRequest` y `PostDto`
  Contratos claros para serializaciÃƒÂ³n y validaciÃƒÂ³n.
- `tests/FrameworkBase.Automation.Api.Tests`
  Casos con `GET` y `POST`.

## QuÃƒÂ© debes dominar en API testing

- status codes,
- headers,
- payload,
- tiempos de respuesta,
- autenticaciÃƒÂ³n,
- idempotencia,
- errores funcionales y tÃƒÂ©cnicos.

## Authorization

`automation.settings.json` deja previsto `BearerToken`. Eso importa porque la mayorÃƒÂ­a de APIs reales no son pÃƒÂºblicas como JSONPlaceholder.

## Validaciones ÃƒÂºtiles

No te quedes solo con `200 OK`. Valida:

- estructura,
- campos obligatorios,
- tipos,
- negocio,
- consistencia entre request y response.

## HttpClient vs RestSharp

### HttpClient

Conviene cuando quieres control fino, dependencia mÃƒÂ­nima y un estilo muy cercano al framework base de .NET.

### RestSharp

Conviene cuando quieres ergonomÃƒÂ­a rÃƒÂ¡pida para requests, headers, cuerpos y serializaciÃƒÂ³n.

Lo importante es que entiendas el tradeoff y no conviertas la herramienta en religiÃƒÂ³n.

## CÃƒÂ³mo llevarlo a nivel senior

1. Agrega validaciÃƒÂ³n de esquema.
2. Integra autenticaciÃƒÂ³n real.
3. Modela errores esperados.
4. Usa test data builders.
5. Encadena API con DB o UI para pruebas integradas.

## ExtensiÃƒÆ’Ã‚Â³n recomendada para este framework

Si quieres ver una versiÃƒÆ’Ã‚Â³n mÃƒÆ’Ã‚Â¡s madura, con mock API local, reglas de negocio y patrones `TDD`, `ATDD` y `BDD`, revisa `11-api-mocking-test-patterns.md`.
