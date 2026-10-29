# Firma de código con SignPath Foundation

Estado: **solicitud rechazada en octubre de 2026**. SignPath Foundation indicó
que el proyecto todavía no muestra suficientes señales públicas de adopción y
visibilidad. No hay certificado activo y los binarios se distribuyen sin firma.

El flujo técnico queda preparado para una futura reaplicación. La `v2.0.1` no
debe presentarse como firmada; si se publica, será sin Authenticode.

## Resultado de la solicitud

SignPath evalúa, entre otras señales:

- estrellas, forks, contribuidores y actividad sostenida;
- artículos externos, referencias independientes y discusiones;
- respaldo institucional;
- adopción y participación de la comunidad.

El rechazo no cuestiona la calidad del proyecto. La recomendación fue seguir
desarrollándolo y reaplicar cuando tenga más reconocimiento. También ofrecieron
la vía de una suscripción paga.

## Requisitos para reaplicar

- Repositorio público con licencia MIT: cumplido.
- Proyecto publicado y documentado: `v2.0.0` está publicado en
  [Releases](https://github.com/Gaspipi/HistoriaClinica/releases).
- Política de firma publicada: [`CODE_SIGNING_POLICY.md`](../CODE_SIGNING_POLICY.md).
- Construcción verificable en GitHub Actions desde `main`: cumplido.
- MFA habilitado en GitHub y, después de la aprobación, en SignPath.
- Adopción pública suficiente: **pendiente**. Es el motivo del rechazo.

## Plan de reaplicación

Antes de volver a aplicar:

1. Publicar una versión estable sin firma y difundirla en comunidades clínicas y
   de software open source.
2. Agregar capturas, una demostración corta y ejemplos de uso con datos
   ficticios.
3. Mantener una hoja de ruta y issues para nuevos contribuidores.
4. Conseguir estrellas, forks, contribuciones y descargas reales.
5. Documentar casos de uso, testimonios o referencias externas sin exponer datos
   de pacientes.
6. Escribir una nota técnica sobre el diseño local-first, la privacidad y las
   migraciones.
7. Revisar el estado en 3-6 meses y reaplicar con enlaces a esas señales.

No inventar métricas ni testimonios. Si el proyecto consigue usuarios reales,
pedir permiso antes de publicar sus nombres.

## Datos para la solicitud

Si se reaplica, usar:

- **Project name:** Historia Clínica
- **Repository:** https://github.com/Gaspipi/HistoriaClinica
- **Download page:** https://github.com/Gaspipi/HistoriaClinica/releases
- **License:** MIT
- **Build system:** GitHub Actions, runner `windows-latest`
- **Release workflow:**
  https://github.com/Gaspipi/HistoriaClinica/blob/main/.github/workflows/windows-signed-release.yml
- **Descripción:** aplicación de escritorio local-first para gestionar pacientes,
  datos clínicos y fichas diarias en una base SQLite local, sin servicios en la
  nube.
- **Categoría:** Other (salud / gestión de consultorios).
- **Artefactos:** ejecutable Windows y instalador Inno Setup por usuario.

## Configuración futura en SignPath

1. Agregar el trusted build system **GitHub.com** a la organización.
2. Crear el proyecto `HistoriaClinica` y vincular el repositorio.
3. Crear la política `release-signing`:
   - propósito: release;
   - certificado: SignPath Foundation;
   - aprobación manual: 1 aprobador;
   - trusted build system: GitHub.com;
   - origin verification: activado;
   - ramas permitidas: `main`.
4. Crear las configuraciones de artefactos con estos slugs:
   - `historia-clinica-app` desde
     `.signpath/artifact-configurations/historia-clinica-app.xml`;
   - `historia-clinica-installer` desde
     `.signpath/artifact-configurations/historia-clinica-installer.xml`.
5. Instalar la GitHub App de SignPath en el repositorio.
6. Crear un usuario/API token con permiso de submitter para la política.

## Configuración en GitHub

- Secret `SIGNPATH_API_TOKEN`: token de API del usuario CI.
- Variable `SIGNPATH_ORGANIZATION_ID`: ID de la organización en SignPath.
- Variable opcional `SIGNPATH_PROJECT_SLUG`: por defecto `HistoriaClinica`.

No guardar tokens, claves ni credenciales en el repositorio.

## Flujo futuro de release

Cuando haya certificado, el workflow manual `Windows signed release` ejecutará:

1. Tests y publicación `win-x64` desde `main`.
2. Sube `HistoriaClinica.exe` a SignPath.
3. Espera la **primera aprobación manual** y descarga el ejecutable firmado.
4. Compila el instalador usando el ejecutable firmado.
5. Normaliza `ProductName`, `ProductVersion` y `FileVersion` del instalador con
   rcedit 2.0.0 (hash SHA-256 fijado). Inno Setup rellena esos campos con
   espacios y SignPath exige una coincidencia exacta.
6. Sube el instalador a SignPath.
7. Espera la **segunda aprobación manual** y descarga el instalador firmado.
8. Verifica ambas firmas, regenera el SHA-256 y sube los artefactos.
9. Si `publish_release` está activo, crea el tag y la GitHub Release.

El workflow **no debe ejecutarse** hasta tener un certificado y los secretos
configurados. Cuando eso ocurra, ejecutarlo desde `main` con
`publish_release=false`, aprobar las dos solicitudes y validar el instalador.
Después repetir con `publish_release=true`. La segunda ejecución genera nuevas
firmas; no reutilizar artefactos de la primera.

## Verificación

Mientras no haya firma, en Windows:

```powershell
Get-AuthenticodeSignature .\HistoriaClinica-2.0.1-setup.exe
Get-FileHash .\HistoriaClinica-2.0.1-setup.exe -Algorithm SHA256
```

`Get-AuthenticodeSignature` devolverá `NotSigned`, que es el estado esperado. El
hash debe coincidir con el `.sha256` publicado.

Cuando exista un certificado, la firma deberá ser válida y el firmante deberá
corresponder al certificado emitido. El desinstalador de Inno Setup queda sin
firma en esta etapa.
