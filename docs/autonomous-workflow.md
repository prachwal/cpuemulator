# Autonomiczny workflow projektu

Ten dokument definiuje sposób prowadzenia prac w `cpuemulator`, aby ograniczyć mikrozarządzanie. Agent ma samodzielnie prowadzić zadanie od analizy do gotowego commita lub serii commitów, zatrzymując się tylko przy ryzyku utraty danych, konflikcie merge, publikacji poza repo albo niejednoznacznej decyzji produktowej.

## Zasady nadrzędne

- Pracuj iteracyjnie, małymi i bezpiecznymi przyrostami.
- Najpierw analizuj istniejący kod, testy, dokumentację i aktualny stan repozytorium.
- Nie zgaduj struktury projektu, kontraktów publicznych ani intencji istniejącego kodu.
- Nie wykonuj dużych refaktoryzacji bez planu w `plans/`.
- Każda zmiana zachowania publicznego wymaga aktualizacji dokumentacji.
- Każda zmiana kodu produkcyjnego wymaga testu jednostkowego albo jawnego uzasadnienia braku testu.
- Preferuj rozwiązania edukacyjne, deterministyczne i łatwe do rozszerzenia.

## Polityka językowa

Repozytorium używa dwóch kontekstów językowych:

| Obszar | Język |
| --- | --- |
| Dokumentacja projektowa, plany, feedback, opisy workflow | polski z polskimi znakami |
| Kod C#, identyfikatory, nazwy typów, komentarze XML, komunikaty wyjątków, logi produkcyjne | angielski |
| Commity | angielski, Conventional Commits |
| Nazwy branchy | angielski, kebab-case |
| Komunikacja końcowa agenta do użytkownika | polski |

Istniejące pliki nie muszą być masowo przepisywane. Przy każdej modyfikacji dotkniętego kodu należy jednak dostosować nowe lub zmieniane fragmenty do tej polityki.

## Standardowy flow bezdotykowy

1. `autonomous-implementer` czyta `AGENTS.md`, ten dokument i minimalny kontekst zadania.
2. Sprawdza stan repo przez `git status --short`.
3. Jeśli są cudze lub niezwiązane zmiany, nie nadpisuje ich. Pracuje obok nich albo zatrzymuje się tylko wtedy, gdy blokują zadanie.
4. Ustala branch bazowy. Jeśli użytkownik nie wskazał inaczej, używa aktualnego brancha.
5. Tworzy branch roboczy `auto/<task-id>-<short-topic>` dla nowych prac. Jeśli użytkownik już wskazał branch roboczy, kontynuuje na nim.
6. Planner definiuje cel, kontrakty, ryzyka i testy akceptacyjne.
7. Atomic planner dzieli pracę na kroki możliwe do samodzielnej weryfikacji.
8. Orchestrator deleguje implementację do najwęższego zdolnego agenta.
9. Coder implementuje minimalną zmianę wraz z testami.
10. Debugger uruchamia build/testy i wymusza minimalne poprawki.
11. Reviewer wykonuje kontrolę jakości w stylu findings-first.
12. Docs writer aktualizuje dokumentację, plany lub feedback, jeśli zmieniło się zachowanie albo workflow.
13. Orchestrator zapisuje podsumowanie decyzji i wyników.
14. Autonomous implementer przygotowuje finalne podsumowanie, testy, ryzyka i status merge.

## Wymuszone checkpointy commitów

Agent musi commitować po każdym stabilnym checkpointcie. Nie wolno kumulować wielu niezależnych obszarów w jednym commicie.

| Checkpoint | Warunek commita | Format commita |
| --- | --- | --- |
| Plan lub workflow | Zmieniono `plans/`, `AGENTS.md`, `agents/`, `.kilo/agents/` albo dokumentację procesu | `docs(workflow): ...` albo `chore(agents): ...` |
| Kod produkcyjny | Zmieniono `src/` i odpowiadające testy przechodzą lokalnie | `feat(scope): ...`, `fix(scope): ...` albo `refactor(scope): ...` |
| Testy regresyjne | Dodano testy bez zmiany produkcyjnej | `test(scope): ...` |
| Dokumentacja zachowania | Zmieniono `docs/`, `README.md` albo przykłady po zmianie publicznego zachowania | `docs(scope): ...` |
| CI/CD | Zmieniono `.github/workflows/` lub konfigurację build/test | `ci(scope): ...` |
| Feedback | Zapisano retrospektywę lub decyzję w `feedback/` | `docs(feedback): ...` |

Commit jest dozwolony dopiero po sprawdzeniu `git diff --check` oraz właściwej weryfikacji dla danego checkpointu. Dla zmian kodu minimalna weryfikacja to:

```powershell
dotnet build CpuEmulator.sln --configuration Release
dotnet test tests/CpuEmulator.Tests/CpuEmulator.Tests.csproj --configuration Release
```

Dla zmian wyłącznie dokumentacyjnych agent nie uruchamia pełnego test suite, chyba że dokumentacja opisuje komendy, CI/CD albo zachowanie wykonywalne.

## Granice autonomii

Agent działa samodzielnie bez pytania użytkownika, gdy:

- wybór wynika bezpośrednio z istniejącego planu, testów lub dokumentacji,
- zmiana jest odwracalna przez zwykły commit revert,
- dotyczy małego, lokalnego zakresu,
- nie publikuje danych poza repozytorium.

Agent musi zatrzymać się i poprosić o decyzję, gdy:

- polecenie może usunąć cudzą pracę,
- potrzebny jest `git reset --hard`, force push, usuwanie brancha albo konfliktowy merge,
- zmiana wymaga decyzji produktowej niewynikającej z dokumentacji,
- build albo testy nie przechodzą po dwóch minimalnych próbach naprawy,
- wymagane jest ujawnienie sekretów, tokenów lub danych prywatnych.

## Definition of Done

Zadanie jest zakończone, gdy:

- branch i zakres zmian są jednoznaczne,
- wszystkie zmienione obszary mają osobne, logiczne commity,
- build i testy wymagane dla zakresu przeszły lokalnie lub w CI,
- reviewer nie zgłasza blockerów,
- dokumentacja jest aktualna dla zmienionego zachowania,
- feedback został zapisany, jeśli zadanie ujawniło nową regułę pracy,
- finalne podsumowanie zawiera commity, testy, ryzyka i ewentualne ręczne decyzje.

## Domyślna kolejność przy zmianach kodu

1. Test reprodukujący oczekiwane zachowanie albo regresję.
2. Minimalna implementacja.
3. Lokalny build.
4. Lokalny test suite.
5. Dokumentacja, jeśli zachowanie publiczne się zmieniło.
6. Review.
7. Commit checkpointu.
8. Feedback, jeśli workflow wymaga korekty.
