using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Push - wkłada wartość rejestru na stos.
/// </summary>
public class PushInstruction : IInstruction
{
    /// <inheritdoc />
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        if (instruction.Operand1 < 0 || instruction.Operand1 >= state.Registers.Count)
        {
            throw new InvalidOperandException(
                $"Invalid register index: {instruction.Operand1}. Valid range: [0, {state.Registers.Count}).");
        }

        int value = state.Registers.GetRegister(instruction.Operand1);
        return state.WithPushedStack(value);
    }
}
