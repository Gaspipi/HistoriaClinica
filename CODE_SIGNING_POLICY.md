# Code signing policy

**Free code signing provided by SignPath.io, certificate by SignPath Foundation.**

> **Estado: la firma de código está inactiva.** El proyecto está pendiente de
> aprobación de SignPath Foundation y la release publicada (`v2.0.0`) no está
> firmada. Esta política describe cómo se firmarán los binarios una vez aprobada
> la solicitud; ver [`docs/SIGNPATH.md`](docs/SIGNPATH.md).

Esta política se aplica a los binarios de Windows publicados desde
[Gaspipi/HistoriaClinica](https://github.com/Gaspipi/HistoriaClinica).

## Alcance

- `HistoriaClinica.exe`, el ejecutable de la aplicación.
- `HistoriaClinica-<version>-setup.exe`, el instalador de Windows.

Cuando la firma esté activa, el ejecutable se firmará antes de compilar el
instalador, de modo que la versión instalada también tenga firma Authenticode.
El instalador se firmará después de incluir el ejecutable firmado.

El desinstalador que genera Inno Setup no se firma en esta etapa. La aplicación
incluye un desinstalador estándar y no modifica la base de datos al desinstalar.

## Construcción y aprobación

- Los binarios se construyen en GitHub Actions sobre runners oficiales de
  GitHub (`windows-latest`) desde el repositorio público.
- La política de firma de release solo acepta builds de la rama `main` con
  verificación de origen de SignPath.
- Cada release requiere dos aprobaciones manuales en SignPath: una para el
  ejecutable y otra para el instalador.
- Las builds de pull requests y ramas de trabajo no usan el certificado de
  release.
- La versión de producto, la versión de archivo y el nombre de producto se
  verifican mediante la configuración de artefactos de SignPath.

## Roles

Proyecto mantenido por una sola persona:

- **Authors/Committers:** [Gaspar Ferreyra](https://github.com/Gaspipi)
- **Reviewers:** [Gaspar Ferreyra](https://github.com/Gaspipi). Todo cambio
  externo se revisa mediante pull request antes de integrarse.
- **Approvers:** [Gaspar Ferreyra](https://github.com/Gaspipi). Aprueba cada
  solicitud de firma en el panel de SignPath.

Todos los accesos a GitHub y SignPath deben usar autenticación multifactor.

## Privacidad

This program will not transfer any information to other networked systems
unless specifically requested by the user or the person installing or operating
it.

La aplicación guarda pacientes, fichas, usuarios y preferencias localmente en
SQLite y JSON. No incluye telemetría ni envía datos clínicos a servicios
externos. Los enlaces externos solo se abren cuando el usuario los selecciona.

## Verificación

Cuando la firma esté activa, los usuarios podrán verificarla desde Windows con:

```powershell
Get-AuthenticodeSignature .\HistoriaClinica-<version>-setup.exe
```

El resultado debe indicar `Status: Valid` y un firmante emitido por SignPath
Foundation. El archivo `.sha256` publicado junto al instalador permite verificar
la integridad de la descarga.
