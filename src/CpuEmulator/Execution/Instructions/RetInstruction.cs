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
