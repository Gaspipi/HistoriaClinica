# HistoriaClinica - Contexto Optimizado para LLM
# HistoriaClinica - Contexto Optimizado para LLM

## Visión General
Sistema de gestión de pacientes e historias médicas. Avalonia 12 (.NET 10), EF Core 10 + SQLite, MVVM con CommunityToolkit.Mvvm, DI, arquitectura en capas (Core/App/Tests). Todo local, sin nube.

## Stack
- Target: net10.0 (Core), net10.0 WinExe (App)
- Nullable/ImplicitUsings habilitados. TreatWarningsAsErrors=true, NoWarn AVLN3001
- EF Core 10.0.13 (Sqlite)
- Avalonia 12.1.4 (Desktop/Themes.Fluent/Fonts.Inter)
- CommunityToolkit.Mvvm 8.4.2
- DI: Microsoft.Extensions.DependencyInjection 10.0.13
- xUnit (tests)
- PBKDF2-SHA256 para contraseñas

## Arquitectura
```
src/
├── HistoriaClinica.Core/  # Dominio: Models, Data, Repositories, Services, Security, Validation
└── HistoriaClinica.App/   # UI: Views (AXAML), ViewModels (MVVM), Services, Converters, Layout
tests/HistoriaClinica.Tests/
```

### Reglas Arquitectónicas (Críticas)
- `HistoriaContext` SOLO se crea con `IDbContextFactory<HistoriaContext>`. NO instanciar `DbContext` en ViewModels/Views.
- Repositorios son ÚNICAS capa que ejecuta consultas EF Core.
- ViewModels hablan con Services. Services orquestan reglas de negocio.
- Operaciones async con `await`, NUNCA fire-and-forget.
- Cultura es-ES, fechas formato dd/MM/yyyy. Usar `FechaClinica`.
- DI para ventanas (sin constructor sin parámetros).
- Login solo controla acceso. Pacientes/fichas compartidos. Sin cifrado de SQLite. Sin roles/recuperación.
- Primer arranque sin usuarios -> registro. Luego siempre login.


## Estructura Detallada

### Core/Models
- `Usuario.cs`: Id, Usuario (unique), PasswordHash, Salt, Iteraciones, CreadoEn
- `Paciente.cs`: Dni (string, PK 8 dígitos), Apellido, Nombre, FechaNacimiento (DateOnly?), Telefono, Celular, Domicilio, Localidad, ObraSocial, Antecedentes, MedicacionHabitual, CreadoEn, ModificadoEn
- `FichaDiaria.cs`: Id (int, identity), DniPaciente (FK->Paciente.Dni), Fecha (DateOnly), Hora (TimeOnly?), MotivoConsulta, EnfermedadActual, Indicaciones, CreadoEn, ModificadoEn

### Core/Data
- `HistoriaContext.cs`: DbSets Usuario/Paciente/FichaDiaria, configuraciones
- `DatabasePathProvider.cs`: Ruta por defecto `Documentos/Historia.db` (cross-platform). Override `HISTORIA_DB_PATH`
- `DatabaseInitializer.cs`: `EnsureCreatedAsync` SOLO para BD nueva. Luego ejecuta migrador. No toca Pacientes/FichasDiarias
- `DatabaseSchemaMigrator.cs`: Versionado con `PRAGMA user_version`. Migraciones SQL propias. `CurrentVersion`. Backup antes de migrar. Rechaza si BD > soportada

### Core/Repositories
- `IUsuarioRepository`, `UsuarioRepository`
- `IPacienteRepository`, `PacienteRepository` (incluye búsquedas)
- `IFichaRepository`, `FichaRepository`

### Core/Services
- `AuthService`: registro/login con PasswordHasher
- `PacienteService`: validación/normalización
- `FichaService`: lógica fichas

### Core/Security
- `PasswordHasher`: PBKDF2-SHA256, 210000 iteraciones, salt 16 bytes, comparación tiempo-constante

### Core/Validation
- `DniValidator`, `UsuarioValidator`, `FechaClinica`, validadores de entidades. Cultura es-ES

### App/ViewModels
- `ViewModelBase` (ObservableObject)
- `EditorViewModelBase<T>`: base para editores
- `AuthViewModel`: login/registro
- `MainWindowViewModel`: principal. Tiene búsqueda pacientes, ficha actual, atajos, etc. `EdadTexto`, `PacienteResumen`
- `PatientEditorViewModel`: alta/edición paciente
- `FichaEditorViewModel`: alta/edición ficha
- `PreferencesViewModel`, `DataMaintenanceViewModel`

### App/Services
- `IDialogService/DialogService`: diálogos modales
- `UserPreferencesService`: `preferences.json` junto a BD (tema System/Light/Dark, tamaño texto 100/115/130/150)
- `ClipboardService`
- `DataMaintenanceService`: backups/restauración

### App/Views
- `AuthWindow`, `MainWindow`, `PatientEditorWindow`, `FichaEditorWindow`, `PreferencesWindow`, `DataMaintenanceWindow`
- KeyBindings: MainWindow (Ctrl+N, Ctrl+Shift+N, Ctrl+E), editores (Ctrl+Enter)


## Flujo de Datos
1. App inicia -> DI configura servicios/repos/factories
2. Sin usuarios -> AuthWindow en modo registro. Con usuarios -> login
3. Login OK -> MainWindow. Carga pacientes, permite buscar/seleccionar, ver fichas en pestaña "Historias Clínicas"
4. Crear/editar paciente -> PatientEditorWindow (modal via DialogService)
5. Crear/editar ficha -> FichaEditorWindow (modal). Ctrl+Enter guarda
6. BD se inicializa con DatabaseInitializer (EnsureCreated solo nueva) + migrador

## Base de Datos
- Ruta: `Documentos/Historia.db` (fallback cross-platform). Env: `HISTORIA_DB_PATH`
- `preferences.json` misma carpeta que BD
- Versionado: `PRAGMA user_version`. Migraciones SQL en `DatabaseSchemaMigrator`. Backup en `Backups/` antes migrar
- Si versión BD > soportada: rechaza sin modificar
- `DateTime.MinValue` interpretado como "sin fecha nacimiento" (datos históricos)
- Login no cifra SQLite

## Comandos
```bash
dotnet build HistoriaClinica.sln -c Release  # debe pasar sin warnings/errores
dotnet test HistoriaClinica.sln -c Release   # 100 tests pasando
dotnet run --project src/HistoriaClinica.App
```

## Convenciones
- Español en UI/mensajes/comentarios nuevos
- Async/await, sin fire-and-forget
- MVVM con CommunityToolkit.Mvvm
- Repositorios únicos con EF Core
- IDbContextFactory obligatorio
- Fechas dd/MM/yyyy, cultura es-ES
- Nullable estricto
- No commitear/pushear/abrir PR sin pedido
- No tocar `Data/Historia.db`, `bin/`, `obj/`
- No agregar NuGet/cambiar TF sin confirmar

## Estado Actual
- Build: OK (Release, 0 warnings, 0 errors)
- Tests: 100/100 pasando
- Sin TODO/FIXME en código fuente
- Atajos ya implementados (Ctrl+N/Shift+N/E, Ctrl+Enter)
- Edad calculada existe (`EdadTexto`)
- Historial por paciente existe (pestaña "Historias Clínicas")
- Async void en handlers con try/catch

