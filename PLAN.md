# Plan: login, header de usuario, paquete 1 y UI responsive 2.0

**Estado:** versión 2.0, migrador de esquema e instalador Windows implementados; PR #6 abierto. Quedan la firma y validación del instalador, el escaneo de vulnerabilidades y la validación visual en Windows.

## Progreso de implementación (23/10/2026)

### Completado

- Core de autenticación:
  - `Usuario` con usuario normalizado, hash, sal, iteraciones y fecha de creación.
  - `PasswordHasher` PBKDF2-SHA256 con 210.000 iteraciones, sal de 16 bytes, hash de 32 bytes y comparación de tiempo constante.
  - `IUsuarioRepository` / `UsuarioRepository`.
  - `AuthService` con `HasUsersAsync`, `RegisterFirstUserAsync` y `LoginAsync`.
  - `HistoriaContext` con `DbSet<Usuario>`.
  - `DatabaseInitializer` crea bases nuevas y delega el esquema en `DatabaseSchemaMigrator`.
  - `DatabaseSchemaMigrator` versiona con `PRAGMA user_version`, crea `Usuarios` e índice único, respalda antes de migrar y hace rollback ante errores.
  - Registro de dependencias de autenticación.
- Tests de Core:
  - `PasswordHasherTests`.
  - `AuthServiceTests`.
  - `DatabaseInitializerTests`.
  - Helper `TestDatabase`.
- Flujo de acceso:
  - `UserSession` con `UsuarioSesion`.
  - `AppNavigator` para `AuthWindow` ↔ `MainWindow`.
  - `AuthViewModel` con inicialización, registro, login, reintento y validaciones inline.
  - `AuthWindow` con estados “Preparando…”, registro, login y error de credenciales.
  - Arranque de la app en `AuthWindow`; `MainWindow` se crea recién después del login.
  - Logout limpia sesión, paciente, fichas, DNI y mensajes.
- Header de usuario:
  - Chip con iniciales, usuario y chevron.
  - Menú con Preferencias, Acerca de, Abrir carpeta de datos, Crear copia de seguridad y Cerrar sesión.
  - Preferencias de apariencia con tema Según el sistema, Claro u Oscuro, y tamaño de texto 100, 115, 130 o 150 %; se guardan en `preferences.json` junto a la base.
  - El paciente queda dentro de las pestañas.
- Paquete 1:
  - Copiar resumen, DNI y teléfono.
  - Menú “Copiar” en la tarjeta del paciente.
  - Doble clic en ficha para editar.
  - `Esc` limpia búsqueda y mensajes a nivel de ventana; dentro del DNI cierra el autocompletado.
  - `Ctrl+F` enfoca el DNI y selecciona el texto.
  - Confirmación de borrado con fecha y motivo.
  - Títulos contextuales en editores de paciente y ficha.
  - Auto-cierre de banners de éxito a los 5 segundos.
  - “Acerca de” con versión, usuario, ruta de base y atajos.
  - Apertura de carpeta de datos.
  - Copia de seguridad SQLite en `Backups/Historia-yyyyMMdd-HHmmss.db`.
- Tests de App:
  - `AuthViewModelTests`.
  - `MainWindowViewModelTests` actualizado con sesión, navegador, portapapeles, mantenimiento y preferencias.
  - `DataMaintenanceServiceTests`.
  - `UserPreferencesServiceTests`.
  - `TestDoubles` actualizado.
- Documentación de la paleta y contrastes en este plan.
- Iconografía Lucide en login, header, menú, búsqueda, tarjetas, pestañas, editores y diálogos, sin paquetes NuGet nuevos.
- Preferencias de tema y tamaño de texto con persistencia local y aplicación inmediata al guardar.
- UI responsive 2.0:
  - `UiMetrics` escala tipografía, alturas, iconos y métricas desde Preferencias.
  - Clases `compact`, `narrow` y `short` por ventana, con umbrales escalados.
  - `ResponsiveGrid` reacomoda formularios de paciente y ficha según el ancho.
  - MainWindow reduce la lista y mantiene lista/detalle visibles en ancho angosto.
  - Editores y diálogos con tamaños mínimos, scroll y footer fijo.
  - Versión de ensamblado 2.0.0.
