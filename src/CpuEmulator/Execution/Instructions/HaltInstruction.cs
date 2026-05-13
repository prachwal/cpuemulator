using CpuEmulator.Abstractions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Halt - zatrzymuje wykonanie programu.
/// </summary>
public class HaltInstruction : IInstruction
{
    /// <inheritdoc />
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        return state.WithHalted(true);
    }
}
