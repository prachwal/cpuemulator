using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Execution;
using CpuEmulator.Model;
using CpuEmulator.Runtime;

namespace CpuEmulator;

/// <summary>
/// Emulator CPU - główna klasa zarządzająca wykonaniem programu.
/// </summary>
public class Cpu : ICpu
{
    private readonly ProgramManager _programManager;
    private readonly CpuExecutor _executor;
    private readonly InstructionSet _instructionSet;
    private CpuState _state;

    /// <summary>
    /// Inicjalizuje nową instancję emulatora CPU.
    /// </summary>
    public Cpu()
    {
        var memory = new Runtime.Memory();
        var registers = new Runtime.RegisterSet();
        var stack = new Stack<int>();
        
        _programManager = new ProgramManager();
        _instructionSet = new InstructionSet();
        _executor = new CpuExecutor(_programManager, _instructionSet);
        
            _state = new Model.CpuState(
                registers,
                memory,
                stack,
            ProgramCounter: 0,
            Flags: new CpuFlags(),
            IsHalted: false);
    }

    /// <summary>
    /// Inicjalizuje nową instancję emulatora CPU z podanymi komponentami.
    /// </summary>
    /// <param name="programManager">Zarządca programu.</param>
    /// <param name="executor">Wykonawca CPU.</param>
    /// <param name="instructionSet">Zestaw instrukcji.</param>
    /// <param name="initialState">Początkowy stan CPU.</param>
    public Cpu(
        ProgramManager programManager,
        CpuExecutor executor,
        InstructionSet instructionSet,
        CpuState initialState)
    {
        _programManager = programManager;
        _executor = executor;
        _instructionSet = instructionSet;
        _state = initialState;
    }

    /// <inheritdoc />
    public void LoadProgram(IEnumerable<Instruction> instructions)
    {
        _programManager.LoadProgram(instructions);
        _state = _state with
        {
            ProgramCounter = 0,
            IsHalted = false,
            Flags = new Model.CpuFlags()
        };
    }

    /// <inheritdoc />
    public void Run()
    {
        _state = _executor.Run(_state);
    }

    /// <inheritdoc />
    public void Step()
    {
        try
        {
            _state = _executor.ExecuteCycle(_state);
            // Synchronizuj ProgramCounter z ProgramManager
            _programManager.SetProgramCounter(_state.ProgramCounter);
        }
        catch (Exceptions.CpuException ex)
        {
            throw new Exceptions.InvalidOperandException(
                $"Error executing instruction at PC={_state.ProgramCounter}: {ex.Message}",
                ex)
            {
                ProgramCounter = _state.ProgramCounter
            };
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        _programManager.LoadProgram(Array.Empty<Instruction>());
        _state = new Model.CpuState(
            new Runtime.RegisterSet(),
            new Runtime.Memory(),
            new Stack<int>(),
            ProgramCounter: 0,
            Flags: new CpuFlags(),
            IsHalted: false);
    }

    /// <inheritdoc />
    public CpuState GetState() => _state;

    /// <summary>
    /// Zwraca zestaw rejestrów (dla wstecznej kompatybilności).
    /// </summary>
    [Obsolete("Use GetState().Registers instead.")]
    public IRegisterSet Registers => _state.Registers;

    /// <summary>
    /// Zwraca pamięć (dla wstecznej kompatybilności).
    /// </summary>
    [Obsolete("Use GetState().Memory instead.")]
    public IMemory Memory => _state.Memory;

    /// <summary>
    /// Zwraca licznik programu (dla wstecznej kompatybilności).
    /// </summary>
    [Obsolete("Use GetState().ProgramCounter instead.")]
    public int ProgramCounter => _state.ProgramCounter;

    /// <summary>
    /// Zwraca flagę ZeroFlag (dla wstecznej kompatybilności).
    /// </summary>
    [Obsolete("Use GetState().Flags.ZeroFlag instead.")]
    public bool ZeroFlag => _state.Flags.ZeroFlag;

    /// <summary>
    /// Zwraca informację, czy program został zatrzymany (dla wstecznej kompatybilności).
    /// </summary>
    [Obsolete("Use GetState().IsHalted instead.")]
    public bool Halted => _state.IsHalted;

    /// <summary>
    /// Zwraca program (dla wstecznej kompatybilności).
    /// </summary>
    [Obsolete("Use ProgramManager.Program instead.")]
    public List<Instruction> Program => _programManager.Program.ToList();
}
