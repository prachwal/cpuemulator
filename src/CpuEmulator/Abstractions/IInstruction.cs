using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Abstractions;

/// <summary>
/// Interfejs dla strategii wykonania pojedynczej instrukcji CPU.
/// Każda instrukcja jest implementowana jako oddzielna klasa implementująca ten interfejs.
/// </summary>
public interface IInstruction
{
    /// <summary>
    /// Wykonuje instrukcję na podanym stanie CPU.
    /// </summary>
    /// <param name="state">Aktualny niemutowalny stan CPU.</param>
    /// <param name="instruction">Instrukcja do wykonania.</param>
    /// <returns>Nowy stan CPU po wykonaniu instrukcji.</returns>
    /// <exception cref="CpuException">Rzucane, gdy wystąpi błąd podczas wykonania instrukcji.</exception>
    CpuState Execute(CpuState state, Instruction instruction);
}
