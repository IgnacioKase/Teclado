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

#define led RB1

int contador = 0;

void interrupt enconder()
{
    if(INTCONbits.INTF == 1)
    {
      INTCONbits.INTF = 0;
      contador = contador + 1;
      if(contador >= 1024)
      {
        contador = 0;
        led = !led;
      }
    }
}

void main ()
{
  TRISB = 0b00000001;   //Entrada 1 - Salida 0

  INTCONbits.GIE = 1;//habilitar todas las interrupciones
  INTCONbits.PEIE = 1;
  INTCONbits.INTE = 1;
  INTCONbits.INTF = 0;//limpiar bandera

  OPTION_REGbits.INTEDG = 1;

  led = 0;

  while(1)
  {

  }
}
