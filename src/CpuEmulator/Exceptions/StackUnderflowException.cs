namespace CpuEmulator.Exceptions;

/// <summary>
/// Wyjątek rzucany, gdy próba ściągnięcia ze stosu na pustym stosie.
/// </summary>
public class StackUnderflowException : CpuException
{
    /// <summary>
    /// Inicjalizuje nowy wyjątek StackUnderflowException.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    public StackUnderflowException(string message) : base(message) { }

    /// <summary>
    /// Inicjalizuje nowy wyjątek StackUnderflowException z inner exception.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="innerException">Wyjątek wewnętrzny.</param>
    public StackUnderflowException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Inicjalizuje nowy wyjątek StackUnderflowException z kontekstem licznika programu.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="innerException">Wyjątek wewnętrzny.</param>
    /// <param name="programCounter">Wartość licznika programu, przy którym wystąpił błąd.</param>
    public StackUnderflowException(string message, Exception innerException, int? programCounter) : base(message, innerException)
    {
        ProgramCounter = programCounter;
    }
}
