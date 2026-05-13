using CpuEmulator;
using CpuEmulator.Assembler;
using CpuEmulator.Exceptions;
using CpuEmulator.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpuEmulator.Tests;

[TestClass]
public class AssemblerTests
{
    private readonly CpuEmulator.Assembler.Assembler _assembler = new();

    // ========== Tokenizer Tests ==========

    [TestMethod]
    public void Tokenizer_ShouldTokenizeEmptyInput()
    {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize("");

        tokens.Should().HaveCount(1);
        tokens[0].Type.Should().Be(TokenType.EndOfFile);
    }

    [TestMethod]
    public void Tokenizer_ShouldTokenizeComment()
    {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize("; This is a comment");

        tokens.Should().ContainSingle(t => t.Type == TokenType.Semicolon);
        tokens.Should().ContainSingle(t => t.Type == TokenType.EndOfFile);
    }

    [TestMethod]
    public void Tokenizer_ShouldTokenizeLabel()
    {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize("start:");

        tokens.Should().ContainSingle(t => t.Type == TokenType.Identifier && t.Value == "start");
        tokens.Should().ContainSingle(t => t.Type == TokenType.Colon);
    }

    [TestMethod]
    public void Tokenizer_ShouldTokenizeRegister()
    {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize("R0 R1 R2 R3");

        tokens.Should().HaveCount(5); // 4 registers + EOF
        tokens[0].Type.Should().Be(TokenType.Register);
        tokens[0].Value.Should().Be("R0");
        tokens[1].Type.Should().Be(TokenType.Register);
        tokens[1].Value.Should().Be("R1");
    }

    [TestMethod]
    public void Tokenizer_ShouldHandleNumbers()
    {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize("42 -10 0");

        tokens.Should().HaveCount(4); // 3 numbers + EOF
        tokens[0].Type.Should().Be(TokenType.Number);
        tokens[0].Value.Should().Be("42");
        tokens[1].Type.Should().Be(TokenType.Number);
        tokens[1].Value.Should().Be("-10");
        tokens[2].Type.Should().Be(TokenType.Number);
        tokens[2].Value.Should().Be("0");
    }

    [TestMethod]
    public void Tokenizer_ShouldTokenizeBrackets()
    {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize("[10]");

        tokens.Should().HaveCount(4); // [, 10, ], EOF
        tokens[0].Type.Should().Be(TokenType.BracketOpen);
        tokens[1].Type.Should().Be(TokenType.Number);
        tokens[1].Value.Should().Be("10");
        tokens[2].Type.Should().Be(TokenType.BracketClose);
    }

    [TestMethod]
    public void Tokenizer_ShouldTokenizeRelativeAddressing()
    {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize("+5");

        tokens.Should().HaveCount(3); // +, 5, EOF
        tokens[0].Type.Should().Be(TokenType.Plus);
        tokens[1].Type.Should().Be(TokenType.Number);
        tokens[1].Value.Should().Be("5");
    }

    // ========== MnemonicMapper Tests ==========

    [TestMethod]
    public void MnemonicMapper_ShouldMapAllOpcodes()
    {
        MnemonicMapper.TryMap("NOP", out _).Should().BeTrue();
        MnemonicMapper.TryMap("LDI", out _).Should().BeTrue();
        MnemonicMapper.TryMap("MOV", out _).Should().BeTrue();
        MnemonicMapper.TryMap("LD", out _).Should().BeTrue();
        MnemonicMapper.TryMap("ST", out _).Should().BeTrue();
        MnemonicMapper.TryMap("ADD", out _).Should().BeTrue();
        MnemonicMapper.TryMap("SUB", out _).Should().BeTrue();
        MnemonicMapper.TryMap("INC", out _).Should().BeTrue();
        MnemonicMapper.TryMap("DEC", out _).Should().BeTrue();
        MnemonicMapper.TryMap("CMP", out _).Should().BeTrue();
        MnemonicMapper.TryMap("JMP", out _).Should().BeTrue();
        MnemonicMapper.TryMap("JZ", out _).Should().BeTrue();
        MnemonicMapper.TryMap("JNZ", out _).Should().BeTrue();
        MnemonicMapper.TryMap("PUSH", out _).Should().BeTrue();
        MnemonicMapper.TryMap("POP", out _).Should().BeTrue();
        MnemonicMapper.TryMap("CALL", out _).Should().BeTrue();
        MnemonicMapper.TryMap("RET", out _).Should().BeTrue();
        MnemonicMapper.TryMap("HALT", out _).Should().BeTrue();
    }

    [TestMethod]
    public void MnemonicMapper_ShouldBeCaseInsensitive()
    {
        MnemonicMapper.TryMap("ldi", out var opcode1).Should().BeTrue();
        MnemonicMapper.TryMap("LDI", out var opcode2).Should().BeTrue();
        opcode1.Should().Be(opcode2);
    }

    // ========== Parser Tests ==========

