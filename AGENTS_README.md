# Agentes para HistoriaClinica

Este documento describe los agentes, tools y skills configurados para trabajar sobre el proyecto **HistoriaClinica**.

## 📋 Resumen

HistoriaClinica es un sistema de gestión de pacientes e historias médicas con:
- **UI**: Avalonia 12 / .NET 10 / MVVM
- **Backend**: EF Core 10 / SQLite
- **Arquitectura**: Clean Architecture con capas separadas (Core, App, Tests)

## 🤖 Agentes

### 1. **analista** - Analista de Requerimientos
**Uso**: Análisis de requerimientos, planificación, identificación de riesgos.

**Ejemplos**:
```
/analista - Analizar el requerimiento de añadir recuperación de contraseña
/analista - Planificar la implementación de un nuevo campo en Paciente
/analista - Identificar riesgos de añadir un nuevo diálogo
```

**Alcance**:
- Avalonia 12, .NET 10, MVVM, EF Core 10, SQLite
- Criterios de verificación
- Alcance de implementación

---

### 2. **datos** - Especialista en Datos
**Uso**: Trabajos con modelos, migraciones, consultas EF Core, persistencia.

**Ejemplos**:
```
/datos - Migrar un nuevo campo en Paciente
/datos - Añadir consulta para obtener pacientes por rango de DNI
/datos - Verificar compatibilidad del esquema con EF Core 10
```

**Modelos**:
- `Usuario`, `Paciente`, `FichaDiaria`
- `Plantilla`, `AuditLog`

**Componentes**:
- `HistoriaContext`
- Repositories
- `DatabaseSchemaMigrator`
- SQLite interceptor

---

### 3. **explore** - Explorador de Código
**Uso**: Encontrar archivos, entender cómo funciona algo, investigación rápida.

**Niveles**:
- `quick` - Búsqueda simple
- `medium` - Exploración moderada
- `very thorough` - Análisis exhaustivo

**Ejemplos**:
```
/explore very thorough - Cómo funciona el flujo de autenticación
/explore medium - Buscar todos los ViewModels de edición
/explore quick - Localizar el archivo de preferencias
```

---

### 4. **general** - Agente Generalista
**Uso**: Investigación compleja, tareas multi-paso, coordinación.

**Ejemplos**:
```
/general - Investigar todos los puntos de entrada para añadir un nuevo servicio
/general - Ejecutar múltiples tareas de preparación para una feature
```

---

### 5. **implementador** - Implementador de Código
**Uso**: Implementar cambios siguiendo el plan y convenciones.

**Restricciones**:
- ❌ NO commit
- ❌ NO push
- ❌ NO abrir PRs
- ✅ Solo cuando el usuario solicita explícitamente
- ✅ Diff mínimo
- ✅ Sin commitear

**Ejemplos**:
```
/implementador - Implementar la feature según el plan aprobado
/implementador - Corregir el bug encontrado en la revisión
```

---

### 6. **revisor** - Revisor de Código
**Uso**: Revisar cambios, detectar bugs, validar arquitectura.

**Alcance**:
- Bugs y regresiones
- Problemas de EF Core
- Problemas de MVVM/Avalonia
- Seguridad de credenciales
- Cumplimiento de AGENTS.md

**Ejemplos**:
```
/revisor - Revisar los cambios propuestos antes de implementar
/revisor - Detectar posibles problemas de seguridad en PasswordHasher
```

---

### 7. **verificador** - Verificador de Build
**Uso**: Compilar y validar cambios, verificar build limpio.

**Alcance**:
- Compilación de solución
- 0 errores, 0 warnings
- Verificación de build exitoso

**Ejemplos**:
```
/verificador - Validar que los cambios compilan correctamente
/verificador - Reportar errores de compilación encontrados
```

---

### 8. **coordinador** - Coordinador de Agentes
**Uso**: Orquestar múltiples agentes para tareas complejas que requieren análisis, implementación y validación.

**Funcionalidad**:
- Analiza el requerimiento inicial
- Decide qué agentes usar y en qué orden
- Ejecuta tareas intermedias con agentes especializados
- Revisa resultados intermedios
- Genera un plan de trabajo completo
- Coordina la validación final

**Ejemplos**:
```
/coordinador - Añadir recuperación de contraseña completa
/coordinador - Refactorizar el módulo de autenticación
/coordinador - Implementar exportación de historiales a PDF
/coordinador - Auditoría completa del sistema de seguridad
```

