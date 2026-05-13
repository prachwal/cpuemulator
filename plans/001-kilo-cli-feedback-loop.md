# Plan 001 - Kilo CLI Feedback Loop

## Cel

Skonfigurowac w repozytorium `cpuemulator` autonomiczny workflow Kilo CLI, w ktorym polecenie typu "zaimplementuj plan" uruchamia caly cykl:

- utworzenie osobnego brancha roboczego,
- doprecyzowanie planu i podzial pracy,
- delegowanie zadan do wyspecjalizowanych subagentow,
- implementacje w malych, bezpiecznych przyrostach,
- lokalny build i testy,
- debugowanie regresji,
- review kodu, testow i dokumentacji,
- zapis wnioskow do petli feedbacku,
- przygotowanie opisu zmian,
- merge po spelnieniu definicji done.

Workflow ma byc mozliwie autonomiczny, ale nie powinien ukrywac decyzji ryzykownych: operacje destrukcyjne, konfliktowy merge, usuwanie pracy uzytkownika i publikacja poza lokalne repo wymagaja jawnej zgody.

## Podstawa z dokumentacji Kilo Code

Kilo Code CLI wspiera agentow i subagentow konfigurowanych w `kilo.jsonc` albo jako pliki Markdown w `.kilo/agents/`. Kluczowe pola:

- `description` - opis uzywany przez orchestratora do wyboru subagenta,
- `mode` - `primary`, `subagent` albo `all`,
- `model` - model w formacie `provider/model`,
- `prompt` - instrukcja agenta,
- `permission` - uprawnienia do narzedzi takich jak `read`, `edit`, `bash`, `task`, `websearch`,
- `steps` - limit iteracji agenta.

Subagenci dzialaja w izolowanych sesjach, a wynik wraca do agenta nadrzednego. Orchestrator moze ograniczyc delegowanie przez `permission.task`, np. dopuszczajac tylko znane role projektu. Project-level config ma pierwszenstwo nad konfiguracja globalna, wiec docelowa konfiguracja repo powinna mieszkac w projekcie, nie tylko w `~/.config/kilo`.

Zrodla sprawdzone 2026-05-13:

- https://kilo.ai/docs/customize/custom-subagents
- https://kilo.ai/docs/customize/custom-modes
- https://kilo.ai/docs/code-with-ai/platforms/cli

## Preferencje modeli

Modele wolne z `ollama-cloud` sa dozwolone glownie dla planowania wysokopoziomowego i lekkiej naprawy typow. Kodowanie, debugowanie, orkiestracja i review powinny preferowac Mistral.

```jsonc
{
  "model": "mistral/mistral-medium-2604",
  "agent": {
    "plan": {
      "model": "ollama-cloud/deepseek-v4-pro"
    },
    "orchestrator": {
      "model": "mistral/mistral-medium-2604"
    },
    "code": {
      "model": "mistral/labs-devstral-small-2512"
    },
    "debug": {
      "model": "mistral/mistral-medium-2604"
    },
    "atomic-planner": {
      "model": "ollama-cloud/deepseek-v4-pro"
    },
    "type-fixer": {
      "model": "ollama-cloud/deepseek-v4-flash"
    },
    "ask": {
      "model": "openai/gpt-5.4"
    }
  }
}
```

## Docelowa struktura

```text
AGENTS.md
.kilo/
  agents/
    autonomous-implementer.md
    repo-orchestrator.md
    planner.md
    atomic-planner.md
    coder.md
    debugger.md
    reviewer.md
    docs-writer.md
    type-fixer.md
agents/
  planner.md
  orchestrator.md
  coder.md
  debugger.md
  reviewer.md
docs/
plans/
feedback/
  decisions/
  retrospectives/
src/
tests/
.github/
```

`agents/*.md` zostaja jako dokumentacja rol repozytorium. `.kilo/agents/*.md` ma byc wykonywalna konfiguracja Kilo CLI.

## Role agentow

### `autonomous-implementer`

Primary agent uruchamiany przez uzytkownika.

- Model: `mistral/mistral-medium-2604`.
- Cel: wykonac plan end-to-end.
- Obowiazki: sprawdzic stan repo, utworzyc branch, uruchomic orchestratora, pilnowac definicji done, przygotowac merge.
- Uprawnienia: `read`, `grep`, `glob`, `edit`, `bash`, `task`, `todowrite`, `todoread`.
- Ograniczenia: nie robi `git reset --hard`, nie usuwa cudzych zmian, nie mergeuje przy konfliktach bez zgody.

