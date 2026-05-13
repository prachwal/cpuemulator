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
        // Wkładamy następny PC (current + 1) na stos i ustawiamy nowy PC
        // Uwaga: ProgramManager zaawansuje PC o 1 po wykonaniu instrukcji,
        // więc tutaj wkładamy state.ProgramCounter + 1
        return state.WithPushedStack(state.ProgramCounter + 1).WithProgramCounter(instruction.Operand1);
    }
}
