# Arquitectura

Este documento explica las capas y los flujos principales para quien se suma al
código. Para las reglas operativas y los comandos, ver
[`CONTRIBUTING.md`](../CONTRIBUTING.md); para el despliegue y las migraciones,
[`DEPLOYMENT.md`](DEPLOYMENT.md).

## Principios

- La aplicación funciona sin servicios en la nube y sin telemetría.
- Los datos clínicos viven en un archivo SQLite local.
- El login controla el acceso a la aplicación, pero no cifra la base.
- La UI es Avalonia + MVVM; la lógica de datos queda fuera de los ViewModels.
- El esquema se versiona con `PRAGMA user_version` y migraciones SQL propias.
- Las operaciones de datos son asíncronas y observadas con `await`.

## Proyectos y dependencias

```text
HistoriaClinica.App  --->  HistoriaClinica.Core
        |                         |
        |                         +-- Models
        |                         +-- Data
        |                         +-- Repositories
        |                         +-- Services
        |                         +-- Validation
        |                         +-- Security
        |
        +-- Views
        +-- ViewModels
        +-- Services de UI
        +-- Layout y estilos
```

`App` referencia a `Core`. `Core` no conoce Avalonia ni los ViewModels.

`tests/HistoriaClinica.Tests` referencia ambos proyectos y prueba servicios,
repositorios, ViewModels y persistencia con bases temporales.

## Capas de Core

| Carpeta | Responsabilidad |
| --- | --- |
| `Models/` | Entidades `Usuario`, `Paciente` y `FichaDiaria`. |
| `Data/` | `HistoriaContext`, ruta de la base, inicialización y migrador. |
| `Repositories/` | Consultas y escrituras EF Core. Única capa que usa el contexto. |
| `Services/` | Reglas de negocio, autenticación y coordinación de repositorios. |
| `Validation/` | Validación de DNI, usuarios, fechas y entidades. |
| `Security/` | Hash y verificación de contraseñas con PBKDF2-SHA256. |

Regla práctica: si una consulta necesita `HistoriaContext`, va en un repositorio;
si es una regla de negocio, va en un servicio.

## Capas de App

| Carpeta | Responsabilidad |
| --- | --- |
| `Views/` | Ventanas y diálogos Avalonia. Sin lógica de negocio. |
| `ViewModels/` | Estado, comandos, validación de formularios y llamadas a servicios. |
| `Services/` | Navegación, diálogos, portapapeles, sesión, preferencias y mantenimiento. |
| `Layout/` | Clases responsive, reacomodo de formularios y tamaños de ventana. |
| `Converters/` | Conversión de fechas y textos para binding. |
| `Styles/` | Tema, métricas y estilos de controles. |
| `Assets/Icons/` | Iconos empaquetados como recursos Avalonia. |

Los ViewModels no crean `DbContext`, no abren SQLite y no acceden a controles de
Avalonia directamente. Los diálogos pasan por `IDialogService` y la navegación
por `IAppNavigator`.

## Arranque

1. `Program.Main` configura la cultura `es-ES` y arranca Avalonia.
2. `App.OnFrameworkInitializationCompleted` arma el contenedor de dependencias.
3. `AddHistoriaClinicaCore()` registra la ruta de la base, el
   `IDbContextFactory<HistoriaContext>`, repositorios y servicios.
4. Se registran los servicios de UI y se inicializan las preferencias.
5. `AppNavigator.ShowAuth()` muestra el login o el registro del primer usuario.
6. Después del login, `AuthViewModel` navega a `MainWindow`.

La `MainWindow` no se crea hasta que la autenticación termina.

## Flujo de datos

Ejemplo: buscar un paciente por DNI.

```text
MainWindow
  -> MainWindowViewModel
    -> PacienteService
      -> IPacienteRepository
        -> HistoriaContext (EF Core)
          -> SQLite
```

- El ViewModel valida la entrada y expone estado observable.
- El servicio normaliza y aplica reglas de negocio.
- El repositorio ejecuta la consulta EF Core.
- La base se abre mediante `IDbContextFactory`, no con un contexto compartido de
  larga vida.

## Autenticación y sesión

- `AuthService` registra solo el primer usuario y valida credenciales.
- `PasswordHasher` usa PBKDF2-SHA256, 210.000 iteraciones, sal de 16 bytes y
  comparación de tiempo constante.
- `UserSession` guarda el usuario autenticado en memoria.
- `IAppNavigator` intercambia `AuthWindow` y `MainWindow`.
- Cerrar sesión limpia la sesión y vuelve al login.
- Pacientes y fichas son compartidos entre todos los usuarios de una base.

## Base de datos y migraciones

- La ruta por defecto es `Documentos/Historia.db`, con fallback multiplataforma.
- `HISTORIA_DB_PATH` permite usar otra ruta.
- `DatabaseInitializer` usa `EnsureCreatedAsync` solo cuando la base es nueva.
- `DatabaseSchemaMigrator` aplica migraciones SQL en una transacción y actualiza
  `PRAGMA user_version`.
- Antes de migrar una base existente se crea un backup en `Backups/`.
- Si la base tiene una versión mayor que la soportada, se rechaza sin modificarla.
- `DatabaseSchemaMigrator.CurrentVersion` debe aumentarse al agregar una
  migración.

Para cambiar el modelo, ver la sección correspondiente en
[`CONTRIBUTING.md`](../CONTRIBUTING.md).

## Interfaz responsive

- Las ventanas aplican clases `compact`, `narrow` y `short` según ancho y alto.
- `ResponsiveGrid` reacomoda formularios.
- `UiMetrics` escala tipografía, alturas e iconos según la preferencia de texto.
- La escala de texto se aplica en caliente desde Preferencias.
- Las pruebas visuales deben cubrir tema claro, oscuro y tamaños chicos.

## Errores y logging

- Los errores esperables de validación se muestran en la UI.
- Los errores de infraestructura se registran con `ILogger`.
- El logging actual va solo a consola; no hay archivo de log ni telemetría.
- No registrar datos clínicos ni credenciales.

## Tests

- `tests/HistoriaClinica.Tests` usa xUnit.
- `TestDatabase` crea bases SQLite temporales.
- Los tests no dependen de `Documentos` ni de la base del usuario.
- Hay pruebas de autenticación, validación, hashing, rutas, migraciones,
  repositorios, ViewModels, preferencias y mantenimiento.
- La CI ejecuta build y tests en Linux y Windows para pull requests.

## Dónde extender

| Cambio | Punto de entrada |
| --- | --- |
| Nuevo campo de paciente | `Paciente`, validador, repositorio, servicio, editor y migración. |
| Nueva pantalla | View + ViewModel + registro en DI + navegación. |
| Nueva regla de negocio | Servicio de Core + tests. |
| Nueva consulta | Repositorio + tests con `TestDatabase`. |
| Nuevo diálogo | `IDialogService` + ventana + ViewModel. |
| Nuevo ajuste visual | `Styles/`, `Layout/` y prueba responsive. |

Antes de una implementación grande, abrí un issue o una discusión para acordar
el alcance.
