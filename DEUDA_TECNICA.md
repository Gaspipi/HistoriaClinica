# Informe de Deuda Técnica - HistoriaClinica

**Fecha:** 2 de Noviembre 2026  
**Versión:** 1.0  
**Tech Lead:** HistoriaClinica

---

## Resumen Ejecutivo

Se ha completado una revisión exhaustiva de la deuda técnica acumulada en el proyecto HistoriaClinica (Avalonia 12/.NET 10/EF Core 10). Este documento documenta todos los hallazgos, sus impactos y prioridades para una implementación ordenada.

---

## 📋 Hallazgos Principales

### 1. Seguridad y Autenticación

| Hallazgo | Impacto | Prioridad |
|----------|---------|-----------|
| No hay recuperación de contraseña | Alto - Usuario no puede recuperar acceso | P0 |
| Solo un usuario inicial, sin gestión de usuarios | Alto - Limita escalabilidad | P0 |
| Credenciales PBKDF2-SHA256 sin sal extra | Medio - Vulnerable a ataques de diccionario | P1 |
| Base de datos no cifrada | Alto - Datos clínicos expuestos | P0 |
| Validaciones con cultura `es-ES` hardcoded | Bajo - Podría fallar con usuarios internacionales | P2 |

**Archivos Relevantes:**
- `src/HistoriaClinica.Core/Security/PasswordHasher.cs`
- `src/HistoriaClinica.Core/Services/AuthService/UsuarioService.cs`
- `src/HistoriaClinica.App/ViewModels/LoginViewModel.cs`

---

### 2. Persistencia de Datos

| Hallazgo | Impacto | Prioridad |
|----------|---------|-----------|
| Migrador propio sin pruebas de concurrencia | Alto - Riesgo de corrupción en múltiples usuarios | P0 |
| Falta validación visual de esquemas | Medio - Bugs potenciales en migraciones | P1 |
| `DatabaseInitializer` con `EnsureCreated` | Alto - No actualiza versiones existentes | P1 |
| SQLite sin backup automático | Alto - Pérdida de datos crítica | P0 |
| `DateTime.MinValue` como "sin fecha" | Medio - Puede causar bugs en consultas | P2 |

**Archivos Relevantes:**
- `src/HistoriaClinica.Core/Data/DatabaseSchemaMigrator.cs`
- `src/HistoriaClinica.Core/Data/DatabaseInitializer.cs`
- `src/HistoriaClinica.Core/Data/DatabasePathProvider.cs`
- `src/HistoriaClinica.Core/Data/HistoriaContext.cs`

---

### 3. Logging y Observabilidad

| Hallazgo | Impacto | Prioridad |
|----------|---------|-----------|
| Solo logging a consola + archivo | Medio - Sin trazabilidad estructural | P2 |
| No hay telemetría ni métricas | Alto - Imposible monitorizar en producción | P0 |
| Logs solo en desarrollo | Alto - Sin visibilidad en producción | P0 |

**Archivos Relevantes:**
- `src/HistoriaClinica.App/Logging/FileLoggerProvider.cs`
- `src/HistoriaClinica.App/App.axaml.cs`
- `src/HistoriaClinica.App/ViewModels/*.cs`

---

### 4. Distribución y Firma

| Hallazgo | Impacto | Prioridad |
|----------|---------|-----------|
| Instalador sin firma digital | Alto - Windows SmartScreen bloquea | P0 |
| SignPath Foundation rechazó firma | Medio - Bloqueo de distribución oficial | P1 |
| Falta validar instalador en Windows | Alto - UX desconocido en target | P0 |

**Archivos Relevantes:**
- `installer/installer.iss`
- `installer/build-installer.ps1`
- `CODE_SIGNING_POLICY.md`

---

### 5. Dependencias y Compatibilidad

