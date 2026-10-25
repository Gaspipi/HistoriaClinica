# HistoriaClinica — Guía para agentes

Sistema de gestión de pacientes e historias médicas con UI **Avalonia 12**, .NET 10, EF Core 10 + SQLite y autenticación local, en C# con nullable e implicit usings habilitados. Arquitectura MVVM con proyectos separados de dominio, UI y tests.

## Comandos

```bash
# Compilar la solución completa
dotnet build HistoriaClinica.sln

# Ejecutar la aplicación (Linux, Windows o macOS)
dotnet run --project src/HistoriaClinica.App

# Ejecutar los tests
dotnet test HistoriaClinica.sln

# Generar el instalador de Windows (requiere Windows, .NET 10 e Inno Setup 6)
powershell -ExecutionPolicy Bypass -File installer/build-installer.ps1
```

- La app ya no depende de Windows Forms ni de `net6.0-windows`; se puede compilar y ejecutar en Linux.
- En Linux, Avalonia requiere `fontconfig`, X11 y GL del sistema. En NixOS usá `shell.nix` (`nix-shell --run "dotnet run --project src/HistoriaClinica.App"`).
- La validación visual definitiva en Windows sigue siendo responsabilidad del usuario.
- No modifiques `Data/Historia.db` (está versionada), `bin/` ni `obj/`.

## Estructura

| Ruta | Rol |
| --- | --- |
| `src/HistoriaClinica.Core/Models/` | Entidades `Usuario`, `Paciente` y `FichaDiaria`; PK `Dni` de 8 dígitos e `Id` identity para fichas. |
| `src/HistoriaClinica.Core/Data/` | `HistoriaContext`, `DatabasePathProvider`, `DatabaseInitializer`, `DatabaseSchemaMigrator`. |
| `src/HistoriaClinica.Core/Repositories/` | Acceso a datos EF Core (`IUsuarioRepository`, `IPacienteRepository`, `IFichaRepository`). |
| `src/HistoriaClinica.Core/Services/` | Reglas de negocio, normalización y validación (`AuthService`, `PacienteService`, `FichaService`). |
| `src/HistoriaClinica.Core/Security/` | `PasswordHasher` con PBKDF2-SHA256. |
| `src/HistoriaClinica.Core/Validation/` | Validadores de DNI, usuario, fechas y entidades; cultura `es-ES`. |
| `src/HistoriaClinica.App/` | Aplicación Avalonia: `Program.cs`, `App.axaml`, `Views/`, `ViewModels/`, `Services/`, `Converters/`, `Assets/Icons/`. |
| `tests/HistoriaClinica.Tests/` | Tests xUnit de autenticación, validación, rutas y persistencia con SQLite temporal. |
| `installer/` | Inno Setup, script de build `win-x64`, firma SignPath y documentación del instalador. |
| `.github/workflows/` | CI de tests, build sin firma (`windows-installer.yml`) y release firmada (`windows-signed-release.yml`). |
| `.signpath/` | Configuraciones de artefactos para firmar el ejecutable y el instalador. |
| `CODE_SIGNING_POLICY.md` | Política pública de firma requerida por SignPath Foundation. |
| `shell.nix` | Entorno Nix con `fontconfig`, X11 y GL para ejecutar Avalonia en NixOS. |
| `Data/Historia.db` | Archivo vacío versionado como referencia; la app usa `Documentos/Historia.db` (o `HISTORIA_DB_PATH`). |

## Convenciones

- UI, mensajes y comentarios nuevos en español; mantené el tono existente.
- `HistoriaContext` se crea únicamente mediante `IDbContextFactory`; no instancies `DbContext` en ViewModels ni en Views.
- Los repositorios son la única capa que ejecuta consultas EF Core. Los ViewModels hablan con servicios.
- El login solo controla el acceso: pacientes y fichas son compartidos y la base no está cifrada.
- El primer arranque sin usuarios muestra registro; después, siempre login. No hay roles, recuperación ni “recordarme”.
- Operaciones async con `await`, nunca fire-and-forget. `EnsureCreatedAsync` se ejecuta solo al crear una base nueva (`DatabaseInitializer`).
- Fechas con formato `dd/MM/yyyy`; parseo/formato explícito con `FechaClinica` y cultura `es-ES`.
- Usá MVVM (`CommunityToolkit.Mvvm`): comandos, propiedades observables y validación en ViewModels. Los diálogos pasan por `IDialogService`.
- Respetá nullable: inicializá propiedades no anulables y evitá `!` innecesarios.
- No agregues paquetes NuGet ni cambies el target framework sin confirmar.
- No commitees, hagas push ni abras PR salvo pedido explícito.

## Base de datos

- Ruta por defecto: `Documentos/Historia.db`, con fallback cross-platform.
- Override opcional: variable de entorno `HISTORIA_DB_PATH` con la ruta completa al archivo.
- Preferencias de UI en `preferences.json`, en la misma carpeta que la base; guarda el tema (`System`, `Light` o `Dark`) y el tamaño de texto (`100`, `115`, `130` o `150 %`).
- La UI se adapta por ancho y alto con clases `compact`, `narrow` y `short`; `ResponsiveGrid` reacomoda formularios y la escala de texto se aplica en caliente desde Preferencias.
- El esquema se versiona con `PRAGMA user_version` y migraciones SQL en `DatabaseSchemaMigrator`; no hay EF Migrations.
- Para cambiar el modelo, agregá una migración y aumentá `DatabaseSchemaMigrator.CurrentVersion`; `EnsureCreated` no actualiza esquemas existentes.
- Antes de migrar una base existente se crea un backup en `Backups/`; si la versión de la base es mayor que la soportada, se rechaza sin modificarla.
- `DatabaseInitializer` usa `EnsureCreated` solo para bases nuevas y luego ejecuta el migrador; no toca datos de `Pacientes` ni `FichasDiarias`.
- Credenciales: PBKDF2-SHA256, 210.000 iteraciones, sal de 16 bytes y comparación de tiempo constante. El login no cifra el archivo SQLite.

## Deuda técnica conocida

- Migrador propio, sin EF Migrations; falta probar concurrencia y una copia real anonimizada.
- `DateTime.MinValue` se interpreta como "sin fecha de nacimiento" para no romper datos históricos.
- Logging solo a consola; no hay archivo de log ni telemetría.
- Avalonia 12.1.4 fija `Tmds.DBus.Protocol` 0.95.1 para evitar un cierre con excepción en Wayland.
- Las ventanas se crean con DI y no exponen constructor sin parámetros; el build emite 6 warnings `AVLN3001` esperados.
- Falta validación visual en Windows de la UI (tema, icono, DPI y pantallas chicas).
- El instalador no está firmado y todavía no se validó en Windows; el rollback es manual abriendo `app\<version>`.
- Sin recuperación de contraseña ni gestión de usuarios; un solo usuario inicial.
