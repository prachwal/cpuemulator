---
description: Run one MSTest by FullyQualifiedName substring
agent: cpuemulator
---
Run one focused MSTest from the repo root.

Use this filter value: `$ARGUMENTS`

Command:
`dotnet test tests/CpuEmulator.Tests/CpuEmulator.Tests.csproj --filter "FullyQualifiedName~$ARGUMENTS"`