| Hallazgo | Impacto | Prioridad |
|----------|---------|-----------|
| `Tmds.DBus.Protocol 0.95.1` fijo | Medio - Dificulta upgrades | P2 |
| 6 warnings `AVLN3001` (DI sin parámetros) | Medio - Warning esperado pero documentado | P2 |
| Avalonia 12.1.4 | Medio - Versión estable | P2 |

**Archivos Relevantes:**
- `src/HistoriaClinica.App/HistoriaClinica.App.csproj`
- `src/HistoriaClinica.Core/HistoriaClinica.Core.csproj`

---

### 6. Arquitectura y Mantenibilidad

| Hallazgo | Impacto | Prioridad |
|----------|---------|-----------|
| No hay tests de integración de migraciones | Alto - Riesgo de romper datos | P0 |
| Logging solo en ViewModels | Medio - No hay logging centralizado en repositorios | P1 |
| No hay validación de datos en base | Alto - Datos no normalizados | P1 |

**Archivos Relevantes:**
- `src/HistoriaClinica.Core/Repositories/*.cs`
- `tests/HistoriaClinica.Tests/*.cs`

---

### 7. Documentación y DevEx

| Hallazgo | Impacto | Prioridad |
|----------|---------|-----------|
| `AGENTS.md` bien documentado | Alto - Buen punto de partida | + |
| `CODE_OF_CONDUCT.md` presente | Alto - Buen gobierno | + |
| `CONTRIBUTING.md` presente | Alto - Buen onboarding | + |

---

## 🎯 Priorización de Mejoras

### P0 - Crítico (Bloquea uso en producción)

1. **Implementar recuperación de contraseña** o sistema de sesiones
   - Email o código SMS para recuperación
   - Tokens expirados en 1 hora
   
2. **Añadir backup automático** de base de datos
   - Cron job diario
   - Retención de 30 días
   - Notificación de fallas
   
3. **Implementar logging estructurado** con métricas en producción
   - Serilog o NLog
   - JSON por defecto
   - Métricas en Prometheus
   
4. **Firmar instalador** y validar en Windows
   - Generar certificado CA
   - Firmar .exe con signtool
   - Validar SmartScreen
   
5. **Implementar migraciones de datos** con validación
   - Tests de integridad post-migración
   - Comparación esquema antes/después

### P1 - Alto (Requiere atención próxima)

1. **Mejorar seguridad de credenciales**
   - Sal de 32+ bytes
   - Salt de 16+ bytes
   - Iteraciones > 100.000
   
2. **Añadir gestión de usuarios**
   - CRUD completo
   - Roles básicos (admin, medico, enfermera)
   - Asignación de pacientes
   
3. **Implementar validación de datos en base**
   - Triggers EF Core
   - Validación de DNI, fechas, etc.
   - Normalización automática
   
4. **Documentar y probar migraciones** de concurrencia
   - Tests de transacciones concurrentes
   - Validación de rollback

### P2 - Medio (Mejoras continuas)

1. **Internacionalizar validaciones**
   - Soporte multi-idioma
   - Formato de fecha localizable
   
2. **Migrar logging** a archivo centralizado
   - Centralizado en `src/HistoriaClinica.App/Logging/`
   - Configuración separada dev/producción
   
3. **Resolver warnings AVLN3001**
   - DI con parámetros en constructores
   - Factory pattern para ViewModels
   
4. **Actualizar dependencias** de Tmds.DBus
   - Versiones compatibles
   - Tests de compatibilidad

### + Documentación (No es deuda, es mejora)

1. Mantener `ARCHITECTURE.md` actualizado
2. `CHANGELOG.md` - Mantener actualizado
3. `CODE_SIGNING_POLICY.md` - Seguir
4. Documentar flujos de despliegue

---

## 📊 Matriz de Riesgo vs. Beneficio