- Pulido de UI y UX de alta fidelidad:
  - Cursors interactivos `Hand` en botones, badge de usuario, toggles, list items, pestañas y menús.
  - Elevación con `CardShadow` en modo claro y oscuro (`BoxShadows`).
  - Separador visual y responsivo en `GridSplitter` (indicador vertical y feedback hover en acento).
  - Pestañas con icono en acento al seleccionarse (`TabItem:selected Path.icon`).
  - Menú desplegable "Copiar" con iconografía dedicada (resumen, DNI, teléfono).
  - Atajo `Ctrl+E` para editar el paciente actual y documentación en "Acerca de".
  - Foco y selección inmediata del DNI al presionar `Esc` para búsquedas consecutivas ágiles.
  - Validación lógica: prevención de sobreescritura accidental ante DNI duplicado al crear nuevo paciente.
  - Botón "Quitar" fecha de nacimiento contextual en el editor de pacientes.
  - Encabezado tipográfico `TitleText` integrado en `MessageDialog`.
  - Migración completa de `Watermark` a `PlaceholderText` sin warnings de obsolescencia.
- Eliminado `UnusedDbContextFactory` de los tests.

### Verificación ejecutada

- `dotnet build HistoriaClinica.sln --nologo -t:Rebuild`: 0 errores, 6 warnings `AVLN3001` esperados.
- `dotnet test HistoriaClinica.sln --nologo`: 89/89 tests aprobados (100% pasando).
- Smoke test con base temporal `/tmp/opencode/historia-auth-ui.db`:
  - Primer arranque: registro visible.
  - Registro de `ana`: usuario creado y auto-login.
  - Header con chip de usuario y menú desplegable.
  - Copia de seguridad creada desde el menú.
  - Logout: vuelve al login y no muestra datos del paciente.
  - Login con contraseña incorrecta: error genérico inline.
  - Login correcto: vuelve a `MainWindow`.
  - Búsqueda de paciente con datos insertados en la base temporal: pestañas Paciente/Historia clínica, tarjeta y datos visibles.
  - Menú “Copiar” visible y comandos cubiertos por tests.
  - Editor de paciente: apertura y validación inline del DNI visibles.
  - Editor de ficha: campos completos visibles sin scroll a 760×700.
  - Revisión visual de login, main vacío, resultado, editor de paciente, editor de ficha y menú en modo claro y oscuro.
- `Data/Historia.db`, `bin/` y `obj/` no se modificaron manualmente.

### Instalador

- `installer/HistoriaClinica.iss`: instalación por usuario, binarios versionados, backup previo y detección de la app abierta.
- `installer/build-installer.ps1`: publica `win-x64`, compila con Inno Setup 6 y genera SHA-256.
- `.github/workflows/windows-installer.yml`: tests y build del instalador en Windows.
- `installer/README.md`, `README.md` y `docs/DEPLOYMENT.md` actualizados.
- Compilado con Inno Setup 6.7.3; instalación silenciosa, backup y desinstalación probados en Wine.

### Pendientes

- Confirmar el pegado real en el portapapeles del sistema.
- Validación visual definitiva en Windows (tema, icono, DPI y tamaño de pantalla).
- Validar el instalador en Windows real: asistente, accesos directos, detección de la app abierta y DPI.
- Firmar el instalador y los ejecutables con Authenticode.
- Escaneo de vulnerabilidades: la sintaxis de `dotnet list package --vulnerable` no funcionó con el SDK 10.0.401.

## Decisiones cerradas

- Pacientes y fichas compartidos entre todos los usuarios.
- El login controla el acceso; no cifra el archivo SQLite.
- El primer usuario no tiene privilegios especiales ni roles.
- El registro solo se muestra si la tabla `Usuarios` está vacía.
- Después del primer registro se entra directo a la aplicación.
- En cada arranque posterior se pide login.
- El header superior pasa a ser de usuario.
- Los datos del paciente quedan dentro de las pestañas.
- Sin recuperación de contraseña, “recordarme” ni gestión de usuarios.
- Sin paquetes NuGet nuevos.
- Sin migraciones de EF Core.

## Alcance

### 1. Autenticación y esquema

- Nuevo modelo `Usuario`:
  - `Id`
  - `Username`
  - `NormalizedUsername`
  - `PasswordHash`
  - `PasswordSalt`
  - `PasswordIterations`
  - `CreatedAt`
- Usuario único sin distinguir mayúsculas mediante `NormalizedUsername`.
- `PasswordHasher`:
  - PBKDF2-SHA256.
  - Sal aleatoria de 16 bytes.
  - Hash de 32 bytes.
  - 210.000 iteraciones.
  - Comparación con `CryptographicOperations.FixedTimeEquals`.
  - Cálculo fuera del hilo de UI.
- `UsuarioRepository`:
  - Contar usuarios.
  - Buscar por usuario normalizado.
  - Agregar usuario.
- `AuthService`:
  - `HasUsersAsync`.
  - `RegisterFirstUserAsync`: solo si la tabla está vacía; usuario de 3–32 caracteres y contraseña mínimo 8.
  - `LoginAsync`: error genérico ante credenciales incorrectas.
