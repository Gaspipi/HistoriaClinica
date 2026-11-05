# Plan de Corrección de Bugs - HistoriaClinica v2.0.1

## 1. Problema: Cierres en Wayland (Tmds.DBus.Protocol 0.95.1)

**Archivos afectados:**
- `src/HistoriaClinica.App/App.axaml` (declaración de Avalonia)
- `src/HistoriaClinica.App/Program.cs` (inicialización)
- `src/HistoriaClinica.App/Services/AppSettingsService.cs` (configuración)

**Diseño propuesto:**
- Actualizar a Avalonia 12.1.5+ que incluye parche para Tmds.DBus.Protocol
- O forzar versión compatible mediante override en Program.cs
- Agregar flag `--disable-dbus` para modo fallback

**Riesgos:**
- Posible pérdida de funcionalidad de autenticación si DBus falla
- Requiere reinicio de app tras actualización

**Impacto:** Base de datos/UI (alta)

**Criterios de verificación:**
- [ ] Build pasa sin errores en Linux
- [ ] App arranca sin cerrar en Wayland
- [ ] Login funciona correctamente

---

## 2. Problema: ViewModels sin constructor sin parámetros (warnings AVLN3001)

**Archivos afectados:**
- `src/HistoriaClinica.App/ViewModels/*.axaml.cs` (todos los ViewModels)
- `src/HistoriaClinica.App/Services/*.cs` (servicios con DI)

**Diseño propuesto:**
- Agregar constructor público sin parámetros en cada ViewModel
- Inyectar dependencias localmente con `services.GetRequiredService<...>()`
- Mover configuración de DI a Program.cs si es necesario

**Riesgos:**
- Inyección de dependencias no verificada puede causar NPE
- Aumenta código pero es requerido por Avalonia DI

**Impacto:** Arquitectura/UI (media)

**Criterios de verificación:**
- [ ] Build pasa con 0 warnings AVLN3001
- [ ] Todos los ViewModels se inicializan correctamente
- [ ] No hay NPE al cargar vistas

---

## 3. Problema: Migrador sin pruebas de concurrencia

**Archivos afectados:**
- `src/HistoriaClinica.Core/Data/DatabaseSchemaMigrator.cs`
- `src/HistoriaClinica.Core/Data/DatabaseInitializer.cs`
- `tests/HistoriaClinica.Tests/Database/ConcurrencyTests.cs` (nuevo)

**Diseño proposed:**
- Agregar pruebas de concurrencia con `ConcurrentDatabaseUtility`
- Verificar que migraciones no se ejecuten dos veces simultáneamente
- Agregar lock basado en UUID por sesión

**Riesgos:**
- Si falla, podría haber duplicados de registros migrados
- Requiere pruebas exhaustivas

**Impacto:** Datos (alta)

**Criterios de verificación:**
- [ ] Tests de concurrencia pasan (10+ hilos simultáneos)
- [ ] Base de datos consistente tras concurrentes
- [ ] Backup automático funciona

---

## 4. Problema: Sin logging a archivo ni telemetría

**Archivos afectados:**
- `src/HistoriaClinica.Core/Data/DatabaseSchemaMigrator.cs` (logs de migración)
- `src/HistoriaClinica.App/Services/AppSettingsService.cs` (telemetría)
- `src/HistoriaClinica.App/ViewModels/MainViewModel.cs` (registro global)

**Diseño propuesto:**
- Agregar `ILogger<T>` a todos los servicios/ViewModels
- Log a `Logs/App.log` con formato JSON + texto
- Telemetría anónima: versiones usadas, crashes, errores críticos
- Enviar logs a `Logs/App.log` con rotación diaria

**Riesgos:**
- Privacidad de logs (no incluir datos sensibles)
- Storage de logs en cliente

**Impacto:** Base de datos/UI (media)

**Criterios de verificación:**
- [ ] Logs generados en `Logs/App.log`
- [ ] Logs no incluyen datos de pacientes
- [ ] Telemetría envía métricas básicas

---

## 5. Problema: Instalador Windows no firmado

**Archivos afectados:**
- `installer/Setup.iss` (Inno Setup script)
- `installer/build-installer.ps1` (script de build)
- `.signpath/signpath.json` (configuración de firma)

**Diseño propuesto:**
- Obtener certificado de CodePlex o open-source alternative
- Actualizar `.signpath/signpath.json` con nueva configuración
- Actualizar `installer/SignFile.iss` para incluir certificado
- Agregar paso de firma en `build-installer.ps1`

**Riesgos:**
- Requiere certificado digital válido
- Posible rechazo de distribuidores sin firma

