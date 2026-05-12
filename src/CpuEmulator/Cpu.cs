namespace CpuEmulator;

public class Cpu
{
    private readonly Stack<int> _stack = new();

    public int[] Registers { get; } = new int[4];

    public int[] Memory { get; } = new int[256];

    public int ProgramCounter { get; private set; }

    public bool ZeroFlag { get; private set; }

    public bool Halted { get; private set; }

    public List<Instruction> Program { get; } = new();

    public void LoadProgram(IEnumerable<Instruction> instructions)
    {
        Program.Clear();
        Program.AddRange(instructions);
        ProgramCounter = 0;
        Halted = false;
    }

    public void Run()
    {
        while (!Halted && ProgramCounter < Program.Count)
        {
            Step();
        }
    }

    public void Step()
    {
        var instruction = Program[ProgramCounter];
        ProgramCounter++;

        switch (instruction.Opcode)
        {
            case Opcode.Nop:
                break;

            case Opcode.LoadImmediate:
                Registers[instruction.Operand1] = instruction.Operand2;
                break;

            case Opcode.Mov:
                Registers[instruction.Operand1] = Registers[instruction.Operand2];
                break;

            case Opcode.Load:
                Registers[instruction.Operand1] = Memory[instruction.Operand2];
                break;

            case Opcode.Store:
                Memory[instruction.Operand2] = Registers[instruction.Operand1];
                break;

            case Opcode.Add:
                Registers[instruction.Operand1] += Registers[instruction.Operand2];
                ZeroFlag = Registers[instruction.Operand1] == 0;
                break;

            case Opcode.Sub:
                Registers[instruction.Operand1] -= Registers[instruction.Operand2];
                ZeroFlag = Registers[instruction.Operand1] == 0;
                break;

            case Opcode.Inc:
                Registers[instruction.Operand1]++;
                break;

            case Opcode.Dec:
                Registers[instruction.Operand1]--;
                break;

            case Opcode.Cmp:
                ZeroFlag = Registers[instruction.Operand1] == Registers[instruction.Operand2];
                break;

            case Opcode.Jump:
                ProgramCounter = instruction.Operand1;
                break;

            case Opcode.JumpIfZero:
                if (ZeroFlag)
                {
                    ProgramCounter = instruction.Operand1;
                }
                break;

            case Opcode.JumpIfNotZero:
                if (!ZeroFlag)
                {
                    ProgramCounter = instruction.Operand1;
                }
                break;

            case Opcode.Push:
                _stack.Push(Registers[instruction.Operand1]);
                break;

            case Opcode.Pop:
                Registers[instruction.Operand1] = _stack.Pop();
                break;

            case Opcode.Call:
                _stack.Push(ProgramCounter);
                ProgramCounter = instruction.Operand1;
                break;

            case Opcode.Ret:
                ProgramCounter = _stack.Pop();
                break;

            case Opcode.Halt:
                Halted = true;
                break;
        }
    }
}