- Actualización de esquema con migrador propio:
  - `EnsureCreatedAsync` solo crea bases nuevas.
  - `DatabaseInitializer` detecta bases existentes y delega en `DatabaseSchemaMigrator`.
  - `DatabaseSchemaMigrator` usa `PRAGMA user_version`, crea `Usuarios` e índice único, respalda antes de migrar y hace rollback ante errores.
  - No se modifican datos de `Pacientes` ni `FichasDiarias`.
- Tests:
  - Hash y verificación.
  - Registro del primer usuario.
  - Registro rechazado si ya hay usuarios.
  - Usuario duplicado.
  - Login correcto e incorrecto.
  - Base nueva y base existente sin tabla `Usuarios`.

### 2. Pantallas de acceso y navegación

- `AuthWindow` + `AuthViewModel`.
- Estados:
  - “Preparando…” mientras inicializa la base.
  - Registro si no hay usuarios.
  - Login si ya existe al menos uno.
- Registro:
  - Usuario, contraseña y confirmación.
  - Errores inline.
  - Mostrar/ocultar contraseña.
  - Botón “Crear usuario”.
- Login:
  - Usuario y contraseña.
  - Error inline genérico.
  - `Enter` para ingresar.
  - Sin enlace de registro.
- `UserSession` en memoria.
- `AppNavigator` para intercambiar `AuthWindow` y `MainWindow`.
- La `MainWindow` no se crea hasta que el login sea exitoso.
- Al cerrar sesión:
  - Limpiar usuario, paciente, fichas y mensajes.
  - Volver a `AuthWindow`.
- `MainWindowViewModel.InitializeAsync` deja de inicializar la base y solo carga los DNIs.

### 3. Header de usuario

- Reemplazar el badge de paciente del header por un chip de usuario:
  - Iniciales.
  - Nombre de usuario.
  - Chevron.
- Menú:
  - Acerca de.
  - Abrir carpeta de datos.
  - Crear copia de seguridad.
  - Separador.
  - Cerrar sesión.
- El menú queda preparado para agregar “Configuración” después, sin opción muerta.
- El paciente sigue visible en:
  - Pestaña “Paciente”.
  - Encabezado compacto de “Historia clínica”.

### 4. Paquete 1

- Copiar resumen del paciente: nombre, DNI, edad y obra social.
- Copiar DNI.
- Copiar teléfono.
- Doble clic sobre una ficha para editarla.
- `Esc` limpia la búsqueda y los mensajes.
- `Ctrl+F` enfoca el DNI y selecciona el texto.
- Confirmación de borrado con fecha y motivo.
- Títulos contextuales:
  - “Editar paciente — Ana Pérez”.
  - “Nueva ficha — Ana Pérez”.
- Auto-cierre de banners de éxito a los 5 segundos.
- Los errores permanecen hasta que se cierren.
- “Acerca de” con:
  - Versión.
  - Usuario actual.
  - Ruta de la base.
  - Atajos.
- Abrir carpeta de datos.
- Copia de seguridad:
  - `SqliteConnection.BackupDatabase`.
  - Archivo `Backups/Historia-yyyyMMdd-HHmmss.db`.

## Archivos previstos

### Nuevos en Core

- `src/HistoriaClinica.Core/Models/Usuario.cs`
- `src/HistoriaClinica.Core/Repositories/IUsuarioRepository.cs`
- `src/HistoriaClinica.Core/Repositories/UsuarioRepository.cs`
- `src/HistoriaClinica.Core/Security/PasswordHasher.cs`
- `src/HistoriaClinica.Core/Services/AuthService.cs`
- `src/HistoriaClinica.Core/Data/DatabaseSchemaMigrator.cs`

### Nuevos en App

- `src/HistoriaClinica.App/Services/UserSession.cs`
- `src/HistoriaClinica.App/Services/AppNavigator.cs`
- `src/HistoriaClinica.App/Services/ClipboardService.cs`
- `src/HistoriaClinica.App/Services/DataMaintenanceService.cs`
- `src/HistoriaClinica.App/Services/UserPreferencesService.cs`
- `src/HistoriaClinica.App/ViewModels/AuthViewModel.cs`
- `src/HistoriaClinica.App/ViewModels/PreferencesViewModel.cs`
- `src/HistoriaClinica.App/Views/AuthWindow.axaml`
- `src/HistoriaClinica.App/Views/AuthWindow.axaml.cs`
- `src/HistoriaClinica.App/Views/PreferencesWindow.axaml`
- `src/HistoriaClinica.App/Views/PreferencesWindow.axaml.cs`