**Impacto:** Distribución (alta)

**Criterios de verificación:**
- [ ] Instalador se firma correctamente
- [ ] SignTools valida firma
- [ ] Distribuidores aceptan instalador firmado

---

## 6. Problema: DateTime.MinValue como "sin fecha de nacimiento"

**Archivos afectados:**
- `src/HistoriaClinica.Core/Models/Paciente.cs`
- `src/HistoriaClinica.Core/Validation/PacienteValidations.cs`
- `src/HistoriaClinica.App/ViewModels/PacienteViewModel.cs`

**Diseño propuesto:**
- Agregar propiedad `IsBirthDateNull` para indicar `DateTime.MinValue`
- Agregar validación explícita: "No se puede crear paciente sin fecha de nacimiento"
- Mostrar mensaje claro al intentar crear paciente con fecha nula
- Agregar opción "N/A" o "Desconocido" en UI de fecha

**Riesgos:**
- Datos históricos con fecha nula podrían interpretarse erróneamente
- Validaciones adicionales pueden romper flujos existentes

**Impacto:** Datos/UI (alta)

**Criterios de verificación:**
- [ ] Pacientes con fecha nula se manejan claramente
- [ ] Mensajes de validación en español es-ES
- [ ] UI muestra "N/A" para fechas nulas

---

## 7. Problema: Sin recuperación de contraseña ni usuarios múltiple

**Archivos afectados:**
- `src/HistoriaClinica.App/Views/Auth/LoginView.axaml`
- `src/HistoriaClinica.App/ViewModels/AuthViewModel.cs`
- `src/HistoriaClinica.Core/Services/AuthService.cs`

**Diseño propuesto:**
- Agregar vista "Olvidé mi contraseña" en LoginView.axaml
- Agregar vista "Agregar nuevo usuario" en AdminView.axaml
- Implementar servicio `IPasswordRecoveryService` con PBKDF2
- Agregar validación de email y recuperación segura
- Agregar validación de email y recuperación segura

**Riesgos:**
- Recuperación de contraseña sin validación puede comprometer seguridad
- Gestión de usuarios requiere backend seguro

**Impacto:** Seguridad/Usuarios (alta)

**Criterios de verificación:**
- [ ] Recuperación de contraseña funciona
- [ ] Usuarios múltiples se gestionan correctamente
- [ ] No hay vulnerabilidades de seguridad

---

## 8. Problema: Falta validación visual en Windows

**Archivos afectados:**
- Ninguno (requisito de validación)

**Diseño propuesto:**
- Crear checklist de validación en `docs/VALIDATION_CHECKLIST.md`
- Validar: tema oscuro/light, iconos, DPI scaling, pantallas pequeñas
- Agregar capturas de pantalla en pruebas
- Validar en Windows 10/11 con diferentes resoluciones

**Riesgos:**
- UI puede fallar en configuraciones específicas
- Problemas de accesibilidad sin validación

**Impacto:** UI (media)

**Criterios de verificación:**
- [ ] App funciona correctamente en Windows
- [ ] Temas alternativos funcionan
- [ ] DPI scaling funciona correctamente
- [ ] Accesibilidad básica (contraste, navegación con teclado)

---

## Priorización de Implementación

### Fase 1 (Crítico) - Prioridad 1
1. Problema #1: Cierres en Wayland (bloquea desarrollo en Linux)
2. Problema #2: ViewModels sin constructor (bloquea DI)

### Fase 2 (Alta) - Prioridad 2
3. Problema #3: Concurrencia de migraciones (riesgo de datos)
4. Problema #6: DateTime.MinValue (riesgo de datos)
5. Problema #7: Recuperación de contraseña (seguridad)

### Fase 3 (Media) - Prioridad 3
6. Problema #4: Logging y telemetría (observabilidad)
7. Problema #5: Instalador firmado (distribución)
8. Problema #8: Validación Windows (calidad)

---

## Convenciones a Respetar

- No commitear hasta verificación completa
- No abrir PRs sin aprobación
- No agregar NuGet sin confirmación
- Fechas con `es-ES`, formato `dd/MM/yyyy`
- Validaciones en cultura `es-ES`
- Operaciones async con `await`

## Criterios de Finalización

- [ ] Todos los bugs críticos corregidos
- [ ] Build: `dotnet build HistoriaClinica.sln -c Release` pasa sin errores
- [ ] Tests: `dotnet test HistoriaClinica.sln -c Release` pasan
- [ ] Validación visual en Windows completada
- [ ] Documentación actualizada
