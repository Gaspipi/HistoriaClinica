# Despliegue, instalación y migraciones

Este documento define cómo distribuir Historia Clínica, cómo reemplazar versiones anteriores sin perder datos y cómo evolucionar el esquema SQLite.

**Estado:** migrador de esquema e instalador Inno Setup implementados y publicados en `v2.0.0`. La `v2.0.1` no está publicada. El flujo de firma Authenticode está preparado, pero SignPath Foundation rechazó la solicitud inicial por falta de adopción pública; por ahora los binarios se distribuyen sin firma. Falta la validación visual en Windows y decidir la vía de firma.

## 1. Objetivos

- Instalar sin permisos de administrador en Windows.
- Mantener la aplicación separada de la base de datos.
- Actualizar conservando `Historia.db`, `preferences.json` y `Backups/`.
- Detectar bases de versiones anteriores y migrarlas con backup previo.
- Permitir rollback a la versión anterior.
- Publicar artefactos reproducibles para Windows, Linux y macOS.

## 2. Separación aplicación / datos

| Elemento | Windows | Linux/macOS |
| --- | --- | --- |
| Aplicación | `%LOCALAPPDATA%\Programs\HistoriaClinica\app\` | `~/.local/share/HistoriaClinica/app/` |
| Lanzador | `%LOCALAPPDATA%\Programs\HistoriaClinica\HistoriaClinica.exe` | `~/.local/bin/historia-clinica` |
| Base | `%USERPROFILE%\Documents\Historia.db` | `~/Documentos/Historia.db` o `~/Documents/Historia.db` |
| Preferencias | junto a la base | junto a la base |
| Backups | `Backups/` junto a la base | `Backups/` junto a la base |

Reglas:

- El instalador **nunca** borra ni sobrescribe la carpeta de datos.
- La aplicación se puede desinstalar sin eliminar la base.
- `HISTORIA_DB_PATH` sigue teniendo prioridad para instalaciones administradas o pruebas.
- La carpeta de datos debe quedar fuera de `Program Files` y fuera de la carpeta de instalación.

### Riesgo de Documentos y OneDrive

En Windows, `Documentos` puede estar redirigido a OneDrive. La app debe:

1. Resolver la ruta con `Environment.SpecialFolder.MyDocuments`.
2. Mostrar la ruta efectiva en “Acerca de” (ya existe).
3. No asumir que el archivo está en `%USERPROFILE%\Documents` si el sistema lo redirigió.

## 3. Instalador recomendado

### Windows: Inno Setup por usuario

Recomendación: **Inno Setup 6**, instalación por usuario, sin `Program Files` y sin UAC.

Motivos:

- Un solo ejecutable de instalación.
- Soporte de instalación por usuario y upgrades.
- Detección de procesos abiertos.
- Personalización de páginas y tareas.
- Firma Authenticode disponible.
- Menor complejidad que MSIX para una app Avalonia sin integración con Store.

Alternativas descartadas por ahora:

- **MSIX:** buena integración con Windows, pero más complejo para datos externos, firma y pruebas fuera de Store.
- **WiX/MSI:** potente, pero sobredimensionado para una instalación por usuario.
- **Copia manual del ZIP:** se mantiene como opción portable.

### Flujo del instalador

1. Verificar que la app no esté ejecutándose.
   - Si está abierta, el instalador aborta y pide cerrarla.
   - No mata procesos automáticamente.
2. Detectar una instalación anterior y reutilizar su carpeta.
3. Crear un backup de la base si existe:
   - `Backups/pre-install-v<version>-<timestamp>.db`.
   - Usa `HISTORIA_DB_PATH` si está definida; si no, `Documentos\Historia.db`.
4. Instalar los binarios en una carpeta versionada:
   - `app\2.0.1\`
   - `app\2.1.0\`
5. Crear o actualizar accesos directos en inicio y escritorio (opcional), apuntando a la versión nueva.
   - Si el acceso del escritorio ya existía, se actualiza aunque no se marque la tarea.
   - Los accesos fijados a la barra de tareas pueden requerir volver a fijarse.
6. Registrar la entrada de desinstalación.
7. Al primer arranque, ejecutar migraciones pendientes.
8. Si la migración falla, mostrar un error y dejar la versión anterior disponible.

La carpeta versionada permite rollback: la versión anterior sigue en `app\<version>` y se puede abrir directamente. No se borran versiones anteriores automáticamente.

Archivos implementados:

- `installer/HistoriaClinica.iss`: definición de Inno Setup.
- `installer/build-installer.ps1`: publica `win-x64`, compila el instalador y genera el checksum SHA-256. Admite `-SkipPublish` y `-SkipInstaller` para el flujo de firma en dos fases.
- `.github/workflows/windows-installer.yml`: tests y build sin firma en Windows.
- `.github/workflows/windows-signed-release.yml`: workflow preparado para firma, verificación, checksum y publicación de la release; inactivo hasta tener certificado.

### Firma Authenticode (preparada, inactiva)

**No hay firma activa.** SignPath Foundation rechazó la solicitud inicial por
falta de señales públicas de adopción. El workflow quedó preparado para una
futura aprobación o para un certificado comercial, pero no debe ejecutarse sin un
certificado y los secretos correspondientes.

Cuando haya certificado, la firma se hará en dos fases:

1. Se publica y firma `HistoriaClinica.exe`.
2. Se compila el instalador usando ese ejecutable firmado y se firma el instalador.

Cada fase esperará una aprobación manual en SignPath. El workflow verificará
ambas firmas y regenerará el SHA-256 antes de publicar. Antes de enviar el
instalador, normaliza `ProductName`, `ProductVersion` y `FileVersion` con rcedit
2.0.0 para que las restricciones de metadatos sean exactas. El desinstalador de
Inno Setup queda sin firma en esta etapa. SmartScreen puede seguir advirtiendo
hasta que el binario acumule reputación.

Mientras no haya firma, la integridad se verifica con el `.sha256` publicado. La
configuración, el rechazo y el plan de reaplicación están en
[`SIGNPATH.md`](SIGNPATH.md); la política objetivo, en
[`CODE_SIGNING_POLICY.md`](../CODE_SIGNING_POLICY.md).

### Linux

Publicar un `tar.gz` portable y, opcionalmente, un script `install.sh`:

- Instalar en `~/.local/share/HistoriaClinica/`.
- Copiar el binario y los recursos.
- Crear `~/.local/bin/historia-clinica`.
- No usar `sudo`.
- Incluir instrucciones para AppImage si se necesita un único archivo.

### macOS

Publicar un `.app` dentro de un `.dmg`:

- Firma y notarización obligatorias para distribución pública.
- Datos en `~/Documentos/Historia.db`.
- No incluir la base dentro del bundle.

## 4. Publicación de binarios

Para Windows, Linux y macOS usar publicaciones self-contained para no exigir .NET Runtime:

```bash
dotnet publish src/HistoriaClinica.App -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false
dotnet publish src/HistoriaClinica.App -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=false
dotnet publish src/HistoriaClinica.App -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=false
```

Recomendaciones:

- Mantener `PublishSingleFile=false` mientras Avalonia necesite recursos y nativos separados; evaluar single-file con pruebas por plataforma.
- Publicar siempre con la misma versión (`<Version>`).
- Generar `SHA256SUMS`.
- Firmar el instalador y los ejecutables de Windows.
- Versionar los artefactos en GitHub Releases.
- Incluir el `README.md` y la licencia en el paquete.

Pipeline mínimo de GitHub Actions:

1. `dotnet restore`.
2. `dotnet build -c Release`.
3. `dotnet test -c Release`.
4. Publicar `win-x64`, `linux-x64`, `osx-arm64` y `osx-x64`.
5. Empaquetar el instalador de Windows.
6. Generar checksums.
7. Adjuntar todo al release.

## 5. Migraciones de base de datos

### Estrategia elegida

Para esta aplicación, usar un **migrador propio basado en `PRAGMA user_version`** en lugar de EF Core Migrations.

Motivos:

- La base existente fue creada con `EnsureCreated` y no tiene historial de EF.
- El esquema es pequeño y SQLite es el único motor.
- Evita agregar `Microsoft.EntityFrameworkCore.Design` y un historial de migraciones que no refleja las bases reales.
- Permite migraciones SQL explícitas, auditables y testeables.

### Implementación actual

- `DatabaseSchemaMigrator` en `src/HistoriaClinica.Core/Data/DatabaseSchemaMigrator.cs`.
- `DatabaseSchemaMigrator.CurrentVersion` vale `1` y debe aumentarse al agregar una migración.
- `DatabaseInitializer` detecta si la base ya existía, ejecuta `EnsureCreated` solo para crear bases nuevas y luego delega en el migrador.
- Las migraciones se aplican en orden dentro de una transacción; si una falla, se hace rollback.
- Antes de migrar una base existente se crea `Backups/Historia-pre-migracion-v<origen>-<timestamp>.db` usando la API de backup de SQLite.
- Una base con `user_version` mayor que `CurrentVersion` se rechaza sin modificarse.
- Los tests están en `tests/HistoriaClinica.Tests/DatabaseInitializerTests.cs`.

### Versionado

- `PRAGMA user_version` guarda la versión del esquema.
- La versión de la aplicación (`2.0.1`) es independiente de la versión del esquema.
- Versión inicial: `1`.
- Una migración nunca se edita después de publicada; se agrega una nueva.

### Secuencia de arranque

1. Abrir la conexión SQLite.
2. Configurar `PRAGMA foreign_keys=ON` y `PRAGMA busy_timeout`.
3. Leer `PRAGMA user_version`.
4. Si el archivo no existe o está vacío:
   - crear el esquema actual con `EnsureCreated`;
   - marcar la versión actual.
5. Si la versión es anterior:
   - validar que una base heredada tenga `Pacientes` y `FichasDiarias`;
   - crear un backup en `Backups/Historia-pre-migracion-v<origen>-<timestamp>.db`;
   - ejecutar las migraciones en orden dentro de una transacción;
   - actualizar `user_version`;
   - registrar la versión aplicada en el log.
6. Si la versión es mayor que la soportada:
   - no modificar la base;
   - mostrar que fue creada por una versión más nueva;
   - no ofrecer reintento ni continuar al login.
7. Si falla una migración:
   - hacer rollback de la transacción;
   - conservar el backup;
   - mostrar un mensaje con la ruta del backup;
   - permitir reintentar después de revisar el problema.

### Migración base para bases heredadas

La versión WinForms no tenía `Usuarios` ni versión de esquema.

Migración `0 -> 1`:

1. Verificar que existan `Pacientes` y `FichasDiarias`.
2. Crear `Usuarios` si falta.
3. Crear el índice único `IX_Usuarios_NormalizedUsername`.
4. No modificar datos de pacientes ni fichas.
5. Marcar `user_version = 1`.

Si el archivo no existe, se crea con el esquema completo y `user_version = 1`.

### Migraciones futuras

Para agregar una migración `1 -> 2`:

1. Agregar `(2, "ALTER TABLE ...")` al arreglo `Migrations` de `DatabaseSchemaMigrator`.
2. Aumentar `CurrentVersion` a `2`.
3. Agregar un test que parta de una base en versión `1`.
4. No incluir `PRAGMA user_version` en el SQL de la migración: el runner lo actualiza.

Ejemplo de SQL:

```sql
ALTER TABLE "Pacientes" ADD COLUMN "Email" TEXT NULL;
```

Reglas:

- Las versiones del arreglo deben ser contiguas y terminar en `CurrentVersion`.
- Usar SQL idempotente cuando sea posible.
- No borrar columnas sin una migración de datos explícita.
- No renombrar tablas sin copia y validación.
- Probar con una copia real anonimizada y con una base vacía.
- Mantener compatibilidad de lectura durante al menos una versión menor.

### Tests

Implementados:

- Base vacía -> esquema actual y sin backup innecesario.
- Base heredada sin `Usuarios` -> migra, conserva pacientes/fichas y crea backup anterior a la migración.
- Base de la versión 2.0 con `Usuarios` pero sin `user_version` -> migra sin perder datos ni usuarios.
- Base actual -> no vuelve a migrar ni crea otro backup.
- Base con `user_version` mayor -> se rechaza antes de crear o modificar el esquema.
- Base existente sin `Pacientes`/`FichasDiarias` -> se rechaza sin modificar.
- Archivo SQLite desconocido -> se rechaza sin crear tablas de Historia Clínica.
- Migración con error -> rollback, `user_version` sin avanzar y backup intacto.

Pendientes:

- Dos aperturas concurrentes -> no corrompe la base.
- Prueba con una copia real anonimizada de una base de producción.

## 6. Reemplazo de versiones anteriores

### Matriz

| Origen | Destino | Acción |
| --- | --- | --- |
| Sin instalación | 2.0 | Instalación limpia; crear base al primer arranque |
| WinForms 1.x | 2.0 | Conservar `Documentos\Historia.db`; agregar `Usuarios`; migrar esquema |
| Portable 2.0 | 2.1 | Reemplazar binarios; conservar datos |
| Instalador 2.0 | 2.1 | Instalar upgrade; migrar si corresponde |
| Base desconocida | Cualquiera | No tocarla; mostrar error y pedir backup |

### Detección de bases heredadas

La app debe buscar, en este orden:

1. `HISTORIA_DB_PATH`.
2. `Documentos/Historia.db`.
3. `Documents/Historia.db`.
4. `AppContext.BaseDirectory/Historia.db` (solo modo portable/pruebas).

Si encuentra una base con tablas pero sin `Usuarios`, se considera heredada y se aplica la migración base.

No se debe importar automáticamente una base desde una carpeta arbitraria. Si se detecta una copia en la carpeta de la app, ofrecer importarla con confirmación.

### Rollback

- El instalador conserva la carpeta de la versión anterior.
- El backup previo a la migración queda en `Backups/`.
- Para volver atrás:
  1. cerrar la app;
  2. restaurar el backup de la base;
  3. apuntar el lanzador a la versión anterior;
  4. abrir y verificar.

No se debe ejecutar una versión anterior contra una base migrada sin restaurar el backup, salvo que la migración sea compatible hacia atrás.

## 7. Criterios de aceptación

- [ ] Instalación limpia en Windows sin UAC.
- [ ] Upgrade desde WinForms conserva pacientes y fichas.
- [ ] Upgrade desde 2.0 conserva base y preferencias.
- [x] La base se respalda antes de cualquier migración.
- [x] Un fallo de migración no deja la base a medias.
- [ ] Desinstalar no elimina datos.
- [ ] Funciona con `Documentos` redirigido a OneDrive.
- [ ] El instalador detecta la app abierta.
- [x] El release incluye checksums y versión visible en “Acerca de”.
- [x] Tests de migración en CI.
- [ ] Firma Authenticode válida del ejecutable y del instalador (bloqueada hasta tener certificado).
- [ ] Validación del instalador en Windows real.
- [ ] SmartScreen sin advertencia (depende de la reputación acumulada).

## 8. Decisiones pendientes

- Vía de firma: reaplicar a SignPath Foundation o comprar un certificado Authenticode.
- Fecha de publicación de `v2.0.1` sin firma.
- Firma del desinstalador de Inno Setup, que queda fuera del alcance inicial.
- Si se publicará una versión portable oficial además del instalador.
- Si el backup previo a la migración se conserva indefinidamente o se rota.
- Ventana mínima soportada y matriz de sistemas operativos.