### Modificados

- `src/HistoriaClinica.Core/Data/HistoriaContext.cs`
- `src/HistoriaClinica.Core/Data/DatabaseInitializer.cs`
- `src/HistoriaClinica.Core/DependencyInjection.cs`
- `src/HistoriaClinica.App/App.axaml.cs`
- `src/HistoriaClinica.App/Views/MainWindow.axaml`
- `src/HistoriaClinica.App/Views/MainWindow.axaml.cs`
- `src/HistoriaClinica.App/ViewModels/MainWindowViewModel.cs`
- `src/HistoriaClinica.App/Services/DialogService.cs`
- `src/HistoriaClinica.App/Services/IDialogService.cs`
- `src/HistoriaClinica.App/ViewModels/PatientEditorViewModel.cs`
- `src/HistoriaClinica.App/ViewModels/FichaEditorViewModel.cs`
- `src/HistoriaClinica.App/Styles/Theme.axaml`
- `src/HistoriaClinica.App/Styles/Controls.axaml`
- `AGENTS.md`

### Tests

- `tests/HistoriaClinica.Tests/PasswordHasherTests.cs`
- `tests/HistoriaClinica.Tests/AuthServiceTests.cs`
- `tests/HistoriaClinica.Tests/DatabaseInitializerTests.cs`
- `tests/HistoriaClinica.Tests/AuthViewModelTests.cs`
- `tests/HistoriaClinica.Tests/DataMaintenanceServiceTests.cs`
- `tests/HistoriaClinica.Tests/UserPreferencesServiceTests.cs`
- `tests/HistoriaClinica.Tests/MainWindowViewModelTests.cs` actualizado.
- `tests/HistoriaClinica.Tests/TestDoubles.cs` actualizado.

## Orden de implementación

1. [x] Modelo, repositorio, hasher, servicio, esquema y tests.
2. [x] `AuthWindow`, `AuthViewModel`, `UserSession` y `AppNavigator`.
3. [x] Header de usuario, menú, logout y limpieza de estado.
4. [x] Copiar, doble clic, `Esc`, títulos y confirmación de borrado.
5. [x] Acerca de, abrir carpeta y copia de seguridad.
6. [x] Build y tests completos; UI revisada en Linux.
7. [ ] Escaneo de vulnerabilidades y validación visual en Windows.

## Esquema SQL de actualización

```sql
CREATE TABLE IF NOT EXISTS "Usuarios" (
  "Id" INTEGER NOT NULL CONSTRAINT "PK_Usuarios" PRIMARY KEY AUTOINCREMENT,
  "Username" TEXT NOT NULL,
  "NormalizedUsername" TEXT NOT NULL,
  "PasswordHash" TEXT NOT NULL,
  "PasswordSalt" TEXT NOT NULL,
  "PasswordIterations" INTEGER NOT NULL,
  "CreatedAt" TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Usuarios_NormalizedUsername"
ON "Usuarios" ("NormalizedUsername");
```

## Criterios de aceptación

- Una base existente con pacientes abre sin perder datos y muestra registro si no hay usuarios.
- El primer registro crea el usuario y entra directo a la app.
- En el siguiente arranque solo aparece login.
- Credenciales incorrectas muestran un error genérico.
- Logout vuelve al login y no deja datos del paciente visibles.
- El header muestra el usuario y su menú funciona.
- Las funciones del paquete 1 funcionan en claro y oscuro.
- Build sin errores, tests pasando y sin paquetes vulnerables.
- `Data/Historia.db`, `bin/` y `obj/` no se modifican.
- Sin commit hasta pedido explícito.

## Fuera de alcance

- Roles y permisos.
- Auditoría de quién editó cada ficha.
- Datos separados por usuario.
- Recuperación o cambio de contraseña.
- Cifrado de la base.
- Gestión de usuarios desde la UI.
- Registro público.

## Verificación

```bash
dotnet build HistoriaClinica.sln
dotnet test HistoriaClinica.sln
dotnet list src/HistoriaClinica.App/HistoriaClinica.App.csproj package --vulnerable
```

- Smoke test con base temporal:
  - Primer registro.
  - Login.
  - Crear paciente y ficha.
  - Copiar resumen.
  - Crear backup.
  - Logout.
  - Login nuevamente.
- Validación visual en claro y oscuro de:
  - Registro.
  - Login.
  - Error de credenciales.
  - Header de usuario y menú.
  - Funciones del paquete 1.
- Los warnings `AVLN3001` adicionales por `AuthWindow` con DI son esperados y deben documentarse.
- Validación visual definitiva en Windows queda a cargo del usuario.

