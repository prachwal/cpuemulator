using CpuEmulator.Abstractions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Halt - zatrzymuje wykonanie programu.
/// </summary>
public class HaltInstruction : IInstruction
{
    /// <inheritdoc />
    /// <summary>
    /// Wykonuje instrukcję Halt - zatrzymuje wykonanie programu.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja do wykonania.</param>
    /// <returns>Nowy stan CPU z ustawioną flagą zatrzymania.</returns>
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        return state.WithHalted(true);
    }
}
