using CpuEmulator.Model;

namespace CpuEmulator;

/// <summary>
/// Reprezentuje pojedynczą instrukcję CPU.
/// </summary>
/// <param name="Opcode">Kod operacji.</param>
/// <param name="Operand1">Pierwszy operand. Domyślnie 0.</param>
/// <param name="Operand2">Drugi operand. Domyślnie 0.</param>
/// <param name="Mode">Tryb adresowania (domyślnie Immediate).</param>
public record Instruction(
    Opcode Opcode,
    int Operand1 = 0,
    int Operand2 = 0,
    AddressingMode Mode = AddressingMode.Immediate);
