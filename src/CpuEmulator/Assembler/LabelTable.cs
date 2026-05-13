using CpuEmulator.Exceptions;

namespace CpuEmulator.Assembler;

/// <summary>
/// Tabela etykiet używana podczas asemblacji.
/// </summary>
public class LabelTable
{
    private readonly Dictionary<string, int> _labels = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _unresolvedLabels = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Definiuje nową etykietę.
    /// </summary>
    /// <param name="label">Nazwa etykiety.</param>
    /// <param name="address">Adres (indeks instrukcji), do którego etykieta się odnosi.</param>
    /// <exception cref="AssemblerException">Rzucane, gdy etykieta jest zduplikowana.</exception>
    public void Define(string label, int address)
    {
        if (_labels.ContainsKey(label))
        {
            throw new AssemblerException($"Duplicate label: '{label}'", 0, 0);
        }
        _labels[label] = address;
    }

    /// <summary>
    /// Próbuje rozwiązać etykietę do adresu.
    /// </summary>
    /// <param name="label">Nazwa etykiety.</param>
    /// <param name="address">Adres, do którego etykieta się odnosi.</param>
    /// <returns>Prawda, jeśli etykieta została znaleziona.</returns>
    public bool TryResolve(string label, out int address)
    {
        return _labels.TryGetValue(label, out address);
    }

    /// <summary>
    /// Rejestruje nierozwiązaną etykietę (używaną w operandzie).
    /// </summary>
    /// <param name="label">Nazwa etykiety.</param>
    public void RegisterUnresolved(string label)
    {
        _unresolvedLabels.Add(label);
    }

    /// <summary>
    /// Zwraca kolekcję nierozwiązanych etykiet.
    /// </summary>
    public IReadOnlyCollection<string> UnresolvedLabels => _unresolvedLabels;

    /// <summary>
    /// Sprawdza, czy wszystkie etykiety zostały rozwiązane.
    /// </summary>
    /// <returns>Prawda, jeśli nie ma nierozwiązanych etykiet.</returns>
    public bool AllResolved => _unresolvedLabels.Count == 0;
}
