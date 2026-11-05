Resumen de la Exploración del Repositorio HistoriaClinica
📁 Estructura de Proyectos
El repositorio tiene 3 proyectos principales:
Proyecto	Ruta	Descripción
HistoriaClinica.App	src/HistoriaClinica.App/	UI con Avalonia 12 (6 ViewModels, 2 Views)
HistoriaClinica.Core	src/HistoriaClinica.Core/	Dominio: modelos, repositorios, servicios
HistoriaClinica.Tests	tests/HistoriaClinica.Tests/	Tests xUnit
🏛️ Modelos de Datos (Core/Models)
1. Usuario - Dni (8 dígitos), Nombre, Email, Contraseña (PBKDF2-SHA256)
2. Paciente - Dni, Nombre, Apellidos, FechaNacimiento, Contacto
3. FichaDiaria - Dni, Fecha (dd/MM/yyyy), Notas, Saliendo (bool), Vencido (bool)
4. Plantilla - Nombre (validado por HistoriaValidators.ValidatePlantilla)
5. AuditLog - Dni, Fecha, Tipo (CRUD)
🔐 Seguridad
- Hasher: /src/HistoriaClinica.Core/Security/PasswordHasher.cs
- PBKDF2-SHA256
- 210.000 iteraciones
- Sal de 16 bytes
- Bloqueo por intentos fallidos
📊 Servicios (Core/Services)
Servicio	Métodos Principales
AuthService	RegisterAsync(), LoginAsync()
PacienteService	GetDnisAsync(), GetByDniAsync(), SearchAsync()
FichaService	GetByDniAsync(), GetByIdAsync(), SaveAsync()
PlantillaService	Normalizar() + validación
🗄️ Base de Datos
- Contexto: HistoriaContext.cs con ValueConverter personalizado
- Migraciones: DatabaseSchemaMigrator.cs (propia implementación)
- Versionado por PRAGMA user_version
- Backups en Backups/
- Ruta por defecto: Documentos/Historia.db
🖥️ UI (App)
ViewModels:
- MainWindowViewModel.cs - Navegación principal
- AuthViewModel.cs - Login/Registro
- PatientEditorViewModel.cs - CRUD de pacientes
- FichaEditorViewModel.cs - CRUD de fichas diarias
- PreferencesViewModel.cs - Configuración tema/tamaño
- DataMaintenanceViewModel.cs - Mantenimiento base de datos
Styles:
- Theme.axaml - Definición de temas
- Metrics.axaml - Métricas de diseño
- Controls.axaml - Estilos de controles
📝 Convenciones Clave
- Cultura: es-ES (fechas dd/MM/yyyy)
- DNI: 8 dígitos, validación con DniValidator
- MVVM: CommunityToolkit.Mvvm
- Async: Siempre con await, nunca fire-and-forget
- Database: EnsureCreated solo para bases nuevas
🚀 Ejecución
# Compilar
dotnet build HistoriaClinica.sln

# Ejecutar (Linux/Windows/macOS)
dotnet run --project src/HistoriaClinica.App
📦 Deuda Técnica
- Migrador propio (sin EF Migrations)
- Logging solo a consola
- Avalonia 12.1.4 fija Tmds.DBus.Protocol 0.95.1
- Instalador no firmado
- Falta validación visual en Windows
