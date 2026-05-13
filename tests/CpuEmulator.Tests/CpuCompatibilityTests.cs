using CpuEmulator;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpuEmulator.Tests;

[TestClass]
public class CpuCompatibilityTests
{
    [TestMethod]
    public void Reset_ShouldClearProgramStateRegistersAndMemory()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.Store, 0, 10),
            new Instruction(Opcode.Halt)
        });
        cpu.Run();

        cpu.Reset();

        var state = cpu.GetState();
        state.ProgramCounter.Should().Be(0);
        state.IsHalted.Should().BeFalse();
        state.Registers.GetRegister(0).Should().Be(0);
        state.Memory.Read(10).Should().Be(0);
    }

    [TestMethod]
    public void CpuFactory_ShouldCreateCpuWithCustomMemorySize()
    {
        var cpu = CpuFactory.CreateWithMemorySize(16);

        var state = cpu.GetState();
        state.Memory.Size.Should().Be(16);
        state.Registers.Count.Should().Be(4);
    }

    [TestMethod]
    public void CpuFactory_ShouldCreateCpuWithCustomRegisterCount()
    {
        var cpu = CpuFactory.CreateWithRegisterCount(8);

        var state = cpu.GetState();
        state.Registers.Count.Should().Be(8);
        state.Memory.Size.Should().Be(256);
    }

    [TestMethod]
    public void BackwardCompatibleProperties_ShouldExposeCurrentState()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 7),
            new Instruction(Opcode.Store, 0, 20),
            new Instruction(Opcode.Halt)
        });
        cpu.Run();

#pragma warning disable CS0618
        cpu.Registers.GetRegister(0).Should().Be(7);
        cpu.Memory.Read(20).Should().Be(7);
        cpu.ProgramCounter.Should().Be(3);
        cpu.ZeroFlag.Should().BeFalse();
        cpu.Halted.Should().BeTrue();
        cpu.Program.Should().HaveCount(3);
#pragma warning restore CS0618
    }
}
