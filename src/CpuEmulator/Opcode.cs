namespace CpuEmulator;

public enum Opcode
{
    Nop,
    LoadImmediate,
    Mov,
    Load,
    Store,
    Add,
    Sub,
    Inc,
    Dec,
    Cmp,
    Jump,
    JumpIfZero,
    JumpIfNotZero,
    Push,
    Pop,
    Call,
    Ret,
    Halt
}
