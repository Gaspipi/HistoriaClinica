# Dependencias y Arquitectura Actual - HistoriaClinica

**Fecha:** 2 de Noviembre 2026  
**Versión:** 1.0

---

## 📦 Dependencias Actuales

### HistoriaClinica.App (UI Layer)
| Paquete | Versión | Estado | Notas |
|---------|---------|--------|-------|
| Avalonia | 12.1.4 | ✅ Estable | UI principal |
| Avalonia.Desktop | 12.1.4 | ✅ Estable | Plataforma Desktop |
| Avalonia.Fonts.Inter | 12.1.4 | ✅ Estable | Fuentes Inter |
| Avalonia.Themes.Fluent | 12.1.4 | ✅ Estable | Tema Fluent |
| CommunityToolkit.Mvvm | 8.4.2 | ✅ Reciente | MVVM framework |
| Microsoft.Extensions.DependencyInjection | 10.0.13 | ✅ Reciente | DI |
| Microsoft.Extensions.Logging.Console | 10.0.13 | ✅ Reciente | Logging console |
| QuestPDF | 2025.12.4 | ⚠️ Futuro 2025 | Generación PDF |
| Tmds.DBus.Protocol | 0.95.1 | ⚠️ Fijo | DBus Protocol |

### HistoriaClinica.Core (Domain Layer)
| Paquete | Versión | Estado | Notas |
|---------|---------|--------|-------|
| Microsoft.EntityFrameworkCore.Sqlite | 10.0.13 | ✅ Reciente | EF Core + SQLite |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.14 | ✅ Reciente | DI Abstractions |
| Microsoft.Extensions.Logging.Abstractions | 10.0.14 | ✅ Reciente | Logging Abstractions |

### HistoriaClinica.Tests (Test Layer)
| Paquete | Versión | Estado | Notas |
|---------|---------|--------|-------|
| coverlet.collector | 10.1.0 | ✅ Estable | Code coverage |
| Microsoft.NET.Test.Sdk | 18.11.0 | ⚠️ Legacy | Tests SDK |
| xunit | 2.9.3 | ✅ Estable | Test framework |
| xunit.runner.visualstudio | 4.0.1 | ✅ Estable | Test explorer |

---

## 🏗️ Arquitectura Actual

### Estructura por Capas

```
HistoriaClinica/
├── src/HistoriaClinica.App/          # UI Layer (Avalonia)
│   ├── Views/                        # Avalonia Views (.axaml)
│   ├── ViewModels/                   # MVVM ViewModels
│   ├── Services/                     # UI Services
│   ├── Logging/                      # FileLoggerProvider
│   └── Assets/                       # Icons, themes
├── src/HistoriaClinica.Core/         # Domain Layer
│   ├── Models/                       # EF Core Entities
│   ├── Data/                         # Context, Migrator
│   ├── Repositories/                 # EF Core Repositories
│   ├── Services/                     # Business Logic Services
│   ├── Validation/                   # Validators
│   ├── Security/                     # PasswordHasher
│   ├── Logging/                      # FileLoggerProvider (Core)
│   └── DependencyInjection.cs        # DI Registration
└── tests/HistoriaClinica.Tests/      # Test Layer
    ├── DatabaseSchemaMigratorV7Tests.cs
    ├── DependencyInjectionTests.cs
    ├── PasswordHasherTests.cs
    └── ...
```

### Patrones de Arquitectura

| Patrones | Implementación | Estado |
|----------|----------------|--------|
| MVVM | CommunityToolkit.Mvvm | ✅ Implementado |
| Dependency Injection | Microsoft.Extensions.DependencyInjection | ✅ Implementado |
| Repository Pattern | EF Core wrappers | ✅ Implementado |
| Clean Architecture | Layers separadas (App/Core) | ✅ Implementado |
| Event-Driven | No implementado | ⚠️ Ausente |
| CQRS | No implementado | ⚠️ Ausente |

---

## 🔍 Análisis de Dependencias

### ✅ Buenas Prácticas Implementadas

1. **Target Framework Unificado**
   - Todos los proyectos usan `net10.0`
   - Compatible con .NET 10 LTS
   - Cross-platform (Linux, Windows, macOS)

2. **Nullable Reference Types**
   - `Nullable>enable` habilitado en todos los proyectos
   - Reduce errores de runtime
   - Mejor type safety

3. **Architecture Separation**
   - Core (dominio) independiente de UI
   - Tests pueden correr sin App UI
   - Fácil refactorización UI

4. **Tests de Migraciones**
   - `DatabaseSchemaMigratorV7Tests.cs`
   - Pruebas de concurrencia
   - Validación de rollback

### ⚠️ Dependencias con Riesgos

1. **Tmds.DBus.Protocol 0.95.1 (Fijo)**
   ```xml
   <PackageReference Include="Tmds.DBus.Protocol" Version="0.95.1" />
   ```
   - **Problema:** Version fija, dificulta upgrades
   - **Impacto:** No se puede actualizar Avalonia sin romper DBus
   - **Solución:** Evaluar si es realmente necesario

2. **QuestPDF 2025.12.4 (Futuro)**
   ```xml
   <PackageReference Include="QuestPDF" Version="2025.12.4" />
   ```
   - **Problema:** Versión del futuro 2025
   - **Impacto:** Posible incompatibilidad con .NET 10
   - **Solución:** Verificar compatibilidad actual

3. **Microsoft.NET.Test.Sdk 18.11.0 (Legacy)**
   ```xml
   <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.11.0" />
   ```
   - **Problema:** Versión antigua de SDK
   - **Impacto:** Tests pueden no correr en .NET 10
   - **Solución:** Actualizar a 18.0.0 o superior

