namespace CpuEmulator.Exceptions;

/// <summary>
/// Wyjątek rzucany, gdy operand instrukcji jest nieprawidłowy.
/// </summary>
public class InvalidOperandException : CpuException
{
    /// <summary>
    /// Inicjalizuje nowy wyjątek InvalidOperandException.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    public InvalidOperandException(string message) : base(message) { }

    /// <summary>
    /// Inicjalizuje nowy wyjątek InvalidOperandException z inner exception.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="innerException">Wyjątek wewnętrzny.</param>
    public InvalidOperandException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Inicjalizuje nowy wyjątek InvalidOperandException z kontekstem licznika programu.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="programCounter">Wartość licznika programu, przy którym wystąpił błąd.</param>
    public InvalidOperandException(string message, int? programCounter) : base(message)
    {
        ProgramCounter = programCounter;
    }

    /// <summary>
    /// Inicjalizuje nowy wyjątek InvalidOperandException z inner exception i kontekstem licznika programu.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="innerException">Wyjątek wewnętrzny.</param>
    /// <param name="programCounter">Wartość licznika programu, przy którym wystąpił błąd.</param>
    public InvalidOperandException(string message, Exception innerException, int? programCounter) : base(message, innerException)
    {
        ProgramCounter = programCounter;
    }
}
