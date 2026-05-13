# Dokumentacja Opcodów - CPU Emulator

Poniższa tabela zawiera kompletny zestaw 22 instrukcji obsługiwanych przez emulator CPU.

## Tabela opcodów

| Opcode | Mnemonik | Operand1 | Operand2 | Opis | Flagi | Przykład |
|--------|----------|----------|----------|------|-------|----------|
| Nop | NOP | – | – | Brak operacji - nie wykonuje żadnej akcji | – | NOP |
| LoadImmediate | LDI | Indeks rejestru docelowego | Wartość stałej | Ładuje stałą wartość do rejestru | – | LDI R0, 42 |
| Mov | MOV | Indeks rejestru docelowego | Indeks rejestru źródłowego | Kopiuje wartość z rejestru źródłowego do docelowego | – | MOV R0, R1 |
| Load | LD | Indeks rejestru docelowego | Adres pamięci | Ładuje wartość z pamięci do rejestru | – | LD R0, [10] |
| Store | ST | Indeks rejestru źródłowego | Adres pamięci | Zapisuje wartość z rejestru do pamięci | – | ST R0, [10] |
| Add | ADD | Indeks rejestru docelowego | Indeks rejestru źródłowego | Dodaje wartości dwóch rejestrów, wynik w docelowym | ZeroFlag, SignFlag, CarryFlag, OverflowFlag | ADD R0, R1 |
| Sub | SUB | Indeks rejestru docelowego | Indeks rejestru źródłowego | Odejmuje wartość drugiego rejestru od pierwszego, wynik w docelowym | ZeroFlag, SignFlag, CarryFlag, OverflowFlag | SUB R0, R1 |
| Inc | INC | Indeks rejestru | – | Inkrementuje wartość rejestru o 1 | ZeroFlag, SignFlag, OverflowFlag | INC R0 |
| Dec | DEC | Indeks rejestru | – | Dekrementuje wartość rejestru o 1 | ZeroFlag, SignFlag, OverflowFlag | DEC R0 |
| Cmp | CMP | Indeks pierwszego rejestru | Indeks drugiego rejestru | Porównuje wartości dwóch rejestrów i ustawia flagi | ZeroFlag, SignFlag, CarryFlag | CMP R0, R1 |
| Jump | JMP | Adres skoku | – | Wykonuje skok do podanego adresu | – | JMP 10 |
| JumpIfZero | JZ | Adres skoku | – | Wykonuje skok, jeśli flaga ZeroFlag jest ustawiona | – | JZ 10 |
| JumpIfNotZero | JNZ | Adres skoku | – | Wykonuje skok, jeśli flaga ZeroFlag nie jest ustawiona | – | JNZ 10 |
| Push | PUSH | Indeks rejestru | – | Wkłada wartość rejestru na stos | – | PUSH R0 |
| Pop | POP | Indeks rejestru | – | Ściąga wartość ze stosu do rejestru | – | POP R0 |
| Call | CALL | Adres procedury | – | Wywołuje podprogram, wkładając adres powrotu na stos | – | CALL 20 |
| Ret | RET | – | – | Powraca z podprogramu, ściągając adres powrotu ze stosu | – | RET |
| Halt | HALT | – | – | Zatrzymuje wykonanie programu | – | HALT |

## Opis flag

- **ZeroFlag (ZF)**: Ustawiana, gdy wynik operacji wynosi 0
- **SignFlag (SF)**: Ustawiana, gdy wynik operacji jest ujemny (bit znaku = 1)
- **CarryFlag (CF)**: Ustawiana przy przepełnieniu bez znaku (dla operacji ADD/SUB)
- **OverflowFlag (OF)**: Ustawiana przy przepełnieniu ze znakiem (dla operacji ADD/SUB/INC/DEC)

## Tryby adresowania

Emulator obsługuje następujące tryby adresowania (zdefiniowane w `AddressingMode`):
- **Immediate**: Operand zawiera wartość (domyślny)
- **Direct**: Operand zawiera adres pamięci
- **Indirect**: Operand zawiera adres, pod którym znajduje się docelowy adres
- **Relative**: Adres jest obliczany względem licznika programu

## Uwagi

1. Wszystkie operandy rejestrowe muszą być w zakresie [0, RegisterCount)
2. Wszystkie adresy pamięci muszą być w zakresie [0, MemorySize)
3. Skoki muszą wskazywać adresy w zakresie [0, Program.Count)
4. Operacje na pustym stosie rzucają `StackUnderflowException`
5. Nieprawidłowe operandy rzucają `InvalidOperandException`
