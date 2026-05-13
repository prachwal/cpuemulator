using CpuEmulator;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;

namespace CpuEmulator.Assembler;

/// <summary>
/// Parser kodu asemblera.
/// </summary>
public class Parser
{
    /// <summary>
    /// Parsuje tokeny w listę instrukcji i tabelę etykiet.
    /// </summary>
    /// <param name="tokens">Lista tokenów do sparsowania.</param>
    /// <returns>Para: lista sparsowanych instrukcji i tabela etykiet.</returns>
    public (List<ParsedInstruction> Instructions, LabelTable Labels) Parse(List<Token> tokens)
    {
        var instructions = new List<ParsedInstruction>();
        var labels = new LabelTable();
        int instructionIndex = 0;

        int i = 0;
        while (i < tokens.Count)
        {
            Token token = tokens[i];

            // Pomijaj NewLine i Semicolon
            if (token.Type == TokenType.NewLine || token.Type == TokenType.Semicolon)
            {
                i++;
                continue;
            }

            // Koniec pliku
            if (token.Type == TokenType.EndOfFile)
            {
                break;
            }

            // Etykieta (Identifier + Colon)
            if (token.Type == TokenType.Identifier && i + 1 < tokens.Count && tokens[i + 1].Type == TokenType.Colon)
            {
                string labelName = token.Value;
                labels.Define(labelName, instructionIndex);
                i += 2; // Pomijaj Identifier i Colon
                continue;
            }

            // Instrukcja (Identifier = mnemonik)
            if (token.Type == TokenType.Identifier)
            {
                if (!MnemonicMapper.TryMap(token.Value, out Opcode opcode))
                {
                    throw new AssemblerException($"Unknown mnemonic: '{token.Value}'", token.Line, token.Column);
                }

                i++; // Pomijaj mnemonik

                // Parsuj operandy w zależności od opcodu
                string? operand1 = null;
                string? operand2 = null;
                AddressingMode mode = GetDefaultAddressingMode(opcode);

                // Sprawdź, czy następny token to + (Relative) dla skoków/wywołań.
                if (i < tokens.Count && tokens[i].Type == TokenType.Plus)
                {
                    mode = AddressingMode.Relative;
                    i++; // Pomijaj +
                    
                    // Parsuj operand po +
                    if (i >= tokens.Count || (tokens[i].Type != TokenType.Number && tokens[i].Type != TokenType.Identifier))
                    {
                        throw new AssemblerException("Expected operand after '+'", token.Line, token.Column);
                    }
                    
                    // Dla Jump/Call: +offset jako operand1
                    if (opcode == Opcode.Jump || opcode == Opcode.JumpIfZero || opcode == Opcode.JumpIfNotZero || opcode == Opcode.Call)
                    {
                        operand1 = tokens[i].Value;
                        i++;
                    }
                    else
                    {
                        // Dla innych: nie obsługiwane
                        throw new AssemblerException($"Relative addressing not supported for {opcode}", token.Line, token.Column);
                    }
                }
                else
                {
                    // Parsuj operandy normalnie
                    (operand1, operand2, var parsedMode) = ParseOperands(tokens, ref i, opcode);
                    mode = parsedMode ?? mode;
                }

                instructions.Add(new ParsedInstruction(opcode, operand1, operand2, mode, token.Line));
                instructionIndex++;
                continue;
            }

            throw new AssemblerException($"Unexpected token: {token.Type} ('{token.Value}')", token.Line, token.Column);
        }

        return (instructions, labels);
    }

    private (string? operand1, string? operand2, AddressingMode? mode) ParseOperands(List<Token> tokens, ref int index, Opcode opcode)
    {
        string? operand1 = null;
        string? operand2 = null;

        // Instrukcje bez operandów
        if (opcode == Opcode.Nop || opcode == Opcode.Halt || opcode == Opcode.Ret)
        {
            return (null, null, null);
        }

        // Instrukcje z 1 operandem
        if (opcode == Opcode.Inc || opcode == Opcode.Dec || opcode == Opcode.Push || opcode == Opcode.Pop ||
            opcode == Opcode.Jump || opcode == Opcode.JumpIfZero || opcode == Opcode.JumpIfNotZero || opcode == Opcode.Call)
        {
            if (index >= tokens.Count || (tokens[index].Type != TokenType.Number && 
                tokens[index].Type != TokenType.Identifier && 
                tokens[index].Type != TokenType.Register))
            {
                throw new AssemblerException($"Expected operand for {opcode}", tokens[index - 1].Line, tokens[index - 1].Column);
            }
            operand1 = tokens[index].Value;
            index++;
            return (operand1, null, null);
        }

        // Instrukcje z 2 operandami
        if (opcode == Opcode.LoadImmediate || opcode == Opcode.Mov || opcode == Opcode.Load || 
            opcode == Opcode.Store || opcode == Opcode.Add || opcode == Opcode.Sub || opcode == Opcode.Cmp)
        {
            // Pierwszy operand
            if (index >= tokens.Count || (tokens[index].Type != TokenType.Number && 
                tokens[index].Type != TokenType.Identifier && 
                tokens[index].Type != TokenType.Register))
            {
                throw new AssemblerException($"Expected first operand for {opcode}", tokens[index - 1].Line, tokens[index - 1].Column);
            }
            operand1 = tokens[index].Value;
            index++;

            // Przecinek (opcjonalny)
            if (index < tokens.Count && tokens[index].Type == TokenType.Comma)
            {
                index++;
            }

            // Drugi operand, opcjonalnie zapisany jako [operand] dla adresowania pośredniego.
            if (index < tokens.Count && tokens[index].Type == TokenType.BracketOpen)
            {
                index++;

                if (index >= tokens.Count || (tokens[index].Type != TokenType.Number &&
                    tokens[index].Type != TokenType.Identifier &&
                    tokens[index].Type != TokenType.Register))
                {
                    throw new AssemblerException($"Expected indirect operand for {opcode}", tokens[index - 1].Line, tokens[index - 1].Column);
                }

                operand2 = tokens[index].Value;
                index++;

                if (index >= tokens.Count || tokens[index].Type != TokenType.BracketClose)
                {
                    throw new AssemblerException($"Expected ']' after indirect operand for {opcode}", tokens[index - 1].Line, tokens[index - 1].Column);
                }

                index++;
                return (operand1, operand2, AddressingMode.Indirect);
            }

            if (index >= tokens.Count || (tokens[index].Type != TokenType.Number && 
                tokens[index].Type != TokenType.Identifier && 
                tokens[index].Type != TokenType.Register))
            {
                throw new AssemblerException($"Expected second operand for {opcode}", tokens[index - 1].Line, tokens[index - 1].Column);
            }
            operand2 = tokens[index].Value;
            index++;
            return (operand1, operand2, null);
        }

        return (null, null, null);
    }

    private AddressingMode GetDefaultAddressingMode(Opcode opcode)
    {
        return opcode switch
        {
            Opcode.LoadImmediate => AddressingMode.Immediate,
            Opcode.Load or Opcode.Store => AddressingMode.Direct,
            Opcode.Jump or Opcode.JumpIfZero or Opcode.JumpIfNotZero or Opcode.Call => AddressingMode.Direct,
            _ => AddressingMode.Immediate // Domyślny tryb
        };
    }
}
