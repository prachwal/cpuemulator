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

        cpu.LoadImmediate(0, 2);
        cpu.LoadImmediate(1, 3);

        cpu.Add(0, 1);

        cpu.Registers[0].Should().Be(5);
    }

    [TestMethod]
    public void Store_ShouldWriteToMemory()
    {
        var cpu = new CpuEmulator.Cpu();

        cpu.LoadImmediate(0, 7);
        cpu.Store(0, 20);

        cpu.Memory[20].Should().Be(7);
    }
}
