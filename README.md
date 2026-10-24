# Historia Clínica

Aplicación de escritorio para gestionar pacientes, sus datos clínicos y fichas diarias. Está pensada para consultorios y equipos pequeños que necesitan una base local, simple y sin servicios en la nube.

- **UI:** Avalonia 12 + MVVM (`CommunityToolkit.Mvvm`)
- **Plataforma:** .NET 10 · Windows, Linux y macOS
- **Datos:** EF Core 10 + SQLite local
- **Seguridad:** login local con PBKDF2-SHA256; la base SQLite **no** está cifrada
- **Versión actual:** 2.0.0

## Funcionalidad

- Alta y edición de pacientes.
- Búsqueda por DNI con autocompletado.
- Datos personales, obra social, antecedentes y medicación.
- Fichas diarias con motivo, enfermedad actual e indicaciones.
- Edición y eliminación de fichas.
- Copia rápida del resumen, DNI o teléfono del paciente.
- Login local con registro del primer usuario.
- Preferencias de tema (sistema, claro u oscuro) y tamaño de texto (100, 115, 130 o 150 %).
- Copias de seguridad SQLite desde el menú de usuario.
- Interfaz adaptable a ventanas chicas, pantallas de poca altura y escalas de texto grandes.

## Requisitos

### Para ejecutar

- Windows 10/11, una distribución Linux de escritorio o macOS.
- .NET 10 Runtime, salvo que uses una publicación self-contained.
- En Linux: `fontconfig`, X11 y soporte OpenGL. En NixOS alcanza con el `shell.nix` incluido.

### Para compilar

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
- Git.

No se necesita Windows Forms, Visual Studio ni permisos de administrador.

## Instalación

Todavía **no hay un instalador publicado**. Hoy la aplicación se distribuye y ejecuta desde el código fuente o como publicación portable.

La estrategia de instalación, reemplazo de versiones anteriores y migraciones está documentada en [`docs/DEPLOYMENT.md`](docs/DEPLOYMENT.md). El objetivo es publicar un instalador por usuario que:

- no pida permisos de administrador;
- instale la aplicación fuera de la carpeta de datos;
- conserve `Historia.db`, `preferences.json` y `Backups/` al actualizar;
- haga una copia de seguridad antes de migrar la base;
- permita volver a la versión anterior sin perder datos.

### Desde el código fuente

```bash
git clone https://github.com/Gaspipi/HistoriaClinica.git
cd HistoriaClinica
dotnet build HistoriaClinica.sln
dotnet run --project src/HistoriaClinica.App
```

### NixOS

```bash
nix-shell --run "dotnet run --project src/HistoriaClinica.App"
```

## Primer uso

1. Si la base no tiene usuarios, la aplicación muestra el registro del primer usuario.
2. Después del registro se entra directamente a la aplicación.
3. En los arranques siguientes se pide login.
4. Buscá un paciente por DNI o creá uno nuevo.

No hay roles, recuperación de contraseña ni opción “recordarme”. El login controla el acceso a la aplicación, pero no cifra el archivo SQLite.

## Datos y respaldo

### Ubicación de la base

La aplicación usa `Historia.db` en `Documentos`:

- **Windows:** `%USERPROFILE%\Documents\Historia.db`
- **Linux/macOS:** `~/Documentos/Historia.db` si existe; si no, `~/Documents/Historia.db`

Se puede indicar otra ruta:

```bash
HISTORIA_DB_PATH=/ruta/a/Historia.db dotnet run --project src/HistoriaClinica.App
```

El archivo `preferences.json` se guarda en la misma carpeta que la base. Las copias de seguridad se crean en `Backups/`, también junto a la base.

El archivo `Data/Historia.db` del repositorio está vacío y es solo una referencia de ruta; **no** debe usarse como base de producción.

### Contraseñas y privacidad

- Las contraseñas se guardan con PBKDF2-SHA256, 210.000 iteraciones, sal aleatoria de 16 bytes y comparación de tiempo constante.
- Pacientes y fichas son compartidos entre todos los usuarios de esa base.
- El archivo SQLite no está cifrado: protegé la carpeta de datos con los permisos del sistema operativo y hacé copias de seguridad periódicas.

### Copias de seguridad

Desde el menú de usuario → **Crear copia de seguridad** se genera un archivo `Historia-yyyyMMdd-HHmmss.db` en `Backups/`.

Antes de actualizar la aplicación, cerrá la app y copiá manualmente `Historia.db` y `preferences.json` a una carpeta segura. Si la app migra el esquema, también crea un backup automático `Historia-pre-migracion-v0-yyyyMMdd-HHmmss.db`.

## Actualizar desde una versión anterior

La versión 2.0 conserva la ubicación de datos de la versión WinForms anterior en Windows: `%USERPROFILE%\Documents\Historia.db`.

