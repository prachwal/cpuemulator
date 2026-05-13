using CpuEmulator;

namespace CpuEmulator.Assembler;

/// <summary>
/// Mapper mnemoników asemblera na kody operacji CPU.
/// </summary>
public static class MnemonicMapper
{
    private static readonly Dictionary<string, Opcode> _mnemonicToOpcode = new(StringComparer.OrdinalIgnoreCase)
    {
        ["NOP"] = Opcode.Nop,
        ["LDI"] = Opcode.LoadImmediate,
        ["MOV"] = Opcode.Mov,
        ["LD"] = Opcode.Load,
        ["ST"] = Opcode.Store,
        ["ADD"] = Opcode.Add,
        ["SUB"] = Opcode.Sub,
        ["INC"] = Opcode.Inc,
        ["DEC"] = Opcode.Dec,
        ["CMP"] = Opcode.Cmp,
        ["JMP"] = Opcode.Jump,
        ["JZ"] = Opcode.JumpIfZero,
        ["JNZ"] = Opcode.JumpIfNotZero,
        ["PUSH"] = Opcode.Push,
        ["POP"] = Opcode.Pop,
        ["CALL"] = Opcode.Call,
        ["RET"] = Opcode.Ret,
        ["HALT"] = Opcode.Halt
    };

    /// <summary>
    /// Zwraca słownik mapujący mnemoniki na opcody.
    /// </summary>
    public static IReadOnlyDictionary<string, Opcode> MnemonicToOpcode => _mnemonicToOpcode;

    /// <summary>
    /// Próbuje zamapować mnemonik na kod operacji.
    /// </summary>
    /// <param name="mnemonic">Mnemonik do zamapowania.</param>
    /// <param name="opcode">Zmienna wyjściowa dla kodu operacji.</param>
    /// <returns>Prawda, jeśli mnemonik został znaleziony.</returns>
    public static bool TryMap(string mnemonic, out Opcode opcode)
    {
        return _mnemonicToOpcode.TryGetValue(mnemonic, out opcode);
    }

    /// <summary>
    /// Mapuje mnemonik na kod operacji.
    /// </summary>
    /// <param name="mnemonic">Mnemonik do zamapowania.</param>
    /// <returns>Kod operacji.</returns>
    /// <exception cref="KeyNotFoundException">Rzucane, gdy mnemonik nie został znaleziony.</exception>
    public static Opcode Map(string mnemonic)
    {
        if (TryMap(mnemonic, out var opcode))
        {
            return opcode;
        }
        throw new KeyNotFoundException($"Unknown mnemonic: {mnemonic}");
    }
}
