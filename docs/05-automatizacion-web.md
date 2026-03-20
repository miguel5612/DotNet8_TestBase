# AutomatizaciÃƒÂ³n web

La parte web del PDF aterriza en Selenium y en cÃƒÂ³mo organizar una soluciÃƒÂ³n mantenible.

## ImplementaciÃƒÂ³n del repo

- `WebDriverFactory`
  Selecciona navegador y opciones comunes.
- `SeleniumWebFormPage`
  Encapsula los elementos y acciones de la pÃƒÂ¡gina demo.
- `WebFormFlow`
  Expresa una intenciÃƒÂ³n funcional: completar y enviar el formulario.
- `tests/FrameworkBase.Automation.Web.Tests`
  Ejecuta el caso con NUnit y tambiÃƒÂ©n con SpecFlow.
- `tests/FrameworkBase.Automation.Web.Tests/TestAssets/web-form.html`
  Elimina la dependencia de una URL pÃƒÂºblica y deja la demo web auto contenida.

## Requisito operativo

La pÃƒÂ¡gina de prueba ya es local, pero Selenium sigue necesitando un driver de navegador disponible en la mÃƒÂ¡quina o acceso saliente para Selenium Manager.

Este repo soporta tres formas de elecciÃƒÂ³n:

- `automation.settings.json`
- variable de entorno `BROWSER`
- variables `CHROMEDRIVER_PATH`, `EDGEDRIVER_PATH`, `GECKODRIVER_PATH`

## Por quÃƒÂ© no meter todo en el test

Porque el test debe decir quÃƒÂ© valida, no cÃƒÂ³mo encuentra cada control.

Malo:

- driver
- selector
- click
- assert
- selector
- click
- assert

Bien:

- el test invoca un flujo,
- el flujo coordina acciones,
- la pÃƒÂ¡gina conoce selectores.

## Buenas prÃƒÂ¡cticas web

- Usa selectores estables.
- Evita sleeps fijos.
- Centraliza configuraciÃƒÂ³n de browser.
- Separa smoke de regresiÃƒÂ³n.
- Captura evidencia cuando falle un paso crÃƒÂ­tico.

## Riesgos comunes

- Dependencia de datos no controlados.
- Selectores frÃƒÂ¡giles.
- Pruebas enormes de punta a punta.
- Reutilizar pÃƒÂ¡ginas que en realidad son flujos.

## CÃƒÂ³mo crecer desde aquÃƒÂ­

1. Agrega waits explÃƒÂ­citos por condiciÃƒÂ³n.
2. Captura screenshots en error.
3. Parametriza browsers y ambientes.
4. Integra tags para ejecuciÃƒÂ³n selectiva.
5. AÃƒÂ±ade parallel execution con aislamiento real de datos.
