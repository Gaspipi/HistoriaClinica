# Instalador de Windows

El instalador usa **Inno Setup 6** y se genera con `build-installer.ps1`.

## Requisitos

- Windows.
- .NET 10 SDK.
- [Inno Setup 6](https://jrsoftware.org/isdl.php) instalado, o `-InnoSetupPath` apuntando a `ISCC.exe`.

## Generar

Desde la raíz del repositorio:

```powershell
powershell -ExecutionPolicy Bypass -File installer/build-installer.ps1
```

Si Inno Setup está en una ruta no estándar:

```powershell
powershell -ExecutionPolicy Bypass -File installer/build-installer.ps1 -InnoSetupPath "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
```

El resultado queda en:

```text
artifacts/installer/HistoriaClinica-<version>-setup.exe
artifacts/installer/HistoriaClinica-<version>-setup.exe.sha256
```

Para el flujo de firma en dos fases:

```powershell
# 1. Publicar solo el ejecutable
./installer/build-installer.ps1 -SkipInstaller

# 2. Firmar HistoriaClinica.exe y compilar el instalador con ese binario
./installer/build-installer.ps1 -SkipPublish
```

## Comportamiento

- Instalación por usuario en `%LOCALAPPDATA%\Programs\HistoriaClinica`, sin UAC.
- Los binarios se guardan en `app\<version>` y los accesos directos apuntan a la versión recién instalada.
- Si ya existía un acceso directo en el escritorio, se actualiza aunque no se vuelva a marcar la tarea.
- Los accesos fijados a la barra de tareas pueden seguir apuntando a la versión anterior; hay que volver a fijarlos.
- No instala ni sobrescribe `Historia.db` ni `preferences.json`.
- Antes de instalar, si existe la base, agrega un backup en `Backups/` junto a ella.
- Respeta `HISTORIA_DB_PATH` para ubicar la base si la variable está definida.
- Aborta si ya hay una ventana de Historia Clínica abierta.
- Al desinstalar elimina los binarios, pero no los datos.

## Firma (preparada, inactiva)

**No hay firma Authenticode activa.** SignPath Foundation rechazó la solicitud
inicial por falta de adopción pública. El workflow `Windows signed release`
quedó preparado para una futura aprobación o para un certificado comercial, pero
no debe ejecutarse sin certificado y secretos.

El flujo previsto firma primero `HistoriaClinica.exe`, compila el instalador con
ese ejecutable y después firma el instalador. Cada release requeriría dos
aprobaciones manuales.

Inno Setup rellena `ProductName`, `ProductVersion` y `FileVersion` con espacios;
el workflow normaliza esos campos con rcedit 2.0.0 antes de enviar el
instalador, para que las restricciones de metadatos sean exactas.

- Estado y plan: [`CODE_SIGNING_POLICY.md`](../CODE_SIGNING_POLICY.md).
- Procedimiento y reaplicación: [`docs/SIGNPATH.md`](../docs/SIGNPATH.md).
- Mientras no haya firma, verificá el `.sha256` publicado.
- El desinstalador que genera Inno Setup no se firma en esta etapa.
- SmartScreen puede advertir hasta que el binario gane reputación.

## Pendiente

- Decidir la vía de firma y reaplicar cuando haya más adopción.
- Publicar `v2.0.1` sin firma si se decide continuar.
- Validación visual del asistente en Windows con DPI y tema del sistema.
