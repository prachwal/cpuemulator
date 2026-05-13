using CpuEmulator.Assembler;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpuEmulator.Tests.Assembler;

[TestClass]
public class ExampleProgramTests
{
    [TestMethod]
    public void ExampleProgram_ShouldAssembleToExpectedBinaryAndRun()
    {
        var assembler = new CpuEmulator.Assembler.Assembler();
        var programPath = FindRepositoryFile("examples/program.asm");

        var instructions = assembler.AssembleFromFile(programPath);

        instructions.Should().HaveCount(5);
        instructions[0].Should().Be(new Instruction(Opcode.LoadImmediate, 0, 10));
        instructions[1].Should().Be(new Instruction(Opcode.LoadImmediate, 1, 20));
        instructions[2].Should().Be(new Instruction(Opcode.Add, 0, 1));
        instructions[3].Should().Be(new Instruction(Opcode.Store, 0, 10, CpuEmulator.Model.AddressingMode.Direct));
        instructions[4].Should().Be(new Instruction(Opcode.Halt));

        var cpu = new Cpu();
        cpu.LoadProgram(instructions);
        cpu.Run();

        var state = cpu.GetState();
        state.Registers.GetRegister(0).Should().Be(30);
        state.Memory.Read(10).Should().Be(30);
    }

    private static string FindRepositoryFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file '{relativePath}'.", relativePath);
    }
}