### `repo-orchestrator`

Subagent koordynujacy prace.

- Model: `mistral/mistral-medium-2604`.
- Cel: rozbic plan na zadania, dobrac subagentow i scalic wyniki.
- Deleguje tylko do: `planner`, `atomic-planner`, `coder`, `debugger`, `reviewer`, `docs-writer`, `type-fixer`, `explore`.
- Odpowiada za spojnosc miedzy kodem emulatora, CLI, testami i dokumentacja.

### `planner`

Subagent planistyczny.

- Model: `ollama-cloud/deepseek-v4-pro`.
- Cel: doprecyzowac zakres, ryzyka, kontrakty publiczne, testy i kolejnosc prac.
- Nie edytuje kodu produkcyjnego.

### `atomic-planner`

Subagent do rozbijania wiekszych planow na male przyrosty.

- Model: `ollama-cloud/deepseek-v4-pro`.
- Cel: przeksztalcic plan w liste atomowych zadan z kryteriami akceptacji.
- Nie edytuje kodu produkcyjnego.

### `coder`

Subagent implementacyjny.

- Model: `mistral/labs-devstral-small-2512`.
- Cel: wprowadzac najmniejsze bezpieczne zmiany w `src/` i odpowiadajacych testach.
- Wymaga testow jednostkowych dla kazdej zmiany kodu albo jawnego uzasadnienia braku testow.

### `debugger`

Subagent diagnostyczny.

- Model: `mistral/mistral-medium-2604`.
- Cel: uruchamiac build/testy, analizowac bledy, wskazywac minimalne poprawki.
- Preferowane komendy: `dotnet build`, `dotnet test`, komendy CI repo.

### `reviewer`

Subagent kontroli jakosci.

- Model: `mistral/mistral-medium-2604`.
- Cel: review w stylu findings-first, z naciskiem na regresje, brak testow, naruszenia struktury i ryzyko merge.
- Domyslnie read-only; moze prosic o poprawki, ale nie powinien sam przepisywac implementacji.

### `docs-writer`

Subagent dokumentacyjny.

- Model: `mistral/mistral-medium-2604`.
- Cel: aktualizowac `docs/`, `plans/`, `feedback/` i opisy zmian, gdy zmienia sie zachowanie publiczne albo workflow.
- Edycja ograniczona do Markdown.

### `type-fixer`

Subagent pomocniczy.

- Model: `ollama-cloud/deepseek-v4-flash`.
- Cel: drobne poprawki typow i kompilacji po implementacji.
- Nie projektuje architektury i nie zmienia zachowania bez akceptacji orchestratora.

## Szkic konfiguracji `.kilo/agents`

Preferowana forma to pliki Markdown z YAML frontmatter, poniewaz prompt pozostaje czytelny i wersjonowany z repo.

Przyklad `autonomous-implementer.md`:

```markdown
---
description: Executes repository plans end-to-end on a separate branch, delegates to project subagents, runs tests, documents results, and prepares merge.
mode: primary
model: mistral/mistral-medium-2604
steps: 80
permission:
  read: allow
  grep: allow
  glob: allow
  edit: allow
  todowrite: allow
  todoread: allow
  task:
    "*": deny
    repo-orchestrator: allow
    planner: allow
    atomic-planner: allow
    coder: allow
    debugger: allow
    reviewer: allow
    docs-writer: allow
    type-fixer: allow
    explore: allow
  bash:
    "git status*": allow
    "git branch*": allow
    "git checkout -b *": allow
    "git switch -c *": allow
    "git switch *": ask
    "git merge *": ask
    "git diff*": allow
    "git log*": allow
    "dotnet build*": allow
    "dotnet test*": allow
    "*": ask
---

You are the autonomous implementation lead for cpuemulator.

Follow AGENTS.md first. Implement requested plans end-to-end on a dedicated branch.
Delegate specialist work to project subagents. Keep changes small, tested, reviewed, and documented.
Never overwrite user work. Stop for approval before destructive commands, conflicted merges, or publishing outside the local repository.
```

Przyklad `repo-orchestrator.md`:

