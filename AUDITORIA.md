# Auditoría Técnica - Historia Clínica
> Documento vivo: se actualiza cada vez que se encuentra una discrepancia entre documentación y código.

## Estado Actual
- **Última revisión:** [Fecha]
- **Archivos revisados:** `docs/ARCHITECTURE.md`, `docs/DEPLOYMENT.md`, `docs/SIGNPATH.md`, `docs/CLINICAL_IDEAS.md`
- **Modelos:** `Usuario.cs`, `Paciente.cs`, `FichaDiaria.cs`
- **Capa de datos:** `HistoriaContext.cs`, `DatabaseInitializer.cs`, `DatabaseSchemaMigrator.cs`
- **Servicios:** `AuthService.cs`, `PacienteService.cs`

---

## Hallazgos por Sección

### 1. Modelo de Datos

| Hallazgo | Evidencia | Severidad | Notas |
|----------|-----------|-----------|-------|
| **Usuario no tiene campo Dni** | `Usuario.cs`: PK es `int Id`, no `Dni` | Alta | Documentación asume Dni compartido entre usuarios |
| **Dni de Paciente no validado** | `Paciente.cs`: solo `StringLength(8)`, no `DniValidator` | Alta | `DniValidator.cs` existe pero no se usa |
| **DOB como DateOnly?** | `HistoriaContext.cs`: conversion con `DateOnly?` | Media | Corresponde con documentación |
| **Alergias/ProblemasActivos son TEXT** | `Paciente.cs`: `string` sin estructura | Media | Documentación promete campos estructurados |

---

### 2. Seguridad

| Hallazgo | Evidencia | Severidad | Notas |
|----------|-----------|-----------|-------|
| **PBKDF2-SHA256 con 210k iteraciones** | `AuthService.cs` + `PasswordHasher.cs` | - | ✅ Correcto |
| **Bloqueo por 5 intentos en 1 min** | `AuthService.cs`: `_failedAttempts` | - | ✅ Correcto |
| **Hash regenerado en login** | `AuthService.cs`: `RegenerarHashAsync` | - | ✅ Correcto |
| **AuditLogs sin uso real** | `AuditLogs` tabla existe, ¿se escribe? | Alta | Tabla creada por migración 7 |
| **Sin cifrado de base** | Documentación: "la base no está cifrada" | - | ✅ Aceptado para MVP |

---

### 3. Migraciones de Base

| Hallazgo | Evidencia | Severidad | Notas |
|----------|-----------|-----------|-------|
| **Migración 7 (AuditLogs)** | `DatabaseSchemaMigrator.cs`: migration 7 | Media | ¿Se usa realmente? |
| **Migración 6 (Plantillas)** | `DatabaseSchemaMigrator.cs`: migration 6 | - | ✅ Corresponde con `CLINICAL_IDEAS.md` |
| **Migración 5 (Signos vitales)** | `DatabaseSchemaMigrator.cs`: migration 5 | - | ✅ Correcto |
| **Backup previo a migración** | `CreateBackupAsync()` | - | ✅ Correcto |
| **Rollback en fallo** | `try/catch` con `transaction.Rollback()` | - | ✅ Correcto |

---

### 4. Ideas Clínicas (`CLINICAL_IDEAS.md`)

| Idea | Estado en Código | Severidad |
|------|------------------|-----------|
| **SOAP** | ❌ No implementado | Alta |
| **Signos vitales completos** | ⚠️ 4 campos (PA, FC, Temp, Peso) | Media |
| **Problemas resueltos** | ❌ Solo texto plano | Alta |
| **Diagnóstico CIE-10** | ❌ No implementado | Media |
| **Alergias estructuradas** | ❌ Solo texto plano | Alta |
| **Antecedentes estructurados** | ❌ Solo texto plano | Alta |
| **Medicación habitual estructurada** | ❌ Solo texto plano | Alta |
| **Cierre de ficha** | ⚠️ `Cerrada` booleano | Media |
| **Adjuntos** | ❌ No implementado | Alta |

---

### 5. Validación

| Hallazgo | Evidencia | Severidad |
|----------|-----------|-----------|
| **DniValidator no usado** | `DniValidator.cs` existe, no se importa en repositorios | Alta |
| **Validación de usuario** | `AuthService.cs`: `ValidarRegistro()` | - |
| **FechaClinica** | `FechaClinica.cs` | - |
| **HistoriaValidators** | `HistoriaValidators.cs`: ¿valida Dni? | Media |

---

## Estado de Cumplimiento

### ✅ Implementado
- [x] PBKDF2-SHA256 con 210.000 iteraciones
- [x] Bloqueo por intentos fallidos
- [x] Migraciones 1-7 con backup
- [x] Cultura es-ES
- [x] Arquitectura MVVM

### ⚠️ Parcial
- [ ] Signos vitales (4/10+ campos)
- [ ] Cierre de ficha (solo bool, sin firma)

### ❌ Faltante
- [ ] Validación de DNI real
- [ ] Campos estructurados (SOAP, alergias, antecedentes, medicación)
- [ ] Problemas resueltos
- [ ] Adjuntos
- [ ] AuditLogs en uso

---

## Próximos Pasos

1. Revisar `AuthService.cs` completo
2. Revisar `PacienteService.cs` completo
3. Revisar ViewModels para validación
4. Verificar uso de `AuditLogs`
5. Revisar `FichaService` (si existe)

---

*Este documento se actualizará automáticamente cada vez que se encuentre una discrepancia.*
