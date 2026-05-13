; Example program assembled by CpuEmulator.Assembler
; Computes R0 = 10 + 20 and stores the result at memory address 10.

start:
    LDI R0, 10
    LDI R1, 20
    ADD R0, R1
    ST R0, 10
    HALT
