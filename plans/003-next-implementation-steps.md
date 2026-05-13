# Plan 003 - Kolejne kroki implementacji

## Cel

Celem tego planu jest uporządkowanie dalszej implementacji `cpuemulator` po dodaniu podstawowego CPU, assemblera, instrukcji, testów i workflow agentowego. Plan ma prowadzić projekt w małych, sprawdzalnych checkpointach, bez konieczności ręcznego mikrozarządzania każdym krokiem.

## Założenia

- Branch roboczy: `feature/cpu-emulator-modernization` albo osobny branch `auto/003-next-implementation-steps`, jeśli prace będą kontynuowane autonomicznie.
- Kod produkcyjny pozostaje w C#/.NET 8.
- Testy pozostają w MSTest, FluentAssertions i Moq.
- Dokumentacja, plany i feedback pozostają po polsku.
- Kod, komentarze XML, komunikaty wyjątków, logi i commity pozostają po angielsku.
- Każdy stabilny checkpoint musi mieć osobny commit zgodnie z `docs/autonomous-workflow.md`.

## Priorytety implementacyjne

### 1. Domknięcie kontraktu assemblera

Zakres:

- Ujednolicić składnię operandów pamięciowych w dokumentacji, przykładach i parserze.
- Jednoznacznie opisać różnicę między adresowaniem bezpośrednim i pośrednim.
- Zweryfikować obsługę etykiet w skokach i wywołaniach podprogramów.
- Usunąć albo wykorzystać martwe API związane z nierozwiązanymi etykietami.
- Zapewnić czytelne komunikaty błędów assemblera po angielsku.

Kryteria akceptacji:

- `docs/opcodes.md` i przykłady używają tej samej składni.
- Testy pokrywają poprawne i błędne etykiety.
- Testy pokrywają adresowanie bezpośrednie oraz pośrednie, jeśli oba tryby są wspierane.
- Brak martwego stanu w `LabelTable` albo istnieje test pokazujący jego użycie.

Sugerowany commit:

```text
fix(assembler): align memory operand handling
```

### 2. Integracyjne testy programów asemblerowych

Zakres:

- Dodać testy uruchamiające kompletne programy `.asm` przez assembler i CPU.
- Oprzeć testy na programach z `examples/`.
- Sprawdzać końcowy stan rejestrów, pamięci, flag i zatrzymania CPU.
- Dodać regresję dla programu z pętlą i podprogramem.

Kryteria akceptacji:

- Istnieje test dla prostego programu z `examples/program.asm`.
- Istnieje test dla bardziej złożonego programu z `examples/complex-program.asm`.
- Testy nie zależą od kolejności wykonywania innych testów.
- Testy są deterministyczne.

Sugerowany commit:

```text
test(assembler): add end-to-end assembly program tests
```

### 3. Jawny model wykonania CPU

Zakres:

- Udokumentować cykl fetch/decode/execute.
- Upewnić się, że publiczne API CPU nie miesza ładowania programu, wykonywania instrukcji i odczytu stanu.
- Rozdzielić odpowiedzialności między `Cpu`, `CpuExecutor`, `ProgramManager`, `Memory` i `RegisterSet`, jeśli obecne granice są nieczytelne.
- Zachować kompatybilność z istniejącymi testami, chyba że plan jawnie zmieni kontrakt.

Kryteria akceptacji:

- Dokumentacja opisuje cykl wykonania.
- Testy sprawdzają pojedynczy krok wykonania oraz pełne uruchomienie programu.
- Publiczne API ma czytelny kontrakt zatrzymania programu.

Sugerowany commit:

```text
refactor(cpu): clarify execution lifecycle
```

### 4. Walidacja programu i pamięci

Zakres:

- Doprecyzować zachowanie dla pustego programu.
- Doprecyzować zachowanie dla program counter poza zakresem.
- Doprecyzować zachowanie dla adresów pamięci poza zakresem.
- Doprecyzować zachowanie dla nieprawidłowych indeksów rejestrów.
- Zapewnić spójne wyjątki domenowe.

Kryteria akceptacji:

- Każdy przypadek błędny ma osobny test regresyjny.
- Komunikaty wyjątków są po angielsku.
- Wyjątki domenowe są udokumentowane w `docs/api.md` albo właściwym dokumencie technicznym.

Sugerowany commit:

```text
fix(runtime): standardize validation failures
```

### 5. CI/CD i raportowanie jakości

Zakres:

- Potwierdzić, że workflow GitHub Actions uruchamia restore, build release i testy.
- Potwierdzić, że coverage threshold działa w praktyce.
- Ustalić artefakty testów i coverage jako standard.
- Dodać instrukcję lokalnego odtworzenia CI w PowerShell.

Kryteria akceptacji:

- CI przechodzi na branchu roboczym.
- Artefakty testów i coverage są publikowane.
- Dokumentacja opisuje lokalny odpowiednik CI.

Sugerowany commit:

```text
ci(dotnet): verify build and coverage gates
```

### 6. Feedback loop po pierwszym pełnym przebiegu

Zakres:

- Po wykonaniu kroków 1-5 zapisać retrospektywę w `feedback/retrospectives/`.
- Wskazać, które reguły agentowe realnie pomogły, a które wymagały ręcznej interwencji.
- Dodać osobny plan korekt agentów tylko wtedy, gdy wystąpiły powtarzalne problemy.

Kryteria akceptacji:

- Retrospektywa zawiera datę, zakres, komendy weryfikacyjne, ryzyka i rekomendacje.
- Zmiany zasad nie są mieszane z kodem produkcyjnym.

Sugerowany commit:

```text
docs(feedback): record implementation retrospective
```

## Proponowana kolejność prac

1. Najpierw domknąć kontrakt assemblera, bo wpływa na przykłady i testy integracyjne.
2. Następnie dodać pełne testy programów `.asm`.
3. Potem uporządkować model wykonania CPU, jeśli testy pokażą niejasności API.
4. Następnie ustabilizować walidację błędów runtime.
5. Na końcu potwierdzić CI/CD i zapisać feedback.

## Ryzyka

- Składnia `LD/ST` może być niespójna między dokumentacją, parserem i przykładami.
- Zmiany w cyklu wykonania CPU mogą naruszyć kompatybilność istniejących testów.
- Zbyt szeroki refactor CPU może utrudnić review i debugowanie.
- Coverage threshold może failować po dodaniu testów integracyjnych, jeśli nie jest poprawnie skonfigurowany.

## Minimalny pakiet regresji

Przed uznaniem planu za zrealizowany należy mieć testy dla:

- prostego programu arytmetycznego,
- programu z pętlą,
- programu z etykietami,
- programu z `CALL` i `RET`,
- programu używającego stosu,
- programu używającego pamięci,
- błędnej etykiety,
- błędnego rejestru,
- błędnego adresu pamięci,
- pustego programu albo programu bez `HALT`, zależnie od przyjętego kontraktu.
