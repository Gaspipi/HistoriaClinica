# Changelog

Los cambios relevantes se documentan en este archivo. El formato sigue
[Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/) y el proyecto usa
versionado semántico.

## [Unreleased]

### Agregado
- Migraciones con EF Core para gestionar la evolución del esquema a partir del esquema 8.
- Buscador unificado en la ventana principal: acepta DNI (solo números, hasta 8 dígitos) o nombres/apellidos, con filtrado por prefijo en búsquedas por DNI y tarjetas de resultados compactas.

### Cambiado
- Esquema y modelo `Paciente` simplificados a campos en español (`Nombre`, `Apellido`, `FechaNacimiento`, `Telefono`, `ObraSocial`, `NumeroSocio`, `AntecedentesFamiliares`, `AntecedentesPersonales`); se eliminaron `Medicacion`, `Alergias` y `ProblemasActivos`.
- Migrador propio marcado como heredado: solo se utiliza para llevar bases anteriores al esquema 8; a partir de ahí el esquema evoluciona con migraciones de EF Core.
- `Data/` deja de estar versionado.

### Mantenido/limpieza
- Instalador sin firma digital: aviso SmartScreen y verificación SHA256 en README.
- Limpieza de referencias a SignPath y workflow firmado.
- Guía de contribución, código de conducta y política de seguridad.
- Plantillas de issues y de pull request.
- Validación de build y tests en Linux y Windows para pull requests.
- Dependabot para NuGet y GitHub Actions.
- Documentación de arquitectura y hoja de ruta pública.
- La documentación de firma refleja el rechazo de SignPath Foundation y el plan de reaplicación; no se anuncia una firma activa.

## [2.0.0] - 2026-10-24

### Added

- Migración de la interfaz a Avalonia 12 y .NET 10.
- Login local con PBKDF2-SHA256 y registro del primer usuario.
- Preferencias de tema y escala de texto.
- UI responsive para ventanas chicas y pantallas de poca altura.
- Backup de SQLite desde la aplicación.
- Migrador de esquema con `PRAGMA user_version`, backup previo y rollback.
- Instalador de Windows por usuario con Inno Setup.
- Tests de autenticación, validación, persistencia y preferencias.

### Security

- Contraseñas con PBKDF2-SHA256, 210.000 iteraciones y comparación de tiempo
  constante.
- La base SQLite local no está cifrada; el login solo controla el acceso.
