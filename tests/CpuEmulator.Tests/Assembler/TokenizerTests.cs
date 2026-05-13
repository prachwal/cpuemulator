using CpuEmulator.Assembler;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CpuEmulator.Tests.Assembler
{
    [TestClass]
    public class TokenizerTests
    {
        [TestMethod]
        public void Tokenizer_ShouldHandleEmptyInput()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            tokens.Should().HaveCount(1);
            tokens[0].Type.Should().Be(TokenType.EndOfFile);
        }

        [TestMethod]
        public void Tokenizer_ShouldHandleComments()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "; This is a comment\nNOP";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            tokens.Should().Contain(t => t.Type == TokenType.NewLine);
            tokens.Should().Contain(t => t.Type == TokenType.Identifier && t.Value == "NOP");
            tokens.Should().NotContain(t => t.Value == "This is a comment");
        }

        [TestMethod]
        public void Tokenizer_ShouldHandleWhitespace()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "  NOP  \t  MOV  ";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            tokens.Should().Contain(t => t.Type == TokenType.Identifier && t.Value == "NOP");
            tokens.Should().Contain(t => t.Type == TokenType.Identifier && t.Value == "MOV");
            tokens.Should().NotContain(t => t.Type == TokenType.Identifier && t.Value == "");
        }

        [TestMethod]
        public void Tokenizer_ShouldHandleNewlines()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "NOP\nMOV\nLDI";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            tokens.Should().Contain(t => t.Type == TokenType.NewLine);
            var newLineTokens = tokens.FindAll(t => t.Type == TokenType.NewLine);
            newLineTokens.Should().HaveCount(2);
        }

        [TestMethod]
        public void Tokenizer_ShouldHandleRegisters()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "R0 R1 R2 R3";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            tokens.Should().Contain(t => t.Type == TokenType.Register && t.Value == "R0");
            tokens.Should().Contain(t => t.Type == TokenType.Register && t.Value == "R1");
            tokens.Should().Contain(t => t.Type == TokenType.Register && t.Value == "R2");
            tokens.Should().Contain(t => t.Type == TokenType.Register && t.Value == "R3");
        }

        [TestMethod]
        public void Tokenizer_ShouldHandleNumbers()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "42 -10 +5 0";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            tokens.Should().Contain(t => t.Type == TokenType.Number && t.Value == "42");
            tokens.Should().Contain(t => t.Type == TokenType.Number && t.Value == "-10");
            tokens.Should().Contain(t => t.Type == TokenType.Plus && t.Value == "+");
            tokens.Should().Contain(t => t.Type == TokenType.Number && t.Value == "5");
            tokens.Should().Contain(t => t.Type == TokenType.Number && t.Value == "0");
        }

        [TestMethod]
        public void Tokenizer_ShouldHandleSingleCharTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = ": , [ ] +";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            tokens.Should().Contain(t => t.Type == TokenType.Colon);
            tokens.Should().Contain(t => t.Type == TokenType.Comma);
            tokens.Should().Contain(t => t.Type == TokenType.BracketOpen);
            tokens.Should().Contain(t => t.Type == TokenType.BracketClose);
            tokens.Should().Contain(t => t.Type == TokenType.Plus);
            // Semicolon is handled as comment starter and skipped, so it shouldn't appear as a token
            tokens.Should().NotContain(t => t.Type == TokenType.Semicolon);
        }

        [TestMethod]
        public void Tokenizer_ShouldHandleIdentifiers()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "start: LDI MOV";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            tokens.Should().Contain(t => t.Type == TokenType.Identifier && t.Value == "start");
            tokens.Should().Contain(t => t.Type == TokenType.Identifier && t.Value == "LDI");
            tokens.Should().Contain(t => t.Type == TokenType.Identifier && t.Value == "MOV");
        }

        [TestMethod]
        public void Tokenizer_ShouldHandleExampleProgram()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "; Example program\nstart: LDI R0, 42  ; Load 42 into R0\n       ADD R0, R1\n";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            // Check for label
            tokens.Should().Contain(t => t.Type == TokenType.Identifier && t.Value == "start");
            tokens.Should().Contain(t => t.Type == TokenType.Colon);
            
            // Check for LDI instruction
            tokens.Should().Contain(t => t.Type == TokenType.Identifier && t.Value == "LDI");
            tokens.Should().Contain(t => t.Type == TokenType.Register && t.Value == "R0");
            tokens.Should().Contain(t => t.Type == TokenType.Comma);
            tokens.Should().Contain(t => t.Type == TokenType.Number && t.Value == "42");
            
            // Check for ADD instruction
            tokens.Should().Contain(t => t.Type == TokenType.Identifier && t.Value == "ADD");
            tokens.Should().Contain(t => t.Type == TokenType.Register && t.Value == "R1");
            
            // Should not contain comment text
            tokens.Should().NotContain(t => t.Value.Contains("Example program"));
            tokens.Should().NotContain(t => t.Value.Contains("Load 42 into R0"));
        }

        [TestMethod]
        public void Tokenizer_ShouldTrackLineAndColumnCorrectly()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "start: LDI R0, 42\nADD R0, R1";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            // Find the LDI token
            var ldiToken = tokens.Find(t => t.Type == TokenType.Identifier && t.Value == "LDI");
            ldiToken.Should().NotBeNull();
            ldiToken!.Line.Should().Be(1);
            ldiToken!.Column.Should().Be(7);
            
            // Find the ADD token
            var addToken = tokens.Find(t => t.Type == TokenType.Identifier && t.Value == "ADD");
            addToken.Should().NotBeNull();
            addToken!.Line.Should().Be(2);
            addToken!.Column.Should().Be(0);
        }

        [TestMethod]
        public void Tokenizer_ShouldEndWithEndOfFile()
        {
            // Arrange
            var tokenizer = new Tokenizer();
            var input = "NOP";

            // Act
            var tokens = tokenizer.Tokenize(input);

            // Assert
            tokens.Should().HaveCountGreaterThan(0);
            tokens[^1].Type.Should().Be(TokenType.EndOfFile);
        }
    }
}