### 🎯 Recomendaciones de Dependencias

| Acción | Prioridad | Esfuerzo |
|--------|-----------|----------|
| Actualizar Microsoft.NET.Test.Sdk | P2 | Bajo |
| Verificar QuestPDF compatibilidad | P2 | Medio |
| Evaluar Tmds.DBus Protocol | P3 | Bajo |
| Actualizar todos a .NET 10 LTS | P0 | Medio |
| Auditar dependencias transitivas | P0 | Bajo |

---

## 🏛️ Arquitectura de Datos

### EF Core Configuración

```csharp
// src/HistoriaClinica.Core/Data/HistoriaContext.cs
public class HistoriaContext : DbContext
{
    public HistoriaContext(IDbContextFactory factory) : base(factory.GetDbContext())
    {
    }

    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<FichaDiaria> FichasDiarias { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Plantilla> Plantillas { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
}
```

### Repositories Pattern

```csharp
// Patrón actual
public class PacienteRepository : IPacienteRepository
{
    private readonly HistoriaContext _context;

    public PacienteRepository(HistoriaContext context)
    {
        _context = context;
    }

    public async Task<Paciente> GetByIdAsync(int id)
    {
        return await _context.Pacientes.FindAsync(id);
    }
}
```

### Migraciones Propias

```csharp
// src/HistoriaClinica.Core/Data/DatabaseSchemaMigrator.cs
public class DatabaseSchemaMigrator
{
    public const int CurrentVersion = 7;
    private static readonly Dictionary<int, string> _migrations = new();

    public void Migrate(IDbContext context)
    {
        // Migraciones manuales, no EF Migrations
    }
}
```

### SQLite Optimizaciones

| Optimización | Implementado | Estado |
|--------------|--------------|--------|
| Busy Timeout Interceptor | ✅ `SqliteBusyTimeoutInterceptor.cs` | Implementado |
| Backup automático | ✅ `SqliteBackupService.cs` | Implementado |
| Validación de DNI | ✅ `DniValidator.cs` | Implementado |
| Triggers EF Core | ❌ No implementado | ⚠️ Falta |

---

## 🔐 Seguridad de Dependencias

### PasswordHasher

```csharp
// src/HistoriaClinica.Core/Security/PasswordHasher.cs
public class PasswordHasher
{
    private readonly _PBKDF2_SHA256;
    private const int Iterations = 210000;
    private const int SaltLength = 16;

    public string Hash(string password)
    {
        // PBKDF2-SHA256
    }

    public bool Verify(string password, string hash)
    {
        // Comparación constante
    }
}
```

**Análisis:**
- ✅ Iteraciones: 210.000 (Bueno)
- ✅ Sal: 16 bytes (Aceptable, pero 32+ sería mejor)
- ✅ Función: PBKDF2-SHA256 (Aceptable, pero Argon2 sería mejor)
- ⚠️ Sin salt automático (Puede ser un problema)

### Recomendaciones de Seguridad

| Acción | Prioridad | Esfuerzo |
|--------|-----------|----------|
| Añadir Argon2 o scrypt | P0 | Medio |
| Incrementar iteraciones a 1M+ | P1 | Bajo |
| Sal de 32+ bytes | P1 | Bajo |
| Cifrado de datos sensibles | P0 | Alto |

---

## 📊 Estado de la Deuda Técnica

| Área | Hallazgos | Impacto | Prioridad |
|------|-----------|---------|-----------|
| **Seguridad** | 5 hallazgos | Alto | P0 |
| **Persistencia** | 5 hallazgos | Alto | P0 |
| **Logging** | 3 hallazgos | Medio | P2 |
| **Distribución** | 3 hallazgos | Alto | P0 |
| **Dependencias** | 3 hallazgos | Medio | P2 |
| **Arquitectura** | 2 hallazgos | Bajo | P2 |

**Total:** 21 hallazgos identificados

---

## 🚀 Plan de Acción

### Fase 1: Seguridad Crítica (P0)
- [ ] Implementar Argon2 o scrypt para hashing
- [ ] Añadir cifrado AES-256 para datos médicos
- [ ] Implementar recuperación de contraseña

### Fase 2: Dependencias (P1)
- [ ] Actualizar Microsoft.NET.Test.Sdk
- [ ] Verificar QuestPDF 2025.12.4 compatibilidad
- [ ] Auditar dependencias transitivas

### Fase 3: Arquitectura (P2)
- [ ] Implementar Event-Driven pattern
- [ ] Implementar CQRS pattern
- [ ] Centralizar logging

### Fase 4: Optimizaciones (P3)
- [ ] Triggers EF Core para validación
- [ ] Optimización de consultas
- [ ] Caching de datos

---

## 📝 Archivos Clave

### Configuración de Dependencias
- `src/HistoriaClinica.App/HistoriaClinica.App.csproj`
- `src/HistoriaClinica.Core/HistoriaClinica.Core.csproj`
- `tests/HistoriaClinica.Tests/HistoriaClinica.Tests.csproj`

### Arquitectura
- `src/HistoriaClinica.Core/DependencyInjection.cs`
- `src/HistoriaClinica.App/App.axaml.cs`
- `src/HistoriaClinica.App/ViewModels/*.cs`

### Datos
- `src/HistoriaClinica.Core/Data/HistoriaContext.cs`
- `src/HistoriaClinica.Core/Data/DatabaseSchemaMigrator.cs`
- `src/HistoriaClinica.Core/Data/SqliteBusyTimeoutInterceptor.cs`

### Seguridad
- `src/HistoriaClinica.Core/Security/PasswordHasher.cs`

---

*Este documento se actualizará automáticamente cuando se implementen mejoras de arquitectura o dependencias.*
