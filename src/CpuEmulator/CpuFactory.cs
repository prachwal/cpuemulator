using CpuEmulator.Abstractions;
using CpuEmulator.Execution;
using CpuEmulator.Model;
using CpuEmulator.Runtime;

namespace CpuEmulator;

/// <summary>
/// Fabryka do tworzenia instancji CPU z domyślną konfiguracją.
/// </summary>
public static class CpuFactory
{
    /// <summary>
    /// Tworzy nową instancję CPU z domyślną konfiguracją.
    /// </summary>
    /// <returns>Nowa instancja <see cref="ICpu"/>.</returns>
    public static ICpu Create()
    {
        return new Cpu();
    }

    /// <summary>
    /// Tworzy nową instancję CPU z podanym rozmiarem pamięci.
    /// </summary>
    /// <param name="memorySize">Rozmiar pamięci w słowach.</param>
    /// <returns>Nowa instancja <see cref="ICpu"/> z pamięcią o podanym rozmiarze.</returns>
    public static ICpu CreateWithMemorySize(int memorySize)
    {
        var memory = new Runtime.Memory(memorySize);
        var registers = new Runtime.RegisterSet();
        var stack = new Stack<int>();
        
        var programManager = new ProgramManager();
        var instructionSet = new InstructionSet();
        var executor = new CpuExecutor(programManager, instructionSet);
        
        var initialState = new Model.CpuState(
            registers,
            memory,
            stack,
            ProgramCounter: 0,
            Flags: new CpuFlags(),
            IsHalted: false);
        
        return new Cpu(programManager, executor, instructionSet, initialState);
    }

    /// <summary>
    /// Tworzy nową instancję CPU z podaną liczbą rejestrów.
    /// </summary>
    /// <param name="registerCount">Liczba rejestrów.</param>
    /// <returns>Nowa instancja <see cref="ICpu"/> z podaną liczbą rejestrów.</returns>
    public static ICpu CreateWithRegisterCount(int registerCount)
    {
        var memory = new Runtime.Memory();
        var registers = new Runtime.RegisterSet(registerCount);
        var stack = new Stack<int>();
        
        var programManager = new ProgramManager();
        var instructionSet = new InstructionSet();
        var executor = new CpuExecutor(programManager, instructionSet);
        
        var initialState = new Model.CpuState(
            registers,
            memory,
            stack,
            ProgramCounter: 0,
            Flags: new CpuFlags(),
            IsHalted: false);
        
        return new Cpu(programManager, executor, instructionSet, initialState);
    }
}
