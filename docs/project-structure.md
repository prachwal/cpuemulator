# Project Structure

## Foldery

```text
src/
  CpuEmulator/
  CpuEmulator.App/

tests/
  CpuEmulator.Tests/

docs/
plans/
agents/
feedback/
```

## Zasady

- emulator w osobnym projekcie library
- CLI/UI w osobnym projekcie
- testy w osobnym projekcie
- brak logiki domenowej w UI
- brak zaleznosci testowych w kodzie produkcyjnym

## CI

- build release
- testy automatyczne
- raporty testow
- artifacts
