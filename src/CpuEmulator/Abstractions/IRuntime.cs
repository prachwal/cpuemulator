using CpuEmulator.Model;

namespace CpuEmulator.Abstractions;

/// <summary>
/// Interfejs dla środowiska wykonawczego CPU.
/// </summary>
public interface IRuntime
{
    /// <summary>
    /// Zwraca listę instrukcji programu.
    /// </summary>
    IReadOnlyList<Instruction> Program { get; }

    /// <summary>
    /// Zwraca aktualną wartość licznika programu.
    /// </summary>
    int ProgramCounter { get; }

    /// <summary>
    /// Ustawia wartość licznika programu.
    /// </summary>
    /// <param name="value">Nowa wartość licznika programu.</param>
    /// <exception cref="ProgramCounterOutOfRangeException">Rzucane, gdy wartość jest poza zakresem programu.</exception>
    void SetProgramCounter(int value);

    /// <summary>
    /// Zatrzymuje wykonanie programu.
    /// </summary>
    void Halt();

    /// <summary>
    /// Zwraca informację, czy program został zatrzymany.
    /// </summary>
    bool IsHalted { get; }

    /// <summary>
    /// Zwraca flagi procesora.
    /// </summary>
    CpuFlags Flags { get; }

    /// <summary>
    /// Wkłada wartość na stos.
    /// </summary>
    /// <param name="value">Wartość do włożenia na stos.</param>
    void PushStack(int value);

    /// <summary>
    /// Ściąga wartość ze stosu.
    /// </summary>
    /// <returns>Wartość ściągnięta ze stosu.</returns>
    /// <exception cref="StackUnderflowException">Rzucane, gdy stos jest pusty.</exception>
    int PopStack();

    /// <summary>
    /// Zwraca liczbę elementów na stosie.
    /// </summary>
    int StackCount { get; }
}
