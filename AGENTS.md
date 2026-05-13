# AGENTS.md

## Repo shape

- `.NET 8` solution with 3 projects:
  - `src/CpuEmulator/` - core library; instruction execution lives in `Cpu.cs`
  - `src/CpuEmulator.App/` - console demo entrypoint in `Program.cs`
  - `tests/CpuEmulator.Tests/` - MSTest suite for emulator behavior
- Source of truth for supported instructions is `src/CpuEmulator/Opcode.cs`, not `README.md`; the README opcode list is stale.

## Commands that match CI

- Restore the same target CI restores: `dotnet restore tests/CpuEmulator.Tests/CpuEmulator.Tests.csproj`
- Run the same focused verification CI uses: `dotnet test tests/CpuEmulator.Tests/CpuEmulator.Tests.csproj --configuration Release`
- Run one MSTest: `dotnet test tests/CpuEmulator.Tests/CpuEmulator.Tests.csproj --filter "FullyQualifiedName~CpuTests.Add_ShouldSumRegisters"`
- Run the demo app: `dotnet run --project src/CpuEmulator.App/CpuEmulator.App.csproj`

## Code gotchas

- `Cpu.LoadProgram(...)` resets `Program`, `ProgramCounter`, and `Halted`, but does not clear registers, memory, stack, or `ZeroFlag`.
- `Cpu.Step()` increments `ProgramCounter` before executing; jump targets are absolute instruction indexes.
- `Push`/`Pop` and `Call`/`Ret` share the same private stack.
- `ZeroFlag` is updated by `Add`, `Sub`, and `Cmp`; `Inc` and `Dec` do not update it.

## Testing conventions

- Tests use MSTest attributes plus FluentAssertions.
- Existing execution tests live in `tests/CpuEmulator.Tests/CpuTests.cs`; keep new behavior tests in the same style unless the suite grows enough to justify splitting.
