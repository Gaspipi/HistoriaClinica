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

Si en el futuro se obtiene un certificado Authenticode, se reactivará el flujo
de firma y se actualizará este documento.
