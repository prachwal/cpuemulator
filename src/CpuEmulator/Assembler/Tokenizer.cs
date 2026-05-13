using System.Text;
using CpuEmulator.Exceptions;

namespace CpuEmulator.Assembler;

/// <summary>
/// Tokenizator kodu asemblera.
/// </summary>
public class Tokenizer
{
    private const string RegisterPrefix = "R";
    private const int MaxRegisterIndex = 3;

    /// <summary>
    /// Tokenizuje kod asemblera na listę tokenów.
    /// </summary>
    /// <param name="input">Kod asemblera do tokenizacji.</param>
    /// <returns>Lista tokenów.</returns>
    public List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        int line = 1;
        int column = 0;
        int position = 0;

        while (position < input.Length)
        {
            char current = input[position];

            // Nowa linia
            if (current == '\n')
            {
                tokens.Add(new Token(TokenType.NewLine, "\n", line, column));
                line++;
                column = 0;
                position++;
                continue;
            }

            // Komentarz (od ; do końca linii)
            if (current == ';')
            {
                tokens.Add(new Token(TokenType.Semicolon, ";", line, column));
                // Pomiń resztę linii
                while (position < input.Length && input[position] != '\n')
                {
                    position++;
                    column++;
                }
                continue;
            }

            // Białe znaki (spacja, tabulator)
            if (char.IsWhiteSpace(current) && current != '\n')
            {
                position++;
                column++;
                continue;
            }

            // Tokeny jednoznakowe
            Token? singleCharToken = TryReadSingleCharToken(current, line, column);
            if (singleCharToken != null)
            {
                tokens.Add(singleCharToken);
                position++;
                column++;
                continue;
            }

            // Liczby
            if (char.IsDigit(current) || (current == '-' && position + 1 < input.Length && char.IsDigit(input[position + 1])))
            {
                tokens.Add(ReadNumber(input, ref position, line, ref column));
                continue;
            }

            // Identyfikatory i rejestry
            if (char.IsLetter(current) || current == '_')
            {
                tokens.Add(ReadIdentifierOrRegister(input, ref position, line, ref column));
                continue;
            }

            // Nieznany znak
            throw new AssemblerException($"Unknown character: '{current}'", line, column);
        }

        tokens.Add(new Token(TokenType.EndOfFile, "", line, column));
        return tokens;
    }

    private Token? TryReadSingleCharToken(char current, int line, int column)
    {
        return current switch
        {
            ',' => new Token(TokenType.Comma, ",", line, column),
            ':' => new Token(TokenType.Colon, ":", line, column),
            '[' => new Token(TokenType.BracketOpen, "[", line, column),
            ']' => new Token(TokenType.BracketClose, "]", line, column),
            '+' => new Token(TokenType.Plus, "+", line, column),
            _ => null
        };
    }

    private Token ReadNumber(string input, ref int position, int line, ref int column)
    {
        var sb = new StringBuilder();

        if (input[position] == '-')
        {
            sb.Append('-');
            position++;
            column++;
        }

        while (position < input.Length && char.IsDigit(input[position]))
        {
            sb.Append(input[position]);
            position++;
            column++;
        }

        return new Token(TokenType.Number, sb.ToString(), line, column - sb.Length);
    }

    private Token ReadIdentifierOrRegister(string input, ref int position, int line, ref int column)
    {
        var sb = new StringBuilder();
        int startColumn = column;

        while (position < input.Length && (char.IsLetterOrDigit(input[position]) || input[position] == '_'))
        {
            sb.Append(input[position]);
            position++;
            column++;
        }

        string value = sb.ToString();

        // Sprawdź, czy to rejestr (R0, R1, R2, R3)
        if (value.StartsWith(RegisterPrefix, StringComparison.OrdinalIgnoreCase) &&
            value.Length == 2 &&
            char.IsDigit(value[1]) &&
            int.Parse(value[1].ToString()) <= MaxRegisterIndex)
        {
            return new Token(TokenType.Register, value, line, startColumn);
        }

        return new Token(TokenType.Identifier, value, line, startColumn);
    }
}