Procedimiento manual mientras no exista instalador:

1. Cerrá completamente la aplicación anterior.
2. Copiá `Historia.db`, `preferences.json` y la carpeta `Backups/` a un lugar seguro.
3. Reemplazá los binarios por los de la versión nueva sin borrar la carpeta de datos.
4. Abrí la aplicación nueva y verificá el login y la búsqueda de pacientes.
5. No reemplaces la base nueva por la copia vieja después de que la aplicación haya migrado el esquema.

La base de la versión anterior ya contiene `Pacientes` y `FichasDiarias`; al abrirla, la aplicación agrega la tabla `Usuarios` y su índice si faltan. Antes de modificar el esquema se crea automáticamente un backup en `Backups/Historia-pre-migracion-v0-yyyyMMdd-HHmmss.db`.

## Base de datos y migraciones

- No hay migraciones de EF Core: el esquema se versiona con `PRAGMA user_version`.
- `DatabaseSchemaMigrator` aplica las migraciones SQL pendientes dentro de una transacción y actualiza la versión del esquema.
- `DatabaseInitializer` usa `EnsureCreated` solo para crear bases nuevas; después ejecuta el migrador.
- Si la base tiene una versión de esquema mayor que la soportada, la aplicación se detiene sin modificarla.
- Antes de migrar una base existente se crea un backup automático en `Backups/`; si la migración falla, se hace rollback y se conserva el backup.
- Para cambiar el modelo hay que agregar una migración SQL y aumentar `DatabaseSchemaMigrator.CurrentVersion`. Ver [`docs/DEPLOYMENT.md`](docs/DEPLOYMENT.md).

## Desarrollo

### Comandos

```bash
# Compilar
dotnet build HistoriaClinica.sln

# Ejecutar
dotnet run --project src/HistoriaClinica.App

# Tests
dotnet test HistoriaClinica.sln

# Publicación self-contained para Windows
dotnet publish src/HistoriaClinica.App -c Release -r win-x64 --self-contained true
```

Reemplazá `win-x64` por `linux-x64` u `osx-x64` según el destino.

### Estructura

```text
src/HistoriaClinica.Core/   Modelos, EF Core, repositorios, autenticación, servicios y validación
src/HistoriaClinica.App/    UI Avalonia, ViewModels, estilos, iconos, diálogos y DI
tests/HistoriaClinica.Tests/ Pruebas xUnit
docs/                       Documentación de despliegue y mantenimiento
Data/Historia.db            Archivo vacío de referencia; no usar en producción
shell.nix                   Entorno NixOS para ejecutar Avalonia
```

### Convenciones

- UI, mensajes y comentarios nuevos en español.
- `HistoriaContext` se crea mediante `IDbContextFactory`; los ViewModels no ejecutan consultas EF Core.
- Operaciones async con `await`, sin fire-and-forget.
- Fechas clínicas en `dd/MM/yyyy` con cultura `es-ES`.
- No agregar paquetes NuGet ni cambiar el target framework sin evaluar impacto y compatibilidad.

## Problemas frecuentes

| Problema | Causa probable | Solución |
| --- | --- | --- |
| La app no abre en Linux | Faltan `fontconfig`, X11 o GL | Instalar las dependencias o usar `nix-shell` en NixOS |
| Error de archivo bloqueado | Hay otra instancia abierta | Cerrar todas las instancias antes de actualizar |
| No aparece el paciente | El DNI tiene menos de 8 dígitos o no está en esa base | Verificar el DNI y la ruta de `HISTORIA_DB_PATH` |
| Se perdió la sesión | No hay “recordarme” | Volver a ingresar; la sesión no se persiste |
| La base no migra | Esquema desconocido, más nuevo o error de SQLite | Revisar el mensaje/log y el backup; consultar [`docs/DEPLOYMENT.md`](docs/DEPLOYMENT.md) |

## Estado y próximos pasos

- [x] Migración a Avalonia 12 y .NET 10.
- [x] UI responsive y escala de texto.
- [x] Tests de autenticación, validación, persistencia y preferencias.
- [x] Migrador de esquema con backup automático y rechazo de bases más nuevas.
- [ ] Instalador Windows por usuario.
- [ ] Publicación automatizada y artefactos por plataforma.
- [ ] Validación visual final en Windows.

## Contribución

1. Crear una rama desde `main` (`feature/mi-cambio`).
2. Mantener los cambios acotados y con tests cuando corresponda.
3. Ejecutar `dotnet build` y `dotnet test` antes de abrir el PR.
4. Describir el cambio, el impacto en datos y los pasos de verificación.

## Licencia

MIT. Ver [`LICENSE`](LICENSE).

## Contacto

Gaspar Ferreyra — [LinkedIn](https://www.linkedin.com/in/gaspar-ferreyra/)
