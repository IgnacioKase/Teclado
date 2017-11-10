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

void main ()
{
    TRISB = 0b111100001;  //  1 - Entrada / 0 - Salida
    PORTBbits.RB3 = 1;
    while (1)
    {
        if((PORTBbits.RB0 == 1) && (PORTBbits.RB2 == 0))  //SET
        {
            PORTBbits.RB2 = 1;
            PORTBbits.RB3 = 0;
            __delay_ms (500);
        }
        if((PORTBbits.RB1 == 1) && (PORTBbits.RB2 == 1))  //RESET
        {
            PORTBbits.RB2 = 0;
            PORTBbits.RB3 = 1;
            __delay_ms (500);
        }    
    }
}