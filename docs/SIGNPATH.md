# Firma de código con SignPath Foundation

Estado: **preparado, pendiente de solicitud y aprobación de SignPath Foundation**.
La release `v2.0.1` todavía no debe publicarse hasta completar la firma.

## Requisitos previos

- Repositorio público con licencia MIT: cumplido.
- Proyecto publicado y documentado: `v2.0.0` está publicado en
  [Releases](https://github.com/Gaspipi/HistoriaClinica/releases).
- Política de firma publicada: [`CODE_SIGNING_POLICY.md`](../CODE_SIGNING_POLICY.md).
- Construcción verificable en GitHub Actions desde `main`: cumplido.
- MFA habilitado en GitHub y, después de la aprobación, en SignPath. Verificar
  en la cuenta antes de aplicar; SignPath lo exige.
- El certificado gratuito se emite a nombre de **SignPath Foundation**; ese será
  el publisher visible en Windows.

## Solicitud

Aplicar en https://signpath.org/apply con estos datos:

- **Project name:** Historia Clínica
- **Repository:** https://github.com/Gaspipi/HistoriaClinica
- **Download page:** https://github.com/Gaspipi/HistoriaClinica/releases
- **License:** MIT
- **Build system:** GitHub Actions, runner `windows-latest`
- **Release workflow:**
  https://github.com/Gaspipi/HistoriaClinica/blob/main/.github/workflows/windows-signed-release.yml
- **Descripción:** aplicación de escritorio para gestionar pacientes, datos
  clínicos y fichas diarias en una base SQLite local, sin servicios en la nube.
- **Artefactos:** ejecutable Windows y instalador Inno Setup por usuario.

SignPath evalúa la reputación del proyecto; la aprobación no está garantizada.

## Configuración en SignPath

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

## Flujo de release

El workflow manual `Windows signed release` ejecuta:

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

Para preparar `v2.0.1`, ejecutar el workflow desde `main` con
`publish_release=false`, aprobar las dos solicitudes y validar el instalador.
Después repetir con `publish_release=true`. La segunda ejecución genera nuevas
firmas; no reutilizar artefactos de la primera.

## Verificación

En Windows:

```powershell
Get-AuthenticodeSignature .\HistoriaClinica-2.0.1-setup.exe
Get-FileHash .\HistoriaClinica-2.0.1-setup.exe -Algorithm SHA256
```

La firma debe ser válida y el hash debe coincidir con el `.sha256` publicado.
El desinstalador de Inno Setup queda sin firma en esta etapa.
