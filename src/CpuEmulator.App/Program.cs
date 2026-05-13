using CpuEmulator;
using CpuEmulator.Abstractions;

ICpu cpu = CpuFactory.Create();

cpu.LoadProgram(new List<Instruction>
{
    new Instruction(Opcode.LoadImmediate, 0, 2),
    new Instruction(Opcode.LoadImmediate, 1, 3),
    new Instruction(Opcode.Add, 0, 1),
    new Instruction(Opcode.Store, 0, 10),
    new Instruction(Opcode.Halt)
});

cpu.Run();

Console.WriteLine($"Result: {cpu.GetState().Memory.Read(10)}");
