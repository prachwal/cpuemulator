using CpuEmulator.Model;

namespace CpuEmulator.Abstractions;

/// <summary>
/// Interfejs dla strategii wykonania pojedynczej instrukcji.
/// </summary>
public interface IInstruction
{
    /// <summary>
    /// Wykonuje instrukcję na podanym stanie CPU.
    /// </summary>
    /// <param name="state">Aktualny stan CPU.</param>
    /// <param name="instruction">Instrukcja do wykonania.</param>
    /// <returns>Nowy stan CPU po wykonaniu instrukcji.</returns>
    CpuState Execute(CpuState state, Instruction instruction);
}