```markdown
---
description: Splits cpuemulator implementation plans into delegated subagent tasks and integrates their results.
mode: subagent
model: mistral/mistral-medium-2604
steps: 60
permission:
  read: allow
  grep: allow
  glob: allow
  edit: allow
  todowrite: allow
  todoread: allow
  task:
    "*": deny
    planner: allow
    atomic-planner: allow
    coder: allow
    debugger: allow
    reviewer: allow
    docs-writer: allow
    type-fixer: allow
    explore: allow
  bash:
    "git status*": allow
    "git diff*": allow
    "dotnet build*": allow
    "dotnet test*": allow
    "*": ask
---

You coordinate autonomous implementation in cpuemulator.

Read only the context needed for the current task. Split work into atomic increments, delegate to the narrowest capable subagent, integrate results, and enforce the repository definition of done.
```

## Autonomiczny cykl wykonania

1. `autonomous-implementer` czyta `AGENTS.md`, wskazany plan i minimalne dodatkowe dokumenty.
2. Sprawdza `git status --short`. Jesli sa cudze zmiany, uwzglednia je albo prosi o decyzje, gdy blokuje to prace.
3. Tworzy branch `auto/<plan-id>-<short-topic>` z aktualnego brancha.
4. Zleca `planner` doprecyzowanie celu, kontraktow i ryzyk.
5. Zleca `atomic-planner` rozbicie planu na male zadania.
6. `repo-orchestrator` przydziela zadania do `coder`, `docs-writer`, `type-fixer` i `debugger`.
7. `coder` implementuje zmiany i testy zgodnie z `docs/coding-conventions.md`.
8. `debugger` uruchamia build oraz testy i zwraca minimalne poprawki.
9. `reviewer` wykonuje review findings-first.
10. `repo-orchestrator` dopilnowuje poprawek po review.
11. `docs-writer` aktualizuje dokumentacje i `feedback/`, jesli zmieniono zachowanie albo workflow.
12. `autonomous-implementer` przygotowuje podsumowanie, liste testow i opis merge.
13. Merge do brancha bazowego jest wykonywany tylko gdy build i testy przechodza, review nie ma blockerow i nie ma konfliktow.

## Polityka branchy i merge

- Nazwa brancha: `auto/<numer-planu>-<temat>`, np. `auto/001-kilo-feedback-loop`.
- Branch bazowy: branch aktywny w momencie startu, chyba ze uzytkownik wskaze inny.
- Commit: maly, opisowy, po przejsciu lokalnych testow.
- Merge lokalny: dozwolony po definicji done.
- Merge konfliktowy: wymaga decyzji uzytkownika.
- Push lub PR: tylko po jawnym poleceniu uzytkownika.

## Minimalna definicja done

Zmiana jest zakonczona, gdy:

- kod sie buduje,
- testy przechodza lokalnie albo w CI,
- dodano albo zaktualizowano testy dla zmienionego kodu,
- brak testow jest jawnie uzasadniony, jesli zmiana nie dotyka logiki wykonywalnej,
- dokumentacja zostala zaktualizowana, jesli zmieniono zachowanie publiczne albo workflow,
- reviewer nie znajduje blockerow,
- podsumowanie zawiera branch, zakres zmian, testy, ryzyka i decyzje merge.

## Petla sprzezenia zwrotnego

Po kazdym autonomicznym wykonaniu orchestrator zapisuje krotki wpis w `feedback/retrospectives/`:

- data,
- plan lub zadanie,
- uzyte subagenty,
- co zadzialalo,
- co spowolnilo prace,
- jakie reguly warto dopisac do `AGENTS.md`, `agents/*.md` albo `.kilo/agents/*.md`,
- czy feedback wymaga osobnej zmiany.

Wpisy feedbacku nie powinny automatycznie zmieniac zasad repo. Zmiana zasad wymaga oddzielnego commita albo jawnego zatwierdzenia w ramach aktualnego zadania.

## Zadania wdrozeniowe

1. Utworzyc `.kilo/agents/` z agentami opisanymi w tym planie.
2. Zsynchronizowac dokumentacyjne role w `agents/*.md` z wykonywalnymi promptami `.kilo/agents/*.md`.
3. Dodac szablon retrospektywy do `feedback/retrospectives/template.md`.
4. Dodac checklisty review/debug do `agents/reviewer.md` i `agents/debugger.md`.
5. Przetestowac konfiguracje przez `kilo agent list`.
6. Wykonac probny suchy przebieg na malym zadaniu dokumentacyjnym.
7. Dopiero po udanym przebiegu uzyc trybu autonomicznego do zmian w `src/`.
