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

void resetearSalidas (int cantidadSalidas)
{
  for(int i = 0; i < cantidadSalidas; i = i + 1)
  {
      PORTBbits.RB2 = 0;
      PORTBbits.RB1 = 1;
      PORTBbits.RB1 = 0;
  }
  PORTBbits.RB3 = 1;
  PORTBbits.RB3 = 0;
}
void palabraBinariaAlAzar (int cantidadSalidas)
{
  for(int i = 0; i < cantidadSalidas; i = i + 1)
  {
      PORTBbits.RB4 = rand();    //Dato
      PORTBbits.RB3 = 1;        //Clock on
      PORTBbits.RB3 = 0;        //Clock off
  }
  PORTBbits.RB5 = 1;            //Latch on
  PORTBbits.RB5 = 0;           //Latch off
  __delay_ms (40);
  resetearSalidas (cantidadSalidas);
}

void main ()
{
    TRISB = 0b01000001;

    CMCON = 0x07;

    int rand(void);
    int palabaraBinaria = 24;
    resetearSalidas (palabaraBinaria);
    while (1)
    {
        if(PORTAbits.RA2 == 1)  //Pulsador de incio /parada
        {
          palabraBinariaAlAzar (palabaraBinaria);
            __delay_ms (200);
        }
    }
}