    [TestMethod]
    public void Parser_ShouldParseNop()
    {
        var tokenizer = new Tokenizer();
        var parser = new Parser();
        var tokens = tokenizer.Tokenize("NOP");
        var (instructions, labels) = parser.Parse(tokens);

        instructions.Should().HaveCount(1);
        instructions[0].Opcode.Should().Be(Opcode.Nop);
        instructions[0].Operand1Text.Should().BeNull();
        instructions[0].Operand2Text.Should().BeNull();
    }

    [TestMethod]
    public void Parser_ShouldParseLoadImmediate()
    {
        var tokenizer = new Tokenizer();
        var parser = new Parser();
        var tokens = tokenizer.Tokenize("LDI R0, 42");
        var (instructions, labels) = parser.Parse(tokens);

        instructions.Should().HaveCount(1);
        instructions[0].Opcode.Should().Be(Opcode.LoadImmediate);
        instructions[0].Operand1Text.Should().Be("R0");
        instructions[0].Operand2Text.Should().Be("42");
    }

    [TestMethod]
    public void Parser_ShouldParseLabel()
    {
        var tokenizer = new Tokenizer();
        var parser = new Parser();
        var tokens = tokenizer.Tokenize("start: LDI R0, 42");
        var (instructions, labels) = parser.Parse(tokens);

        labels.TryResolve("start", out int address).Should().BeTrue();
        address.Should().Be(0);
        instructions.Should().HaveCount(1);
    }

    [TestMethod]
    public void Parser_ShouldParseIndirectAddressing()
    {
        var tokenizer = new Tokenizer();
        var parser = new Parser();
        var tokens = tokenizer.Tokenize("LD R0, [10]");
        var (instructions, labels) = parser.Parse(tokens);

        instructions.Should().HaveCount(1);
        instructions[0].Opcode.Should().Be(Opcode.Load);
        instructions[0].Operand1Text.Should().Be("R0");
        instructions[0].Operand2Text.Should().Be("10");
        instructions[0].Mode.Should().Be(AddressingMode.Indirect);
    }

    [TestMethod]
    public void Parser_ShouldParseRelativeAddressing()
    {
        var tokenizer = new Tokenizer();
        var parser = new Parser();
        var tokens = tokenizer.Tokenize("JMP +5");
        var (instructions, labels) = parser.Parse(tokens);

        instructions.Should().HaveCount(1);
        instructions[0].Opcode.Should().Be(Opcode.Jump);
        instructions[0].Operand1Text.Should().Be("5");
        instructions[0].Mode.Should().Be(AddressingMode.Relative);
    }

    // ========== InstructionResolver Tests ==========

    [TestMethod]
    public void InstructionResolver_ShouldResolveRegisters()
    {
        var labels = new LabelTable();
        var resolver = new InstructionResolver(labels);
        var parsed = new ParsedInstruction(Opcode.LoadImmediate, "R0", "42", AddressingMode.Immediate, 1);
        var instructions = resolver.Resolve(new List<ParsedInstruction> { parsed });

        instructions.Should().HaveCount(1);
        instructions[0].Operand1.Should().Be(0);
        instructions[0].Operand2.Should().Be(42);
    }

    [TestMethod]
    public void InstructionResolver_ShouldResolveLabels()
    {
        var labels = new LabelTable();
        labels.Define("start", 5);
        var resolver = new InstructionResolver(labels);
        var parsed = new ParsedInstruction(Opcode.Jump, "start", null, AddressingMode.Direct, 1);
        var instructions = resolver.Resolve(new List<ParsedInstruction> { parsed });

        instructions.Should().HaveCount(1);
        instructions[0].Operand1.Should().Be(5);
    }

    [TestMethod]
    [ExpectedException(typeof(AssemblerException))]
    public void InstructionResolver_ShouldThrowOnUnresolvedLabel()
    {
        var labels = new LabelTable();
        var resolver = new InstructionResolver(labels);
        var parsed = new ParsedInstruction(Opcode.Jump, "undefined", null, AddressingMode.Direct, 1);
        resolver.Resolve(new List<ParsedInstruction> { parsed });
    }

    // ========== Assembler End-to-End Tests ==========

    [TestMethod]
    public void Assembler_ShouldAssembleSimpleProgram()
    {
        string asmCode = @"
LDI R0, 5
LDI R1, 3
ADD R0, R1
HALT
";
        var instructions = _assembler.Assemble(asmCode);

        instructions.Should().HaveCount(4);
        instructions[0].Opcode.Should().Be(Opcode.LoadImmediate);
        instructions[0].Operand1.Should().Be(0);
        instructions[0].Operand2.Should().Be(5);

        instructions[1].Opcode.Should().Be(Opcode.LoadImmediate);
        instructions[1].Operand1.Should().Be(1);
        instructions[1].Operand2.Should().Be(3);

        instructions[2].Opcode.Should().Be(Opcode.Add);
        instructions[2].Operand1.Should().Be(0);
        instructions[2].Operand2.Should().Be(1);

        instructions[3].Opcode.Should().Be(Opcode.Halt);
    }

