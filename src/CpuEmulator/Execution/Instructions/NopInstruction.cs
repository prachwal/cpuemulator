using CpuEmulator.Abstractions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja NOP (No Operation) - nie wykonuje żadnej operacji.
/// </summary>
public class NopInstruction : IInstruction
{
    /// <inheritdoc />
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        return state;
    }
}
