# Diagramas de Arquitectura — UI

**Versión:** 1.0
**Formato:** Markdown + Mermaid

---

## 📐 Diagrama de Arquitectura UI

```
┌─────────────────────────────────────────────────────────────┐
│                    Historia Clínica                         │
│                         UI Layer                            │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  MainWindow (Dashboard) ───┬────────────────────────────────┤
│  AuthWindow ───────────────┤                                │
│  PatientEditor ────────────┤                                │
│  FichaEditor ──────────────┤                                │
│  PreferencesWindow ────────┤                                │
│  DataMaintenanceWindow ─────┤                                │
│  MessageDialog ─────────────┤                                │
└────────────────────────────┼────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│  ViewModels (Lógica de UI)                                   │
│  ────────────────────────────────────────────────────────── │
│  AuthViewModel           │  MainWindowViewModel            │
│  PatientEditorViewModel  │  FichaEditorViewModel           │
│  PreferencesViewModel    │  DataMaintenanceViewModel       │
│  MessageDialogViewModel  │  EditorViewModelBase             │
└─────────────────────────────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│  Services (Lógica de Negocio)                                │
│  ────────────────────────────────────────────────────────── │
│  AuthService            │  PatientService                  │
│  FichaService           │  PreferencesService              │
│  DataMaintenanceService │  MessageService                  │
└─────────────────────────────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│  Core (Dominio y Datos)                                       │
│  ────────────────────────────────────────────────────────── │
│  Models                │  Repositories                     │
│  (Usuario, Paciente,   │  (IUsuarioRepository,             │
│   Ficha)               │   IPacienteRepository,            │
│                        │   IFichaRepository)               │
│  Services             │  Converters                        │
│  (AuthService,         │  (DateTimeOffsetConverter,         │
│   PatientService)      │  FechaConverter)                   │
│                        │                                    │
│  Security             │  Layout                            │
│  (PasswordHasher)     │  (ResponsiveGrid, FooterGrid)       │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔄 Flujo de Datos

```
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│  Usuario     │      │   Paciente   │      │   Ficha       │
│  Login       │      │  Dni         │      │  Motivo      │
│  Password    │      │  Fecha       │      │  Enfermedad  │
└──────┬──────┘      └──────┬──────┘      └──────┬───────┘
       │                    │                    │
       ▼                    ▼                    ▼
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│  AuthService │      │ PatientRepo │      │  FichaRepo   │
│  Validate    │      │  Save/Get   │      │  Save/Get    │
└──────┬──────┘      └──────┬──────┘      └──────┬───────┘
       │                    │                    │
       ▼                    ▼                    ▼
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│  Historia     │      │   SQLite    │      │   SQLite     │
│  Clinica.db   │      │  Pacientes  │      │  Fichas      │
│               │      │               │      │              │
└──────────────┘      └──────────────┘      └──────────────┘
```

---

## 📦 Arquitectura MVVM

```
┌─────────────────────────────────────────────────────────────┐
│  XAML (Vista)                                               │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  AuthWindow.axaml                                    │  │
│  │  MainWindow.axaml                                    │  │
│  │  PatientEditorWindow.axaml                           │  │
│  │  FichaEditorWindow.axaml                             │  │
│  │  PreferencesWindow.axaml                             │  │
│  │  DataMaintenanceWindow.axaml                         │  │
│  │  MessageDialog.axaml                                 │  │
│  └───────────────────────────────────────────────────────┘  │
│                                                             │
│  ────────────────────────────────────────────────────────── │
│  ─── Binding (TwoWay) ─────────────────────────────────── │
│  ────────────────────────────────────────────────────────── │
│                                                             │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  ViewModels (ViewModels)                              │  │
│  │  ┌──────────────────────────────────────────────────┐ │  │
│  │  │  AuthViewModel                                  │ │  │
│  │  │  MainWindowViewModel                            │ │  │
│  │  │  PatientEditorViewModel                         │ │  │
│  │  │  FichaEditorViewModel                           │ │  │
│  │  │  PreferencesViewModel                           │ │  │
│  │  │  DataMaintenanceViewModel                        │ │  │
│  │  │  MessageDialogViewModel                          │ │  │
│  │  └──────────────────────────────────────────────────┘ │  │
│  └─────────────────────────────────────────────────────────┘  │
│                                                             │
│  ─── Commands ─────────────────────────────────────────── │
│  ────────────────────────────────────────────────────────── │
│  ─── Observables (INotifyPropertyChanged) ────────────── │
│  ────────────────────────────────────────────────────────── │
│                                                             │
┌─────────────────────────────────────────────────────────────┐
│  Services (Lógica de Negocio)                               │
│  ────────────────────────────────────────────────────────── │
│  AuthService, PatientService, FichaService, etc.           │
│                                                             │
┌─────────────────────────────────────────────────────────────┐
│  Repositories (Acceso a Datos)                             │
│  ────────────────────────────────────────────────────────── │
│  IUsuarioRepository, IPacienteRepository, IFichaRepository │
│                                                             │
┌─────────────────────────────────────────────────────────────┐
│  Core (Dominio)                                             │
│  ────────────────────────────────────────────────────────── │
│  Usuario, Paciente, FichaDiaria                             │
│                                                             │
┌─────────────────────────────────────────────────────────────┐
│  Database (SQLite)                                          │
│  ────────────────────────────────────────────────────────── │
│  Historia.db (Documentos/Historia.db)                      │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 🎨 Componentes UI

