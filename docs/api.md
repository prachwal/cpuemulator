# Dokumentacja API - CPU Emulator

## Architektura

Emulator CPU zaimplementowany jest zgodnie z wzorcem architektonicznym opartym na separacji odpowiedzialności:

```
┌─────────────────────────────────────────────────────────────┐
│                         ICpu                                │
│  LoadProgram() | Run() | Step() | Reset() | GetState()       │
└─────────────────────┬───────────────────────────────────────┘
                      │ implements
┌─────────────────────▼───────────────────────────────────────┐
│                        Cpu                                  │
│  Kompozycja: CpuState + CpuExecutor + ProgramManager        │
│  Deleguje wykonanie do IInstruction (Strategy)             │
└─────────────────────────────────────────────────────────────┘
        │                │                    │
        ▼                ▼                    ▼
  ┌──────────┐   ┌──────────────┐   ┌────────────────┐
  │ CpuState │   │ CpuExecutor  │   │ ProgramManager │
  │(immutable)│   │(fetch-decode │   │(program list,  │
  │          │   │ -execute)    │   │ PC management) │
  └──────────┘   └──────────────┘   └────────────────┘
        │                │
        ▼                ▼
  ┌──────────┐   ┌──────────────────────────────┐
  │IRegisterSet│  │     IInstruction (Strategy)    │
  │ IMemory   │  │  NopInstruction : IInstruction│
  │(mockable) │  │  AddInstruction : IInstruction│
  └──────────┘  │  ... (22 strategie)             │
                └──────────────────────────────┘
```

## Interfejsy

### `ICpu`

Główny interfejs emulatora CPU. Dostarcza metody do zarządzania programem i jego wykonaniem.

**Metody:**
- `void LoadProgram(IEnumerable<Instruction> instructions)` - ładuje program do pamięci CPU i resetuje stan wykonania
- `void Run()` - wykonuje program do zakończenia (Halt lub koniec programu)
- `void Step()` - wykonuje pojedynczy krok programu (jedną instrukcję)
- `void Reset()` - resetuje stan CPU do stanu początkowego
- `CpuState GetState()` - zwraca aktualny stan CPU

### `IMemory`

Interfejs dla pamięci CPU.

**Metody:**
- `int Read(int address)` - odczytuje wartość z podanego adresu pamięci
- `void Write(int address, int value)` - zapisuje wartość pod podany adres pamięci
- `int Size { get; }` - zwraca rozmiar pamięci w słowach

**Wyjątki:**
- `InvalidOperandException` - rzucane, gdy adres jest poza zakresem pamięci

### `IRegisterSet`

Interfejs dla zestawu rejestrów CPU.

**Metody:**
- `int GetRegister(int index)` - odczytuje wartość z podanego rejestru
- `void SetRegister(int index, int value)` - zapisuje wartość do podanego rejestru
- `int Count { get; }` - zwraca liczbę rejestrów

**Wyjątki:**
- `InvalidOperandException` - rzucane, gdy indeks jest poza zakresem

### `IRuntime`

Interfejs dla środowiska wykonawczego CPU. Zarządza programem, licznikiem programu, stosem i flagami.

**Właściwości:**
- `IReadOnlyList<Instruction> Program { get; }` - zwraca niezmienialną listę instrukcji programu
- `int ProgramCounter { get; }` - zwraca aktualną wartość licznika programu
- `bool IsHalted { get; }` - zwraca informację, czy program został zatrzymany
- `CpuFlags Flags { get; }` - zwraca aktualne flagi procesora
- `int StackCount { get; }` - zwraca liczbę elementów na stosie

**Metody:**
- `void SetProgramCounter(int value)` - ustawia wartość licznika programu
- `void Halt()` - zatrzymuje wykonanie programu
- `void PushStack(int value)` - wkłada wartość na stos
- `int PopStack()` - ściąga wartość ze stosu

**Wyjątki:**
- `ProgramCounterOutOfRangeException` - rzucane, gdy wartość PC jest poza zakresem
- `StackUnderflowException` - rzucane, gdy stos jest pusty

### `IInstruction`

Interfejs dla strategii wykonania pojedynczej instrukcji CPU.

**Metody:**
- `CpuState Execute(CpuState state, Instruction instruction)` - wykonuje instrukcję na podanym stanie CPU

**Wyjątki:**
- `CpuException` - rzucane, gdy wystąpi błąd podczas wykonania instrukcji

## Klasy

### `Cpu`

Główna klasa emulatora CPU implementująca interfejs `ICpu`.

### `CpuState`

Niemutowalna struktura (`readonly record struct`) reprezentująca stan CPU.

**Właściwości:**
- `IRegisterSet Registers` - zestaw rejestrów
- `IMemory Memory` - pamięć
- `Stack<int> Stack` - stos
- `int ProgramCounter` - licznik programu
- `CpuFlags Flags` - flagi procesora
- `bool IsHalted` - czy program został zatrzymany

**Metody:**
- `CpuState WithRegister(int index, int value)`
- `CpuState WithMemory(int address, int value)`
- `CpuState WithProgramCounter(int programCounter)`
- `CpuState WithFlags(CpuFlags flags)`
- `CpuState WithHalted(bool isHalted)`
- `CpuState WithPushedStack(int value)`
- `(CpuState NewState, int Value) WithPoppedStack()`

### `CpuFlags`

Struktura (`readonly record struct`) reprezentująca flagi procesora.

**Właściwości:**
- `bool ZeroFlag` - flaga zerowa
- `bool CarryFlag` - flaga przeniesienia
- `bool OverflowFlag` - flaga przepełnienia
- `bool SignFlag` - flaga znaku

### `CpuFactory`

Fabryka do tworzenia instancji CPU.

**Metody:**
- `static ICpu Create()`
- `static ICpu CreateWithMemorySize(int memorySize)`
- `static ICpu CreateWithRegisterCount(int registerCount)`

## Hierarchia wyjątków

```
Exception
└── CpuException (abstract)
    ├── InvalidOperandException
    ├── ProgramCounterOutOfRangeException
    └── StackUnderflowException
```

## Przykłady użycia

### Tworzenie CPU

```csharp
ICpu cpu = CpuFactory.Create();
ICpu cpuWithLargeMemory = CpuFactory.CreateWithMemorySize(1024);
ICpu cpuWithMoreRegisters = CpuFactory.CreateWithRegisterCount(8);
```

### Ładowanie programu i uruchamianie

```csharp
var program = new List<Instruction>
{
    new Instruction(Opcode.LoadImmediate, 0, 10),
    new Instruction(Opcode.LoadImmediate, 1, 20),
    new Instruction(Opcode.Add, 0, 1),
    new Instruction(Opcode.Halt)
};

ICpu cpu = CpuFactory.Create();
cpu.LoadProgram(program);
cpu.Run();

var state = cpu.GetState();
Console.WriteLine($
Wynik: {state.Registers.GetRegister(0)});

### Dostęp do stanu

```csharp
var state = cpu.GetState();
int r0 = state.Registers.GetRegister(0);
int memoryValue = state.Memory.Read(10);
bool isZero = state.Flags.ZeroFlag;
```

### Obsługa błędów

```csharp
try
{
    ICpu cpu = CpuFactory.Create();
    cpu.LoadProgram(new List<Instruction> { new Instruction(Opcode.Pop, 0) });
    cpu.Step();
}
catch (CpuException ex) when (ex is StackUnderflowException)
{
    Console.WriteLine("Stack underflow!");
}
```
