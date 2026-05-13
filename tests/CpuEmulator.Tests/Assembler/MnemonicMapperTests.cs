using CpuEmulator.Assembler;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpuEmulator.Tests.Assembler
{
    [TestClass]
    public class MnemonicMapperTests
    {
        [TestMethod]
        public void MnemonicMapper_ShouldMapAll18Mnemonics()
        {
            // Arrange
            var mnemonics = new[]
            {
                "NOP", "LDI", "MOV", "LD", "ST", "ADD", "SUB", "INC", "DEC", "CMP",
                "JMP", "JZ", "JNZ", "PUSH", "POP", "CALL", "RET", "HALT"
            };

            // Act & Assert
            foreach (var mnemonic in mnemonics)
            {
                MnemonicMapper.TryMap(mnemonic, out var opcode).Should().BeTrue(
                    $"because mnemonic '{mnemonic}' should be mapped");
            }
        }

        [TestMethod]
        public void MnemonicMapper_ShouldBeCaseInsensitive()
        {
            // Arrange & Act & Assert
            MnemonicMapper.TryMap("nop", out var opcode1).Should().BeTrue();
            MnemonicMapper.TryMap("NOP", out var opcode2).Should().BeTrue();
            MnemonicMapper.TryMap("Nop", out var opcode3).Should().BeTrue();
            MnemonicMapper.TryMap("NoP", out var opcode4).Should().BeTrue();
            
            opcode1.Should().Be(opcode2);
            opcode2.Should().Be(opcode3);
            opcode3.Should().Be(opcode4);
        }

        [TestMethod]
        public void MnemonicMapper_ShouldReturnFalseForUnknownMnemonic()
        {
            // Arrange & Act & Assert
            MnemonicMapper.TryMap("UNKNOWN", out var opcode).Should().BeFalse();
            MnemonicMapper.TryMap("INVALID", out opcode).Should().BeFalse();
            MnemonicMapper.TryMap("", out opcode).Should().BeFalse();
        }

        [TestMethod]
        public void MnemonicMapper_ShouldMapSpecificMnemonicsCorrectly()
        {
            // Arrange & Act & Assert
            MnemonicMapper.TryMap("LDI", out var ldiOpcode).Should().BeTrue();
            ldiOpcode.Should().Be(Opcode.LoadImmediate);
            
            MnemonicMapper.TryMap("MOV", out var movOpcode).Should().BeTrue();
            movOpcode.Should().Be(Opcode.Mov);
            
            MnemonicMapper.TryMap("ADD", out var addOpcode).Should().BeTrue();
            addOpcode.Should().Be(Opcode.Add);
            
            MnemonicMapper.TryMap("HALT", out var haltOpcode).Should().BeTrue();
            haltOpcode.Should().Be(Opcode.Halt);
        }
    }
}