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

#define controlLed RB0

unsigned int contador = 0;

void led()
{
    if (RB0 == 1)
    {
        RB0 = 0;
    }
    else
    {
        RB0 = 1;
    }
}

void interrupt leds()
{
    if(INTCONbits.T0IF == 1)
    {
      INTCONbits.T0IF = 0;
      contador++;
      if(contador >= 9)
      {
        contador = 0;
        led();
      }
    }
}

void main ()
{
  TRISB = 0b00000000;

  RB0 = 0;

  INTCONbits.GIE=1;//habilitar todas las interrupciones
  INTCONbits.PEIE=1;
  INTCONbits.T0IE=1;//HABILITA EL TIMER 0
  INTCONbits.T0IF=0;//limpiar bandera

  OPTION_REGbits.T0CS=1;//CONTARDOR O TEMPORIZADOR
  OPTION_REGbits.T0SE=0;//EDGE
  OPTION_REGbits.PSA=0;//PRESCALER A TIMER 0

  OPTION_REGbits.PS0=1;
  OPTION_REGbits.PS1=1;
  OPTION_REGbits.PS2=1;

  while (1)
  {
      __delay_ms (100);
  }
}
