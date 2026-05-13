namespace CpuEmulator.Abstractions;

/// <summary>
/// Interfejs dla pamięci CPU.
/// </summary>
public interface IMemory
{
    /// <summary>
    /// Odczytuje wartość z podanego adresu pamięci.
    /// </summary>
    /// <param name="address">Adres pamięci do odczytu.</param>
    /// <returns>Wartość przechowywana pod podanym adresem.</returns>
    /// <exception cref="InvalidOperandException">Rzucane, gdy adres jest poza zakresem pamięci.</exception>
    int Read(int address);

    /// <summary>
    /// Zapisuje wartość pod podany adres pamięci.
    /// </summary>
    /// <param name="address">Adres pamięci do zapisu.</param>
    /// <param name="value">Wartość do zapisu.</param>
    /// <exception cref="InvalidOperandException">Rzucane, gdy adres jest poza zakresem pamięci.</exception>
    void Write(int address, int value);

    /// <summary>
    /// Zwraca rozmiar pamięci w słowach.
    /// </summary>
    int Size { get; }
}
