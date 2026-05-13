namespace CpuEmulator.Exceptions;

/// <summary>
/// Bazowa klasa wyjątków dla emulatora CPU.
/// </summary>
public abstract class CpuException : Exception
{
    /// <summary>
    /// Inicjalizuje nowy wyjątek CPU.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    protected CpuException(string message) : base(message) { }

    /// <summary>
    /// Inicjalizuje nowy wyjątek CPU z inner exception.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="innerException">Wyjątek wewnętrzny.</param>
    protected CpuException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Zwraca licznik programu, przy którym wystąpił błąd (jeśli dostępny).
    /// </summary>
    public int? ProgramCounter { get; init; }
}
