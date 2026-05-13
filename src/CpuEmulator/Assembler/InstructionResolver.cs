using CpuEmulator;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Assembler;

/// <summary>
/// Rozwiązuje sparsowane instrukcje do finalnych instrukcji CPU.
/// </summary>
public class InstructionResolver
{
    private readonly LabelTable _labelTable;

    /// <summary>
    /// Inicjalizuje nowy resolver instrukcji.
    /// </summary>
    /// <param name="labelTable">Tabela etykiet.</param>
    public InstructionResolver(LabelTable labelTable)
    {
        _labelTable = labelTable;
    }

    /// <summary>
    /// Rozwiązuje listę sparsowanych instrukcji do listy instrukcji CPU.
    /// </summary>
    /// <param name="parsedInstructions">Lista sparsowanych instrukcji.</param>
    /// <returns>Lista instrukcji CPU.</returns>
    /// <exception cref="AssemblerException">Rzucane, gdy etykieta nie została rozwiązana.</exception>
    public List<Instruction> Resolve(List<ParsedInstruction> parsedInstructions)
    {
        var instructions = new List<Instruction>();

        for (int i = 0; i < parsedInstructions.Count; i++)
        {
            var parsed = parsedInstructions[i];
            instructions.Add(ResolveInstruction(parsed, i));
        }

        return instructions;
    }

    private Instruction ResolveInstruction(ParsedInstruction parsed, int currentIndex)
    {
        int operand1 = ResolveOperand(parsed.Operand1Text, currentIndex);
        int operand2 = ResolveOperand(parsed.Operand2Text, currentIndex);

        return new Instruction(parsed.Opcode, operand1, operand2, parsed.Mode);
    }

    private int ResolveOperand(string? operandText, int currentIndex)
    {
        if (string.IsNullOrEmpty(operandText))
        {
            return 0;
        }

        // Rejestr (R0, R1, R2, R3)
        if (operandText.StartsWith("R", StringComparison.OrdinalIgnoreCase) && 
            operandText.Length == 2 && 
            char.IsDigit(operandText[1]))
        {
            int registerIndex = int.Parse(operandText[1].ToString());
            if (registerIndex < 0 || registerIndex > 3)
            {
                throw new AssemblerException($"Invalid register: {operandText}", 0, 0);
            }
            return registerIndex;
        }

        // Liczba
        if (int.TryParse(operandText, out int number))
        {
            return number;
        }

        // Etykieta
        if (_labelTable.TryResolve(operandText, out int address))
        {
            return address;
        }

        throw new AssemblerException($"Unresolved label or invalid operand: '{operandText}'", 0, 0);
    }
}
