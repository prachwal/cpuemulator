using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Sub - odejmuje wartość drugiego rejestru od pierwszego.
/// </summary>
public class SubInstruction : IInstruction
{
    /// <inheritdoc />
    /// <summary>
    /// Wykonuje instrukcję Sub - odejmuje wartość drugiego rejestru od pierwszego i ustawia flagi.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja do wykonania.</param>
    /// <returns>Nowy stan CPU z zaktualizowanym rejestrem i flagami.</returns>
    /// <exception cref="InvalidOperandException">Rzucane, gdy indeks rejestru źródłowego lub docelowego jest nieprawidłowy.</exception>
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        if (instruction.Operand1 < 0 || instruction.Operand1 >= state.Registers.Count)
        {
            throw new InvalidOperandException(
                $"Invalid destination register index: {instruction.Operand1}. Valid range: [0, {state.Registers.Count}).");
        }

        if (instruction.Operand2 < 0 || instruction.Operand2 >= state.Registers.Count)
        {
            throw new InvalidOperandException(
                $"Invalid source register index: {instruction.Operand2}. Valid range: [0, {state.Registers.Count}).");
        }

        int value1 = state.Registers.GetRegister(instruction.Operand1);
        int value2 = state.Registers.GetRegister(instruction.Operand2);
        int result = value1 - value2;

        var newState = state.WithRegister(instruction.Operand1, result);
        var newFlags = newState.Flags
            .WithZeroFlag(result == 0)
            .WithSignFlag(result < 0)
            .WithCarryFlag((uint)value1 < (uint)value2)
            .WithOverflowFlag(IsOverflow(value1, value2, result));

        return newState.WithFlags(newFlags);
    }

    private static bool IsOverflow(int a, int b, int result)
    {
        return (a > 0 && b < 0 && result < 0) || (a < 0 && b > 0 && result > 0);
    }
}
