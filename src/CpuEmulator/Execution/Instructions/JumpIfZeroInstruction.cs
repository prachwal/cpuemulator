using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja JumpIfZero - wykonuje skok, jeśli flaga ZeroFlag jest ustawiona.
/// </summary>
public class JumpIfZeroInstruction : IInstruction
{
    /// <inheritdoc />
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        if (state.Flags.ZeroFlag)
        {
            // Walidacja jest wykonywana w ProgramManager.Jump
            return state.WithProgramCounter(instruction.Operand1);
        }
        return state;
    }
}
