using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpuEmulator.Tests;

[TestClass]
public class CpuTests
{
     [TestMethod]
    public void Add_ShouldSumRegisters()
    {
        var cpu = new CpuEmulator.Cpu();

        cpu.LoadProgram(new[]
        {
            new CpuEmulator.Instruction(CpuEmulator.Opcode.LoadImmediate, 0, 2),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.LoadImmediate, 1, 3),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.Add, 0, 1),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(5);
    }

     [TestMethod]
    public void Store_ShouldWriteToMemory()
    {
        var cpu = new CpuEmulator.Cpu();

        cpu.LoadProgram(new[]
        {
            new CpuEmulator.Instruction(CpuEmulator.Opcode.LoadImmediate, 0, 7),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.Store, 0, 20),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Memory.Read(20).Should().Be(7);
    }

     [TestMethod]
    public void JumpIfZero_ShouldJump()
    {
        var cpu = new CpuEmulator.Cpu();

        cpu.LoadProgram(new[]
        {
            new CpuEmulator.Instruction(CpuEmulator.Opcode.LoadImmediate, 0, 1),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.LoadImmediate, 1, 1),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.Cmp, 0, 1),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.JumpIfZero, 6),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.LoadImmediate, 2, 99),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.Halt),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.LoadImmediate, 2, 42),
            new CpuEmulator.Instruction(CpuEmulator.Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(2).Should().Be(42);
    }
}
