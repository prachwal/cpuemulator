namespace CpuEmulator.Model;

/// <summary>
/// Tryby adresowania dla instrukcji CPU.
/// </summary>
public enum AddressingMode
{
    /// <summary>
    /// Adresowanie natychmiastowe - operand zawiera wartość.
    /// </summary>
    Immediate,

    /// <summary>
    /// Adresowanie bezpośrednie - operand zawiera adres pamięci.
    /// </summary>
    Direct,

    /// <summary>
    /// Adresowanie pośrednie - operand zawiera adres, pod którym znajduje się docelowy adres.
    /// </summary>
    Indirect,

    /// <summary>
    /// Adresowanie względne - adres jest obliczany względem licznika programu.
    /// </summary>
    Relative
}
