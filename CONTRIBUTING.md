# Contribuir a Historia Clínica

Gracias por querer sumarte. Historia Clínica es una aplicación de escritorio para
consultorios y equipos pequeños, con datos clínicos almacenados localmente. Las
contribuciones de código, documentación, pruebas, traducciones y reportes de uso
son bienvenidas.

Podés escribir issues, discusiones y pull requests en español o inglés. La
documentación del proyecto se mantiene en español.

Antes de participar, leé el [código de conducta](CODE_OF_CONDUCT.md) y la
[política de seguridad](SECURITY.md).

## Regla principal: no incluir datos reales de pacientes

No publiques nombres, DNI, teléfonos, direcciones, diagnósticos, imágenes,
capturas, logs ni bases de datos de pacientes reales en issues, pull requests,
discusiones o comentarios.

Para reproducir un problema:

- usá una base temporal;
- inventá datos de prueba;
- recortá o tapá cualquier dato identificable en las capturas;
- compartí solo el error y los pasos mínimos.

La base versionada `Data/Historia.db` está vacía y es solo una referencia. No la
uses para pruebas ni la reemplaces por una base real.

## Formas de contribuir

No hace falta escribir código para ayudar:

- reportar errores con pasos claros y una versión;
- probar la aplicación en Windows, Linux o macOS;
- mejorar la documentación y los mensajes de error;
- agregar capturas o un video corto de uso;
- proponer mejoras de accesibilidad o de interfaz;
- revisar pull requests;
- difundir el proyecto en comunidades clínicas o de software libre.

## Buenas primeras contribuciones

- Agregar capturas o un video corto con datos ficticios.
- Mejorar la sección de problemas frecuentes del README.
- Documentar la instalación en Linux o macOS.
- Agregar tests para backups, migraciones o validaciones.
- Revisar textos, íconos y contraste en tema claro y oscuro.
- Probar la UI con escala de texto al 150 % y ventanas chicas.

