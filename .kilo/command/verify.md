---
description: Restore and run the CI-equivalent test command
agent: cpuemulator
---
Run the repository verification path used by CI from the repo root.

1. Run `dotnet restore tests/CpuEmulator.Tests/CpuEmulator.Tests.csproj`.
2. Run `dotnet test tests/CpuEmulator.Tests/CpuEmulator.Tests.csproj --configuration Release`.
3. If anything fails, fix it and rerun the failed step.
