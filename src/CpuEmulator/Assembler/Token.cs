namespace CpuEmulator.Assembler;

/// <summary>
/// Reprezentuje token wyekstrahowany z kodu asemblera.
/// </summary>
/// <param name="Type">Typ tokena.</param>
/// <param name="Value">Wartość tokena (tekst).</param>
/// <param name="Line">Numer linii (1-based).</param>
/// <param name="Column">Numer kolumny (0-based).</param>
public record Token(
    TokenType Type,
    string Value,
    int Line,
    int Column);
