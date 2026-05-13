using CpuEmulator.Exceptions;

namespace CpuEmulator.Abstractions;

/// <summary>
/// Interfejs dla zestawu rejestrów CPU.
/// </summary>
public interface IRegisterSet
{
    /// <summary>
    /// Odczytuje wartość z podanego rejestru.
    /// </summary>
    /// <param name="index">Indeks rejestru (0 do Count-1).</param>
    /// <returns>Wartość przechowywana w rejestrze.</returns>
    /// <exception cref="InvalidOperandException">Rzucane, gdy indeks jest poza zakresem.</exception>
    int GetRegister(int index);

    /// <summary>
    /// Zapisuje wartość do podanego rejestru.
    /// </summary>
    /// <param name="index">Indeks rejestru (0 do Count-1).</param>
    /// <param name="value">Wartość do zapisu.</param>
    /// <exception cref="InvalidOperandException">Rzucane, gdy indeks jest poza zakresem.</exception>
    void SetRegister(int index, int value);

    /// <summary>
    /// Zwraca liczbę rejestrów.
    /// </summary>
    int Count { get; }
}
