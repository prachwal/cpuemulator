# AGENTS.md

Minimalne instrukcje kaskadowe dla agentow pracujacych w repozytorium `cpuemulator`.

## Zasada nadrzedna

Agent nie powinien ladowac calej dokumentacji naraz. Najpierw czyta ten plik, potem dobiera dodatkowe pliki tylko wtedy, gdy sa potrzebne do aktualnego zadania.

## Pliki ladowane domyslnie

1. `AGENTS.md` - ten plik.
2. `docs/coding-conventions.md` - gdy zadanie dotyczy kodu C#/.NET.
3. `docs/project-structure.md` - gdy zadanie dotyczy struktury projektow, referencji, CI albo testow.
4. `plans/001-kilo-cli-feedback-loop.md` - gdy zadanie dotyczy pracy agentowej, Kilo CLI, planowania, orkiestracji albo samodoskonalenia.

## Kaskada rol

- Planowanie: `agents/planner.md`
- Orkiestracja: `agents/orchestrator.md`
- Kodowanie: `agents/coder.md`
- Debugowanie: `agents/debugger.md`
- Kontrola jakosci: `agents/reviewer.md`

## Domyslny przeplyw pracy

1. Planner definiuje cel, ograniczenia, ryzyka i plan.
2. Orchestrator dzieli prace na kroki i dobiera agentow.
3. Coder implementuje najmniejszy bezpieczny przyrost.
4. Debugger uruchamia testy, analizuje bledy i wskazuje poprawki.
5. Reviewer sprawdza jakosc, zgodnosc z konwencjami i komplet testow.
6. Orchestrator zapisuje wnioski do petli sprzezenia zwrotnego.

## Reguly repozytorium

- Jezyk produkcyjny: C#/.NET.
- Testy: MSTest, Moq, FluentAssertions.
- Logowanie: NLog.
- Skrypty systemowe: PowerShell.
- Kazda zmiana kodu wymaga testow jednostkowych albo wyraznego uzasadnienia braku testow.
- Nie mieszac kodu emulatora, CLI, testow i dokumentacji w jednym projekcie.

## Minimalna definicja done

Zmiana jest zakonczona, gdy:

- kod sie buduje,
- testy przechodza lokalnie lub w CI,
- dodano albo zaktualizowano testy,
- dokumentacja zostala zaktualizowana, jesli zmieniono zachowanie publiczne,
- reviewer nie znajduje blokujacych problemow.