**Flujo de coordinación**:
1. Analista: Análisis inicial y plan
2. Datos: Verificación de impacto en modelos
3. Revisor: Validación de diseño
4. Implementador: Implementación de cambios
5. Verificador: Build limpio
6. Revisor: Validación final
**Uso**: Orquestar múltiples agentes para tareas complejas que requieren análisis, implementación y validación.

**Funcionalidad**:
- Analiza el requerimiento inicial
- Decide qué agentes usar y en qué orden
- Ejecuta tareas intermedias con agentes especializados
- Revisa resultados intermedios
- Genera un plan de trabajo completo
- Coordina la validación final

**Ejemplos**:
```
/coordinador - Añadir recuperación de contraseña completa
/coordinador - Refactorizar el módulo de autenticación
/coordinador - Implementar exportación de historiales a PDF
/coordinador - Auditoría completa del sistema de seguridad
```

**Flujo de coordinación**:
1. Analista: Análisis inicial y plan
2. Datos: Verificación de impacto en modelos
3. Revisor: Validación de diseño
4. Implementador: Implementación de cambios
5. Verificador: Build limpio
6. Revisor: Validación final

---

## 🔧 Tools Disponibles

### Dotnet Commands

| Comando | Uso |
|---------|-----|
| `dotnet build` | Compilar la solución completa |
| `dotnet run --project src/HistoriaClinica.App` | Ejecutar la aplicación |
| `dotnet test` | Ejecutar tests xUnit |
| `dotnet add {project}.csproj {package}` | Añadir paquetes NuGet |

### Búsqueda

| Comando | Uso |
|---------|-----|
| `find src -name '**/filename.cs'` | Buscar archivos |
| `rg '{pattern}' --project '{project}'` | Buscar en código |

### Git

| Comando | Uso |
|---------|-----|
| `git status` | Ver estado del repositorio |
| `git diff` | Ver cambios |
| `git log --oneline` | Ver historial |
| `git commit` | Hacer commit |
| `git push` | Publicar cambios |

---

## 💡 Skills Disponibles

### built-in-browser
- Leer documentación web de Avalonia, .NET, EF Core
- Verificar compatibilidad de versiones
- Buscar best practices

### general
- Investigación de temas complejos
- Tareas multi-paso
- Coordinación de trabajo

### opencode
- Preguntas sobre OpenCode
- Configuración de herramientas
- Debugging de OpenCode

### explore
- Exploración rápida del código
- Localización de archivos
- Análisis de estructura

**Niveles**: `quick`, `medium`, `very thorough`

### datos
- EF Core 10, SQLite, esquemas
- Migrador propio, consultas
- Persistencia de datos
- Modelos: Paciente, FichaDiaria, Usuario, Plantilla, AuditLog

### analista
- Avalonia 12, .NET 10, MVVM
- EF Core 10, SQLite
- Arquitectura de capas
- Planificación de implementación

### implementador
- Implementar cambios de código
- Seguimiento de plan y convenciones
- Diff mínimo
- Sin commitear

### revisor
- Revisión de código read-only
- Detección de bugs, regresiones
- Problemas de EF Core, MVVM/Avalonia
- Seguridad de credenciales

### verificador
- Compilar y validar cambios
- Verificar build exitoso
- Reportar errores de compilación

---

## 📚 Archivos de Referencia

| Archivo | Contenido |
|---------|-----------|
| `AGENTS.md` | Guía principal de agentes |
| `docs/ARCHITECTURE.md` | Arquitectura del proyecto |
| `DEPENDENCIAS_Y_ARCHITECTURA.md` | Dependencias actuales |
| `DEUDA_TECNICA.md` | Informe de deuda técnica |
| `CONTRIBUTING.md` | Guía de contribución |
| `CODE_OF_CONDUCT.md` | Código de conducta |
| `SECURITY.md` | Seguridad y reporte de vulnerabilidades |

---

## 🚀 Flujo de Trabajo Recomendado

> 💡 **Para tareas complejas**: Usa `/coordinador - [describe la tarea]` para que automatice todo el proceso de análisis, implementación y validación.

### Para Tareas Complejas (Recomendado)

1. **Coordinador**: Orquestar todo el proceso completo
    ```
    /coordinador - [describe la tarea completa]
    ```

    El coordinador automatizará automáticamente:
    - Análisis inicial con 'analista'
    - Verificación de modelos con 'datos'
    - Revisión de diseño con 'revisor'
    - Implementación con 'implementador'
    - Validación de build con 'verificador'
    - Revisión final

