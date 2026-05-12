namespace CpuEmulator;

public record Instruction(
    Opcode Opcode,
    int Operand1 = 0,
    int Operand2 = 0);
