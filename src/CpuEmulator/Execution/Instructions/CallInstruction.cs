using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Call - wywołuje podprogram, wkładając adres powrotu na stos.
/// </summary>
public class CallInstruction : IInstruction
{
    /// <inheritdoc />
    /// <summary>
    /// Wykonuje instrukcję Call - wywołuje podprogram, wkładając adres powrotu na stos.
    /// Obsługuje tryby adresowania: Immediate, Direct, Indirect, Relative.
    /// </summary>
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        // Wkładamy następny PC (current + 1) na stos i ustawiamy nowy PC
        // Uwaga: ProgramManager zaawansuje PC o 1 po wykonaniu instrukcji,
        // więc tutaj wkładamy state.ProgramCounter + 1
        int targetAddress = GetEffectiveAddress(state, instruction, instruction.Operand1);
        return state.WithPushedStack(state.ProgramCounter + 1).WithProgramCounter(targetAddress);
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
