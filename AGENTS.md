# AGENTS.md

Minimalne instrukcje kaskadowe dla agentów pracujących w repozytorium `cpuemulator`.

## Zasada nadrzędna

Agent nie powinien ładować całej dokumentacji naraz. Najpierw czyta ten plik, potem dobiera dodatkowe pliki tylko wtedy, gdy są potrzebne do aktualnego zadania.

Agent ma prowadzić zadania możliwie bezdotykowo. Nie pyta użytkownika o decyzje wynikające z istniejących planów, testów, dokumentacji albo lokalnego kontekstu repozytorium. Zatrzymuje się wyłącznie przy ryzyku utraty danych, konfliktowym merge, publikacji poza repozytorium, dostępie do sekretów albo decyzji produktowej niewynikającej z dokumentacji.

## Pliki ładowane domyślnie

1. `AGENTS.md` - ten plik.
2. `docs/autonomous-workflow.md` - gdy zadanie dotyczy sposobu pracy, autonomii, commitów, branchy, review albo feedbacku.
3. `docs/coding-conventions.md` - gdy zadanie dotyczy kodu C#/.NET.
4. `docs/project-structure.md` - gdy zadanie dotyczy struktury projektów, referencji, CI albo testów.
5. `plans/001-kilo-cli-feedback-loop.md` - gdy zadanie dotyczy pracy agentowej, Kilo CLI, planowania, orkiestracji albo samodoskonalenia.

## Kaskada ról

- Planowanie: `agents/planner.md`
- Orkiestracja: `agents/orchestrator.md`
- Kodowanie: `agents/coder.md`
- Debugowanie: `agents/debugger.md`
- Kontrola jakości: `agents/reviewer.md`

## Domyślny przepływ pracy

1. Planner definiuje cel, ograniczenia, ryzyka, kontrakty publiczne i kryteria akceptacji.
2. Atomic planner dzieli pracę na najmniejsze checkpointy możliwe do samodzielnej weryfikacji.
3. Orchestrator dobiera agentów i pilnuje kolejności prac.
4. Coder implementuje najmniejszy bezpieczny przyrost wraz z testami.
5. Debugger uruchamia build/testy, analizuje błędy i wskazuje minimalne poprawki.
6. Reviewer sprawdza jakość, zgodność z konwencjami, komplet testów i ryzyko merge.
7. Docs writer aktualizuje dokumentację, jeśli zmieniono zachowanie publiczne albo workflow.
8. Orchestrator zapisuje wnioski do pętli sprzężenia zwrotnego, jeśli zadanie ujawniło nową regułę.
9. Autonomous implementer commitował każdy stabilny checkpoint zgodnie z `docs/autonomous-workflow.md`.

## Polityka językowa

- Dokumentacja projektowa, plany, feedback i opisy workflow: język polski z polskimi znakami.
- Kod C#, identyfikatory, nazwy typów, komentarze XML, komunikaty wyjątków i logi produkcyjne: język angielski.
- Commity: język angielski, Conventional Commits.
- Nazwy branchy: język angielski, kebab-case.
- Komunikacja końcowa agenta do użytkownika: język polski.

Nie wykonuj masowego przepisywania istniejących plików wyłącznie w celu zmiany języka. Uspójniaj język w plikach dotykanych przez bieżące zadanie.

## Reguły repozytorium

- Język produkcyjny: C#/.NET 8.
- Testy: MSTest, Moq, FluentAssertions.
- Logowanie: NLog.
- Skrypty systemowe: PowerShell.
- Każda zmiana kodu wymaga testów jednostkowych albo wyraźnego uzasadnienia braku testów.
- Nie mieszać kodu emulatora, CLI, testów i dokumentacji w jednym projekcie.
- Brak logiki domenowej w UI.
- Brak `Console.WriteLine` w kodzie produkcyjnym.
- Preferuj immutable models, jawne modele danych, małe klasy i małe metody.

## Polityka commitów

Agent musi tworzyć osobne commity po stabilnych checkpointach:

- `docs(workflow): ...` albo `chore(agents): ...` po zmianach procesu, agentów, planów lub konfiguracji pracy.
- `feat(scope): ...`, `fix(scope): ...` albo `refactor(scope): ...` po zmianach kodu produkcyjnego i odpowiadających testów.
- `test(scope): ...` po dodaniu testów bez zmiany kodu produkcyjnego.
- `docs(scope): ...` po zmianach dokumentacji zachowania publicznego.
- `ci(scope): ...` po zmianach CI/CD.
- `docs(feedback): ...` po zapisaniu retrospektywy lub decyzji w `feedback/`.

Nie wolno łączyć niezależnych obszarów w jednym commicie. Przed commitem sprawdź `git diff --check` oraz właściwą weryfikację dla zakresu zmiany.

## Minimalna definicja done

Zmiana jest zakończona, gdy:

- kod się buduje, jeśli zmiana dotyka kodu, projektu albo CI,
- testy przechodzą lokalnie lub w CI, jeśli zmiana dotyka kodu wykonywalnego,
- dodano albo zaktualizowano testy,
- brak testów jest jawnie uzasadniony, jeśli zmiana nie dotyka logiki wykonywalnej,
- dokumentacja została zaktualizowana, jeśli zmieniono zachowanie publiczne albo workflow,
- reviewer nie znajduje blokujących problemów,
- każdy stabilny checkpoint ma osobny commit,
- finalne podsumowanie zawiera branch, commity, testy, ryzyka i decyzje wymagające uwagi użytkownika.
