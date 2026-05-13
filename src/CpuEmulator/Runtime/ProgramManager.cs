using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Runtime;

/// <summary>
/// Zarządza programem i licznikiem programu.
/// </summary>
public class ProgramManager : IRuntime
{
    private readonly List<Instruction> _program = new();
    private int _programCounter;
    private bool _isHalted;
    private readonly Stack<int> _stack = new();
    private CpuFlags _flags = new();

    /// <inheritdoc />
    public IReadOnlyList<Instruction> Program => _program.AsReadOnly();

    /// <inheritdoc />
    public int ProgramCounter => _programCounter;

    /// <inheritdoc />
    public bool IsHalted => _isHalted;

    /// <inheritdoc />
    public CpuFlags Flags => _flags;

    /// <inheritdoc />
    public int StackCount => _stack.Count;

    /// <summary>
    /// Ładuje program do zarządcy.
    /// </summary>
    /// <param name="instructions">Kolekcja instrukcji do załadowania.</param>
    public void LoadProgram(IEnumerable<Instruction> instructions)
    {
        _program.Clear();
        _program.AddRange(instructions);
        _programCounter = 0;
        _isHalted = false;
        _stack.Clear();
        _flags = new CpuFlags();
    }

    /// <inheritdoc />
    public void SetProgramCounter(int value)
    {
        if (value < 0 || value > _program.Count)
        {
            throw new ProgramCounterOutOfRangeException(
                $"ProgramCounter {value} is out of range [0, {_program.Count}).");
        }
        _programCounter = value;
    }

    /// <summary>
    /// Pobiera następną instrukcję z programu.
    /// </summary>
    /// <returns>Instrukcja lub null, jeśli program się zakończył.</returns>
    public Instruction? Fetch()
    {
        if (_programCounter >= 0 && _programCounter < _program.Count)
        {
            return _program[_programCounter];
        }
        return null;
    }

    /// <summary>
    /// Przesuwa licznik programu o jeden.
    /// </summary>
    public void Advance()
    {
        _programCounter++;
    }

    /// <summary>
    /// Wykonuje skok do podanego adresu.
    /// </summary>
    /// <param name="address">Adres skoku.</param>
    public void Jump(int address)
    {
        if (address < 0 || address > _program.Count)
        {
            throw new ProgramCounterOutOfRangeException(
                $"Jump address {address} is out of range [0, {_program.Count}).");
        }
        _programCounter = address;
    }

    /// <inheritdoc />
    public void Halt()
    {
        _isHalted = true;
    }

    /// <inheritdoc />
    public void PushStack(int value)
    {
        _stack.Push(value);
    }

    /// <inheritdoc />
    public int PopStack()
    {
        if (_stack.Count == 0)
        {
            throw new StackUnderflowException("Attempted to pop from an empty stack.");
        }
        return _stack.Pop();
    }

    /// <summary>
    /// Ustawia flagi procesora.
    /// </summary>
    /// <param name="flags">Nowe flagi procesora.</param>
    public void SetFlags(CpuFlags flags)
    {
        _flags = flags;
    }
}
