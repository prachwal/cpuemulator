using CpuEmulator.Assembler;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CpuEmulator.Tests.Assembler
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public void Tokenizer_ShouldHandleExampleProgramFromSpecification()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "; Example program\nstart: LDI R0, 42  ; Load 42 into R0\n       ADD R0, R1\n";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert - Check the expected tokens from the specification
            // Line 2: start: LDI R0, 42
            var startToken = tokens.FirstOrDefault(t => t.Type == TokenType.Identifier && t.Value == "start");
            startToken.Should().NotBeNull();
            startToken!.Line.Should().Be(2);
            startToken.Column.Should().Be(0);

            var colonToken = tokens.FirstOrDefault(t => t.Type == TokenType.Colon);
            colonToken.Should().NotBeNull();
            colonToken!.Line.Should().Be(2);
            colonToken.Column.Should().Be(5);

            var ldiToken = tokens.FirstOrDefault(t => t.Type == TokenType.Identifier && t.Value == "LDI");
            ldiToken.Should().NotBeNull();
            ldiToken!.Line.Should().Be(2);
            ldiToken.Column.Should().Be(7);

            var r0Token = tokens.FirstOrDefault(t => t.Type == TokenType.Register && t.Value == "R0");
            r0Token.Should().NotBeNull();
            r0Token!.Line.Should().Be(2);
            r0Token.Column.Should().Be(11);

            var commaToken = tokens.FirstOrDefault(t => t.Type == TokenType.Comma);
            commaToken.Should().NotBeNull();
            commaToken!.Line.Should().Be(2);
            commaToken.Column.Should().Be(13);

            var numberToken = tokens.FirstOrDefault(t => t.Type == TokenType.Number && t.Value == "42");
            numberToken.Should().NotBeNull();
            numberToken!.Line.Should().Be(2);
            numberToken.Column.Should().Be(15);

            // Line 3: ADD R0, R1
            var addToken = tokens.FirstOrDefault(t => t.Type == TokenType.Identifier && t.Value == "ADD");
            addToken.Should().NotBeNull();
            addToken!.Line.Should().Be(3);
            addToken.Column.Should().Be(7);

            var r0Token2 = tokens.FirstOrDefault(t => t.Type == TokenType.Register && t.Value == "R0" && t.Line == 3);
            r0Token2.Should().NotBeNull();
            r0Token2!.Line.Should().Be(3);
            r0Token2.Column.Should().Be(11);

            var commaToken2 = tokens.FirstOrDefault(t => t.Type == TokenType.Comma && t.Line == 3);
            commaToken2.Should().NotBeNull();
            commaToken2!.Line.Should().Be(3);
            commaToken2.Column.Should().Be(13);

            var r1Token = tokens.FirstOrDefault(t => t.Type == TokenType.Register && t.Value == "R1");
            r1Token.Should().NotBeNull();
            r1Token!.Line.Should().Be(3);
            r1Token.Column.Should().Be(15);

            // Should not contain comment text
            tokens.Should().NotContain(t => t.Value.Contains("Example program"));
            tokens.Should().NotContain(t => t.Value.Contains("Load 42 into R0"));
        }

        [TestMethod]
        public void MnemonicMapper_ShouldMapAll18MnemonicsCaseInsensitive()
        {
            // Arrange
            var mnemonics = new[]
            {
                "NOP", "nop", "Nop", "NoP",
                "LDI", "ldi", "Ldi", "LdI",
                "MOV", "mov", "Mov", "MoV",
                "LD", "ld", "Ld", "LD",
                "ST", "st", "St", "ST",
                "ADD", "add", "Add", "AdD",
                "SUB", "sub", "Sub", "SuB",
                "INC", "inc", "Inc", "InC",
                "DEC", "dec", "Dec", "DeC",
                "CMP", "cmp", "Cmp", "CmP",
                "JMP", "jmp", "Jmp", "JmP",
                "JZ", "jz", "Jz", "JZ",
                "JNZ", "jnz", "Jnz", "JnZ",
                "PUSH", "push", "Push", "PuSh",
                "POP", "pop", "Pop", "PoP",
                "CALL", "call", "Call", "CaLl",
                "RET", "ret", "Ret", "ReT",
                "HALT", "halt", "Halt", "HaLt"
            };

            // Act & Assert
            foreach (var mnemonic in mnemonics)
            {
                MnemonicMapper.TryMap(mnemonic, out var opcode).Should().BeTrue(
                    $"because mnemonic '{mnemonic}' should be mapped regardless of case");
            }
        }
    }
}
