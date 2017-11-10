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

#define pulsador RB0
#define clockManual RB1
#define dato RB2
#define latch RB3

void resetearSalidas (int cantidadSalidas)
{
  for(int i = 0; i < cantidadSalidas; i = i + 1)
  {
      dato = 0;
      clockManual = 1;
      clockManual = 0;
  }
  latch = 1;
  latch = 0;
}
void palabraBinariaAlAzar (int cantidadSalidas)
{
  int rand(void);
  for(int i = 0; i < cantidadSalidas; i = i + 1)
  {
      //dato = rand();   //Dato
      dato = 1; 
      clockManual = 1;        //clockManual on
      clockManual = 0;        //clockManual off
  }
  latch = 1;            //Latch on
  latch = 0;           //Latch off
  __delay_ms (60);
  resetearSalidas(cantidadSalidas);
}

void main ()
{
    TRISB = 0b00000001;
    CMCON = 0x07;
    TRISA = 0b00000100;


    int palabaraBinaria = 48;

    resetearSalidas(palabaraBinaria);
    
    while (1)
    {
        if(pulsador == 1)  //Pulsador de incio /parada
        {
          palabraBinariaAlAzar (palabaraBinaria);
            __delay_ms (100);
        }
    }
}
