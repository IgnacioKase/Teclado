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

#define clockManual RB1
#define dato RB2
#define latch RB3
#define led RB4

unsigned int codificacion[10] = {0b01111110, 0b00100010, 0b10111100, 0b10110110, 0b11100010, 0b11010110, 0b11011110, 0b00110010, 0b11111110, 0b11110010};

unsigned int contador = 0;
unsigned int contadorLed = 0;
unsigned int velocidad = 0;
unsigned int contadorTemporizador = 0;

int powKase (int base, int exponente)
{
    int resultado = 1;
    if (exponente != 0)
    {
      for (int i = 1; i <= exponente; i = i + 1)
      {
          resultado = resultado * base;
      }
    }
    return resultado;
}

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

void escribirSalida ()
{
  unsigned int resultado[8] = {0,0,0,0,0,0,0,0};
  unsigned int velocidadDecena = velocidad/10;
  unsigned int velocidadUnidad = velocidad - velocidadDecena * 10;
  unsigned int numero = codificacion[velocidadUnidad];
  for (int i = 0; i < 8; i++)
  {
    resultado[i] = (numero >> (7 - i)) & 1;
  }
  for (int i = 0; i < 8; i++)
  {
    dato = resultado[7 -i];          //Dato
    clockManual = 1;              //clockManual on
    clockManual = 0;              //clockManual off
  }
  latch = 1;            //Latch on
  latch = 0;           //Latch off
  numero = codificacion[velocidadDecena];
  for (int i = 0; i < 8; i++)
  {
    resultado[i] = (numero >> (7 - i)) & 1;
  }
  for (int i = 0; i < 8; i++)
  {
    dato = resultado[7 -i];          //Dato
    clockManual = 1;              //clockManual on
    clockManual = 0;              //clockManual off
  }
  latch = 1;            //Latch on
  latch = 0;           //Latch off
  velocidad = 0;
}


void interrupt enconder()
{
    if(INTCONbits.INTF == 1)
    {
      INTCONbits.INTF = 0;
      contadorLed = contadorLed + 1;
      contador = contador + 1;
      if(contadorLed >= 1024)
      {
        contadorLed = 0;
        led = !led;
      }
    }
    if(INTCONbits.T0IF == 1)
    {
      INTCONbits.T0IF = 0;
      TMR0 = 6;
      contadorTemporizador++;
      if (contadorTemporizador >= 125)
      {
        contadorTemporizador = 0;
        velocidad = (contador * 60) / 1024 ;
        escribirSalida();
        contador = 0;
      }
    }
}

void main ()
{
  TRISB = 0b00000001;

  INTCONbits.GIE = 1;//habilitar todas las interrupciones
  INTCONbits.PEIE = 1;
  INTCONbits.T0IE = 1;//HABILITA EL TIMER 0
  INTCONbits.INTE = 1;
  INTCONbits.T0IF = 0;//limpiar bandera
  INTCONbits.INTF = 0;//limpiar bandera

  OPTION_REGbits.T0CS = 0;//CONTARDOR O TEMPORIZADOR
  OPTION_REGbits.T0SE = 0;//EDGE

  OPTION_REGbits.PSA = 0;//PRESCALER A TIMER 0

  OPTION_REGbits.PS0 = 1;
  OPTION_REGbits.PS1 = 0;
  OPTION_REGbits.PS2 = 0;

  TMR0 = 6;

  int palabaraBinaria = 16;

  resetearSalidas(palabaraBinaria);
  while (1)
  {
  }
}
