using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Execution.Instructions;
using CpuEmulator.Model;

namespace CpuEmulator.Execution;

/// <summary>
/// Zestaw instrukcji CPU, implementujący wzorzec Strategy.
/// </summary>
public class InstructionSet
{
    private readonly Dictionary<Opcode, IInstruction> _handlers = new();

    /// <summary>
    /// Inicjalizuje nowy zestaw instrukcji z domyślnymi handlerami.
    /// </summary>
    public InstructionSet()
    {
        RegisterDefaultHandlers();
    }

    /// <summary>
    /// Rejestruje handler dla podanego opcodu.
    /// </summary>
    /// <param name="opcode">Kod operacji.</param>
    /// <param name="handler">Handler instrukcji.</param>
    public void Register(Opcode opcode, IInstruction handler)
    {
        _handlers[opcode] = handler;
    }

    /// <summary>
    /// Pobiera handler dla podanego opcodu.
    /// </summary>
    /// <param name="opcode">Kod operacji.</param>
    /// <returns>Handler instrukcji.</returns>
    /// <exception cref="InvalidOperationException">Rzucane, gdy handler nie jest zarejestrowany dla podanego opcodu.</exception>
    public IInstruction Resolve(Opcode opcode)
    {
        if (_handlers.TryGetValue(opcode, out var handler))
        {
            return handler;
        }
        throw new InvalidOperationException($"No handler registered for opcode: {opcode}");
    }

    private void RegisterDefaultHandlers()
    {
        Register(Opcode.Nop, new NopInstruction());
        Register(Opcode.LoadImmediate, new LoadImmediateInstruction());
        Register(Opcode.Mov, new MovInstruction());
        Register(Opcode.Load, new LoadInstruction());
        Register(Opcode.Store, new StoreInstruction());
        Register(Opcode.Add, new AddInstruction());
        Register(Opcode.Sub, new SubInstruction());
        Register(Opcode.Inc, new IncInstruction());
        Register(Opcode.Dec, new DecInstruction());
        Register(Opcode.Cmp, new CmpInstruction());
        Register(Opcode.Jump, new JumpInstruction());
        Register(Opcode.JumpIfZero, new JumpIfZeroInstruction());
        Register(Opcode.JumpIfNotZero, new JumpIfNotZeroInstruction());
        Register(Opcode.Push, new PushInstruction());
        Register(Opcode.Pop, new PopInstruction());
        Register(Opcode.Call, new CallInstruction());
        Register(Opcode.Ret, new RetInstruction());
        Register(Opcode.Halt, new HaltInstruction());
    }
}
