using CpuEmulator.Abstractions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja JumpIfNotZero - wykonuje skok, jeśli flaga ZeroFlag nie jest ustawiona.
/// </summary>
public class JumpIfNotZeroInstruction : IInstruction
{
    /// <inheritdoc />
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        if (!state.Flags.ZeroFlag)
        {
            return state.WithProgramCounter(instruction.Operand1);
        }
        return state;
    }
}
