; Complex example program assembled by CpuEmulator.Assembler
; Computes the sum 1 + 2 + 3 + 4 + 5.
;
; Expected final state:
; - memory[20] = 15
; - memory[21] = 15
; - R0 = 15
; - R1 = 6
; - R2 = 6
; - R3 = 15
;
; Covered instructions:
; - LDI, MOV, ADD, INC, CMP
; - JMP, JZ, CALL, RET
; - PUSH, POP
; - ST, LD
; - HALT

start:
    LDI R0, 0      ; accumulator
    LDI R1, 1      ; current value
    LDI R2, 6      ; exclusive upper bound

loop:
    ADD R0, R1
    INC R1
    CMP R1, R2
    JZ done
    JMP loop

done:
    CALL persist
    HALT

persist:
    PUSH R0
    ST R0, 20
    LD R3, 20
    ST R3, 21
    POP R0
    RET
