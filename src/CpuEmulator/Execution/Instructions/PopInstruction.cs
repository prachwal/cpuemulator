using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Pop - ściąga wartość ze stosu do rejestru.
/// </summary>
public class PopInstruction : IInstruction
{
    /// <inheritdoc />
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        if (instruction.Operand1 < 0 || instruction.Operand1 >= state.Registers.Count)
        {
            throw new InvalidOperandException(
                $"Invalid register index: {instruction.Operand1}. Valid range: [0, {state.Registers.Count}).");
        }

        if (state.Stack.Count == 0)
        {
            throw new StackUnderflowException("Attempted to pop from an empty stack.");
        }

        var (newState, value) = state.WithPoppedStack();
        return newState.WithRegister(instruction.Operand1, value);
    }
}
