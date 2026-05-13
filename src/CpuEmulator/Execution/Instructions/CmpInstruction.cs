using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Cmp - porównuje wartości dwóch rejestrów i ustawia flagi.
/// </summary>
public class CmpInstruction : IInstruction
{
    /// <inheritdoc />
    /// <summary>
    /// Wykonuje instrukcję Cmp - porównuje wartości dwóch rejestrów i ustawia flagi.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja do wykonania.</param>
    /// <returns>Nowy stan CPU z zaktualizowanymi flagami.</returns>
    /// <exception cref="InvalidOperandException">Rzucane, gdy indeks pierwszego lub drugiego rejestru jest nieprawidłowy.</exception>
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        if (instruction.Operand1 < 0 || instruction.Operand1 >= state.Registers.Count)
        {
            throw new InvalidOperandException(
                $"Invalid first register index: {instruction.Operand1}. Valid range: [0, {state.Registers.Count}).");
        }

        if (instruction.Operand2 < 0 || instruction.Operand2 >= state.Registers.Count)
        {
            throw new InvalidOperandException(
                $"Invalid second register index: {instruction.Operand2}. Valid range: [0, {state.Registers.Count}).");
        }

        int value1 = state.Registers.GetRegister(instruction.Operand1);
        int value2 = state.Registers.GetRegister(instruction.Operand2);
        int result = value1 - value2;

        var newFlags = state.Flags
            .WithZeroFlag(result == 0)
            .WithSignFlag(result < 0)
            .WithCarryFlag((uint)value1 < (uint)value2);

        return state.WithFlags(newFlags);
    }
}
