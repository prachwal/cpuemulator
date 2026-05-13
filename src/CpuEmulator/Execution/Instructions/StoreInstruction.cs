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
    /// <summary>
    /// Wykonuje instrukcję Store - zapisuje wartość z rejestru do pamięci.
    /// Obsługuje tryby adresowania: Immediate, Direct, Indirect, Relative.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja do wykonania.</param>
    /// <returns>Nowy stan CPU z zaktualizowaną pamięcią.</returns>
    /// <exception cref="InvalidOperandException">Rzucane, gdy indeks rejestru lub adres pamięci jest nieprawidłowy.</exception>
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        if (instruction.Operand1 < 0 || instruction.Operand1 >= state.Registers.Count)
        {
            throw new InvalidOperandException(
                $"Invalid register index: {instruction.Operand1}. Valid range: [0, {state.Registers.Count}).");
        }

        int address = GetEffectiveAddress(state, instruction, instruction.Operand2);
        int value = state.Registers.GetRegister(instruction.Operand1);
        return state.WithMemory(address, value);
    }

    /// <summary>
    /// Oblicza efektywny adres na podstawie trybu adresowania.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja zawierająca tryb adresowania.</param>
    /// <param name="operand">Operand z instrukcji.</param>
    /// <returns>Efektywny adres pamięci.</returns>
    private int GetEffectiveAddress(CpuState state, Instruction instruction, int operand)
    {
        return instruction.Mode switch
        {
            AddressingMode.Immediate or AddressingMode.Direct => operand,
            AddressingMode.Indirect => state.Memory.Read(operand),
            AddressingMode.Relative => state.ProgramCounter + operand,
            _ => throw new InvalidOperandException($"Unknown addressing mode: {instruction.Mode}")
        };
    }
}
