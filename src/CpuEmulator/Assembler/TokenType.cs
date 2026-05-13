namespace CpuEmulator.Assembler;

/// <summary>
/// Typ tokena używanego podczas tokenizacji kodu asemblera.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Identyfikator (np. etykieta, mnemonik).
    /// </summary>
    Identifier,

    /// <summary>
    /// Liczba całkowita (znakowa lub bez znaku).
    /// </summary>
    Number,

    /// <summary>
    /// Rejestr (R0, R1, R2, R3).
    /// </summary>
    Register,

    /// <summary>
    /// Przecinek.
    /// </summary>
    Comma,

    /// <summary>
    /// Dwukropek (używany w etykietach).
    /// </summary>
    Colon,

    /// <summary>
    /// Lewy nawias kwadratowy.
    /// </summary>
    BracketOpen,

    /// <summary>
    /// Prawy nawias kwadratowy.
    /// </summary>
    BracketClose,

    /// <summary>
    /// Plus (używany w adresowaniu względnym).
    /// </summary>
    Plus,

    /// <summary>
    /// Średnik (używany w komentarzach).
    /// </summary>
    Semicolon,

    /// <summary>
    /// Nowa linia.
    /// </summary>
    NewLine,

    /// <summary>
    /// Koniec pliku.
    /// </summary>
    EndOfFile
}
