# Sistema de Gestión de Pacientes y Historias Médicas

Aplicación de escritorio para administrar pacientes y sus fichas diarias, ahora con interfaz **Avalonia 12** y arquitectura MVVM. Funciona en Windows, Linux y macOS sobre **.NET 10**, con **EF Core 10 + SQLite** para la persistencia.

## Características

- Alta y edición de pacientes con datos personales, obra social, antecedentes y medicación.
- Registro de fichas diarias con motivo, enfermedad actual e indicaciones.
- Acceso local con registro del primer usuario y login en arranques posteriores.
- Header de usuario con cierre de sesión, “Acerca de”, preferencias de tema, apertura de la carpeta de datos y copia de seguridad SQLite.
- Búsqueda de pacientes por DNI con autocompletado.
- Copia rápida del resumen, DNI o teléfono del paciente.
- Edición y borrado de fichas por identificador, sin duplicados por diferencias de fecha.
- Validación de DNI, fechas y campos obligatorios con mensajes claros.
- Persistencia local en SQLite.
- Interfaz responsive para ventanas chicas y escalas de texto grandes, con scroll y reacomodo de formularios.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) para compilar.
- .NET 10 Runtime para ejecutar una publicación framework-dependent.
- No requiere Windows Forms ni Windows para desarrollo.
- En Linux, Avalonia necesita las fuentes y bibliotecas nativas del sistema (`fontconfig`, X11 y GL). En NixOS se resuelve con el `shell.nix` incluido.

## Compilar y ejecutar

```bash
dotnet build HistoriaClinica.sln
dotnet run --project src/HistoriaClinica.App
```

Tests:

```bash
dotnet test HistoriaClinica.sln
```

### NixOS

`shell.nix` expone `fontconfig`, X11, GL y el resto de las bibliotecas nativas que necesita Avalonia. El SDK .NET 10 debe estar en el `PATH`:

```bash
nix-shell --run "dotnet run --project src/HistoriaClinica.App"
```

## Acceso y datos

El primer arranque sin usuarios muestra el registro; se crea un único usuario y se entra directo a la aplicación. En los arranques siguientes se pide login. No hay roles, recuperación de contraseña ni “recordarme”.

Las contraseñas se guardan con PBKDF2-SHA256 (210.000 iteraciones, sal aleatoria de 16 bytes). El login controla el acceso, pero **no cifra** el archivo SQLite: pacientes y fichas quedan compartidos entre los usuarios de esa base.

El tema y el tamaño del texto se eligen en el menú de usuario → Preferencias. La escala (100, 115, 130 o 150 %) se aplica al guardar y se recuerda para la próxima apertura. Las preferencias se guardan en `preferences.json`, junto a la base de datos.

## Base de datos

Por defecto la aplicación usa `Documentos/Historia.db` (con fallback a una carpeta de usuario en Linux/macOS). Para indicar otra ubicación:

```bash
HISTORIA_DB_PATH=/ruta/a/Historia.db dotnet run --project src/HistoriaClinica.App
```

El archivo `Data/Historia.db` versionado se conserva como referencia y no se modifica desde la aplicación.

No hay migraciones automáticas: si se cambia el modelo, hay que crear una migración o recrear la base (con respaldo previo). Al abrir una base existente, `DatabaseInitializer` agrega la tabla `Usuarios` y su índice sin modificar `Pacientes` ni `FichasDiarias`.

## Estructura

```
src/HistoriaClinica.Core/   Modelos, EF Core, repositorios, autenticación, servicios y validación
src/HistoriaClinica.App/    UI Avalonia, ViewModels, iconos, diálogos y DI
tests/HistoriaClinica.Tests/ Pruebas xUnit
```

## Publicación

```bash
dotnet publish src/HistoriaClinica.App -c Release -r win-x64 --self-contained true
```

Reemplazá `win-x64` por `linux-x64` u `osx-x64` según el destino.

## Contribución

1. Fork del repositorio.
2. Crear una rama para la funcionalidad (`git checkout -b feature/nueva-funcionalidad`).
3. Realizar cambios con mensajes descriptivos.
4. Enviar un pull request.

## Licencia

Este proyecto se distribuye bajo la licencia [MIT](/LICENSE).

## Contacto

Si tiene alguna pregunta o sugerencia, puede contactarme en [LinkedIn](https://www.linkedin.com/in/gaspar-ferreyra/).
