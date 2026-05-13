using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja JumpIfNotZero - wykonuje skok, jeśli flaga ZeroFlag nie jest ustawiona.
/// </summary>
public class JumpIfNotZeroInstruction : IInstruction
{
    /// <inheritdoc />
    /// <summary>
    /// Wykonuje instrukcję JumpIfNotZero - wykonuje skok, jeśli flaga ZeroFlag nie jest ustawiona.
    /// Obsługuje tryby adresowania: Immediate, Direct, Indirect, Relative.
    /// </summary>
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        if (!state.Flags.ZeroFlag)
        {
            int targetAddress = GetEffectiveAddress(state, instruction, instruction.Operand1);
            return state.WithProgramCounter(targetAddress);
        }
        return state;
    }

    /// <summary>
    /// Oblicza efektywny adres skoku na podstawie trybu adresowania.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja zawierająca tryb adresowania.</param>
    /// <param name="operand">Operand z instrukcji (adres skoku).</param>
    /// <returns>Efektywny adres skoku.</returns>
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
