using CpuEmulator.Abstractions;
using CpuEmulator.Exceptions;

namespace CpuEmulator.Runtime;

/// <summary>
/// Implementacja pamięci CPU.
/// </summary>
public class Memory : IMemory
{
    private readonly int[] _data;

    /// <summary>
    /// Inicjalizuje nową instancję pamięci o podanym rozmiarze.
    /// </summary>
    /// <param name="size">Rozmiar pamięci w słowach.</param>
    public Memory(int size = 256)
    {
        _data = new int[size];
    }

    /// <summary>
    /// Inicjalizuje nową instancję pamięci jako kopię istniejącej pamięci.
    /// </summary>
    /// <param name="other">Pamięć do skopiowania.</param>
    public Memory(IMemory other)
    {
        _data = new int[other.Size];
        for (int i = 0; i < other.Size; i++)
        {
            _data[i] = other.Read(i);
        }
    }

    /// <inheritdoc />
    public int Read(int address)
    {
        if (address < 0 || address >= _data.Length)
        {
            throw new InvalidOperandException(
                $"Invalid memory address: {address}. Valid range: [0, {_data.Length}).");
        }
        return _data[address];
    }

    /// <inheritdoc />
    public void Write(int address, int value)
    {
        if (address < 0 || address >= _data.Length)
        {
            throw new InvalidOperandException(
                $"Invalid memory address: {address}. Valid range: [0, {_data.Length}).");
        }
        _data[address] = value;
    }