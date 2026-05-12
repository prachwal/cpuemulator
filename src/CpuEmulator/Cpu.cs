namespace CpuEmulator;

public class Cpu
{
    public int[] Registers { get; } = new int[4];

    public int[] Memory { get; } = new int[256];

    public bool Halted { get; private set; }

    public void LoadImmediate(int register, int value)
    {
        Registers[register] = value;
    }

    public void Add(int destination, int source)
    {
        Registers[destination] += Registers[source];
    }

    public void Store(int register, int address)
    {
        Memory[address] = Registers[register];
    }

    public void Halt()
    {
        Halted = true;
    }
}
