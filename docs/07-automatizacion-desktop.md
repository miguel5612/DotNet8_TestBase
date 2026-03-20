# AutomatizaciÃƒÂ³n desktop

El usuario pidiÃƒÂ³ explÃƒÂ­citamente cubrir desktop. El PDF no especifica la librerÃƒÂ­a, asÃƒÂ­ que esta implementaciÃƒÂ³n toma una decisiÃƒÂ³n tÃƒÂ©cnica coherente con el ecosistema `.NET`: `Appium/WinAppDriver`.

## ImplementaciÃƒÂ³n del repo

- `WindowsApplicationSessionFactory`
  Crea la sesiÃƒÂ³n contra un servidor Appium/WinAppDriver.
- `CalculatorScreen`
  Modela acciones y lectura de resultados en Windows Calculator.
- `tests/FrameworkBase.Automation.Desktop.Tests`
  Deja un smoke test y un escenario BDD.

## Por quÃƒÂ© Calculator

Porque es:

- estÃƒÂ¡ndar en Windows,
- suficientemente simple,
- ÃƒÂºtil para demostrar localizadores por `AccessibilityId`.

## Requisitos de ejecuciÃƒÂ³n

- Windows.
- Servicio WinAppDriver o Appium con soporte Windows.
- AplicaciÃƒÂ³n objetivo accesible por `ApplicationId`.
- `APPIUM_WAD_PATH` apuntando a `WinAppDriver.exe` si no estÃƒÂ¡ instalado en la ruta por defecto.
- Windows Developer Mode habilitado.

En este repo se dejÃƒÂ³ ademÃƒÂ¡s un script de arranque para Appium en `scripts/start-appium.ps1`.

## Buenas prÃƒÂ¡cticas desktop

- Usa `AccessibilityId` antes que coordenadas.
- AÃƒÂ­sla acciones por pantalla.
- No mezcles setup de sesiÃƒÂ³n con asserts.
- Controla bien el ciclo de vida del proceso.
- Documenta prerequisitos del ambiente.

## Riesgos reales

- Cambios de versiÃƒÂ³n de la app.
- Variaciones del sistema operativo.
- Localizadores dependientes del idioma.
- Sesiones huÃƒÂ©rfanas cuando un test falla a mitad.
