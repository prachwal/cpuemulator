using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Dec - dekrementuje wartość rejestru.
/// </summary>
public class DecInstruction : IInstruction
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
        int result = value - 1;

        var newState = state.WithRegister(instruction.Operand1, result);
        var newFlags = newState.Flags
            .WithZeroFlag(result == 0)
            .WithSignFlag(result < 0)
            .WithOverflowFlag(value == int.MinValue);

        return newState.WithFlags(newFlags);
    }
}
