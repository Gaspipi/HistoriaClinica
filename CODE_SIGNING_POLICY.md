# Política de firma de código

**Estado: la aplicación se distribuye sin firma digital (sin Authenticode).**

No hay certificado activo ni flujo de firma en uso. Los instaladores se publican
tal como los genera Inno Setup. Windows SmartScreen puede mostrar una advertencia
al ejecutarlos; el usuario debe elegir "Más información" → "Ejecutar de todas
formas".

## Verificación de integridad

Cada release incluye un archivo `SHA256SUMS.txt`. Verificá el hash del
instalador antes de instalarlo:

- Windows (PowerShell): `Get-FileHash .\HistoriaClinica-<version>-setup.exe -Algorithm SHA256`
- Linux/macOS: `sha256sum HistoriaClinica-<version>-setup.exe`

Compará el resultado con el valor publicado en `SHA256SUMS.txt`.

## Reaplicación futura

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

Si en el futuro se obtiene un certificado Authenticode, se reactivará el flujo
de firma y se actualizará este documento.
