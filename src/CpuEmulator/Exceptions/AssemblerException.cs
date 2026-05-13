namespace CpuEmulator.Exceptions;

/// <summary>
/// Wyjątek rzucany podczas asemblacji kodu źródłowego.
/// </summary>
public class AssemblerException : Exception
{
    /// <summary>
    /// Inicjalizuje nowy wyjątek AssemblerException.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="line">Numer linii, na której wystąpił błąd.</param>
    /// <param name="column">Numer kolumny, na której wystąpił błąd.</param>
    public AssemblerException(string message, int line, int column) : base(message)
    {
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Inicjalizuje nowy wyjątek AssemblerException z inner exception.
    /// </summary>
    /// <param name="message">Komunikat o błędzie.</param>
    /// <param name="line">Numer linii, na której wystąpił błąd.</param>
    /// <param name="column">Numer kolumny, na której wystąpił błąd.</param>
    /// <param name="innerException">Wyjątek wewnętrzny.</param>
    public AssemblerException(string message, int line, int column, Exception innerException) : base(message, innerException)
    {
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Zwraca numer linii, na której wystąpił błąd.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Zwraca numer kolumny, na której wystąpił błąd.
    /// </summary>
    public int Column { get; }
}
