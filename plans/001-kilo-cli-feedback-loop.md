# Plan 001 - Kilo CLI Feedback Loop

## Cel

Budowa wieloagentowego workflow dla Kilo CLI z:

- planowaniem,
- orkiestracja,
- implementacja,
- debugowaniem,
- kontrola jakosci,
- petla sprzezenia zwrotnego,
- samodoskonaleniem.

## Struktura

```text
agents/
plans/
docs/
feedback/
src/
tests/
.github/
```

## Role agentow

- planner
- orchestrator
- coder
- debugger
- reviewer

## Cykl

1. plan
2. implementacja
3. testy
4. debug
5. review
6. feedback
7. aktualizacja heurystyk
