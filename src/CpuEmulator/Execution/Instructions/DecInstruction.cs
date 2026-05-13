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
    /// <summary>
    /// Wykonuje instrukcję Dec - dekrementuje wartość rejestru i ustawia flagi.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja do wykonania.</param>
    /// <returns>Nowy stan CPU z zaktualizowanym rejestrem i flagami.</returns>
    /// <exception cref="InvalidOperandException">Rzucane, gdy indeks rejestru jest nieprawidłowy.</exception>
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
            .WithOverflowFlag(value == int.MinValue)
            .WithCarryFlag(value == 0);

        return newState.WithFlags(newFlags);
    }
}
