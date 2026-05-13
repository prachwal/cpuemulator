using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;
using CpuEmulator.Runtime;

namespace CpuEmulator.Model;

/// <summary>
/// Reprezentuje niemutowalny stan CPU.
/// </summary>
/// <param name="Registers">Zestaw rejestrów.</param>
/// <param name="Memory">Pamięć.</param>
/// <param name="Stack">Stos (kopia dla niemutowalności).</param>
/// <param name="ProgramCounter">Licznik programu.</param>
/// <param name="Flags">Flagi procesora.</param>
/// <param name="IsHalted">Czy program został zatrzymany.</param>
public readonly record struct CpuState(
    IRegisterSet Registers,
    IMemory Memory,
    Stack<int> Stack,
    int ProgramCounter = 0,
    CpuFlags Flags = new(),
    bool IsHalted = false)
{
    /// <summary>
    /// Tworzy nowy stan z zadaną wartością rejestru.
    /// </summary>
    /// <param name="index">Indeks rejestru.</param>
    /// <param name="value">Nowa wartość rejestru.</param>
    /// <returns>Nowy stan CPU.</returns>
    public CpuState WithRegister(int index, int value)
    {
        var newRegisters = new RegisterSet(Registers);
        newRegisters.SetRegister(index, value);
        return this with { Registers = newRegisters };
    }

    /// <summary>
    /// Tworzy nowy stan z zadaną wartością w pamięci.
    /// </summary>
    /// <param name="address">Adres pamięci.</param>
    /// <param name="value">Nowa wartość w pamięci.</param>
    /// <returns>Nowy stan CPU.</returns>
    public CpuState WithMemory(int address, int value)
    {
        var newMemory = new Runtime.Memory(Memory);
        newMemory.Write(address, value);
        return this with { Memory = newMemory };
    }

    /// <summary>
    /// Tworzy nowy stan z zadanym licznikiem programu.
    /// </summary>
    /// <param name="programCounter">Nowa wartość licznika programu.</param>
    /// <returns>Nowy stan CPU.</returns>
    public CpuState WithProgramCounter(int programCounter) => this with { ProgramCounter = programCounter };

    /// <summary>
    /// Tworzy nowy stan z zadanymi flagami.
    /// </summary>
    /// <param name="flags">Nowe flagi procesora.</param>
    /// <returns>Nowy stan CPU.</returns>
    public CpuState WithFlags(CpuFlags flags) => this with { Flags = flags };

    /// <summary>
    /// Tworzy nowy stan z zadanym stanem zatrzymania.
    /// </summary>
    /// <param name="isHalted">Czy program jest zatrzymany.</param>
    /// <returns>Nowy stan CPU.</returns>
    public CpuState WithHalted(bool isHalted) => this with { IsHalted = isHalted };

    /// <summary>
    /// Tworzy nowy stan z wartością włożoną na stos.
    /// </summary>
    /// <param name="value">Wartość do włożenia na stos.</param>
    /// <returns>Nowy stan CPU.</returns>
    public CpuState WithPushedStack(int value)
    {
        var newStack = new Stack<int>(Stack);
        newStack.Push(value);
        return this with { Stack = newStack };
    }

    /// <summary>
    /// Tworzy nowy stan z wartością ściągniętą ze stosu.
    /// </summary>
    /// <returns>Nowy stan CPU i ściągnięta wartość.</returns>
    /// <exception cref="StackUnderflowException">Rzucane, gdy stos jest pusty.</exception>
    public (CpuState NewState, int Value) WithPoppedStack()
    {
        if (Stack.Count == 0)
        {
            throw new StackUnderflowException("Attempted to pop from an empty stack.");
        }

        var newStack = new Stack<int>(Stack);
        var value = newStack.Pop();
        return (this with { Stack = newStack }, value);
    }
}
