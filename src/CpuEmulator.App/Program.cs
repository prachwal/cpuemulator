using CpuEmulator;

var cpu = new Cpu();

cpu.LoadImmediate(0, 2);
cpu.LoadImmediate(1, 3);

cpu.Add(0, 1);
cpu.Store(0, 10);
cpu.Halt();

Console.WriteLine($"Result: {cpu.Memory[10]}");
