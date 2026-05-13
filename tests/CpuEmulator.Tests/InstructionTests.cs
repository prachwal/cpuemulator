using CpuEmulator;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpuEmulator.Tests;

[TestClass]
public class InstructionTests
{
    [TestMethod]
    public void Nop_ShouldDoNothing()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.Nop),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(42);
        cpu.GetState().ProgramCounter.Should().Be(3);
    }

    [TestMethod]
    public void Mov_ShouldCopyRegister()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.Mov, 1, 0),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(42);
        cpu.GetState().Registers.GetRegister(1).Should().Be(42);
    }

    [TestMethod]
    public void Load_ShouldReadFromMemory()
    {
        var cpu = new Cpu();
        // Najpierw zapisz wartość do pamięci
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 99),
            new Instruction(Opcode.Store, 0, 10),
            new Instruction(Opcode.LoadImmediate, 0, 0),
            new Instruction(Opcode.Load, 1, 10),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(1).Should().Be(99);
    }

    [TestMethod]
    public void Sub_ShouldSubtractRegisters()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 10),
            new Instruction(Opcode.LoadImmediate, 1, 3),
            new Instruction(Opcode.Sub, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(7);
    }

    [TestMethod]
    public void Inc_ShouldIncrementRegister()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.Inc, 0),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(6);
    }

    [TestMethod]
    public void Dec_ShouldDecrementRegister()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.Dec, 0),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(4);
    }

    [TestMethod]
    public void Cmp_ShouldSetZeroFlag_WhenEqual()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.LoadImmediate, 1, 5),
            new Instruction(Opcode.Cmp, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Flags.ZeroFlag.Should().BeTrue();
    }

    [TestMethod]
    public void Cmp_ShouldClearZeroFlag_WhenNotEqual()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.LoadImmediate, 1, 10),
            new Instruction(Opcode.Cmp, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Flags.ZeroFlag.Should().BeFalse();
    }

    [TestMethod]
    public void Jump_ShouldSetProgramCounter()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.Jump, 3),
            new Instruction(Opcode.LoadImmediate, 0, 99),
            new Instruction(Opcode.Halt),
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(42);
    }

    [TestMethod]
    public void JumpIfNotZero_ShouldJump_WhenNotZero()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 1),
            new Instruction(Opcode.LoadImmediate, 1, 2),
            new Instruction(Opcode.Cmp, 0, 1),
            new Instruction(Opcode.JumpIfNotZero, 6),
            new Instruction(Opcode.LoadImmediate, 2, 99),
            new Instruction(Opcode.Halt),
            new Instruction(Opcode.LoadImmediate, 2, 42),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(2).Should().Be(42);
    }

    [TestMethod]
    public void JumpIfNotZero_ShouldNotJump_WhenZero()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.LoadImmediate, 1, 5),
            new Instruction(Opcode.Cmp, 0, 1),
            new Instruction(Opcode.JumpIfNotZero, 6),
            new Instruction(Opcode.LoadImmediate, 2, 42),
            new Instruction(Opcode.Halt),
            new Instruction(Opcode.LoadImmediate, 2, 99),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(2).Should().Be(42);
    }

    [TestMethod]
    public void Push_ShouldPushValueOntoStack()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.Push, 0),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Stack.Count.Should().Be(1);
    }

    [TestMethod]
    public void Pop_ShouldPopValueFromStack()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.Push, 0),
            new Instruction(Opcode.Pop, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(1).Should().Be(42);
        cpu.GetState().Stack.Count.Should().Be(0);
    }

    [TestMethod]
    public void Call_ShouldPushPCAndJump()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 1),
            new Instruction(Opcode.Call, 4),
            new Instruction(Opcode.Halt), // Adres 2 - tutaj wraca Ret
            new Instruction(Opcode.LoadImmediate, 0, 99),
            new Instruction(Opcode.LoadImmediate, 0, 42), // Adres 4 - cel Call
            new Instruction(Opcode.Ret)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(42);
    }

    [TestMethod]
    public void Ret_ShouldPopPCAndContinue()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 1),
            new Instruction(Opcode.Call, 4),
            new Instruction(Opcode.Halt), // Adres 2 - tutaj wraca Ret
            new Instruction(Opcode.LoadImmediate, 0, 99),
            new Instruction(Opcode.LoadImmediate, 0, 42), // Adres 4 - cel Call
            new Instruction(Opcode.Ret)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(42);
    }

    [TestMethod]
    public void Halt_ShouldStopExecution()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.Halt),
            new Instruction(Opcode.LoadImmediate, 1, 99)
        });

        cpu.Run();

        cpu.GetState().IsHalted.Should().BeTrue();
        cpu.GetState().Registers.GetRegister(0).Should().Be(42);
        cpu.GetState().Registers.GetRegister(1).Should().Be(0); // Nie powinno zostać wykonane
    }

    [TestMethod]
    public void Step_ShouldExecuteSingleInstruction()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.LoadImmediate, 1, 10),
            new Instruction(Opcode.Halt)
        });

        cpu.Step();

        cpu.GetState().Registers.GetRegister(0).Should().Be(5);
        cpu.GetState().Registers.GetRegister(1).Should().Be(0);
        cpu.GetState().ProgramCounter.Should().Be(1);
    }
}
