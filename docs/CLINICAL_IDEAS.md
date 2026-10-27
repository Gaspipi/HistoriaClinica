# Ideas clínicas y funcionales

> Documento de trabajo. No implica compromiso, fecha ni alcance cerrado.
> Enfoque: medicina clínica ambulatoria, un solo profesional y datos locales.
> El multiusuario y los roles quedan como opcionales a futuro.

## Criterios

- Priorizar seguridad del paciente, trazabilidad y rapidez de registro.
- Evitar convertir la app en un sistema hospitalario, de facturación o de receta
  electrónica validada.
- No agregar telemetría ni dependencia de la nube por defecto.
- Cualquier cálculo, escala o plantilla clínica debe tener fuente, versión y
  revisión profesional.

## Registro clínico

- **SOAP**: separar Subjetivo, Objetivo, Análisis y Plan. Los campos actuales
  Motivo, Enfermedad e Indicaciones pueden mapearse, pero no cubren examen
  físico, problemas ni plan estructurado.
- **Revisión por sistemas** con plantillas y opción de marcar “sin particularidades”.
- **Examen físico por aparatos**, con valores normales por defecto y texto libre.
- **Signos vitales y antropometría**: presión arterial, frecuencia cardíaca,
  frecuencia respiratoria, temperatura, saturación, peso, talla, IMC y
  perímetro abdominal.
- **Lista de problemas** activos y resueltos, con fecha de inicio, estado y
  diagnóstico asociado.
- **Diagnóstico principal y secundarios**, con CIE-10 opcional.
- **Alergias e intolerancias** como campo destacado, con tipo de reacción y
  gravedad.
- **Antecedentes estructurados**: médicos, quirúrgicos, hospitalizaciones,
  transfusionales, familiares, gineco-obstétricos, sociales y tóxicos.
- **Medicación habitual** con principio activo, dosis, vía, frecuencia,
  indicación, inicio y fin.
- **Conciliación medicamentosa** para detectar duplicados y medicación sin
  indicación.
- **Evolución** con notas de seguimiento y comparación con la consulta previa.
- **Adjuntos** de laboratorio, imágenes y estudios.
- **Fecha y hora editables** para cargar consultas fuera de fecha.
- **Cierre o firma de ficha** para evitar ediciones accidentales y conservar
  trazabilidad.
- **Historial de cambios** local, con fecha y motivo.

## Seguimiento y prevención

- **Línea de tiempo del paciente**: consultas, estudios, medicación y eventos.
- **Tendencias** de peso, IMC, presión arterial, glucemia, HbA1c, perfil
  lipídico, creatinina y filtrado glomerular.
- **Objetivos terapéuticos** por problema, por ejemplo presión arterial, HbA1c,
  LDL o peso.
- **Controles pendientes** y próximo control como fecha simple.
- **Chequeos preventivos** según edad y sexo: vacunas, PAP/HPV, mamografía,
  screening colorrectal, tabaquismo y riesgo cardiovascular.
- **Vacunas** con fecha y dosis.
- **Escalas clínicas** solo si están validadas y documentadas: IMC, riesgo
  cardiovascular, CKD-EPI, FRAX, AUDIT-C, PHQ-9, GAD-7, Barthel o Pfeiffer.
- **Resumen clínico de una página** para derivación o interconsulta.

## Documentos y salida

- **Imprimir o exportar en PDF**: ficha, historia completa, resumen, indicaciones,
  receta simple y certificados de reposo o aptitud.
- **Copiar resumen** para pegar en otro sistema.
- **Exportación CSV/JSON** para respaldo o migración.
- **Plantillas de notas y documentos** reutilizables.
- **Plantillas por patología** revisadas y versionadas: hipertensión, diabetes,
  dislipemia, obesidad, asma/EPOC, hipotiroidismo, infecciones respiratorias o
  urinarias, entre otras.
- **Receta simple** con campos estructurados. No es receta electrónica ni
  valida interacciones en la primera etapa.

## Búsqueda y productividad

- Búsqueda por nombre, apellido, teléfono y pacientes recientes.
- Búsqueda de texto completo en notas, motivos e indicaciones con SQLite FTS5.
- Filtros por diagnóstico, fecha, problema o medicación.
- Atajos de teclado y navegación rápida.
- Autoguardado de borradores.
- Duplicar una ficha como plantilla.
- Favoritos de motivos e indicaciones frecuentes.

## Seguridad y confiabilidad

- Backups automáticos y restauración guiada desde la app.
- Verificación de integridad antes y después de restaurar.
- Cifrado de la base o de los backups.
- Bloqueo de sesión por inactividad.
- Borrado lógico y archivo de pacientes y fichas con motivo.
- Registro local de cambios y accesos.
- Advertencia visible cuando la base no está cifrada.
- Sin telemetría ni nube por defecto.

## Opcional a futuro

- Multiusuario concurrente, servidor central o sincronización entre equipos.
- Roles, permisos y gestión de usuarios, solo si el uso real lo justifica.
- Agenda de turnos.
- Portal del paciente.
- Interacciones medicamentosas.
- Receta electrónica.
- Historia clínica hospitalaria.
- Resumen asistido por IA local, solo con datos privados y validación humana.

## Prioridad sugerida

1. SOAP, signos vitales, problemas activos y alergias.
2. Búsqueda por nombre y búsqueda de texto completo.
3. Impresión/PDF y resumen para derivación.
4. Backups restaurables y verificación de integridad.
5. Cierre de ficha e historial de cambios.
6. Tendencias, prevención y objetivos terapéuticos.
7. Multiusuario y roles, solo si se confirma la necesidad.
