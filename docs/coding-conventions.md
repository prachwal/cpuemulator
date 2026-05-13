# Coding Conventions

## C#/.NET

- .NET 8
- nullable enabled
- implicit usings enabled
- jedna klasa na plik
- jawne modele danych
- brak magic numbers
- prefer immutable models
- małe metody
- małe klasy
- SOLID
- brak logiki domenowej w UI
- brak `Console.WriteLine` w kodzie produkcyjnym
- logowanie produkcyjne tylko przez NLog, jeśli logowanie jest potrzebne

## Polityka językowa kodu

Kod wykonywalny i artefakty techniczne muszą być po angielsku:

- nazwy klas, rekordów, struktur, enumów i interfejsów,
- nazwy metod, właściwości, pól, parametrów i zmiennych,
- komentarze XML,
- komunikaty wyjątków,
- komunikaty logów,
- nazwy branchy,
- komunikaty commitów.

Dokumentacja projektowa, plany, retrospektywy i opisy workflow pozostają po polsku z polskimi znakami.

Nie wykonujemy masowego przepisywania istniejących plików wyłącznie dla zmiany języka. Przy każdej zmianie kodu należy jednak utrzymać nowe i zmienione fragmenty w języku angielskim.

## Testy

- MSTest
- FluentAssertions
- Moq tylko przy rzeczywistej granicy zależności
- testy dla logiki domenowej
- regression tests dla bugfixów
- każda instrukcja CPU powinna mieć test
- każda zmiana kodu produkcyjnego wymaga testu albo jawnego uzasadnienia braku testu

## Logowanie

- NLog
- brak `Console.WriteLine` w kodzie produkcyjnym
- komunikaty logów po angielsku
- logi nie mogą zawierać sekretów ani danych prywatnych

## Git

- małe commity
- Conventional Commits po angielsku
- osobny commit po każdym stabilnym checkpointcie
- nie mieszać niezależnych obszarów w jednym commicie
- aktualizacja dokumentacji przy zmianie zachowania publicznego albo workflow

Preferowane typy commitów:

- `feat(scope): ...` dla nowej funkcjonalności
- `fix(scope): ...` dla poprawek błędów
- `refactor(scope): ...` dla zmian struktury bez zmiany zachowania
- `test(scope): ...` dla zmian wyłącznie testowych
- `docs(scope): ...` dla dokumentacji
- `docs(workflow): ...` dla procesu pracy
- `chore(agents): ...` dla konfiguracji agentów
- `ci(scope): ...` dla CI/CD

## Weryfikacja przed commitem

Dla zmian kodu produkcyjnego:

```powershell
dotnet build CpuEmulator.sln --configuration Release
dotnet test tests/CpuEmulator.Tests/CpuEmulator.Tests.csproj --configuration Release
```

Dla zmian wyłącznie dokumentacyjnych wymagane jest sprawdzenie zakresu diffu i spójności z `AGENTS.md` oraz `docs/autonomous-workflow.md`. Pełny test suite nie jest wymagany, chyba że dokumentacja zmienia komendy build/test, CI/CD albo opis zachowania wykonywalnego.
