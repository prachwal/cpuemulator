namespace CpuEmulator;

/// <summary>
/// Kod operacji (opcode) dla instrukcji CPU.
/// </summary>
public enum Opcode
{
    /// <summary>
    /// Brak operacji - instrukcja NOP.
    /// </summary>
    Nop,

    /// <summary>
    /// Ładowanie stałej do rejestru.
    /// </summary>
    LoadImmediate,

    /// <summary>
    /// Kopiowanie wartości między rejestrami.
    /// </summary>
    Mov,

    /// <summary>
    /// Ładowanie wartości z pamięci do rejestru.
    /// </summary>
    Load,

    /// <summary>
    /// Zapisywanie wartości z rejestru do pamięci.
    /// </summary>
    Store,

    /// <summary>
    /// Dodawanie dwóch rejestrów.
    /// </summary>
    Add,

    /// <summary>
    /// Odejmowanie dwóch rejestrów.
    /// </summary>
    Sub,

    /// <summary>
    /// Inkrementacja rejestru.
    /// </summary>
    Inc,

    /// <summary>
    /// Dekrementacja rejestru.
    /// </summary>
    Dec,

    /// <summary>
    /// Porównanie dwóch rejestrów.
    /// </summary>
    Cmp,

    /// <summary>
    /// Skok bezwarunkowy.
    /// </summary>
    Jump,

    /// <summary>
    /// Skok warunkowy, jeśli flaga ZeroFlag jest ustawiona.
    /// </summary>
    JumpIfZero,

    /// <summary>
    /// Skok warunkowy, jeśli flaga ZeroFlag nie jest ustawiona.
    /// </summary>
    JumpIfNotZero,

    /// <summary>
    /// Wkładanie wartości na stos.
    /// </summary>
    Push,

    /// <summary>
    /// Ściąganie wartości ze stosu.
    /// </summary>
    Pop,

    /// <summary>
    /// Wywołanie podprogramu.
    /// </summary>
    Call,

    /// <summary>
    /// Powrót z podprogramu.
    /// </summary>
    Ret,

    /// <summary>
    /// Zatrzymanie wykonania programu.
    /// </summary>
    Halt
}
