# Plan Modernizacji CpuEmulator – v2.0

**ID**: `002-cpu-modernization`
**Data**: 2026-05-13
**Status**: Do realizacji
**Szacowany czas całkowity**: 24–38 godzin

---

## Spis treści

1. [Analiza stanu obecnego](#analiza-stanu-obecnego)
2. [Wizja docelowa](#wizja-docelowa)
3. [Faza 1: Refaktoryzacja architektury](#faza-1-refaktoryzacja-architektury)
4. [Faza 2: Poprawa jakości kodu](#faza-2-poprawa-jakości-kodu)
5. [Faza 3: Pełne pokrycie testami](#faza-3-pełne-pokrycie-testami)
6. [Faza 4: Dokumentacja](#faza-4-dokumentacja)
7. [Faza 5: Rozszerzenia specyficzne dla CPU](#faza-5-rozszerzenia-specyficzne-dla-cpu)
8. [Faza 6: CI/CD i metryki](#faza-6-cicd-i-metryki)
9. [Mapa zależności](#mapa-zależności)
10. [Rejestr ryzyk](#rejestr-ryzyk)
11. [Kryteria akceptacji końcowej](#kryteria-akceptacji-końcowej)

---

## Analiza stanu obecnego

### Pliki źródłowe

| Plik | Linie | Problemy |
|---|---|---|
| `src/CpuEmulator/Cpu.cs` | 123 | God Object: stan (rejestry, pamięć, stos, flagi, PC, program, halted) + logika wykonania (20-opcode switch) + zarządzanie programem. Publiczne mutowalne tablice `Registers[]`, `Memory[]`. Brak walidacji operandów. |
| `src/CpuEmulator/Instruction.cs` | 6 | OK – `record` z `Opcode, Operand1, Operand2`. |
| `src/CpuEmulator/Opcode.cs` | 23 | 22 opcodów w `enum`. OK. |
| `tests/CpuEmulator.Tests/CpuTests.cs` | 65 | Tylko 3 testy (`Add`, `Store`, `JumpIfZero`). Pokrycie ~13% opcodów. Brak testów edge-case. |
| `src/CpuEmulator.App/Program.cs` | 16 | Bezpośrednia zależność od konkretnej klasy `Cpu`. Trzeba przepiąć na interfejs. |
| `.github/workflows/dotnet.yml` | 46 | Działa, ale brak coverlet/coverage. |

### Dług techniczny

1. **Brak interfejsów** – kod jest nietestowalny w izolacji, nie można użyć mocków.
2. **Brak walidacji** – `Registers[instruction.Operand1]` akceptuje dowolny indeks (potencjalny `IndexOutOfRangeException`).
3. **Brak obsługi błędów** – `_stack.Pop()` na pustym stosie rzuca surowy `InvalidOperationException`.
4. **Złamana enkapsulacja** – `Registers` i `Memory` są `public int[]` z setterem.
5. **Ubogie testy** – 3 testy na 22 opcodów, zero testów negatywnych.
6. **Niespójny `ProgramCounter`** – `Jump`/`Call` akceptują dowolną wartość PC, bez sprawdzania zakresu `Program.Count`.
7. **Tylko jedna flaga** – `ZeroFlag`; brak `Carry`, `Overflow`, `Sign` dla operacji arytmetycznych.
8. **Brak trybów adresowania** – tylko natychmiastowe i rejestrowe.
9. **Brak komentarzy XML** – zero dokumentacji API.
10. **Brak metryk coverage** w CI.

---

## Wizja docelowa

```
┌─────────────────────────────────────────────────────────────┐
│                         ICpu                                │
│  LoadProgram() | Run() | Step() | Reset()                   │
└─────────────────────┬───────────────────────────────────────┘
                      │ implements
┌─────────────────────▼───────────────────────────────────────┐
│                        Cpu                                  │
│  Kompozycja: CpuState + CpuExecutor + ProgramManager        │
│  Deleguje wykonanie do IInstructionSet (Strategy)           │
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
  │IRegisterSet│  │     IInstructionSet           │
  │ IMemory   │  │  NopInstruction : IInstruction│
  │(mockable) │  │  AddInstruction : IInstruction│
  └──────────┘  │  ... (1 Strategy per opcode)   │
                └──────────────────────────────┘
```

### Docelowa struktura plików w `src/CpuEmulator/`

```
src/CpuEmulator/
├── CpuEmulator.csproj
├── Abstractions/
│   ├── ICpu.cs
│   ├── IMemory.cs
│   ├── IRuntime.cs                  # interfejs wykonawczy
│   └── IRegisterSet.cs
├── Model/
│   ├── Instruction.cs
│   ├── Opcode.cs
│   ├── AddressingMode.cs            # nowy
│   ├── CpuFlags.cs                  # nowy (flags aggregate)
│   └── CpuState.cs                  # nowy (rejestry + pamięć + stos + flagi)
├── Execution/
│   ├── IInstruction.cs              # Strategy interface
│   ├── InstructionSet.cs            # rejestr strategii
│   └── Instructions/
│       ├── NopInstruction.cs
│       ├── AddInstruction.cs
│       ├── SubInstruction.cs
│       ├── JumpInstruction.cs
│       ├── ... (22 plików)
│       └── HaltInstruction.cs
├── Runtime/
│   ├── CpuExecutor.cs
│   ├── Memory.cs
│   ├── RegisterSet.cs
│   └── ProgramManager.cs
├── Exceptions/
│   ├── CpuException.cs
│   ├── StackUnderflowException.cs
│   ├── InvalidOperandException.cs
│   └── ProgramCounterOutOfRangeException.cs
├── Cpu.cs                          # zrefaktoryzowana fasada
└── CpuFactory.cs                   # opcjonalnie, jeśli potrzebna DI
```

---

## Faza 1: Refaktoryzacja architektury

**Cel**: Eliminacja God Object – podział `Cpu` na spójne komponenty, wprowadzenie interfejsów i wzorca Strategy.
**Priorytet**: Wysoki
**Czas**: 8–10 godzin

### Zadania

#### 1.1 Utworzenie interfejsów abstrakcyjnych
**Szacowany czas**: 1.5 h | **Subagent**: `coder` | **Może być równoległe**: tak

- [ ] `ICpu.cs` – `void LoadProgram(IEnumerable<Instruction>)`, `void Run()`, `void Step()`, `void Reset()`, `CpuState GetState()`
- [ ] `IMemory.cs` – `int Read(int address)`, `void Write(int address, int value)`, `int Size { get; }`
- [ ] `IRegisterSet.cs` – `int GetRegister(int index)`, `void SetRegister(int index, int value)`, `int Count { get; }`
- [ ] `IRuntime.cs` – `IReadOnlyList<Instruction> Program { get; }`, `int ProgramCounter { get; }`, `void SetProgramCounter(int value)`, `void Halt()`, `bool IsHalted { get; }`, `CpuFlags Flags { get; }`, `void PushStack(int value)`, `int PopStack()`, `int StackCount { get; }`

**Kryterium**: Pliki interfejsów istnieją, kod się kompiluje (`dotnet build`).

#### 1.2 Implementacja `CpuState` (holder stanu)
**Szacowany czas**: 1 h | **Subagent**: `coder` | **Zależność**: 1.1

- [ ] `CpuState.cs` – rekord/klasa immutable: rejestry (`RegisterSet`), pamięć (`Memory`), stos (`Stack<int>` wewnętrzny), flagi (`CpuFlags` – tymczasowo tylko `ZeroFlag`), `ProgramCounter`, `IsHalted`
- [ ] Metody: `WithRegister(int index, int value)` → zwraca nowy `CpuState`, `WithMemory(int addr, int val)`, `WithPC(int)`, `WithFlags(CpuFlags)`, `WithHalted(bool)`

**Kryterium**: `CpuState` jest immutable, każda mutacja zwraca nowy stan.

#### 1.3 Implementacja `CpuFlags`
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Zależność**: brak

- [ ] `CpuFlags.cs` – `readonly record struct` z polem `bool ZeroFlag`. Na tym etapie tylko `ZeroFlag`. W fazie 5 rozszerzone o `Carry`, `Overflow`, `Sign`.

**Kryterium**: Struktura jest `readonly`, tworzona przez `new CpuFlags(zeroFlag: ...)`.

#### 1.4 Implementacja `Memory : IMemory`
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Zależność**: 1.1

- [ ] `Memory.cs` – wewnętrzna tablica `int[256]`, metody `Read(int)`, `Write(int, int)`, `Size`.
- [ ] Walidacja adresu `0..Size-1`, rzucanie `ArgumentOutOfRangeException`.

**Kryterium**: Hermetyzacja tablicy, dostęp tylko przez metody.

#### 1.5 Implementacja `RegisterSet : IRegisterSet`
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Zależność**: 1.1

- [ ] `RegisterSet.cs` – wewnętrzna tablica `int[4]`, metody `GetRegister`, `SetRegister`, `Count`.
- [ ] Walidacja indeksu `0..Count-1`.

**Kryterium**: Hermetyzacja tablicy, dostęp tylko przez metody.

#### 1.6 Implementacja `ProgramManager`
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Zależność**: 1.1

- [ ] `ProgramManager.cs` – zarządza `List<Instruction>`, `ProgramCounter`, `IsHalted`.
- [ ] `LoadProgram(...)`, `Fetch()` → `Instruction?` (null gdy PC poza zakresem), `Advance()`, `Jump(int)`, `Halt()`.
- [ ] Walidacja `PC` przy `Jump` (sprawdzanie czy adres jest w zakresie `0..Program.Count`).

**Kryterium**: Separacja logiki zarządzania programem od wykonania.

#### 1.7 Implementacja wzorca Strategy – `IInstruction`
**Szacowany czas**: 1 h | **Subagent**: `coder` | **Zależność**: 1.2, 1.3, 1.5, 1.6

- [ ] `IInstruction.cs` – interfejs `CpuState Execute(CpuState state, Instruction instruction)`.
- [ ] `InstructionSet.cs` – słownik `Dictionary<Opcode, IInstruction>` + metoda `IInstruction Resolve(Opcode)`.

**Kryterium**: Każdy opcode mapowany na osobną strategię.

#### 1.8 Implementacja strategii dla 22 opcodów
**Szacowany czas**: 3 h | **Subagent**: `coder` | **Zależność**: 1.7

- [ ] `Instructions/NopInstruction.cs` – `state`
- [ ] `Instructions/LoadImmediateInstruction.cs` – `state.WithRegister(op1, op2)`
- [ ] `Instructions/MovInstruction.cs` – `state.WithRegister(op1, state.Registers.GetRegister(op2))`
- [ ] `Instructions/LoadInstruction.cs` – `state.WithRegister(op1, state.Memory.Read(op2))`
- [ ] `Instructions/StoreInstruction.cs` – `state.WithMemory(op2, state.Registers.GetRegister(op1))`
- [ ] `Instructions/AddInstruction.cs` – suma + ustawienie `ZeroFlag`
- [ ] `Instructions/SubInstruction.cs` – różnica + `ZeroFlag`
- [ ] `Instructions/IncInstruction.cs` – inkrementacja
- [ ] `Instructions/DecInstruction.cs` – dekrementacja
- [ ] `Instructions/CmpInstruction.cs` – `ZeroFlag` = (op1 == op2)
- [ ] `Instructions/JumpInstruction.cs` – `state.WithPC(op1)`
- [ ] `Instructions/JumpIfZeroInstruction.cs` – skok warunkowy przy `ZeroFlag`
- [ ] `Instructions/JumpIfNotZeroInstruction.cs` – skok przy `!ZeroFlag`
- [ ] `Instructions/PushInstruction.cs` – push na stos
- [ ] `Instructions/PopInstruction.cs` – pop ze stosu
- [ ] `Instructions/CallInstruction.cs` – push PC, jump do procedury
- [ ] `Instructions/RetInstruction.cs` – pop PC
- [ ] `Instructions/HaltInstruction.cs` – `state.WithHalted(true)`

**Kryterium**: Każda strategia jest testowalna w izolacji (czysta funkcja: `CpuState → CpuState`).

#### 1.9 Implementacja `CpuExecutor`
**Szacowany czas**: 1 h | **Subagent**: `coder` | **Zależność**: 1.6, 1.7, 1.8

- [ ] `CpuExecutor.cs` – `void ExecuteCycle(CpuState state, ProgramManager pm, InstructionSet iset)`.
- [ ] Pętla fetch-decode-execute: `Fetch() → Resolve(opcode).Execute(state, instr) → Advance()`.
- [ ] Obsługa `Halt` – przerwanie pętli.

**Kryterium**: Logika wykonania oddzielona od stanu.

#### 1.10 Zrefaktoryzowanie klasy `Cpu` (fasada)
**Szacowany czas**: 1 h | **Subagent**: `coder` | **Zależność**: 1.2–1.9

- [ ] `Cpu.cs` implementuje `ICpu`, wewnętrznie komponuje `CpuState`, `ProgramManager`, `CpuExecutor`, `InstructionSet`.
- [ ] Zachowanie wstecznie kompatybilne – `LoadProgram`, `Run`, `Step` działają jak przed refaktoryzacją.
- [ ] `CpuFactory.cs` – statyczna metoda `Create()` do tworzenia domyślnie skonfigurowanego CPU.

**Kryterium**: Wszystkie 3 istniejące testy przechodzą bez zmian.

#### 1.11 Aktualizacja `CpuEmulator.App/Program.cs`
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Zależność**: 1.10

- [ ] Przełączenie z `new Cpu()` na `ICpu` (np. `CpuFactory.Create()`).
- [ ] Dostęp do pamięci przez `cpu.GetState().Memory.Read(10)` zamiast `cpu.Memory[10]`.

**Kryterium**: `dotnet run --project src/CpuEmulator.App` wypisuje `Result: 5`.

### Kryteria _done_ dla Fazy 1

| Kryterium | Sposób weryfikacji |
|---|---|
| Kod się kompiluje | `dotnet build CpuEmulator.sln --configuration Release` |
| Wszystkie 3 istniejące testy przechodzą | `dotnet test tests/CpuEmulator.Tests/CpuEmulator.Tests.csproj` |
| Nowa architektura jest spójna z interfejsami | `dotnet build` nie pokazuje warningów o brakujących implementacjach |
| `CpuState` jest immutable | Code review – każda mutacja zwraca nowy obiekt |
| `App` działa poprawnie po refaktoryzacji | `dotnet run --project src/CpuEmulator.App` → `Result: 5` |

### Subagenci przypisani do Fazy 1

| Subagent | Odpowiedzialność |
|---|---|
| `coder` | Implementacja wszystkich zadań 1.1–1.11 |
| `reviewer` | Code review zmian architektonicznych po każdej grupie zadań (interfejsy → stan → strategie → fasada) |
| `type-fixer` | Drobne poprawki typów/kompilacji po implementacji |

### Zadania równoległe w Fazie 1

```
1.1 (interfejsy) ──┬── 1.2 (CpuState) ──┬── 1.8 (strategie) ──┬── 1.10 (fasada)
                   │                      │                      │
                   ├── 1.4 (Memory)       │                      │
                   ├── 1.5 (RegisterSet) ─┘                      │
                   └── 1.6 (ProgramManager) ── 1.9 (Executor) ──┤
                                                                 │
                   1.3 (CpuFlags) ───────────────────────────────┘
```

Zadania 1.3, 1.4, 1.5, 1.6 mogą być wykonywane równolegle po ukończeniu 1.1.

---

## Faza 2: Poprawa jakości kodu

**Cel**: Walidacja operandów, dedykowane klasy wyjątków, bezpieczny dostęp do stanu.
**Priorytet**: Wysoki
**Czas**: 4–6 godzin

### Zadania

#### 2.1 Implementacja hierarchii wyjątków
**Szacowany czas**: 1 h | **Subagent**: `coder` | **Może być równoległe**: tak

- [ ] `Exceptions/CpuException.cs` – `abstract class CpuException : Exception { public int? ProgramCounter { get; } }`
- [ ] `Exceptions/StackUnderflowException.cs` – `CpuException`, `"Attempted to pop from an empty stack"`
- [ ] `Exceptions/InvalidOperandException.cs` – `CpuException`, zawiera informację o nieprawidłowym operandzie (rejestr/adres, wartość).
- [ ] `Exceptions/ProgramCounterOutOfRangeException.cs` – `CpuException`, `"ProgramCounter X is out of range [0, Y)"`.

**Kryterium**: Wyjątki są częścią publicznego API, rzucane w strategicznych miejscach.

#### 2.2 Walidacja operandów w strategiach
**Szacowany czas**: 1.5 h | **Subagent**: `coder` | **Zależność**: 1.8, 2.1

- [ ] `LoadImmediateInstruction` – sprawdza `Operand1 ∈ [0, RegisterCount)`.
- [ ] `MovInstruction` – sprawdza oba operandy.
- [ ] `LoadInstruction` – sprawdza `Operand1` (register) i `Operand2` (adres `∈ [0, MemorySize)`).
- [ ] `StoreInstruction` – sprawdza `Operand1` (register) i `Operand2` (adres).
- [ ] Arytmetyczne (`Add`, `Sub`, `Inc`, `Dec`, `Cmp`) – sprawdzają operand rejestrowy.
- [ ] `PushInstruction` – sprawdza operand rejestrowy.
- [ ] `PopInstruction` – sprawdza operand rejestrowy + czy stos nie jest pusty (rzuca `StackUnderflowException`).
- [ ] `CallInstruction` – sprawdza adres skoku.
- [ ] `RetInstruction` – sprawdza czy stos nie jest pusty.
- [ ] Skoki (`Jump`, `JumpIfZero`, `JumpIfNotZero`) – sprawdzają czy `Operand1 ∈ [0, Program.Count]` (rzuca `ProgramCounterOutOfRangeException`).

**Kryterium**: Każdy nieprawidłowy operand skutkuje wyjątkiem `InvalidOperandException` lub pochodnym.

#### 2.3 Walidacja w `Memory` i `RegisterSet`
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Zależność**: 2.1

- [ ] `Memory.Read/Write` – walidacja adresu, `InvalidOperandException` zamiast surowego `ArgumentOutOfRangeException`.
- [ ] `RegisterSet.Get/SetRegister` – walidacja indeksu, `InvalidOperandException`.

**Kryterium**: Spójny model błędów – wszystkie problemy walidacji zgłaszane przez `CpuException`.

#### 2.4 Uodpornienie `Cpu.Run()` i `Cpu.Step()`
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Zależność**: 2.2, 2.3

- [ ] `Step()` łapie `CpuException` i propaguje z kontekstem (PC, instrukcja).
- [ ] `Run()` propaguje wyjątki – klient może zdecydować czy kontynuować.
- [ ] Domyślne zachowanie `CpuFactory.Create()` – `Step()` rzuca wyjątek przy błędzie; klient może skonfigurować inne zachowanie.

**Kryterium**: Błędy w czasie wykonania nie są ukrywane.

### Kryteria _done_ dla Fazy 2

| Kryterium | Sposób weryfikacji |
|---|---|
| Kod się kompiluje | `dotnet build` |
| Testy z Fazy 1 nadal przechodzą | `dotnet test` |
| Nieprawidłowy operand rejestru rzuca `InvalidOperandException` | Test jednostkowy (dodany w Fazie 3 lub tymczasowo tutaj) |
| Pop na pustym stosie rzuca `StackUnderflowException` | Test jednostkowy |
| Jump poza zakres rzuca `ProgramCounterOutOfRangeException` | Test jednostkowy |

### Subagenci przypisani do Fazy 2

| Subagent | Odpowiedzialność |
|---|---|
| `coder` | Implementacja 2.1–2.4 |
| `debugger` | Weryfikacja, że nowe wyjątki są poprawnie rzucane w scenariuszach brzegowych |
| `reviewer` | Code review pod kątem kompletności walidacji |

---

## Faza 3: Pełne pokrycie testami

**Cel**: 100% pokrycia opcodów testami jednostkowymi + testy edge-case + użycie mocków.
**Priorytet**: Wysoki
**Czas**: 6–8 godzin

### Zadania

#### 3.1 Testy jednostkowe dla każdej strategii instrukcji (22 opcodów)
**Szacowany czas**: 3 h | **Subagent**: `coder` | **Zależność**: 1.8, 2.2

- [ ] `NopTests.cs` – stan niezmieniony
- [ ] `LoadImmediateTests.cs` – ładuje wartość do rejestru
- [ ] `MovTests.cs` – kopiuje między rejestrami
- [ ] `LoadTests.cs` – ładuje z pamięci do rejestru
- [ ] `StoreTests.cs` – zapisuje rejestr do pamięci
- [ ] `AddTests.cs` – dodawanie, ZeroFlag, overflow (przygotowanie pod Fazę 5)
- [ ] `SubTests.cs` – odejmowanie, ZeroFlag
- [ ] `IncTests.cs` – inkrementacja
- [ ] `DecTests.cs` – dekrementacja, wynik ujemny
- [ ] `CmpTests.cs` – porównanie równe, nierówne
- [ ] `JumpTests.cs` – skok zmienia PC
- [ ] `JumpIfZeroTests.cs` – skok gdy ZF, brak skoku gdy !ZF
- [ ] `JumpIfNotZeroTests.cs` – skok gdy !ZF, brak skoku gdy ZF
- [ ] `PushTests.cs` – wartość na stosie
- [ ] `PopTests.cs` – wartość w rejestrze
- [ ] `CallTests.cs` – push PC + jump
- [ ] `RetTests.cs` – pop PC, powrót
- [ ] `HaltTests.cs` – IsHalted = true, Step nie wykonuje kolejnych instrukcji

**Kryterium**: Minimum 1 test na opcode (pozytywna ścieżka). Każdy test testuje strategię w izolacji (tworzy `CpuState`, woła `IInstruction.Execute()`, asercja na zwróconym stanie).

#### 3.2 Testy edge-case i negatywne
**Szacowany czas**: 2 h | **Subagent**: `coder` + `debugger` | **Zależność**: 2.2

- [ ] **Pusty stos** – `PopInstruction` na pustym stosie → `StackUnderflowException`
- [ ] **Pusty stos** – `RetInstruction` na pustym stosie → `StackUnderflowException`
- [ ] **Nieprawidłowy rejestr** – `LoadImmediate(reg: 5, val: 10)` → `InvalidOperandException`
- [ ] **Nieprawidłowy adres** – `Load(reg: 0, addr: 300)` → `InvalidOperandException`
- [ ] **Nieprawidłowy adres** – `Store(reg: 0, addr: -1)` → `InvalidOperandException`
- [ ] **Skok poza program** – `Jump(addr: 999)` gdy program ma 5 instrukcji → `ProgramCounterOutOfRangeException`
- [ ] **Przepełnienie pamięci** – `Store` pod adres `255` (ostatni bajt) → OK
- [ ] **Przepełnienie pamięci** – `Store` pod adres `256` → `InvalidOperandException`
- [ ] **Program pusty** – `Run()` na pustym programie → kończy natychmiast (bez wyjątku)
- [ ] **Podwójny `Halt`** – `Step()` po `Halt` → bez efektu (stan niezmieniony)

**Kryterium**: Każdy edge-case ma dedykowany test.

#### 3.3 Testy integracyjne z użyciem mocków
**Szacowany czas**: 1.5 h | **Subagent**: `coder` | **Zależność**: 1.1–1.11

- [ ] `CpuExecutorTests` – mock `IMemory`, mock `IRegisterSet`, testowanie cyklu fetch-decode-execute.
- [ ] `CpuIntegrationTests` – pełny scenariusz: load → run → asercja stanu.
- [ ] `CpuFactoryTests` – test tworzenia CPU z domyślną konfiguracją.

**Kryterium**: Mocki `Moq` używane dla zależności, testy izolują `CpuExecutor` i `Cpu`.

#### 3.4 Testy regresyjne dla istniejących 3 testów
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Zależność**: 1.11

- [ ] Upewnienie się, że istniejące testy (`Add`, `Store`, `JumpIfZero`) przechodzą na nowej architekturze.
- [ ] Ewentualna aktualizacja asercji (np. `cpu.Registers[0]` → `cpu.GetState().Registers.GetRegister(0)`).

**Kryterium**: 3 oryginalne testy przechodzą bez zmian w logice testowej (co najwyżej zmiana API dostępu).

### Kryteria _done_ dla Fazy 3

| Kryterium | Sposób weryfikacji |
|---|---|
| Każdy z 22 opcodów ma ≥1 test jednostkowy | `dotnet test --filter "FullyQualifiedName~InstructionTests"` |
| Wszystkie testy przechodzą | `dotnet test` |
| Pokrycie kodu ≥ 80% | `dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura` (po Fazie 6 – tymczasowo manualne) |
| Testy edge-case pokrywają min. 8 scenariuszy | Przegląd listy testów |
| Mocki `Moq` użyte w ≥3 testach | `grep -r "new Mock<" tests/` |

### Subagenci przypisani do Fazy 3

| Subagent | Odpowiedzialność |
|---|---|
| `coder` | Implementacja wszystkich testów 3.1–3.4 |
| `debugger` | Uruchamianie testów, analiza failures, wskazywanie przyczyn |
| `reviewer` | Review testów – czy testy są czytelne, czy pokrywają deklarowane scenariusze |

### Zadania równoległe w Fazie 3

Wszystkie testy w 3.1 mogą być pisane równolegle (każdy zestaw testów dla jednego opcode'a jest niezależny). 3.2 i 3.3 mogą być pisane po ukończeniu 3.1.

---

## Faza 4: Dokumentacja

**Cel**: Kompletna dokumentacja API, opcodów i użycia.
**Priorytet**: Średni
**Czas**: 4–5 godzin

### Zadania

#### 4.1 Komentarze XML dla wszystkich metod publicznych
**Szacowany czas**: 2 h | **Subagent**: `coder` | **Zależność**: Faza 1 i 2 ukończone

- [ ] `<summary>` dla wszystkich metod w interfejsach (`ICpu`, `IMemory`, `IRegisterSet`, `IRuntime`).
- [ ] `<summary>` dla `IInstruction.Execute()`.
- [ ] `<summary>` dla wszystkich klas wyjątków.
- [ ] `<param>`, `<returns>`, `<exception>` gdzie stosowne.

**Kryterium**: `dotnet build /warnaserror:CS1591` nie zgłasza błędów (wymagane włączenie `<GenerateDocumentationFile>true</GenerateDocumentationFile>` w `.csproj`).

#### 4.2 Utworzenie `docs/api.md`
**Szacowany czas**: 1 h | **Subagent**: `docs-writer` | **Zależność**: 4.1

- [ ] Opis architektury (diagram jak w wizji docelowej).
- [ ] Opis interfejsów: `ICpu`, `IMemory`, `IRegisterSet`, `IInstruction`.
- [ ] Opis `CpuState` i jego niemutowalności.
- [ ] Opis klas wyjątków.
- [ ] Przykład tworzenia CPU: `ICpu cpu = CpuFactory.Create()`.

**Kryterium**: Plik zawiera kompletny opis publicznego API.

#### 4.3 Utworzenie `docs/opcodes.md`
**Szacowany czas**: 1.5 h | **Subagent**: `docs-writer` | **Może być równoległe**: tak (niezależne od 4.2)

- [ ] Tabela wszystkich 22 opcodów: nazwa, mnemonik, opis, operandy, efekt, ustawiane flagi, przykład.
- [ ] Format:
  ```
  | Opcode | Mnemonik | Operand1 | Operand2 | Opis | Flagi | Przykład |
  |--------|----------|----------|----------|------|-------|----------|
  | Nop    | NOP      | –        | –        | Brak operacji | – | NOP |
  | LoadImmediate | LDI | Reg      | Imm      | Ładuje stałą do rejestru | – | LDI R0, 42 |
  | ...    | ...      | ...      | ...      | ...  | ...   | ...      |
  ```

**Kryterium**: Każdy z 22 opcodów ma wiersz w tabeli z kompletnym opisem.

#### 4.4 Aktualizacja `README.md`
**Szacowany czas**: 0.5 h | **Subagent**: `docs-writer` | **Zależność**: 4.2, 4.3

- [ ] Aktualizacja tabeli instrukcji (obecnie tylko 9 wpisów, brakuje 13).
- [ ] Odnośniki do `docs/api.md` i `docs/opcodes.md`.
- [ ] Zaktualizowany przykład kodu z użyciem `ICpu` / `CpuFactory`.
- [ ] Sekcja "Architektura" z diagramem tekstowym.

**Kryterium**: `README.md` jest spójny z nowym API.

### Kryteria _done_ dla Fazy 4

| Kryterium | Sposób weryfikacji |
|---|---|
| Brak warningów CS1591 | `dotnet build /p:GenerateDocumentationFile=true /warnaserror:CS1591` |
| `docs/api.md` opisuje wszystkie interfejsy | Manualny przegląd |
| `docs/opcodes.md` zawiera wszystkie 22 opcodów | Manualny przegląd / `grep -c "|.*|.*|" docs/opcodes.md` |
| `README.md` ma odnośniki do dokumentacji API i opcodów | Manualny przegląd |

### Subagenci przypisani do Fazy 4

| Subagent | Odpowiedzialność |
|---|---|
| `coder` | 4.1 – komentarze XML w kodzie |
| `docs-writer` | 4.2–4.4 – dokumentacja Markdown |
| `reviewer` | Review dokumentacji pod kątem kompletności i poprawności |

---

## Faza 5: Rozszerzenia specyficzne dla CPU

**Cel**: Dodanie flag procesora (Carry, Overflow, Sign) oraz trybów adresowania.
**Priorytet**: Średni
**Czas**: 4–6 godzin

### Zadania

#### 5.1 Rozszerzenie `CpuFlags`
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Zależność**: 1.3

- [ ] Dodanie do `CpuFlags`: `bool CarryFlag`, `bool OverflowFlag`, `bool SignFlag`.
- [ ] Domyślne `false` dla wszystkich.

**Kryterium**: `CpuFlags` zawiera 4 flagi, nadal `readonly record struct`.

#### 5.2 Aktualizacja strategii arytmetycznych o flagi
**Szacowany czas**: 1.5 h | **Subagent**: `coder` | **Zależność**: 5.1

- [ ] `AddInstruction` – oprócz `ZeroFlag`, ustawia:
  - `CarryFlag` = (wynik przekracza `int.MaxValue` lub `int.MinValue` – overflow unsigned)
  - `OverflowFlag` = (przekroczenie zakresu signed int: `(a>0 && b>0 && wynik<0) || (a<0 && b<0 && wynik>0)`
  - `SignFlag` = (`wynik < 0`)
- [ ] `SubInstruction` – j.w. dla odejmowania.
- [ ] `IncInstruction` – aktualizacja flag.
- [ ] `DecInstruction` – aktualizacja flag.
- [ ] `CmpInstruction` – `SignFlag` = (`a < b` w sensie signed), `ZeroFlag` jak dotychczas.

**Kryterium**: Wszystkie operacje arytmetyczne ustawiają komplet flag.

#### 5.3 Dodanie `AddressingMode` do `Instruction`
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Zależność**: brak (zmiana istniejącego rekordu)

- [ ] `AddressingMode.cs` – `enum AddressingMode { Immediate, Direct, Indirect, Relative }`.
- [ ] Rozszerzenie `Instruction` o pole `AddressingMode Mode = AddressingMode.Immediate`.
- [ ] Backward compatibility: istniejący kod tworzący `Instruction(opcode, op1, op2)` działa bez zmian (domyślny `Immediate`).

**Kryterium**: `Instruction` ma pole `Mode`, istniejące testy nie wymagają zmian.

#### 5.4 Implementacja trybów adresowania w strategiach
**Szacowany czas**: 1.5 h | **Subagent**: `coder` | **Zależność**: 5.3

- [ ] `LoadInstruction`:
  - `Immediate` / `Direct`: `Memory.Read(op2)` (obecne zachowanie)
  - `Indirect`: `Memory.Read(Memory.Read(op2))`
  - `Relative`: `Memory.Read(op2 + state.ProgramCounter)`
- [ ] `StoreInstruction` – analogicznie.
- [ ] `JumpInstruction`, `JumpIfZeroInstruction`, `JumpIfNotZeroInstruction`, `CallInstruction`:
  - `Immediate`: `op1` (obecne zachowanie)
  - `Indirect`: `Memory.Read(op1)` (adres skoku pobrany z pamięci)
  - `Relative`: `state.ProgramCounter + op1`

**Kryterium**: Tryby `Indirect` i `Relative` działają poprawnie, przetestowane.

#### 5.5 Testy dla nowych flag i trybów adresowania
**Szacowany czas**: 1 h | **Subagent**: `coder` | **Zależność**: 5.2, 5.4

- [ ] Testy `CarryFlag`, `OverflowFlag`, `SignFlag` dla `Add`, `Sub`, `Inc`, `Dec`, `Cmp`.
- [ ] Testy trybów adresowania: `Indirect`, `Relative` dla `Load`, `Store`, `Jump`.

**Kryterium**: ≥2 testy na każdą nową funkcjonalność.

### Kryteria _done_ dla Fazy 5

| Kryterium | Sposób weryfikacji |
|---|---|
| `CpuFlags` zawiera 4 flagi | Przegląd kodu |
| Arytmetyka ustawia wszystkie flagi | Testy jednostkowe |
| Tryby adresowania `Indirect` i `Relative` działają | Testy jednostkowe |
| Wszystkie testy przechodzą | `dotnet test` |

### Subagenci przypisani do Fazy 5

| Subagent | Odpowiedzialność |
|---|---|
| `coder` | Implementacja 5.1–5.5 |
| `debugger` | Weryfikacja poprawności flag (szczególnie overflow detection) |
| `reviewer` | Code review pod kątem poprawności arytmetyki flag |

---

## Faza 6: CI/CD i metryki

**Cel**: Automatyczne mierzenie pokrycia kodu, raportowanie w CI.
**Priorytet**: Średni
**Czas**: 2–3 godziny

### Zadania

#### 6.1 Dodanie `coverlet` do projektu testowego
**Szacowany czas**: 0.5 h | **Subagent**: `coder` | **Może być równoległe**: tak

- [ ] Dodanie `coverlet.collector` NuGet do `CpuEmulator.Tests.csproj`:
  ```xml
  <PackageReference Include="coverlet.collector" Version="6.0.2">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
  ```
- [ ] Dodanie `<CollectCoverage>true</CollectCoverage>` i `<CoverletOutputFormat>cobertura</CoverletOutputFormat>` do `.csproj`.
- [ ] Weryfikacja lokalna: `dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/coverage`.

**Kryterium**: `dotnet test` generuje plik `coverage.cobertura.xml`.

#### 6.2 Aktualizacja workflow CI
**Szacowany czas**: 1 h | **Subagent**: `coder` | **Zależność**: 6.1

- [ ] Modyfikacja `.github/workflows/dotnet.yml`:
  - Krok `Test` z flagą coverlet: `dotnet test ... /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/coverage`
  - Krok `Publish coverage` z `dorny/test-reporter@v1` z `path: TestResults/coverage.cobertura.xml` i `reporter: dotnet-cobertura`.
- [ ] Dodanie progu coverage w coverlet (np. `CoverletOutputFormat=cobertura,json` z `Threshold=80`).

**Kryterium**: CI generuje raport pokrycia i failuje build jeśli coverage < 80%.

#### 6.3 Weryfikacja CI na branchu
**Szacowany czas**: 0.5 h | **Subagent**: `debugger` | **Zależność**: 6.2

- [ ] Push brancha i weryfikacja, że workflow przechodzi.
- [ ] Sprawdzenie, że raport coverage jest poprawnie wyświetlany w GitHub Actions.

**Kryterium**: Zielony check w CI, raport coverage widoczny.

### Kryteria _done_ dla Fazy 6

| Kryterium | Sposób weryfikacji |
|---|---|
| `coverlet` generuje raport | `ls tests/CpuEmulator.Tests/TestResults/coverage.cobertura.xml` |
| CI pokazuje coverage | GitHub Actions UI – zakładka "Summary" z coverage |
| Build failuje przy coverage < 80% | Test: tymczasowe obniżenie coverage → spodziewany fail |
| Wszystkie testy przechodzą w CI | Zielony status na PR/branchu |

### Subagenci przypisani do Fazy 6

| Subagent | Odpowiedzialność |
|---|---|
| `coder` | Implementacja 6.1 i 6.2 |
| `debugger` | Weryfikacja 6.3 – czy CI działa poprawnie |

---

## Mapa zależności

```
Faza 1 (Architektura)
├── 1.1 Interfejsy
│   ├── 1.2 CpuState ──────┐
│   ├── 1.3 CpuFlags ──────┤
│   ├── 1.4 Memory ────────┤
│   ├── 1.5 RegisterSet ───┤
│   └── 1.6 ProgramManager─┤
│                          ▼
│                    1.7 IInstruction (Strategy)
│                          │
│                          ▼
│                    1.8 Strategie (22 plików) ──┐
│                                                │
│                    1.9 CpuExecutor ────────────┤
│                                                ▼
│                                          1.10 Cpu (fasada)
│                                                │
│                                          1.11 App update
│
Faza 2 (Jakość kodu) ← zależy od Fazy 1
├── 2.1 Wyjątki
├── 2.2 Walidacja w strategiach
├── 2.3 Walidacja w Memory/RegisterSet
└── 2.4 Uodpornienie Run/Step

Faza 3 (Testy) ← zależy od Fazy 1 i 2
├── 3.1 Testy 22 opcodów
├── 3.2 Edge-cases
├── 3.3 Mocki
└── 3.4 Regresja

Faza 4 (Dokumentacja) ← zależy od Fazy 1 i 2 (może być równoległa z Fazą 3)
├── 4.1 XML comments
├── 4.2 docs/api.md
├── 4.3 docs/opcodes.md
└── 4.4 README.md

Faza 5 (Rozszerzenia CPU) ← zależy od Fazy 1 i 2 (może być równoległa z Fazą 3 i 4)
├── 5.1 Rozszerzenie CpuFlags
├── 5.2 Aktualizacja strategii
├── 5.3 AddressingMode
├── 5.4 Tryby adresowania
└── 5.5 Testy

Faza 6 (CI/CD) ← zależy od Fazy 3 (potrzebuje testów do pomiaru coverage)
├── 6.1 Coverlet
├── 6.2 Workflow CI
└── 6.3 Weryfikacja
```

### Ścieżka krytyczna

```
Faza 1 (10h) → Faza 2 (6h) → Faza 3 (8h) → Faza 6 (3h) = 27h
```

Fazy 4 i 5 mogą być realizowane równolegle z Fazą 3, co skraca całkowity czas.

---

## Rejestr ryzyk

| ID | Ryzyko | Prawdopodobieństwo | Wpływ | Mitygacja |
|----|--------|--------------------|-------|-----------|
| R1 | Zmiana interfejsu `ICpu` może złamać zależności w `CpuEmulator.App` | Średnie | Niski | `CpuFactory.Create()` jako backward-compat bridge. Aktualizacja `App` w podzadaniu 1.11 jako część tej samej fazy. |
| R2 | Strategie w Fazie 1.8 mogą mieć różne interpretacje operandów | Niskie | Wysoki | Centralna walidacja w abstrakcyjnej klasie bazowej. Każda strategia testowana w izolacji. |
| R3 | `StackUnderflowException` przy istniejących testach (które nie testują scenariuszy brzegowych) nie zostanie wykryty | Niskie | Niski | Istniejące testy nie dotykają Pop/Ret na pustym stosie, więc nie ma ryzyka false-positive. Nowe testy w Fazie 3 to pokryją. |
| R4 | Coverlet może nie wspierać .NET 8 w najnowszej wersji | Niskie | Średni | Weryfikacja `coverlet.collector 6.0.2` przed implementacją. W razie problemów – `coverlet.msbuild` jako alternatywa. |
| R5 | Refaktoryzacja może wprowadzić regresje wydajnościowe (więcej alokacji przez immutable state) | Niskie | Niski | `CpuState` jako `record struct` zamiast `record class` eliminuje alokacje na stercie. Profilowanie po Fazie 1. |
| R6 | `ProgramCounterOutOfRangeException` przy `Jump(Program.Count)` – czy skok na "za ostatnią instrukcję" jest legalny? | Średnie | Niski | Definicja: `Jump` na `Program.Count` jest legalny (oznacza koniec wykonania, jak `Halt`). Skok poza zakres: wyjątek. Uzgodnione w dokumentacji. |

---

## Kryteria akceptacji końcowej

Po ukończeniu wszystkich 6 faz:

| # | Kryterium | Weryfikacja |
|---|-----------|-------------|
| 1 | **Build**: `dotnet build CpuEmulator.sln --configuration Release` – 0 błędów, 0 warningów (w tym CS1591) | Automatycznie |
| 2 | **Testy**: Wszystkie testy przechodzą | `dotnet test` → 0 failures |
| 3 | **Pokrycie kodu**: ≥ 80% dla `CpuEmulator.dll` | Coverlet w CI |
| 4 | **Pokrycie opcodów**: 22/22 opcodów ma test jednostkowy | `grep -c "TestMethod" tests/` |
| 5 | **Testy edge-case**: ≥ 10 testów negatywnych | Przegląd listy testów |
| 6 | **Mocki**: ≥ 3 testy z `Moq` | `grep -r "Mock<" tests/` |
| 7 | **Dokumentacja**: `docs/api.md`, `docs/opcodes.md`, `README.md` zaktualizowane | Manualny przegląd |
| 8 | **XML docs**: Brak warningów CS1591 | `dotnet build /warnaserror:CS1591` |
| 9 | **CI**: Zielony workflow z raportem coverage | GitHub Actions UI |
| 10 | **Immutable state**: `CpuState` jest `record` lub ma metody `With*` zwracające nowy stan | Code review |
| 11 | **Nowe flagi**: `CarryFlag`, `OverflowFlag`, `SignFlag` ustawiane przez arytmetykę | Testy |
| 12 | **Tryby adresowania**: `Indirect` i `Relative` działają dla `Load`, `Store`, `Jump` | Testy |
| 13 | **App działa**: `dotnet run --project src/CpuEmulator.App` → `Result: 5` | Automatycznie |

---

## Narzędzia weryfikacji

| Narzędzie | Zastosowanie | Faza |
|-----------|--------------|------|
| `dotnet build` | Kompilacja | Wszystkie |
| `dotnet test` | Uruchamianie testów | 1, 2, 3, 5 |
| `dotnet test /p:CollectCoverage=true` | Pomiar pokrycia | 6 |
| `dotnet build /warnaserror:CS1591` | Weryfikacja XML docs | 4 |
| `dotnet format` | Formatowanie kodu | Wszystkie |
| GitHub Actions | CI/CD pipeline | 6 |
| `grep` | Szybkie wyszukiwanie w kodzie | 3, 4, 6 |

---

## Podsumowanie przypisania subagentów

| Faza | `coder` | `reviewer` | `debugger` | `docs-writer` | `type-fixer` |
|------|---------|------------|------------|---------------|--------------|
| 1 (10h) | ✅ Wszystkie zadania | ✅ Review architektury | – | – | ✅ Drobne poprawki |
| 2 (6h) | ✅ Implementacja | ✅ Review walidacji | ✅ Test edge-case | – | – |
| 3 (8h) | ✅ Testy | ✅ Review testów | ✅ Analiza failures | – | – |
| 4 (5h) | ✅ XML docs | ✅ Review dokumentacji | – | ✅ Markdown docs | – |
| 5 (6h) | ✅ Implementacja | ✅ Review flag/adresacji | ✅ Weryfikacja arytmetyki | – | – |
| 6 (3h) | ✅ CI config | – | ✅ Weryfikacja CI | – | – |
