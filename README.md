# CPU Emulator

Minimalna edukacyjna implementacja emulatora procesora w C#/.NET.

## Cel

Projekt pokazuje podstawy działania prostego CPU:

- rejestry ogólnego przeznaczenia,
- licznik programu `PC`,
- flagę zera `ZF`,
- pamięć RAM,
- cykl fetch-decode-execute,
- prosty zestaw instrukcji.

## Zestaw instrukcji

| Instrukcja | Opis |
|---|---|
| `Nop` | brak operacji |
| `LoadImmediate` | ładuje stałą do rejestru |
| `Load` | ładuje wartość z pamięci do rejestru |
| `Store` | zapisuje wartość z rejestru do pamięci |
| `Add` | dodaje dwa rejestry |
| `Subtract` | odejmuje dwa rejestry |
| `Jump` | skok bezwarunkowy |
| `JumpIfZero` | skok, gdy `ZF = true` |
| `Halt` | zatrzymuje CPU |

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
  CpuEmulator.App/
tests/
  CpuEmulator.Tests/
```

## Przykład programu

Program demonstracyjny dodaje `2 + 3`, zapisuje wynik do pamięci pod adresem `10` i zatrzymuje procesor.

## Licencja

MIT
