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

## Pendiente

- Firma Authenticode del instalador y de los ejecutables.
- Publicación automática del artefacto en GitHub Releases.
- Validación visual del asistente en Windows con DPI y tema del sistema.
