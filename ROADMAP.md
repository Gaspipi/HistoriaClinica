# Hoja de ruta

Esta hoja de ruta es orientativa. No implica fechas ni compromisos de
implementación. Las prioridades pueden cambiar según el feedback, la seguridad y
el tiempo disponible del mantenedor.

Para proponer o discutir un tema, abrí una
[discusión](https://github.com/Gaspipi/HistoriaClinica/discussions) o un issue
con la plantilla correspondiente.

## Ahora

- Consolidar la documentación para contribuidores.
- Ejecutar build y tests en pull requests.
- Agregar capturas y una demostración corta.
- Validar la interfaz y el instalador en Windows real.
- Mejorar la accesibilidad y los mensajes de error.
- Publicar una versión sin firma mientras no haya un certificado disponible.
- Definir y validar con uso real el alcance clínico para un solo profesional.

## Próximo

- Reaplicar a SignPath Foundation cuando el proyecto tenga más señales públicas,
  o evaluar un certificado Authenticode comercial.
- Buscar pacientes por nombre, apellido y recientes, además del DNI.
- Mostrar alergias y problemas activos en la ficha del paciente.
- Permitir editar la fecha y hora al crear una ficha.
- Cerrar la ficha para evitar ediciones accidentales y conservar trazabilidad.
- Restaurar backups desde la app, con copia previa y verificación de integridad.
- Imprimir o exportar en PDF la ficha y la historia clínica.
- Archivar pacientes y aplicar borrado lógico de fichas con motivo.
- Cifrar la base o los backups y bloquear la sesión por inactividad.
- Agregar exportación de datos en formatos abiertos.
- Documentar y probar el empaquetado en Linux y macOS.
- Ampliar la cobertura de tests de migraciones y recuperación.
- Revisar dependencias y alertas de seguridad de forma periódica.

## Ideas

Ver el detalle en [ideas clínicas y funcionales](docs/CLINICAL_IDEAS.md).

- Signos vitales opcionales por consulta.
- Adjuntos de laboratorio, imágenes y estudios.
- Plantillas de motivo e indicaciones frecuentes.
- Próximo control como fecha simple, sin agenda completa.
- Registro local de cambios y accesos.
- Importación desde CSV u otros sistemas.
- Traducciones de la interfaz.

## Opcional a futuro

- Multiusuario concurrente, servidor central o sincronización entre equipos.
- Roles, permisos y gestión de usuarios, solo si el uso real lo justifica.

## Fuera de alcance por ahora

- Historia clínica hospitalaria completa o prescripción electrónica validada.
- Sincronización en la nube.
- Telemetría.
- Aplicación móvil.
- Gestión de turnos o facturación.

Cualquier cambio de alcance debe discutirse antes de abrir una implementación
grande.
