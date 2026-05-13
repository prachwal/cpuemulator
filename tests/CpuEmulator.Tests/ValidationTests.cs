using CpuEmulator;
using CpuEmulator.Exceptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpuEmulator.Tests;

[TestClass]
public class ValidationTests
{
    [TestMethod]
    [ExpectedException(typeof(InvalidOperandException))]
    public void LoadImmediate_InvalidRegister_ShouldThrow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 5, 10) // Invalid register index
        });

        cpu.Step();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperandException))]
    public void Mov_InvalidSourceRegister_ShouldThrow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.Mov, 0, 5) // Invalid source register index
        });

        cpu.Step();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperandException))]
    public void Load_InvalidMemoryAddress_ShouldThrow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.Load, 0, 300) // Invalid memory address
        });

        cpu.Step();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperandException))]
    public void Store_InvalidMemoryAddress_ShouldThrow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.Store, 0, -1) // Invalid memory address
        });

        cpu.Step();
    }

    [TestMethod]
    [ExpectedException(typeof(StackUnderflowException))]
    public void Pop_EmptyStack_ShouldThrow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.Pop, 0) // Pop from empty stack
        });

        cpu.Step();
    }

    [TestMethod]
    [ExpectedException(typeof(StackUnderflowException))]
    public void Ret_EmptyStack_ShouldThrow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.Ret) // Ret from empty stack
        });

        cpu.Step();
    }

    [TestMethod]
    [ExpectedException(typeof(ProgramCounterOutOfRangeException))]
    public void Jump_OutOfRange_ShouldThrow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.Jump, 999) // Jump to invalid address
        });

        cpu.Step();
    }

    [TestMethod]
    public void Memory_InvalidReadAddress_ShouldThrow()
    {
        var cpu = new Cpu();
        var state = cpu.GetState();
        
        Action act = () => state.Memory.Read(300);
        act.Should().Throw<InvalidOperandException>();
    }

    [TestMethod]
    public void Memory_InvalidWriteAddress_ShouldThrow()
    {
        var cpu = new Cpu();
        var state = cpu.GetState();
        
        Action act = () => state.Memory.Write(-1, 42);
        act.Should().Throw<InvalidOperandException>();
    }

    [TestMethod]
    public void RegisterSet_InvalidGetIndex_ShouldThrow()
    {
        var cpu = new Cpu();
        var state = cpu.GetState();
        
        Action act = () => state.Registers.GetRegister(10);
        act.Should().Throw<InvalidOperandException>();
    }

    [TestMethod]
    public void RegisterSet_InvalidSetIndex_ShouldThrow()
    {
        var cpu = new Cpu();
        var state = cpu.GetState();
        
        Action act = () => state.Registers.SetRegister(-1, 42);
        act.Should().Throw<InvalidOperandException>();
    }
}
