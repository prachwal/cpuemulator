using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
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
        // Wkładamy aktualny PC na stos i ustawiamy nowy PC
        return state.WithPushedStack(state.ProgramCounter).WithProgramCounter(instruction.Operand1);
    }
}
