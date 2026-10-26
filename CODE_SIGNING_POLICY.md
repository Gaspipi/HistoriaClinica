# Política de firma de código

**Estado: no hay firma Authenticode activa.**

SignPath Foundation rechazó la solicitud inicial en octubre de 2026 por falta de
señales públicas de adopción. El proyecto mantiene preparado el flujo técnico de
firma, pero no anuncia ni distribuye binarios firmados hasta contar con un
certificado emitido. Se puede reaplicar cuando el proyecto tenga más visibilidad
o usar un certificado Authenticode comercial.

Esta política describe el alcance y las reglas que se aplicarán cuando exista un
certificado. El estado operativo y el plan de reaplicación están en
[`docs/SIGNPATH.md`](docs/SIGNPATH.md).

## Alcance previsto

- `HistoriaClinica.exe`, el ejecutable de la aplicación.
- `HistoriaClinica-<version>-setup.exe`, el instalador de Windows.

El ejecutable se firmaría antes de compilar el instalador, de modo que la
versión instalada también tenga firma Authenticode. El instalador se firmaría
después de incluir el ejecutable firmado.

El desinstalador que genera Inno Setup no se firmaría en esta etapa. La
aplicación incluye un desinstalador estándar y no modifica la base de datos al
desinstalar.

## Construcción y aprobación

Cuando haya un certificado:

- Los binarios se construirán en GitHub Actions sobre runners oficiales de
  GitHub (`windows-latest`) desde el repositorio público.
- La política de firma de release solo aceptará builds de la rama `main` con
  verificación de origen.
- Cada release requerirá dos aprobaciones manuales: una para el ejecutable y
  otra para el instalador.
- Las builds de pull requests y ramas de trabajo no usarán el certificado de
  release.
- La versión de producto, la versión de archivo y el nombre de producto se
  verificarán mediante la configuración de artefactos.

## Roles

Proyecto mantenido por una sola persona:

- **Authors/Committers:** [Gaspar Ferreyra](https://github.com/Gaspipi)
- **Reviewers:** [Gaspar Ferreyra](https://github.com/Gaspipi). Todo cambio
  externo se revisa mediante pull request antes de integrarse.
- **Approvers:** [Gaspar Ferreyra](https://github.com/Gaspipi). Aprueba cada
  solicitud de firma.

Todos los accesos a GitHub y al proveedor de firma deben usar autenticación
multifactor.

## Privacidad

This program will not transfer any information to other networked systems
unless specifically requested by the user or the person installing or operating
it.

La aplicación guarda pacientes, fichas, usuarios y preferencias localmente en
SQLite y JSON. No incluye telemetría ni envía datos clínicos a servicios
externos. Los enlaces externos solo se abren cuando el usuario los selecciona.

## Verificación mientras no haya firma

Mientras no exista un certificado, verificá la integridad con el archivo
`.sha256` publicado junto a cada instalador:

```powershell
Get-FileHash .\HistoriaClinica-<version>-setup.exe -Algorithm SHA256
```

El hash debe coincidir con el archivo `.sha256` de la release. La ausencia de
firma Authenticode es esperada y puede provocar advertencias de SmartScreen.

Cuando exista un certificado, la verificación será:

```powershell
Get-AuthenticodeSignature .\HistoriaClinica-<version>-setup.exe
```

El resultado deberá indicar `Status: Valid` y el firmante correspondiente.
