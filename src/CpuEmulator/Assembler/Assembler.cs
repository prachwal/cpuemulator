using CpuEmulator.Exceptions;

namespace CpuEmulator.Assembler;

/// <summary>
/// Assembler konwertujący kod asemblera na instrukcje CPU.
/// </summary>
public class Assembler
{
    private readonly Tokenizer _tokenizer = new();
    private readonly Parser _parser = new();

    /// <summary>
    /// Asembluje kod asemblera na listę instrukcji CPU.
    /// </summary>
    /// <param name="asmCode">Kod asemblera.</param>
    /// <returns>Lista instrukcji CPU.</returns>
    /// <exception cref="AssemblerException">Rzucane, gdy wystąpi błąd podczas asemblacji.</exception>
    public List<Instruction> Assemble(string asmCode)
    {
        // Tokenizacja
        var tokens = _tokenizer.Tokenize(asmCode);

        // Parsowanie (przebieg 1: zbieranie etykiet)
        var (parsedInstructions, labels) = _parser.Parse(tokens);

        // Rozwiązywanie etykiet (przebieg 2: zamiana etykiet na adresy)
        var resolver = new InstructionResolver(labels);
        return resolver.Resolve(parsedInstructions);
    }

    /// <summary>
    /// Asembluje plik asemblera na listę instrukcji CPU.
    /// </summary>
    /// <param name="filePath">Ścieżka do pliku asemblera.</param>
    /// <returns>Lista instrukcji CPU.</returns>
    /// <exception cref="AssemblerException">Rzucane, gdy wystąpi błąd podczas asemblacji.</exception>
    /// <exception cref="FileNotFoundException">Rzucane, gdy plik nie istnieje.</exception>
    public List<Instruction> AssembleFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        string asmCode = File.ReadAllText(filePath);
        return Assemble(asmCode);
    }
}
