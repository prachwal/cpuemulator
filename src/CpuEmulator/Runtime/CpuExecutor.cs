using CpuEmulator.Abstractions;
using CpuEmulator.Execution;
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
    /// Wykonuje pojedynczy cykl rozkazowy (fetch-decode-execute).
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <returns>Nowy stan CPU po wykonaniu instrukcji.</returns>
    /// <exception cref="CpuException">Rzucane, gdy wystąpi błąd podczas wykonania instrukcji.</exception>
    public CpuState ExecuteCycle(CpuState state)
    {
        // Synchronizuj ProgramManager z stanem na początku cyklu
        _programManager.SetProgramCounter(state.ProgramCounter);
        _programManager.SetFlags(state.Flags);
        
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
            
            // Walidacja ProgramCounter po wykonaniu instrukcji
            if (newState.ProgramCounter < 0 || newState.ProgramCounter > _programManager.Program.Count)
            {
                throw new Exceptions.ProgramCounterOutOfRangeException(
                    $"ProgramCounter {newState.ProgramCounter} is out of range [0, {_programManager.Program.Count}).");
            }
            
            // Synchronizuj ProgramManager z nowym stanem
            _programManager.SetProgramCounter(newState.ProgramCounter);
            _programManager.SetFlags(newState.Flags);
            
            // Jeśli nie było skoku, zaawansuj PC
            if (newState.ProgramCounter == state.ProgramCounter)
            {
                _programManager.Advance();
                newState = newState.WithProgramCounter(_programManager.ProgramCounter);
            }

            return newState;
        }
        catch (Exceptions.CpuException ex)
        {
            // Zachowaj oryginalny typ wyjątku, dodaj kontekst
            if (ex is Exceptions.StackUnderflowException stackEx)
            {
                throw new Exceptions.StackUnderflowException(
                    $"Error executing {instruction.Opcode} at PC={_programManager.ProgramCounter}: {ex.Message}",
                    ex)
                {
                    ProgramCounter = _programManager.ProgramCounter
                };
            }
            else if (ex is Exceptions.ProgramCounterOutOfRangeException pcEx)
            {
                throw new Exceptions.ProgramCounterOutOfRangeException(
                    $"Error executing {instruction.Opcode} at PC={_programManager.ProgramCounter}: {ex.Message}",
                    ex)
                {
                    ProgramCounter = _programManager.ProgramCounter
                };
            }
            else if (ex is Exceptions.InvalidOperandException opEx)
            {
                throw new Exceptions.InvalidOperandException(
                    $"Error executing {instruction.Opcode} at PC={_programManager.ProgramCounter}: {ex.Message}",
                    ex)
                {
                    ProgramCounter = _programManager.ProgramCounter
                };
            }
            else
            {
                throw new Exceptions.InvalidOperandException(
                    $"Error executing {instruction.Opcode} at PC={_programManager.ProgramCounter}: {ex.Message}",
                    ex)
                {
                    ProgramCounter = _programManager.ProgramCounter
                };
            }
        }
    }

    /// <summary>
    /// Wykonuje program do zakończenia (Halt lub koniec programu).
    /// </summary>
    /// <param name="initialState">Początkowy stan CPU.</param>
    /// <returns>Końcowy stan CPU po zakończeniu wykonania.</returns>
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
