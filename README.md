# CPU Emulator

Minimalna edukacyjna implementacja emulatora procesora w C#/.NET.

## Cel

Projekt pokazuje podstawy działania prostego CPU:

- rejestry ogólnego przeznaczenia,
- licznik programu `PC`,
- flagi procesora (`ZF`, `CF`, `OF`, `SF`),
- pamięć RAM,
- cykl fetch-decode-execute,
- wzorzec Strategy dla instrukcji,
- niemutowalny stan CPU.

## Zestaw instrukcji

| Instrukcja | Opis |
|---|---|
| `Nop` | brak operacji |
| `LoadImmediate` | ładuje stałą do rejestru |
| `Mov` | kopiuje wartość między rejestrami |
| `Load` | ładuje wartość z pamięci do rejestru |
| `Store` | zapisuje wartość z rejestru do pamięci |
| `Add` | dodaje dwa rejestry |
| `Sub` | odejmuje dwa rejestry |
| `Inc` | inkrementuje rejestr |
| `Dec` | dekrementuje rejestr |
| `Cmp` | porównuje dwa rejestry |
| `Jump` | skok bezwarunkowy |
| `JumpIfZero` | skok, gdy `ZF = true` |
| `JumpIfNotZero` | skok, gdy `ZF = false` |
| `Push` | wkłada wartość na stos |
| `Pop` | ściąga wartość ze stosu |
| `Call` | wywołuje podprogram |
| `Ret` | powraca z podprogramu |
| `Halt` | zatrzymuje CPU |

## Dokumentacja

- [Dokumentacja API](docs/api.md) - Opis interfejsów, klas i hierarchii wyjątków
- [Lista instrukcji](docs/opcodes.md) - Kompletna dokumentacja wszystkich 22 opcodów

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
  │(mockable) │  │  ... (22 strategie)             │
  └──────────┘  └──────────────────────────────┘
```

## Uruchomienie

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/CpuEmulator.App/CpuEmulator.App.csproj
```

## Struktura

```text
src/
  CpuEmulator/
    Abstractions/     # Interfejsy: ICpu, IMemory, IRegisterSet, IRuntime, IInstruction
    Execution/        # Strategie instrukcji i InstructionSet
      Instructions/   # 22 klasy instrukcji
    Exceptions/       # Hierarchia wyjątków CPU
    Model/           # CpuState, CpuFlags, AddressingMode, Instruction
    Runtime/         # Memory, RegisterSet, ProgramManager, CpuExecutor
    Cpu.cs           # Główna klasa CPU
    CpuFactory.cs    # Fabryka CPU
  CpuEmulator.App/   # Aplikacja demonstracyjna
tests/
  CpuEmulator.Tests/ # Testy jednostkowe
docs/
  api.md            # Dokumentacja API
  opcodes.md        # Dokumentacja opcodów
```

## Przykłady użycia

### Tworzenie CPU i uruchamianie programu

```csharp
using CpuEmulator;
using CpuEmulator.Abstractions;

// Tworzenie CPU
ICpu cpu = CpuFactory.Create();

// Tworzenie programu: R0 = 10, R1 = 20, R0 = R0 + R1, Halt
var program = new List<Instruction>
{
    new Instruction(Opcode.LoadImmediate, 0, 10),
    new Instruction(Opcode.LoadImmediate, 1, 20),
    new Instruction(Opcode.Add, 0, 1),
    new Instruction(Opcode.Halt)
};

// Ładowanie i uruchamianie
cpu.LoadProgram(program);
cpu.Run();

// Pobieranie wyniku
var state = cpu.GetState();
Console.WriteLine($"Wynik: {state.Registers.GetRegister(0)}");
```

### Wykonanie pojedynczego kroku

```csharp
ICpu cpu = CpuFactory.Create();
cpu.LoadProgram(new List<Instruction> { new Instruction(Opcode.LoadImmediate, 0, 42) });

// Wykonanie jednego kroku
cpu.Step();

// Sprawdzenie stanu
var state = cpu.GetState();
Console.WriteLine($"R0 = {state.Registers.GetRegister(0)}");
```

### Obsługa błędów

```csharp
try
{
    ICpu cpu = CpuFactory.Create();
    cpu.LoadProgram(new List<Instruction> { new Instruction(Opcode.Pop, 0) });
    cpu.Step(); // Próba ściągnięcia ze pustego stosu
}
catch (CpuException ex) when (ex is StackUnderflowException)
{
    Console.WriteLine("Błąd: próba ściągnięcia ze pustego stosu!");
}
```

## Licencja

MIT
