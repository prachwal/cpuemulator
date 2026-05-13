using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution.Instructions;

/// <summary>
/// Instrukcja Ret - powraca z podprogramu, ściągając adres powrotu ze stosu.
/// </summary>
public class RetInstruction : IInstruction
{
    /// <inheritdoc />
    /// <summary>
    /// Wykonuje instrukcję Ret - powraca z podprogramu, ściągając adres powrotu ze stosu.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja do wykonania.</param>
    /// <returns>Nowy stan CPU z adresem powrotu ze stosu jako nowym licznikiem programu.</returns>
    /// <exception cref="StackUnderflowException">Rzucane, gdy stos jest pusty.</exception>
    public CpuState Execute(CpuState state, Instruction instruction)
    {
        if (state.Stack.Count == 0)
        {
            throw new StackUnderflowException("Attempted to return from an empty stack.");
        }

        var (newState, address) = state.WithPoppedStack();
        return newState.WithProgramCounter(address);
    }
}
