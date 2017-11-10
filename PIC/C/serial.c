#include <stdio.h>
#include <stdlib.h>
#include <xc.h>

#define _XTAL_FREQ 4000000

#define RAND_MAX = 1

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
    TRISA = 0b00000100;
    INTCONbits.GIE = 1;//habilitar todas las interrupciones
    INTCONbits.PEIE = 1;

    TXSTAbits.SYNC = 0; //Transmisión Asíncrona
    TXSTAbits.TXEN = 1; //Habilitación transmisión
    TXSTAbits.TX9 = 1;
    RCSTAbits.SPEN = 1; //Habilita puerto serie


    TXSTAbits.BRGH = 1;    //Baja velocidad
    SPBRG = 103;            // Baud Rate 9600

    PIE1bits.TXIE = 1;
    int x = 0;
    while(1)
    {
        for(x=65;x<=90;x++)
        {
            TXREG = x ;
            __delay_ms(500);
        }
    }
}
