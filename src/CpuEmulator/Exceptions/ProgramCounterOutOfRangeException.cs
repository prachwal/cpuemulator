namespace CpuEmulator.Exceptions;

/// <summary>
/// Wyjątek rzucany, gdy licznik programu jest poza zakresem.
/// </summary>
public class ProgramCounterOutOfRangeException : CpuException
{
    /// <summary>
    /// Inicjalizuje nowy wyjątek ProgramCounterOutOfRangeException.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    public ProgramCounterOutOfRangeException(string message) : base(message) { }

    /// <summary>
    /// Inicjalizuje nowy wyjątek ProgramCounterOutOfRangeException z inner exception.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="innerException">Wyjątek wewnętrzny.</param>
    public ProgramCounterOutOfRangeException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Inicjalizuje nowy wyjątek ProgramCounterOutOfRangeException z kontekstem licznika programu.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="programCounter">Wartość licznika programu, przy którym wystąpił błąd.</param>
    public ProgramCounterOutOfRangeException(string message, int? programCounter) : base(message)
    {
        ProgramCounter = programCounter;
    }

    /// <summary>
    /// Inicjalizuje nowy wyjątek ProgramCounterOutOfRangeException z inner exception i kontekstem licznika programu.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="innerException">Wyjątek wewnętrzny.</param>
    /// <param name="programCounter">Wartość licznika programu, przy którym wystąpił błąd.</param>
    public ProgramCounterOutOfRangeException(string message, Exception innerException, int? programCounter) : base(message, innerException)
    {
        ProgramCounter = programCounter;
    }
}
