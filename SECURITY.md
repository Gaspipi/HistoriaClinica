# Política de seguridad

## Versiones soportadas

| Versión | Soporte de seguridad |
| --- | --- |
| `2.0.x` | Sí |
| `< 2.0` | No |

La versión publicada más reciente está en
[Releases](https://github.com/Gaspipi/HistoriaClinica/releases).

## Cómo reportar una vulnerabilidad

No abras un issue público para problemas de seguridad.

Usá el reporte privado de GitHub:

1. Entrá en la pestaña **Security** del repositorio.
2. Elegí **Report a vulnerability**.
3. Describí el problema con el mayor detalle posible.

Enlace directo:
https://github.com/Gaspipi/HistoriaClinica/security/advisories/new

Si no podés usar ese canal, contactá a
[Gaspar Ferreyra (@Gaspipi)](https://www.linkedin.com/in/gaspar-ferreyra/) por
mensaje privado y no publiques los detalles.

## Qué incluir en el reporte

- Versión de Historia Clínica y sistema operativo.
- Pasos para reproducir el problema.
- Impacto esperado: lectura o modificación de datos, ejecución de código,
  elevación de privilegios, denegación de servicio, filtración, etc.
- Una prueba de concepto mínima, si es segura de compartir.
- Logs o capturas **sin datos reales de pacientes**.

No adjuntes bases de datos reales, credenciales, tokens ni información clínica
identificable. Si necesitás compartir una base, anonimizala y reducila al mínimo.

## Alcance

Se consideran problemas de seguridad:

- autenticación y manejo de contraseñas;
- lectura o escritura de datos fuera de la carpeta permitida;
- inyección SQL o command injection;
- manejo inseguro de archivos, backups o rutas;
- escalada de privilegios en el instalador o en el desinstalador;
- exposición de datos clínicos o credenciales;
- dependencias vulnerables con impacto demostrable;
- bypass de las validaciones de DNI, fechas o entidades.

Quedan fuera de alcance, salvo que exista un impacto adicional:

- que la base SQLite local no esté cifrada, porque es una decisión documentada;
- que el login no cifre el archivo, porque el login solo controla el acceso;
- que un atacante con control total del sistema operativo y de la sesión del
  usuario pueda leer sus archivos;
- advertencias de SmartScreen por falta de reputación del binario;
- problemas que requieran ingeniería social o acceso físico no autorizado.

## Modelo de seguridad y límites conocidos

- Los datos se guardan en SQLite local. **El archivo no está cifrado.**
- El login controla el acceso a la aplicación, pero no protege el archivo frente
  a otro usuario del sistema operativo con permisos de lectura.
- Pacientes y fichas son compartidos entre todos los usuarios de una misma base.
- No hay roles ni recuperación de contraseña.
- Las contraseñas usan PBKDF2-SHA256 con 210.000 iteraciones, sal de 16 bytes y
  comparación de tiempo constante.
- No hay telemetría ni envío automático de datos clínicos a servicios externos.
- La aplicación no está firmada con Authenticode en este momento. Ver
  [`CODE_SIGNING_POLICY.md`](CODE_SIGNING_POLICY.md).

## Respuesta

El proyecto es mantenido por una sola persona. El objetivo es:

- acusar recibo del reporte en un plazo de 7 días;
- evaluar el impacto y confirmar o descartar el problema;
- coordinar una corrección y una divulgación responsable;
- acreditar al reportante si lo desea.

Estos plazos son un objetivo de buena fe, no una garantía contractual.

## Divulgación

No publiques detalles de una vulnerabilidad sin coordinar antes una fecha de
divulgación. Se intentará publicar un aviso y una versión corregida antes de
hacer públicos los detalles técnicos.
