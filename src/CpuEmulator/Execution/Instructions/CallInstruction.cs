using CpuEmulator.Abstractions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Call - wywołuje podprogram, wkładając adres powrotu na stos.
/// </summary>
public class CallInstruction : IInstruction
{
    /// <inheritdoc />
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        // Adres skoku jest sprawdzany w ProgramManager.Jump
        // Tutaj wkładamy aktualny PC na stos
        return state.WithPushedStack(state.ProgramCounter).WithProgramCounter(instruction.Operand1);
    }
}
