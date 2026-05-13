using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Mov - kopiuje wartość z jednego rejestru do drugiego.
/// </summary>
public class MovInstruction : IInstruction
{
    /// <inheritdoc />
    /// <summary>
    /// Wykonuje instrukcję Mov - kopiuje wartość z jednego rejestru do drugiego.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja do wykonania.</param>
    /// <returns>Nowy stan CPU z zaktualizowanym rejestrem docelowym.</returns>
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

        int value = state.Registers.GetRegister(instruction.Operand2);
        return state.WithRegister(instruction.Operand1, value);
    }
}
