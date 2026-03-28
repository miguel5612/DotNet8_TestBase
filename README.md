# Framework Base - Test Automation .NET

Este repositorio convierte el contenido del PDF `Test Automation dotnet-V1 (1).pdf` en una soluciÃƒÂ³n prÃƒÂ¡ctica de automatizaciÃƒÂ³n con `.NET`, cubriendo `web`, `api` y `desktop`, mÃƒÂ¡s una documentaciÃƒÂ³n progresiva en Markdown para estudiar desde fundamentos hasta nivel avanzado.

## QuÃƒÂ© incluye

- Framework por capas con proyectos separados para `Core`, `Infrastructure`, `Web`, `Api` y `Desktop`.
- AutomatizaciÃƒÂ³n web con `Selenium`.
- AutomatizaciÃƒÂ³n API con `HttpClient` y `RestSharp`.
- Base de automatizaciÃƒÂ³n desktop en Windows con `Appium/WinAppDriver`.
- Pruebas de ejemplo con `NUnit`.
- Ejemplos BDD con `SpecFlow`.
- Pipelines de ejemplo para `GitHub Actions`, `Azure DevOps` y `Jenkins`.
- DocumentaciÃƒÂ³n funcional en `docs/` con trazabilidad directa al PDF.

- Mock API local con escenarios de `banking` y `retail` para validar `status code`, `body` y reglas de negocio.
- Ejemplos `TDD`, `ATDD` y `BDD` con `NUnit` y `SpecFlow`.

## Estructura

```text
src/
  FrameworkBase.Automation.Core/
  FrameworkBase.Automation.Infrastructure/
  FrameworkBase.Automation.Web/
  FrameworkBase.Automation.Api/
  FrameworkBase.Automation.Desktop/
tests/
  Common/
  FrameworkBase.Automation.Web.Tests/
  FrameworkBase.Automation.Api.Tests/
  FrameworkBase.Automation.Desktop.Tests/
docs/
```

## Inicio rÃƒÂ¡pido

1. Revisa primero [docs/00-indice.md](docs/00-indice.md).
2. Ajusta `automation.settings.json` segÃƒÂºn tu entorno.
3. Puedes elegir browser sin editar JSON usando `BROWSER=chrome`, `BROWSER=edge` o `BROWSER=firefox`.
4. Si ya tienes drivers locales, usa `CHROMEDRIVER_PATH`, `EDGEDRIVER_PATH` o `GECKODRIVER_PATH`.
5. Restaura paquetes con `dotnet restore FrameworkBase.Automation.sln`.
6. Compila con `dotnet build FrameworkBase.Automation.sln -m:1`.
7. Ejecuta por categorÃƒÂ­a:
   - `dotnet test tests/FrameworkBase.Automation.Api.Tests/FrameworkBase.Automation.Api.Tests.csproj --filter "Category=Api"`
   - `dotnet test tests/FrameworkBase.Automation.Web.Tests/FrameworkBase.Automation.Web.Tests.csproj --filter "Category=Web"`
   - `dotnet test tests/FrameworkBase.Automation.Desktop.Tests/FrameworkBase.Automation.Desktop.Tests.csproj --filter "Category=Desktop"`

## EjecuciÃƒÂ³n multi-browser

- Chrome:
  `$env:BROWSER='chrome'; dotnet test tests/FrameworkBase.Automation.Web.Tests/FrameworkBase.Automation.Web.Tests.csproj --configuration Release --no-build --filter "Category=Web" -m:1`
- Edge:
  `$env:BROWSER='edge'; $env:EDGEDRIVER_PATH='C:\Users\Usuario\AppData\Local\Microsoft\WinGet\Packages\Microsoft.EdgeDriver_Microsoft.Winget.Source_8wekyb3d8bbwe\msedgedriver.exe'; dotnet test tests/FrameworkBase.Automation.Web.Tests/FrameworkBase.Automation.Web.Tests.csproj --configuration Release --no-build --filter "Category=Web" -m:1`
