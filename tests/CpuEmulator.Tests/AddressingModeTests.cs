using CpuEmulator;
using CpuEmulator.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpuEmulator.Tests;

[TestClass]
public class AddressingModeTests
{
    // ========== Indirect Addressing Tests ==========

    [TestMethod]
    public void Load_Indirect_ShouldReadFromPointer()
    {
        var cpu = new Cpu();
        // Ustawiamy pamięć: Memory[10] = 20, Memory[20] = 42
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 20),
            new Instruction(Opcode.Store, 0, 10),  // Memory[10] = 20
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.Store, 0, 20),  // Memory[20] = 42
            new Instruction(Opcode.Load, 0, 10) { Mode = AddressingMode.Indirect }, // R0 = Memory[Memory[10]] = Memory[20] = 42
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(42);
    }

    [TestMethod]
    public void Store_Indirect_ShouldWriteToPointer()
    {
        var cpu = new Cpu();
        // Ustawiamy pamięć: Memory[10] = 20
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 20),
            new Instruction(Opcode.Store, 0, 10),  // Memory[10] = 20
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.Store, 0, 10) { Mode = AddressingMode.Indirect }, // Memory[Memory[10]] = Memory[20] = 42
            new Instruction(Opcode.LoadImmediate, 1, 0),
            new Instruction(Opcode.Load, 1, 20),  // R1 = Memory[20]
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(1).Should().Be(42);
    }

    // ========== Relative Addressing Tests ==========

    [TestMethod]
    public void Jump_Relative_ShouldJumpToOffset()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.Jump, 2) { Mode = AddressingMode.Relative }, // Jump to PC + 2 = 3
            new Instruction(Opcode.LoadImmediate, 0, 99),  // Adres 1 (pominięty)
            new Instruction(Opcode.LoadImmediate, 0, 42),  // Adres 2 (pominięty)
            new Instruction(Opcode.LoadImmediate, 0, 100), // Adres 3 (cel skoku)
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(100);
    }

    [TestMethod]
    public void JumpIfZero_Relative_ShouldJumpToOffset()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.LoadImmediate, 1, 5),
            new Instruction(Opcode.Cmp, 0, 1),          // ZF = true
            new Instruction(Opcode.JumpIfZero, 2) { Mode = AddressingMode.Relative }, // Jump to PC + 2 = 6
            new Instruction(Opcode.LoadImmediate, 0, 99), // Adres 4 (pominięty)
            new Instruction(Opcode.LoadImmediate, 0, 42), // Adres 5 (pominięty)
            new Instruction(Opcode.LoadImmediate, 0, 100),// Adres 6 (cel skoku)
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(100);
    }

    [TestMethod]
    public void JumpIfZero_Relative_ShouldNotJump_WhenNotZero()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.LoadImmediate, 1, 10),
            new Instruction(Opcode.Cmp, 0, 1),         // ZF = false
            new Instruction(Opcode.JumpIfZero, 2) { Mode = AddressingMode.Relative }, // Nie skacze
            new Instruction(Opcode.LoadImmediate, 0, 42), // Adres 4 (wykonany)
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(42);
    }

    [TestMethod]
    public void JumpIfNotZero_Relative_ShouldJumpToOffset()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.LoadImmediate, 1, 10),
            new Instruction(Opcode.Cmp, 0, 1),            // ZF = false
            new Instruction(Opcode.JumpIfNotZero, 2) { Mode = AddressingMode.Relative }, // Jump to PC + 2 = 6
            new Instruction(Opcode.LoadImmediate, 0, 99),  // Adres 4 (pominięty)
            new Instruction(Opcode.LoadImmediate, 0, 42),  // Adres 5 (pominięty)
            new Instruction(Opcode.LoadImmediate, 0, 100), // Adres 6 (cel skoku)
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(100);
    }

    [TestMethod]
    public void JumpIfNotZero_Relative_ShouldNotJump_WhenZero()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 5),
            new Instruction(Opcode.LoadImmediate, 1, 5),
            new Instruction(Opcode.Cmp, 0, 1),           // ZF = true
            new Instruction(Opcode.JumpIfNotZero, 2) { Mode = AddressingMode.Relative }, // Nie skacze
            new Instruction(Opcode.LoadImmediate, 0, 42), // Adres 4 (wykonany)
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(42);
    }

    [TestMethod]
    public void Call_Relative_ShouldCallToOffset()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 1),
            new Instruction(Opcode.Call, 3) { Mode = AddressingMode.Relative }, // Call to PC + 3 = 4
            new Instruction(Opcode.LoadImmediate, 0, 99),  // Adres 2 (pominięty)
            new Instruction(Opcode.Halt),                // Adres 3 (pominięty)
            new Instruction(Opcode.LoadImmediate, 0, 42), // Adres 4 (cel Call)
            new Instruction(Opcode.Ret),                // Adres 5 (powrót do adresu 2)
            new Instruction(Opcode.Halt)                // Adres 2 (kontynuacja po Ret)
        });

        cpu.Run();

        // After Call to PC+3=4, Ret returns to PC+1=2 (where Call was executed)
        // So the next instruction after Ret is at address 2: LoadImmediate R0, 99
        cpu.GetState().Registers.GetRegister(0).Should().Be(99);
    }

    // ========== Direct Addressing Tests (default) ==========

    [TestMethod]
    public void Load_Direct_ShouldReadFromAddress()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.LoadImmediate, 0, 42),
            new Instruction(Opcode.Store, 0, 10),  // Memory[10] = 42
            new Instruction(Opcode.LoadImmediate, 0, 0),
            new Instruction(Opcode.Load, 1, 10),   // R1 = Memory[10] (Direct - domyślny)
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(1).Should().Be(42);
    }

    [TestMethod]
    public void Jump_Direct_ShouldJumpToAddress()
    {
        var cpu = new Cpu();
        cpu.LoadProgram(new[]
        {
            new Instruction(Opcode.Jump, 3),       // Jump to address 3 (Direct - domyślny)
            new Instruction(Opcode.LoadImmediate, 0, 99),  // Adres 1 (pominięty)
            new Instruction(Opcode.LoadImmediate, 0, 42),  // Adres 2 (pominięty)
            new Instruction(Opcode.LoadImmediate, 0, 100), // Adres 3 (cel skoku)
            new Instruction(Opcode.Halt)
        });

        cpu.Run();

        cpu.GetState().Registers.GetRegister(0).Should().Be(100);
    }
}
