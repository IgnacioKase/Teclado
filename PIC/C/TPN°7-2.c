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
      PORTBbits.RB2 = rand();
      PORTBbits.RB1 = 1;
      PORTBbits.RB1 = 0;
  }
  PORTBbits.RB3 = 1;
  PORTBbits.RB3 = 0;
  __delay_ms (40);
  resetearSalidas (cantidadSalidas);
}

void main ()
{
    TRISB = 0b01000001;
    int rand(void);
    int palabaraBinaria = 24;
    resetearSalidas (palabaraBinaria);
    while (1)
    {
        if(PORTBbits.RB0 == 1)
        {
          palabraBinariaAlAzar (palabaraBinaria);
            __delay_ms (200);
        }
        if(PORTBbits.RB6 == 1)
        {
          resetearSalidas (palabaraBinaria);
          __delay_ms (200);
        }
    }
}