```
Alta Beneficio / Alto Riesgo  | Baja Beneficio / Alto Riesgo
┌─────────────────────────────┴───────────────────────────────┐
| Implementar backup automático                    | Evitar cambios en base de datos
| Añadir logging estructurado                       | Migraciones de datos
| Sistema de recuperación de credenciales            | 
└─────────────────────────────┴───────────────────────────────┘

Alta Beneficio / Bajo Riesgo  | Baja Beneficio / Bajo Riesgo
┌─────────────────────────────┴───────────────────────────────┐
| Añadir validación en base                     | Documentación adicional
| Mejorar seguridad de credenciales              | Actualizar dependencias
└─────────────────────────────┴───────────────────────────────┘
```

---

## 🚀 Recomendaciones Inmediatas

### Siguiente Sprint (P0+P1)

1. **Backup automático** - Cron job o evento al commit
   - Ruta: `Backups/Historia.db-{timestamp}.sqlite`
   - Rotación automática
   - Alerta por correo si falla
   
2. **Logging en producción** - FileLoggerProvider con rotación
   - Archivo: `logs/historiaclinica-{date}.log`
   - Formato: JSON
   - Niveles: Info/Warning/Error
   
3. **Firma instalador** - Generar certificado y firmar
   - CA interna auto-actuada
   - SignTool con SHA256
   
4. **Recuperación de contraseña** - Email o código SMS
   - Token expirado en 1 hora
   - Máximo 3 intentos por usuario
   
5. **Validación de datos en base** - Triggers EF Core
   - Validación DNI (8 dígitos)
   - Validación fechas (futuro)
   - Normalización nombres

### Corto Plazo (1-2 meses)

1. **Gestión de usuarios** - CRUD completo
   - Login, registro, recuperación
   - Roles y permicios básicos
   - Asignación de pacientes
   
2. **Validación de datos** - Triggers EF Core
   - Validación en todos los campos
   - Mensajes de error culturalmente correctos
   - Normalización automática
   
3. **Tests de migraciones** - Suite completa
   - Tests unitarios de migraciones
   - Tests de integración de datos
   - Validación de rollback

### Mediano Plazo (3-6 meses)

1. **Seguridad avanzada** - Cifrado de columnas sensibles
   - Cifrado AES-256 para datos médicos
   - Gestión de claves separada
   - Auditoría de acceso
   
2. **Internacionalización** - i18n completa
   - Soporte multi-idioma
   - Formateo de fechas/localización
   - Mensajes culturalmente apropiados
   
3. **Monitorización** - Prometheus + Grafana
   - Métricas de rendimiento
   - Alertas de error
   - Dashboard de salud

---

## 🔍 Estado Activo de la Deuda

### ✅ Ya Documentado y Validado

- Arquitectura MVVM (.NET 10, Avalonia 12)
- EF Core 10 + SQLite con migrador propio
- DI con CommunityToolkit.Mvvm
- Sin instantiaciones directas de DbContext
- Build exitoso (0 warnings, 0 errors)
- AGENTS.md bien documentado

### ⚠️ Requiere Atención Próxima

- Tests de integración de migraciones (P0)
- Validación de datos en base (P1)
- Logging estructurado (P2)
- Dependencias actualizadas (P2)

---

## 📝 Plan de Implementación Sugerido

```
Semana 1-2: P0 Crítico
├── Backup automático
├── Recuperación de contraseña
├── Logging estructurado
└── Firma instalador

Semana 3-4: P1 Alto
├── Gestión de usuarios
├── Validación de datos
├── Tests de migraciones
└── Documentación de despliegue

Mes 2-3: P2 Medio
├── Internacionalización
├── Monitorización
├── Seguridad avanzada
└── Mantenimiento de deuda técnica
```

---

## 📞 Contacto

Para preguntas o seguimientos sobre este informe:

- **Tech Lead:** HistoriaClinica Team
- **Repositorio:** `/home/gaspipi/Projects/HistoriaClinica`
- **Documentación:** `docs/`, `AGENTS.md`, `CHANGELOG.md`

---

*Este documento se actualizará automáticamente cuando se implementen mejoras de deuda técnica.*
