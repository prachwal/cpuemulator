namespace CpuEmulator.Model;

/// <summary>
/// Reprezentuje flagi procesora.
/// </summary>
/// <param name="ZeroFlag">Flaga zerowa, ustawiana gdy wynik operacji wynosi 0.</param>
/// <param name="CarryFlag">Flaga przeniesienia, ustawiana przy przepełnieniu bez znaku.</param>
/// <param name="OverflowFlag">Flaga przepełnienia, ustawiana przy przepełnieniu ze znakiem.</param>
/// <param name="SignFlag">Flaga znaku, ustawiana gdy wynik operacji jest ujemny.</param>
public readonly record struct CpuFlags(
    bool ZeroFlag = false,
    bool CarryFlag = false,
    bool OverflowFlag = false,
    bool SignFlag = false)
{
    /// <summary>
    /// Tworzy nowy zestaw flag z zadaną flagą zerową.
    /// </summary>
    /// <param name="zeroFlag">Nowa wartość flagi zerowej.</param>
    /// <returns>Nowy zestaw flag z zaktualizowaną flagą zerową.</returns>
    public CpuFlags WithZeroFlag(bool zeroFlag) => this with { ZeroFlag = zeroFlag };

    /// <summary>
    /// Tworzy nowy zestaw flag z zadaną flagą przeniesienia.
    /// </summary>
    /// <param name="carryFlag">Nowa wartość flagi przeniesienia.</param>
    /// <returns>Nowy zestaw flag z zaktualizowaną flagą przeniesienia.</returns>
    public CpuFlags WithCarryFlag(bool carryFlag) => this with { CarryFlag = carryFlag };

    /// <summary>
    /// Tworzy nowy zestaw flag z zadaną flagą przepełnienia.
    /// </summary>
    /// <param name="overflowFlag">Nowa wartość flagi przepełnienia.</param>
    /// <returns>Nowy zestaw flag z zaktualizowaną flagą przepełnienia.</returns>
    public CpuFlags WithOverflowFlag(bool overflowFlag) => this with { OverflowFlag = overflowFlag };

    /// <summary>
    /// Tworzy nowy zestaw flag z zadaną flagą znaku.
    /// </summary>
    /// <param name="signFlag">Nowa wartość flagi znaku.</param>
    /// <returns>Nowy zestaw flag z zaktualizowaną flagą znaku.</returns>
    public CpuFlags WithSignFlag(bool signFlag) => this with { SignFlag = signFlag };
}