Buscá el label `good first issue` en
[Issues](https://github.com/Gaspipi/HistoriaClinica/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22).
Si una tarea no está creada, abrí una propuesta antes de empezar.

## Antes de abrir un issue

1. Buscá en los issues existentes y en las
   [discusiones](https://github.com/Gaspipi/HistoriaClinica/discussions).
2. Verificá que estás usando la última release o `main`.
3. Prepará una reproducción mínima con datos ficticios.
4. Si es un problema de seguridad, no abras un issue público: seguí
   [SECURITY.md](SECURITY.md).

Usá las plantillas de issue. Si el reporte no entra en ninguna, abrí una
discusión para conversarlo antes.

## Entorno de desarrollo

### Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
- Git.
- En Linux: `fontconfig`, X11 y soporte OpenGL.
- En NixOS: `nix-shell` usa el `shell.nix` incluido.
- Para el instalador: Windows e Inno Setup 6.

### Puesta en marcha

```bash
git clone https://github.com/Gaspipi/HistoriaClinica.git
cd HistoriaClinica
dotnet restore HistoriaClinica.sln
dotnet build HistoriaClinica.sln
dotnet test HistoriaClinica.sln
dotnet run --project src/HistoriaClinica.App
```

Para no mezclar tus datos personales con las pruebas:

```bash
HISTORIA_DB_PATH=/tmp/historia-desarrollo.db \
  dotnet run --project src/HistoriaClinica.App
```

En NixOS:

```bash
nix-shell --run "dotnet run --project src/HistoriaClinica.App"
```

## Flujo de trabajo

1. Abrí un issue o una discusión antes de un cambio grande. Así evitamos
   trabajar dos veces sobre lo mismo.
2. Hacé un fork o creá una rama desde `main`:
   - `feature/descripcion-corta`
   - `fix/descripcion-corta`
   - `docs/descripcion-corta`
3. Mantené la pull request acotada a un problema.
4. Agregá o actualizá tests cuando cambie el comportamiento.
5. Actualizá la documentación afectada.
6. Ejecutá build y tests.
7. Abrí la pull request con la plantilla y describí el impacto en datos.

No hagas push directo a `main`; los cambios externos se integran por pull
request.

## Comandos de verificación

```bash
dotnet build HistoriaClinica.sln --configuration Release
dotnet test HistoriaClinica.sln --configuration Release
```

La pull request ejecuta build y tests en Linux y Windows mediante
`.github/workflows/pr-validation.yml`.

Para el instalador, en Windows:

```powershell
powershell -ExecutionPolicy Bypass -File installer/build-installer.ps1
```

## Convenciones de código

- La interfaz, los mensajes nuevos y los comentarios van en español.
- Seguí el patrón MVVM con `CommunityToolkit.Mvvm`.
- `HistoriaContext` se crea mediante `IDbContextFactory`. No instancies
  `DbContext` en ViewModels ni en Views.
- Los repositorios son la única capa que ejecuta consultas EF Core.
- Los ViewModels hablan con servicios, no con repositorios.
- Usá `await` en operaciones asíncronas; no dejes tareas sin observar.
- Respetá nullable e inicializá las propiedades no anulables.
- Las fechas clínicas usan `dd/MM/yyyy` y cultura `es-ES` mediante
  `FechaClinica`.
- No agregues paquetes NuGet ni cambies el target framework sin abrir un issue
  y acordarlo antes.
- No modifiques `Data/Historia.db`, `bin/` ni `obj/`.

El documento [AGENTS.md](AGENTS.md) resume la arquitectura y las reglas que usan
los asistentes de código. [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) explica
las capas y los flujos principales.

## Cambios en la base de datos

El esquema no usa EF Migrations. Se versiona con `PRAGMA user_version` y
migraciones SQL en `DatabaseSchemaMigrator`.

Si cambiás el modelo:

1. Agregá una migración SQL nueva; no edites una migración ya publicada.
2. Aumentá `DatabaseSchemaMigrator.CurrentVersion`.
3. Agregá tests para una base nueva y para una base existente.
4. Verificá que se cree el backup previo y que haya rollback ante errores.
5. Documentá el cambio en `docs/DEPLOYMENT.md`.

## Cambios de interfaz

- Mantené la UI usable con las clases `compact`, `narrow` y `short`.
- Probá tema claro, oscuro y “según el sistema”.
- Probá escalas de texto de 100, 115, 130 y 150 %.
- No agregues iconos de imagen si se pueden resolver con los iconos Lucide
  existentes.
- Los diálogos pasan por `IDialogService`.

## Tests

- Los tests viven en `tests/HistoriaClinica.Tests/`.
- Usá `TestDatabase` y bases SQLite temporales.
- No dependas de la base del usuario ni de archivos en `Documentos`.
- Los nombres de los tests y los mensajes de fallo deben ser claros.
- Si encontrás un bug, agregá primero un test que lo reproduzca.

## Documentación

- Mantené los enlaces relativos funcionando.
- Actualizá `README.md` si cambia el uso, la instalación o los requisitos.
- Actualizá `docs/DEPLOYMENT.md` si cambia el despliegue, el esquema o la
  instalación.
- No presentes la firma Authenticode como activa mientras no exista un
  certificado emitido. El estado real está en
  [`CODE_SIGNING_POLICY.md`](CODE_SIGNING_POLICY.md).

## Pull requests

Una pull request debería incluir:

- qué problema resuelve y por qué;
- qué cambió;
- cómo se probó;
- capturas si cambió la UI;
- impacto en datos, migraciones o compatibilidad;
- limitaciones o trabajo pendiente.

El mantenedor puede pedir cambios, dividir la PR o rechazarla si no encaja con el
alcance del proyecto. La revisión es humana; la CI no reemplaza la revisión.

## Licencia

Al contribuir aceptás que tu aporte se publique bajo la licencia MIT del
proyecto. Ver [`LICENSE`](LICENSE).

## Contacto

- Mantenedor: [Gaspar Ferreyra (@Gaspipi)](https://github.com/Gaspipi)
- Discusiones: https://github.com/Gaspipi/HistoriaClinica/discussions
- LinkedIn: https://www.linkedin.com/in/gaspar-ferreyra/
