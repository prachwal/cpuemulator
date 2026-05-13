using CpuEmulator;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpuEmulator.Tests;

[TestClass]
public class FlagTests
{
    // ========== ADD Tests ==========

    [TestMethod]
    public void Add_ShouldSetZeroFlag_WhenResultIsZero()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.LoadImmediate, 1, -5),
            new Instruction(Opcode.Add, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.ZeroFlag.Should().BeTrue("5 + (-5) = 0");
        state.Flags.SignFlag.Should().BeFalse();
        // Note: In unsigned arithmetic, 5 + 0xFFFFFFFB = 0x100000004 > uint.MaxValue, so CarryFlag is true
        // This is correct behavior for unsigned overflow
        state.Flags.CarryFlag.Should().BeTrue("Unsigned overflow: 5 + 0xFFFFFFFB = 0x100000004");
        state.Flags.OverflowFlag.Should().BeFalse();
    }

    [TestMethod]
    public void Add_ShouldSetSignFlag_WhenResultIsNegative()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, -10),
            new Instruction(Opcode.LoadImmediate, 1, 5),
            new Instruction(Opcode.Add, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.SignFlag.Should().BeTrue("-10 + 5 = -5 < 0");
        state.Flags.ZeroFlag.Should().BeFalse();
    }

    [TestMethod]
    public void Add_ShouldSetCarryFlag_WhenUnsignedOverflow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, -1), // -1 = 0xFFFFFFFF (uint.MaxValue)
            new Instruction(Opcode.LoadImmediate, 1, 1),
            new Instruction(Opcode.Add, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.CarryFlag.Should().BeTrue("0xFFFFFFFF + 1 causes unsigned overflow");
    }

    [TestMethod]
    public void Add_ShouldSetOverflowFlag_WhenSignedOverflow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, int.MaxValue),
            new Instruction(Opcode.LoadImmediate, 1, 1),
            new Instruction(Opcode.Add, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.OverflowFlag.Should().BeTrue("int.MaxValue + 1 causes signed overflow");
        // CarryFlag is not set for signed overflow (only OverflowFlag)
    }

    [TestMethod]
    public void Add_ShouldSetAllFlags_Correctly()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 10),
            new Instruction(Opcode.LoadImmediate, 1, 20),
            new Instruction(Opcode.Add, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.ZeroFlag.Should().BeFalse("10 + 20 = 30 != 0");
        state.Flags.SignFlag.Should().BeFalse("30 > 0");
        state.Flags.CarryFlag.Should().BeFalse("No unsigned overflow");
        state.Flags.OverflowFlag.Should().BeFalse("No signed overflow");
    }

    // ========== SUB Tests ==========

    [TestMethod]
    public void Sub_ShouldSetZeroFlag_WhenResultIsZero()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 10),
            new Instruction(Opcode.LoadImmediate, 1, 10),
            new Instruction(Opcode.Sub, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.ZeroFlag.Should().BeTrue("10 - 10 = 0");
    }

    [TestMethod]
    public void Sub_ShouldSetSignFlag_WhenResultIsNegative()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.LoadImmediate, 1, 10),
            new Instruction(Opcode.Sub, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.SignFlag.Should().BeTrue("5 - 10 = -5 < 0");
    }

    [TestMethod]
    public void Sub_ShouldSetCarryFlag_WhenBorrow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 0),
            new Instruction(Opcode.LoadImmediate, 1, 1),
            new Instruction(Opcode.Sub, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.CarryFlag.Should().BeTrue("0 - 1 causes borrow (unsigned underflow)");
    }

    [TestMethod]
    public void Sub_ShouldSetOverflowFlag_WhenSignedOverflow()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, int.MinValue),
            new Instruction(Opcode.LoadImmediate, 1, 1),
            new Instruction(Opcode.Sub, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.OverflowFlag.Should().BeTrue("int.MinValue - 1 causes signed overflow");
    }

    // ========== INC Tests ==========

    [TestMethod]
    public void Inc_ShouldSetZeroFlag_WhenResultIsZero()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, -1),
            new Instruction(Opcode.Inc, 0),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.ZeroFlag.Should().BeTrue("-1 + 1 = 0");
    }

    [TestMethod]
    public void Inc_ShouldSetSignFlag_WhenResultIsNegative()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, -2),
            new Instruction(Opcode.Inc, 0),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.SignFlag.Should().BeTrue("-2 + 1 = -1 < 0");
    }

    [TestMethod]
    public void Inc_ShouldSetOverflowFlag_AtMaxValue()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, int.MaxValue),
            new Instruction(Opcode.Inc, 0),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.OverflowFlag.Should().BeTrue("int.MaxValue + 1 causes overflow");
        state.Flags.CarryFlag.Should().BeTrue();
    }

    // ========== DEC Tests ==========

    [TestMethod]
    public void Dec_ShouldSetZeroFlag_WhenResultIsZero()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 1),
            new Instruction(Opcode.Dec, 0),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.ZeroFlag.Should().BeTrue("1 - 1 = 0");
    }

    [TestMethod]
    public void Dec_ShouldSetSignFlag_WhenResultIsNegative()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 0),
            new Instruction(Opcode.Dec, 0),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.SignFlag.Should().BeTrue("0 - 1 = -1 < 0");
    }

    [TestMethod]
    public void Dec_ShouldSetOverflowFlag_AtMinValue()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, int.MinValue),
            new Instruction(Opcode.Dec, 0),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.OverflowFlag.Should().BeTrue("int.MinValue - 1 causes overflow");
        // CarryFlag is set when value == 0, but int.MinValue != 0
        state.Flags.CarryFlag.Should().BeFalse();
    }

    // ========== CMP Tests ==========

    [TestMethod]
    public void Cmp_ShouldSetZeroFlag_WhenEqual()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.LoadImmediate, 1, 42),
            new Instruction(Opcode.Cmp, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.ZeroFlag.Should().BeTrue("42 == 42");
        state.Flags.SignFlag.Should().BeFalse();
        state.Flags.CarryFlag.Should().BeFalse();
    }

    [TestMethod]
    public void Cmp_ShouldSetSignFlag_WhenLessThan()
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

        var state = cpu.GetState();
        state.Flags.SignFlag.Should().BeTrue("5 < 10");
        state.Flags.ZeroFlag.Should().BeFalse();
        state.Flags.CarryFlag.Should().BeTrue("5 < 10 (unsigned)");
    }

    [TestMethod]
    public void Cmp_ShouldClearSignFlag_WhenGreaterThan()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 10),
            new Instruction(Opcode.LoadImmediate, 1, 5),
            new Instruction(Opcode.Cmp, 0, 1),
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        var state = cpu.GetState();
        state.Flags.SignFlag.Should().BeFalse("10 > 5");
        state.Flags.ZeroFlag.Should().BeFalse();
        state.Flags.CarryFlag.Should().BeFalse();
    }
}
