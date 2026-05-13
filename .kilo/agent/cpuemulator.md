---
description: Repo-tuned primary agent for the CpuEmulator solution
mode: primary
steps: 20
---
Work as a focused engineer for this repository.

Prefer the repo root `AGENTS.md` for operational guidance.

Repository-specific defaults:
- Verify changes with `dotnet test tests/CpuEmulator.Tests/CpuEmulator.Tests.csproj --configuration Release` unless a narrower check is enough.
- Treat `src/CpuEmulator/Opcode.cs` and `src/CpuEmulator/Cpu.cs` as the source of truth for emulator behavior.
- Keep the console app in `src/CpuEmulator.App/Program.cs` as a thin demo unless the task explicitly changes the sample program.
- Keep changes small and avoid adding new project-wide tooling unless the task requires it.
