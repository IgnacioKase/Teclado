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

#define led RA0
#define set RA1
#define parada RA2
#define inicio RA3

unsigned int contador = 0;
int cuentaTemporizador = 0;

void encenderLed()
{
    if (led == 1)
    {
      led = 0;
    }
    else
    {
      led = 1;
    }
}

void interrupt temporizadorPrincipal()
{
    if(INTCONbits.T0IF == 1)
    {
      INTCONbits.T0IF = 0;
      contador++;
      if(contador >= 64)
      {
        contador = 0;
        cuentaTemporizador++;
        if (cuentaTemporizador >= 10)
        {
          encenderLed();
          cuentaTemporizador = 0;
          INTCONbits.T0IE = 0;
        }
        else
        {
            char j[10];
            //ABCDEFGp
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

            PORTB = j[cuentaTemporizador];
        }
      }
    }
}

void setearTemporizador()
{
  cuentaTemporizador++;
  if (cuentaTemporizador >= 8)
  {
    cuentaTemporizador = 0;
  }
  char j[10];
  //ABCDEFGp
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

  PORTB = j[cuentaTemporizador];

  if(led == 1)
  {
    led = 0;
  }
}

void main ()
{
  CMCON = 0x07;
  TRISA= 0b00001110;
  TRISB= 0x00;

  led = 0;

  TMR0 = 12;

  INTCONbits.GIE=1;//habilitar todas las interrupciones
  INTCONbits.PEIE=1;
  INTCONbits.T0IE=0;//HABILITA EL TIMER 0
  INTCONbits.T0IF=0;//limpiar bandera

  OPTION_REGbits.T0CS=0;//CONTARDOR(1) O TEMPORIZADOR(0)
  OPTION_REGbits.T0SE=0;//EDGE
  OPTION_REGbits.PSA=0;//PRESCALER A TIMER 0

  OPTION_REGbits.PS0=1;
  OPTION_REGbits.PS1=1;
  OPTION_REGbits.PS2=1;

  while (1)
  {
    if (set == 1)
    {
      setearTemporizador();
      __delay_ms (100);
    }

    if (inicio == 1)
    {
      INTCONbits.T0IE = 1;
      __delay_ms (100);
    }

    if (parada == 1)
    {
      INTCONbits.T0IE = 0;
      __delay_ms (100);
    }
  }
}