### Para Tareas Específicas

#### Antes de Implementar

1. **Analista**: Analizar requerimiento e identificar alcance
   ```
   /analista - Analizar requerimiento: [describe]
   ```

2. **Datos**: Verificar impacto en modelos y base de datos
   ```
   /datos - Verificar impacto de: [describe]
   ```

3. **Revisor**: Revisar cambios propuestos
   ```
   /revisor - Revisar los cambios antes de implementar
   ```

4. **Verificador**: Confirmar que compila
   ```
   /verificador - Validar build limpio
   ```

### Durante la Implementación

5. **Explorador**: Localizar archivos si es necesario
   ```
   /explore quick - Buscar archivos relacionados
   ```

6. **Implementador**: Implementar los cambios
   ```
   /implementador - Implementar: [describe]
   ```

### Después de la Implementación

7. **Verificador**: Compilar y validar
   ```
   /verificador - Validar que compila
   ```

8. **Tests**: Ejecutar suite de tests
   ```
   dotnet test
   ```

9. **Revisor**: Revisión final
   ```
   /revisor - Revisión final de los cambios
   ```

---

## ⚠️ Restricciones Importantes

1. **NO modificar** `Data/Historia.db` (está versionada)
2. **NO tocar** `bin/` ni `obj/`
3. **UI, mensajes y comentarios** en español
4. **Fechas**: formato `dd/MM/yyyy`, cultura `es-ES`
5. **Async con await**, nunca fire-and-forget
6. **No agregar** paquetes NuGet sin confirmar
7. **No incluir** datos reales en tests
8. **No commit**, **no push**, **no PR** salvo pedido explícito

---

## 📖 Ejemplos de Uso

### Añadir un nuevo campo a Paciente

```
/analista - Analizar requerimiento: añadir campo "Alergia" a Paciente
/datos - Crear modelo Alergia, migración y validación
/implementador - Implementar todo el flujo completo
/verificador - Validar que compila sin errores
```

### Corregir un bug en autenticación

```
/analista - Analizar bug: usuario no puede loguearse
/revisor - Revisar PasswordHasher y AuthService
/implementador - Corregir el bug encontrado
/verificador - Validar que compila
```

### Implementar un nuevo diálogo

```
/analista - Analizar requerimiento: crear diálogo para exportar a PDF
/explore very thorough - Cómo se crean diálogos existentes
/datos - Verificar si se necesita nuevo modelo
/implementador - Implementar ventana y ViewModel
/verificador - Validar build
```

---

## 🎯 Preguntas Frecuentes

**Q: ¿Cuándo usar 'analista' vs 'general'?**
- `analista`: Para análisis específicos de requerimientos, planificación
- `general`: Para investigación amplia, tareas complejas multi-paso

**Q: ¿Cuándo usar 'datos' vs 'implementador'?**
- `datos`: Cuando toca modelos, migraciones, consultas EF Core
- `implementador`: Para implementar cualquier lógica de negocio o UI

**Q: ¿Cuándo usar 'revisor' vs 'verificador'?**
- `revisor`: Para revisar código, detectar bugs potenciales
- `verificador`: Para compilar y verificar que el build es limpio

**Q: ¿Necesito usar todos los agentes?**
- No, usa solo los necesarios según la tarea
- `analista` + `implementador` para features nuevas
- `revisor` + `verificador` para validación

**Q: ¿Cuándo usar 'coordinador'?**
- `coordinador`: Para tareas complejas que requieren múltiples pasos
  - Añadir funcionalidades completas (ej: exportación a PDF)
  - Refactorizaciones de módulos
  - Auditorías de seguridad
  - Implementaciones integras (frontend + backend + DB)
- El coordinador automatiza todo el flujo de trabajo

**Q: ¿Cuándo NO usar 'coordinador'?**
- Para cambios simples (una línea, un pequeño fix)
- Cuando ya tienes el plan de implementación
- Cuando necesitas iteraciones rápidas de feedback

---

## 📝 Notas de Seguridad

- Los datos clínicos en la base de datos **NO están cifrados** (deuda técnica P0)
- Las credenciales usan PBKDF2-SHA256 con 210.000 iteraciones
- **No incluir** datos reales en issues, PRs, capturas o logs
- **No registrar** credenciales ni datos médicos sensibles

---

*Documento generado para HistoriaClinica - 2 de Noviembre 2026*
