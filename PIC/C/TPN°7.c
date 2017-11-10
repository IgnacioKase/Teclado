#include <stdio.h>
#include <stdlib.h>
#include <xc.h>

#define _XTAL_FREQ 4000000

#define RAND_MAX = 7

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
    CMCON = 07;
    TRISA=0xFF;
    TRISB=0x00;
    int i=0, inicioParada=0;
    char j[11];
    /*ABCDEFGp*/
        j[0] = 0b11111100;
        j[1] = 0b01100000;
        j[2] = 0b11011010;
        j[3] = 0b11110010;
        j[4] = 0b01100110;
        j[5] = 0b10110110;
        j[6] = 0b10111110;
        j[7] = 0b11100000;
        j[8] = 0b11111110;
        j[9] = 0b11100110;
        j[10]= 0b00000001;
    while(1)
    {
        if(PORTAbits.RA0==1)
        {
            inicioParada=!inicioParada;
        }
        if(inicioParada==1)
        {
            for(i=0;i<11;i++)
            {
                PORTB=j[i];
                __delay_ms(1000);
                if(PORTAbits.RA0==1)
                {
                    break;
                }
            }
        }
    }
}