    [TestMethod]
    public void Assembler_ShouldAssembleWithLabels()
    {
        string asmCode = @"
start: LDI R0, 10
       JMP end
       LDI R1, 20
end:   HALT
";
        var instructions = _assembler.Assemble(asmCode);

        instructions.Should().HaveCount(4);
        instructions[0].Opcode.Should().Be(Opcode.LoadImmediate);
        instructions[1].Opcode.Should().Be(Opcode.Jump);
        instructions[1].Operand1.Should().Be(3); // end label is at index 3
        instructions[3].Opcode.Should().Be(Opcode.Halt);
    }

    [TestMethod]
    public void Assembler_ShouldAssembleWithIndirectAddressing()
    {
        string asmCode = @"
LDI R0, 20
ST R0, 10
LD R1, [10]
HALT
";
        var instructions = _assembler.Assemble(asmCode);

        instructions.Should().HaveCount(4);
        instructions[2].Opcode.Should().Be(Opcode.Load);
        instructions[2].Operand1.Should().Be(1);
        instructions[2].Operand2.Should().Be(10);
        instructions[2].Mode.Should().Be(AddressingMode.Indirect);
    }

    [TestMethod]
    public void Assembler_ShouldAssembleWithRelativeAddressing()
    {
        string asmCode = @"
LDI R0, 1
JMP +2
LDI R1, 99
HALT
";
        var instructions = _assembler.Assemble(asmCode);

        instructions.Should().HaveCount(4);
        instructions[1].Opcode.Should().Be(Opcode.Jump);
        instructions[1].Operand1.Should().Be(2); // +2 from current PC (which is 1)
        instructions[1].Mode.Should().Be(AddressingMode.Relative);
    }

    [TestMethod]
    public void Assembler_ShouldHandleComments()
    {
        string asmCode = @"
; This is a comment
LDI R0, 42  ; Load 42 into R0
HALT        ; Stop execution
";
        var instructions = _assembler.Assemble(asmCode);

        instructions.Should().HaveCount(2);
        instructions[0].Opcode.Should().Be(Opcode.LoadImmediate);
        instructions[0].Operand1.Should().Be(0);
        instructions[0].Operand2.Should().Be(42);
    }

    [TestMethod]
    [ExpectedException(typeof(AssemblerException))]
    public void Assembler_ShouldThrowOnUnknownMnemonic()
    {
        _assembler.Assemble("UNKNOWN R0, 10");
    }

    [TestMethod]
    [ExpectedException(typeof(AssemblerException))]
    public void Assembler_ShouldThrowOnDuplicateLabel()
    {
        _assembler.Assemble(@"
start: LDI R0, 10
start: LDI R1, 20
");
    }

    [TestMethod]
    [ExpectedException(typeof(AssemblerException))]
    public void Assembler_ShouldThrowOnUnresolvedLabel()
    {
        _assembler.Assemble("JMP undefined_label");
    }

    // ========== Integration Tests (Assembler + CPU) ==========

    [TestMethod]
    public void Assembler_IntegrationTest_SimpleAddition()
    {
        string asmCode = @"
LDI R0, 5
LDI R1, 3
ADD R0, R1
HALT
";
        var cpu = new Cpu();
        var instructions = _assembler.Assemble(asmCode);
        cpu.LoadProgram(instructions);
        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(8);
    }

    [TestMethod]
    public void Assembler_IntegrationTest_WithLabelsAndJump()
    {
        string asmCode = @"
LDI R0, 1
CMP R0, R0
JZ skip
LDI R0, 99
skip: HALT
";
        var cpu = new Cpu();
        var instructions = _assembler.Assemble(asmCode);
        cpu.LoadProgram(instructions);
        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(1);
    }

    [TestMethod]
    public void Assembler_IntegrationTest_IndirectLoad()
    {
        string asmCode = @"
LDI R0, 20
ST R0, 10
LDI R0, 42
ST R0, 20
LD R1, [10]
HALT
";
        var cpu = new Cpu();
        var instructions = _assembler.Assemble(asmCode);
        cpu.LoadProgram(instructions);
        cpu.Run();

        cpu.GetState().Registers.GetRegister(1).Should().Be(42);
    }

    [TestMethod]
    public void AssembleFromFile_ShouldWork()
    {
        // Utworzenie tymczasowego pliku
        string tempFile = Path.GetTempFileName();
        string asmCode = "LDI R0, 42\nHALT";
        File.WriteAllText(tempFile, asmCode);

        try
        {
            var instructions = _assembler.AssembleFromFile(tempFile);
            instructions.Should().HaveCount(2);
            instructions[0].Opcode.Should().Be(Opcode.LoadImmediate);
            instructions[0].Operand1.Should().Be(0);
            instructions[0].Operand2.Should().Be(42);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