## Anexo: paleta actual

Fuente: `src/HistoriaClinica.App/Styles/Theme.axaml`.

### Modo claro

| Token | Hex | Uso |
| --- | --- | --- |
| `AppBackgroundBrush` | `#F4F7FB` | Fondo general |
| `SurfaceBrush` | `#FFFFFF` | Tarjetas, inputs, superficies |
| `SurfaceMutedBrush` | `#EDF2F7` | Superficies secundarias y listas |
| `SurfaceHoverBrush` | `#E8F0F7` | Hover de filas y controles |
| `BorderBrush` | `#D6E0EA` | Bordes y separadores |
| `TextPrimaryBrush` | `#16232E` | Texto principal |
| `TextSecondaryBrush` | `#51606F` | Texto secundario |
| `TextMutedBrush` | `#7A8794` | Metadatos y placeholders |
| `AccentBrush` | `#0F6B8A` | Acción principal y foco |
| `AccentHoverBrush` | `#0B5A75` | Hover del acento |
| `AccentSoftBrush` | `#E1F1F7` | Fondos suaves del acento |
| `AccentForegroundBrush` | `#FFFFFF` | Texto sobre acento |
| `DangerBrush` | `#B42318` | Error y acciones destructivas |
| `DangerSoftBrush` | `#FDECEA` | Fondo de error |
| `SuccessBrush` | `#176B3A` | Confirmaciones |
| `SuccessSoftBrush` | `#E7F6EC` | Fondo de éxito |
| `WarningBrush` | `#9A6700` | Advertencias |
| `FocusBrush` | `#0F6B8A` | Anillo de foco |
| `OverlayBrush` | `#6616232E` | Overlay oscuro al 40 % |
| `TabItemHeaderSelectedPipeFill` | `#0F6B8A` | Indicador de pestaña activa |

Contrastes principales en claro:

- `TextPrimary` sobre `Surface`: 15,98:1.
- `TextPrimary` sobre `AppBackground`: 14,87:1.
- `TextSecondary` sobre `Surface`: 6,46:1.
- `TextMuted` sobre `Surface`: 3,67:1. Usar solo en textos auxiliares, metadatos o tamaños grandes.
- `Accent` sobre `Surface`: 6,02:1.
- Blanco sobre `Accent`: 6,02:1.
- `Danger` sobre `DangerSoft`: 5,75:1.
- `Success` sobre `SuccessSoft`: 5,87:1.
- `Warning` sobre `Surface`: 4,87:1.

### Modo oscuro

| Token | Hex | Uso |
| --- | --- | --- |
| `AppBackgroundBrush` | `#0D151D` | Fondo general |
| `SurfaceBrush` | `#16222E` | Tarjetas, inputs, superficies |
| `SurfaceMutedBrush` | `#1E2D3B` | Superficies secundarias y listas |
| `SurfaceHoverBrush` | `#26394A` | Hover de filas y controles |
| `BorderBrush` | `#33495C` | Bordes y separadores |
| `TextPrimaryBrush` | `#F2F7FB` | Texto principal |
| `TextSecondaryBrush` | `#B7C5D3` | Texto secundario |
| `TextMutedBrush` | `#8FA2B4` | Metadatos y placeholders |
| `AccentBrush` | `#5CC0DD` | Acción principal y foco |
| `AccentHoverBrush` | `#7AD1EA` | Hover del acento |
| `AccentSoftBrush` | `#173A49` | Fondos suaves del acento |
| `AccentForegroundBrush` | `#06232E` | Texto sobre acento |
| `DangerBrush` | `#FF8A80` | Error y acciones destructivas |
| `DangerSoftBrush` | `#3A2020` | Fondo de error |
| `SuccessBrush` | `#7DDBA0` | Confirmaciones |
| `SuccessSoftBrush` | `#153322` | Fondo de éxito |
| `WarningBrush` | `#F2C94C` | Advertencias |
| `FocusBrush` | `#7AD1EA` | Anillo de foco |
| `OverlayBrush` | `#99000000` | Overlay negro al 60 % |
| `TabItemHeaderSelectedPipeFill` | `#5CC0DD` | Indicador de pestaña activa |

Contrastes principales en oscuro:

- `TextPrimary` sobre `Surface`: 14,95:1.
- `TextSecondary` sobre `Surface`: 9,17:1.
- `TextMuted` sobre `Surface`: 6,14:1.
- `Accent` sobre `Surface`: 7,71:1.
- `AccentForeground` sobre `Accent`: 7,80:1.
- `Danger` sobre `DangerSoft`: 6,54:1.
- `Success` sobre `SuccessSoft`: 8,19:1.
