using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Jump - wykonuje skok do podanego adresu.
/// </summary>
public class JumpInstruction : IInstruction
{
    /// <inheritdoc />
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        // Walidacja jest wykonywana w ProgramManager.Jump
        // Tutaj tylko zwracamy stan z nowym PC
        return state.WithProgramCounter(instruction.Operand1);
    }
}
