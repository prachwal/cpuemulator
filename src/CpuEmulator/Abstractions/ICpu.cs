using CpuEmulator.Model;

namespace CpuEmulator.Abstractions;

/// <summary>
/// Interfejs dla emulatora CPU.
/// </summary>
public interface ICpu
{
    /// <summary>
    /// Ładuje program do pamięci CPU i resetuje stan wykonania.
    /// </summary>
    /// <param name="instructions">Kolekcja instrukcji do załadowania.</param>
    void LoadProgram(IEnumerable<Instruction> instructions);

    /// <summary>
    /// Wykonuje program do zakończenia (Halt lub koniec programu).
    /// </summary>
    void Run();

    /// <summary>
    /// Wykonuje pojedynczy krok programu (jedną instrukcję).
    /// </summary>
    void Step();

    /// <summary>
    /// Resetuje stan CPU do stanu początkowego.
    /// </summary>
    void Reset();

    /// <summary>
    /// Zwraca aktualny stan CPU.
    /// </summary>
    /// <returns>Obiekt <see cref="CpuState"/> reprezentujący stan CPU.</returns>
    CpuState GetState();
}
