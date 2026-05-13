using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;

namespace CpuEmulator.Runtime;

/// <summary>
/// Implementacja zestawu rejestrów CPU.
/// </summary>
public class RegisterSet : IRegisterSet
{
    private readonly int[] _registers;

    /// <summary>
    /// Inicjalizuje nowy zestaw rejestrów o podanej liczbie rejestrów.
    /// </summary>
    /// <param name="count">Liczba rejestrów.</param>
    public RegisterSet(int count = 4)
    {
        _registers = new int[count];
    }

    /// <summary>
    /// Inicjalizuje nowy zestaw rejestrów jako kopię istniejącego zestawu.
    /// </summary>
    /// <param name="other">Zestaw rejestrów do skopiowania.</param>
    public RegisterSet(IRegisterSet other)
    {
        _registers = new int[other.Count];
        for (int i = 0; i < other.Count; i++)
        {
            _registers[i] = other.GetRegister(i);
        }
    }

    /// <inheritdoc />
    public int GetRegister(int index)
    {
        if (index < 0 || index >= _registers.Length)
        {
            throw new InvalidOperandException(
                $"Invalid register index: {index}. Valid range: [0, {_registers.Length}).");
        }
        return _registers[index];
    }

    /// <inheritdoc />
    public void SetRegister(int index, int value)
    {
        if (index < 0 || index >= _registers.Length)
        {
            throw new InvalidOperandException(
                $"Invalid register index: {index}. Valid range: [0, {_registers.Length}).");
        }
        _registers[index] = value;
    }

    /// <inheritdoc />
    public int Count => _registers.Length;
}
