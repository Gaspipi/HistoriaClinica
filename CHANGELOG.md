# Changelog

Los cambios relevantes se documentan en este archivo. El formato sigue
[Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/) y el proyecto usa
versionado semántico.

## [Unreleased]

### Added

- Guía de contribución, código de conducta y política de seguridad.
- Plantillas de issues y de pull request.
- Validación de build y tests en Linux y Windows para pull requests.
- Dependabot para NuGet y GitHub Actions.
- Documentación de arquitectura y hoja de ruta pública.

### Changed

- La documentación de firma ahora refleja el rechazo de SignPath Foundation y
  el plan de reaplicación; no se anuncia una firma activa.

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
