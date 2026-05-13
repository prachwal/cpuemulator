using CpuEmulator.Abstractions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja NOP (No Operation) - nie wykonuje żadnej operacji.
/// </summary>
public class NopInstruction : IInstruction
{
    /// <inheritdoc />
    /// <summary>
    /// Wykonuje instrukcję NOP.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja do wykonania.</param>
    /// <returns>Niezmieniony stan CPU.</returns>
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        return state;
    }
}
