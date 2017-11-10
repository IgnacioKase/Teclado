#include <stdio.h>
#include <stdlib.h>
#include <xc.h>

#define _XTAL_FREQ 4000000

#pragma config FOSC = INTOSCIO
#pragma config WDTE = OFF
#pragma config PWRTE = OFF
#pragma config MCLRE = OFF
#pragma config BOREN = ON
#pragma config LVP = OFF
#pragma config CPD = OFF 
#pragma config CP = OFF

int powKase (int base, int exponente)
{
    int resultado = 1;
    for (int i = 1; i <= exponente; i = i + 1)
    {
        resultado = resultado * base;
    }
    return resultado;
}

void main ()
{
    int buffer = 0;
    TRISA = 0xFF;
    TRISB = 0x00;  //  1 - Entrada / 0 - Salida
    while (1)
    {
        ///if(PORTAbits.RA0 == 1)  //SET
        //{
            for (int i = 1; i <= 7; i = i + 1)
            {
                PORTB = powKase(2,i);
                __delay_ms (100);
            }
            for (int i = 6; i >= 0; i = i - 1)
            {
                PORTB = powKase(2,i);
                __delay_ms (100);
            }
            __delay_ms (500);
        //}
    }
}