- Firefox:
  `$env:BROWSER='firefox'; dotnet test tests/FrameworkBase.Automation.Web.Tests/FrameworkBase.Automation.Web.Tests.csproj --configuration Release --no-build --filter "Category=Web" -m:1`

## Desktop con Appium

1. Instala Node.js en su versiÃƒÆ’Ã‚Â³n mÃƒÆ’Ã‚Â¡s reciente y verifica que `node -v` y `npm -v` respondan sin error.
2. Instala Appium fijando la major:
   `npm install -g appium@3`
3. No uses `npm install -g appium` a secas en este setup. Deja la major explÃƒÆ’Ã‚Â­cita para evitar terminar con Appium 2 por un tag o cachÃƒÆ’Ã‚Â© previo del entorno.
4. Instala el driver de Windows que usa este repo:
   `appium driver install --source=npm appium-novawindows-driver`
5. Arranca Appium:
   `powershell -ExecutionPolicy Bypass -File scripts/start-appium.ps1`
6. Ejecuta desktop:
   `dotnet test tests/FrameworkBase.Automation.Desktop.Tests/FrameworkBase.Automation.Desktop.Tests.csproj --configuration Release --no-build --filter "Category=Desktop" -m:1`
7. Si WinAppDriver no levanta, verifica que Windows tenga habilitado Developer Mode.
8. Si WinAppDriver ya corre aparte, define `Desktop.WinAppDriverUrl` para que Appium reuse ese backend y no intente arrancar otro.
9. Para validar Appium, el puerto WAD y el proceso real que lo ocupa, ejecuta:
   `powershell -ExecutionPolicy Bypass -File scripts/check-desktop-driver.ps1`

## Nota importante sobre desktop

El PDF extraÃƒÂ­do no nombra una tecnologÃƒÂ­a concreta para automatizaciÃƒÂ³n desktop. Para materializar ese frente con las tecnologÃƒÂ­as del ecosistema `.NET` se eligiÃƒÂ³ `Appium/WinAppDriver`, porque conserva el enfoque de driver, capas y objetos de pantalla compatible con el resto del framework.

El helper de Appium quedÃƒÂ³ en [scripts/start-appium.ps1](scripts/start-appium.ps1), usando la ruta de `WinAppDriver.exe` extraÃƒÂ­da localmente.

La configuraciÃƒÂ³n desktop soporta ademÃƒÂ¡s estas opciones en `automation.settings.json`:

- `Desktop.WinAppDriverUrl`: URL de un WinAppDriver ya levantado externamente, por ejemplo `http://127.0.0.1:4790/wd/hub`.
- `Desktop.SystemPort`: puerto que Appium usarÃƒÂ¡ para arrancar WinAppDriver cuando no se defina `WinAppDriverUrl`.
- `Desktop.CreateSessionTimeoutMilliseconds` y `Desktop.WaitForAppLaunchSeconds`: tolerancias de arranque para aplicaciones UWP como Calculator.

## Ruta de lectura recomendada

1. [docs/00-indice.md](docs/00-indice.md)
2. [docs/01-trazabilidad-pdf.md](docs/01-trazabilidad-pdf.md)
3. [docs/02-fundamentos-testing.md](docs/02-fundamentos-testing.md)
4. [docs/03-csharp-sql-y-net.md](docs/03-csharp-sql-y-net.md)
5. [docs/04-arquitectura-framework.md](docs/04-arquitectura-framework.md)
6. [docs/05-automatizacion-web.md](docs/05-automatizacion-web.md)
7. [docs/06-automatizacion-api.md](docs/06-automatizacion-api.md)
8. [docs/07-automatizacion-desktop.md](docs/07-automatizacion-desktop.md)
9. [docs/08-bdd-runners-reporting.md](docs/08-bdd-runners-reporting.md)
10. [docs/09-ci-cd-quality-gates.md](docs/09-ci-cd-quality-gates.md)
11. [docs/10-ruta-0-a-experto.md](docs/10-ruta-0-a-experto.md)
12. [docs/11-api-mocking-test-patterns.md](docs/11-api-mocking-test-patterns.md)
