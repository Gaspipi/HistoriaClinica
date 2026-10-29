# Cambios — Restauración de backups con diálogo de archivo

> Generado el 2026-10-29. Documenta los cambios realizados en esta sessión para que
> el agente de opencode (u otro agente continue) pueda retomar el trabajo.

## Estado actual

- **Build**: `dotnet build HistoriaClinica.sln -c Release` → 0 errores, 0 warnings.
- **Tests**: `dotnet test HistoriaClinica.sln -c Release` → 118/118 pasando.
- **Rama**: `docs/comunidad` con un merge en progreso (branch `docs/comunidad` ← `main`).
  El `git diff HEAD` muestra ~1173 líneas porque incluye cambios del merge y de otros
  agentes. **Mis cambios son solo los 5 archivos listados abajo.**

## Cambios realizados

### 1. `IDialogService` — nuevo método de selección de archivo

**Archivo**: `src/HistoriaClinica.App/Services/IDialogService.cs`

Agrega `ShowOpenFileDialogAsync` para permitir al usuario seleccionar un archivo
de backup arbitrario (no solo desde la lista de respaldos generados por la app).

```csharp
Task<string?> ShowOpenFileDialogAsync(
    string titulo,
    IReadOnlyList<FilePickerFileType>? fileTypes = null,
    CancellationToken cancellationToken = default);
```

Agrega `using Avalonia.Platform.Storage;`.

### 2. `DialogService` — implementación

**Archivo**: `src/HistoriaClinica.App/Services/DialogService.cs`

Implementa `ShowOpenFileDialogAsync` usando `TopLevel.StorageProvider.OpenFilePickerAsync`
con `AllowMultiple = false`. Usa `FilePickerOpenOptions` (no `FileTypes` — esa propiedad
no existe en esta versión de Avalonia; se pasan solo `Title` y `AllowMultiple`).

Agrega `using Avalonia.Platform.Storage;`.

### 3. `DataMaintenanceViewModel` — comando de restauración desde archivo

**Archivo**: `src/HistoriaClinica.App/ViewModels/DataMaintenanceViewModel.cs`

Nuevo comando `RestaurarBackupDesdeArchivoAsync` (`[RelayCommand]`):

1. Abre `ShowOpenFileDialogAsync("Seleccionar respaldo", fileTypes)` con filtros:
   - `*.db`, `*.db.bak`, `*.bak`
   - `*.db`
2. Si el usuario cancela, regresa.
3. Muestra `ConfirmAsync` destructiva con el nombre del archivo.
4. Llama `_maintenance.RestaurarBackupAsync(ruta)`.
5. Muestra mensaje de éxito y recarga la lista de backups.
6. Cierra la ventana (`CloseRequested`).

También agrega `using Avalonia.Platform.Storage;`.

### 4. `DataMaintenanceWindow.axaml` — botón en UI

**Archivo**: `src/HistoriaClinica.App/Views/DataMaintenanceWindow.axaml`

Agrega botón "Restaurar desde archivo..." al lado de "Cargar" y "Restaurar":

```xml
<Button Content="Restaurar desde archivo..." Command="{Binding RestaurarBackupDesdeArchivoCommand}"
        automation:AutomationProperties.Name="Restaurar respaldo desde archivo" />
```

También agrega `xmlns:models="using:HistoriaClinica.Core.Models"` para el
DataTemplate de `AuditLog`.

### 5. `TestDoubles.cs` — fake del nuevo método

**Archivo**: `tests/HistoriaClinica.Tests/TestDoubles.cs`

Agrega a `FakeDialogService`:

```csharp
public string? RutaApertura { get; set; }
public int AperturasSolicitadas { get; private set; }

public Task<string?> ShowOpenFileDialogAsync(...) { ... }
```

Agrega `using Avalonia.Platform.Storage;`.

## Cómo probar

```bash
dotnet build HistoriaClinica.sln -c Release
dotnet test HistoriaClinica.sln -c Release
```

## Pendientes (NO tocados en esta sessión)

El usuario pidió implementar 5 características adicionales. Fueron planificadas
por el agente `analista` pero **no implementadas**:

1. **Exportar PDF** — depende de herramientas externas por SO (sin nuevos NuGet).
2. **Empaquetado Linux/macOS** — solo Windows con Inno Setup actualmente.
3. **Archivar pacientes + soft-delete de fichas** — requiere migración v8
   (`DatabaseSchemaMigrator.CurrentVersion` actual es 7). Nuevas columnas:
   `Pacientes.Archivado`, `Pacientes FechaArchivado`, `Pacientes.MotivoArchivo`,
   `FichasDiarias.EliminadaLogica`, `FichasDiarias FechaEliminacionLogica`,
   `FichasDiarias.MotivoEliminacion`, `FichasDiarias.EliminadaPor`.
4. **Cifrado de backups** — AES-256 + PBKDF2 (sin SQLCipher). Requiere changes
   en `SqliteBackupService`, `DataMaintenanceService` y UI para pedir contraseña.
5. **Bloqueo por inactividad** — `InactivityService` + timer en `App.axaml.cs`,
   preferencias en `UserPreferencesService`, reset con eventos de `MainWindow`.

Ver plan completo en el resultado del agente `analista` (sesión `ses_e7acb2cd8ffe5POPn8FCNHtRJd`).

## Convenciones respetadas

- Español en UI, comentarios y mensajes.
- `async/await`, sin fire-and-forget.
- MVVM con `CommunityToolkit.Mvvm`.
- `IDbContextFactory` obligatorio (no se usó `DbContext` directamente).
- Nullable estricto.
- Sin nuevos paquetes NuGet.
- Sin cambios en `Data/Historia.db`, `bin/`, `obj/`.
- Tests pasando (118/118).