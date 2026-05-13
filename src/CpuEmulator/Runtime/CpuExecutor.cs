using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Runtime;

/// <summary>
/// Wykonuje cykl rozkazowy CPU.
/// </summary>
public class CpuExecutor
{
    private readonly ProgramManager _programManager;
    private readonly InstructionSet _instructionSet;

    /// <summary>
    /// Inicjalizuje nowy wykonawcę CPU.
    /// </summary>
    /// <param name="programManager">Zarządca programu.</param>
    /// <param name="instructionSet">Zestaw instrukcji.</param>
    public CpuExecutor(ProgramManager programManager, InstructionSet instructionSet)
    {
        _programManager = programManager;
        _instructionSet = instructionSet;
    }

    /// <summary>
    /// Wykonuje pojedynczy cykl rozkazowy.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <returns>Nowy stan CPU po wykonaniu instrukcji.</returns>
    /// <exception cref="CpuException">Rzucane, gdy wystąpi błąd podczas wykonania instrukcji.</exception>
    public CpuState ExecuteCycle(CpuState state)
    {
        if (_programManager.IsHalted || _programManager.ProgramCounter >= _programManager.Program.Count)
        {
            return state.WithHalted(true);
        }

        var instruction = _programManager.Fetch();
        if (instruction == null)
        {
            return state.WithHalted(true);
        }

        try
        {
            var newState = _instructionSet.Resolve(instruction.Opcode).Execute(state, instruction);
            _programManager.Advance();

            // Synchronizuj flagi z ProgramManager
            _programManager.SetFlags(newState.Flags);

            return newState;
        }
        catch (CpuException ex)
        {
            throw new CpuException(
                $"Error executing {instruction.Opcode} at PC={_programManager.ProgramCounter}: {ex.Message}",
                ex)
            {
                ProgramCounter = _programManager.ProgramCounter
            };
        }
    }

    /// <summary>
    /// Wykonuje program do zakończenia.
    /// </summary>
    /// <param name="initialState">Początkowy stan CPU.</param>
    /// <returns>Końcowy stan CPU.</returns>
    public CpuState Run(CpuState initialState)
    {
        var state = initialState;
        while (!state.IsHalted && _programManager.ProgramCounter < _programManager.Program.Count)
        {
            state = ExecuteCycle(state);
        }
        return state;
    }
}
