using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Load - ładuje wartość z pamięci do rejestru.
/// </summary>
public class LoadInstruction : IInstruction
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

        int value = state.Memory.Read(instruction.Operand2);
        return state.WithRegister(instruction.Operand1, value);
    }
}
