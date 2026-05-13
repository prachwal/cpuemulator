using CpuEmulator;
using CpuEmulator.Model;

namespace CpuEmulator.Assembler;

/// <summary>
/// Reprezentuje sparsowaną instrukcję przed rozwiązaniem etykiet.
/// </summary>
/// <param name="Opcode">Kod operacji.</param>
/// <param name="Operand1Text">Tekst pierwszego operandu (przed rozwiązaniem).</param>
/// <param name="Operand2Text">Tekst drugiego operandu (przed rozwiązaniem).</param>
/// <param name="Mode">Tryb adresowania.</param>
/// <param name="Line">Numer linii w kodzie źródłowym.</param>
public record ParsedInstruction(
    Opcode Opcode,
    string? Operand1Text,
    string? Operand2Text,
    AddressingMode Mode,
    int Line);
