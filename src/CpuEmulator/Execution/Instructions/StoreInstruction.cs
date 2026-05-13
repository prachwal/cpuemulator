using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Store - zapisuje wartość z rejestru do pamięci.
/// </summary>
public class StoreInstruction : IInstruction
{
    /// <inheritdoc />
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        if (instruction.Operand1 < 0 || instruction.Operand1 >= state.Registers.Count)
        {
            throw new InvalidOperandException(
                $"Invalid register index: {instruction.Operand1}. Valid range: [0, {state.Registers.Count}).");
        }

        if (instruction.Operand2 < 0 || instruction.Operand2 >= state.Memory.Size)
        {
            throw new InvalidOperandException(
                $"Invalid memory address: {instruction.Operand2}. Valid range: [0, {state.Memory.Size}).");
        }

        int value = state.Registers.GetRegister(instruction.Operand1);
        return state.WithMemory(instruction.Operand2, value);
    }
}