### **1. Window Base**

```
┌─────────────────────────────────────────────────────────────┐
│  Window.axaml                                               │
│  ┌───────────────────────────────────────────────────────┐ │
│  │  Header (Logo, Título, Subtítulo)                     │ │
│  │  ┌──────────────────────────────────────────────────┐ │ │
│  │  │  StackPanel (Contenido Principal)               │ │ │
│  │  │  ┌────────────────────────────────────────────┐ │ │ │
│  │  │  │  Grid (Layout)                            │ │ │ │
│  │  │  │  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐       │ │ │
│  │  │  │  Card   │ Card   │ Card   │ Card   │       │ │ │
│  │  │  │         │         │         │         │       │ │ │
│  │  │  └─────────┴─────────┴─────────┴─────────┘       │ │ │
│  │  │                                                   │ │ │
│  │  └──────────────────────────────────────────────────┘ │ │
│  │                                                       │ │
│  │  ┌──────────────────────────────────────────────────┐ │ │
│  │  │  Footer (Actions, Progress)                      │ │ │
│  │  └──────────────────────────────────────────────────┘ │ │
│  └───────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### **2. Card Component**

```
┌─────────────────────────────────────────────────────────────┐
│  Border Classes="card"                                     │
│  ┌───────────────────────────────────────────────────────┐ │
│  │  StackPanel Spacing="12"                             │ │
│  │  ┌──────────────────────────────────────────────────┐ │ │
│  │  │  TextBlock SectionTitle                         │ │ │
│  │  │  ┌────────────────────────────────────────────┐ │ │ │
│  │  │  │  StackPanel (Campo 1)                     │ │ │ │
│  │  │  │  ┌──────────────────────────────────────┐ │ │ │
│  │  │  │  │  StackPanel (Etiqueta)              │ │ │ │
│  │  │  │  └──────────────────────────────────────┘ │ │ │
│  │  │  │  ┌──────────────────────────────────────┐ │ │ │
│  │  │  │  │  StackPanel (Control)               │ │ │ │
│  │  │  │  └──────────────────────────────────────┘ │ │ │
│  │  │  └────────────────────────────────────────────┘ │ │
│  │  └──────────────────────────────────────────────────┘ │
│  └───────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔄 Flujo de Usuario

### **1. Inicio de Sesión**

```
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│  AuthWindow  │      │  AuthService │      │   Historia   │
│              │      │              │      │  Clinica.db   │
└──────┬───────┘      └──────┬───────┘      └──────┬───────┘
       │                     │                      │
       ▼                     ▼                      ▼
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│  Validate    │      │  Validate    │      │  Load/Verify │
│  Credentials  │      │  Credentials │      │  Database    │
└──────┬───────┘      └──────┬───────┘      └──────┬───────┘
       │                     │                      │
       ▼                     ▼                      ▼
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│  MainWindow  │      │  Success     │      │  Ready        │
│              │      │               │      │               │
└──────────────┘      └──────────────┘      └───────────────┘
```

### **2. CRUD Paciente**

```
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│  MainWindow  │      │ PatientEditor│      │   Save       │
│              │      │              │      │              │
└──────┬───────┘      └──────┬───────┘      └──────┬───────┘
       │                     │                      │
       ▼                     ▼                      ▼
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│  + Nuevo     │      │  Formulario  │      │  PatientRepo  │
│  Paciente    │      │  TwoWay      │      │  Save/Get    │
└──────┬───────┘      └──────┬───────┘      └──────┬───────┘
       │                     │                      │
       ▼                     ▼                      ▼
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│  Update UI   │      │  Validate    │      │  Update UI    │
│              │      │  Errors      │      │  Ready        │
└──────────────┘      └──────────────┘      └───────────────┘
```

---

## 📊 Métricas UI

| Métrica | Valor |
|---------|-------|
| **Archivos Views** | 8 |
| **Archivos ViewModels** | 7 |
| **Tamaño total Views** | ~136 KB |
| **Tamaño total ViewModels** | ~85 KB |
| **Keybindings** | 4 |
| **Clases responsive** | 3 (`compact`, `narrow`, `short`) |
| **Conversiones** | 5 |
| **Layouts** | 2 (`ResponsiveGrid`, `FooterGrid`) |

---

## 📝 Leyendas

- `┌─────┐` Inicio de bloque
- `──────┘` Fin de bloque
- `│` Línea vertical
- `├────┤` Línea horizontal con conexión
- `└────┘` Conexión cruzada

---

**Documento generado:** 2024
**Formato:** Markdown + Mermaid
**Versión:** 1.0
