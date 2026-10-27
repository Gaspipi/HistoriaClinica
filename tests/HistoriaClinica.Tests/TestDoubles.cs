using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;

namespace HistoriaClinica.Tests;

internal sealed class FakePacienteRepository : IPacienteRepository
{
    private readonly Dictionary<string, Paciente> _pacientes = new(StringComparer.Ordinal);

    public IReadOnlyCollection<Paciente> Pacientes => _pacientes.Values;

    public void Agregar(Paciente paciente) => _pacientes[paciente.Dni] = paciente;

    public Task<IReadOnlyList<string>> GetDnisAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<string>>(_pacientes.Keys.OrderBy(dni => dni).ToList());

    public Task<Paciente?> GetByDniAsync(string dni, CancellationToken cancellationToken = default) =>
        Task.FromResult(_pacientes.TryGetValue(dni, out var paciente) ? paciente : null);

    public Task SaveAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        _pacientes[paciente.Dni] = paciente;
        return Task.CompletedTask;
    }
}

internal sealed class FakeFichaRepository : IFichaRepository
{
    private readonly List<FichaDiaria> _fichas = new();
    private int _siguienteId = 1;

    public IReadOnlyCollection<FichaDiaria> Fichas => _fichas;

    public FichaDiaria Agregar(FichaDiaria ficha)
    {
        if (ficha.Id == 0)
        {
            ficha.Id = _siguienteId++;
        }

        _fichas.Add(ficha);
        return ficha;
    }

    public Task<IReadOnlyList<FichaDiaria>> GetByDniAsync(string dni, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<FichaDiaria>>(_fichas
            .Where(ficha => ficha.Dni == dni)
            .OrderByDescending(ficha => ficha.Fecha)
            .ThenByDescending(ficha => ficha.Id)
            .ToList());

    public Task<FichaDiaria?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_fichas.FirstOrDefault(ficha => ficha.Id == id));

    public Task SaveAsync(FichaDiaria ficha, CancellationToken cancellationToken = default)
    {
        if (ficha.Id == 0)
        {
            ficha.Id = _siguienteId++;
        }

        var indice = _fichas.FindIndex(actual => actual.Id == ficha.Id);
        if (indice >= 0)
        {
            _fichas[indice] = ficha;
        }
        else
        {
            _fichas.Add(ficha);
        }

        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_fichas.RemoveAll(ficha => ficha.Id == id) > 0);
}

internal sealed class FakeDialogService : IDialogService
{
    public Paciente? PacienteResultado { get; set; }

    public FichaDiaria? FichaResultado { get; set; }

    public bool ConfirmarResultado { get; set; } = true;

    public int Confirmaciones { get; private set; }

    public Paciente? PacienteSolicitado { get; private set; }

    public FichaDiaria? FichaSolicitada { get; private set; }

    public string? NombrePacienteSolicitado { get; private set; }

    public bool? EsNuevoSolicitado { get; private set; }

    public string? UltimoMensajeConfirmacion { get; private set; }

    public bool? UltimaConfirmacionDestructiva { get; private set; }

    public int PreferenciasMostradas { get; private set; }

    public Task<Paciente?> ShowPatientEditorAsync(Paciente paciente, bool esNuevo)
    {
        PacienteSolicitado = paciente;
        EsNuevoSolicitado = esNuevo;
        return Task.FromResult(PacienteResultado);
    }

    public Task<FichaDiaria?> ShowFichaEditorAsync(
        FichaDiaria ficha,
        bool esNuevo,
        string nombrePaciente = "")
    {
        FichaSolicitada = ficha;
        EsNuevoSolicitado = esNuevo;
        NombrePacienteSolicitado = nombrePaciente;
        return Task.FromResult(FichaResultado);
    }

    public Task<bool> ConfirmAsync(
        string titulo,
        string mensaje,
        string aceptar = "Sí",
        string cancelar = "No",
        bool esDestructiva = false)
    {
        Confirmaciones++;
        UltimoMensajeConfirmacion = mensaje;
        UltimaConfirmacionDestructiva = esDestructiva;
        return Task.FromResult(ConfirmarResultado);
    }

    public Task ShowPreferencesAsync()
    {
        PreferenciasMostradas++;
        return Task.CompletedTask;
    }

    public List<(string Titulo, string Mensaje)> MensajesInfo { get; } = new();

    public Task ShowErrorAsync(string titulo, string mensaje) => Task.CompletedTask;

    public Task ShowInfoAsync(string titulo, string mensaje)
    {
        MensajesInfo.Add((titulo, mensaje));
        return Task.CompletedTask;
    }
}

internal sealed class FakeClipboardService : IClipboardService
{
    public string? Texto { get; private set; }

    public Task CopiarAsync(string texto)
    {
        Texto = texto;
        return Task.CompletedTask;
    }
}

internal sealed class FakeAppNavigator : IAppNavigator
{
    public int VecesAuth { get; private set; }

    public int VecesMain { get; private set; }

    public void ShowAuth() => VecesAuth++;

    public void ShowMain() => VecesMain++;
}

internal sealed class FakeDataMaintenanceService : IDataMaintenanceService
{
    public string DatabasePath { get; set; } = Path.Combine(Path.GetTempPath(), "Historia.db");

    public bool CarpetaAbierta { get; private set; }

    public bool CopiaCreada { get; private set; }

    public string RutaCopia { get; set; } = Path.Combine(Path.GetTempPath(), "Backups", "Historia.db");

    public void AbrirCarpetaDatos() => CarpetaAbierta = true;

    public Task<string> CrearCopiaSeguridadAsync(CancellationToken cancellationToken = default)
    {
        CopiaCreada = true;
        return Task.FromResult(RutaCopia);
    }

    public Task<string> RestaurarBackupAsync(string backupPath, CancellationToken cancellationToken = default) =>
        Task.FromResult(string.Empty);

    public Task<IReadOnlyList<BackupInfo>> ListarBackupsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<BackupInfo>>(Array.Empty<BackupInfo>());
}
