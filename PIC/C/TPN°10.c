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

unsigned int contador = 0;

int codificacion[10] = {23333332, 22322232, 32333322, 32332332, 33322232, 33232332, 33233332, 22332232, 33333332, 33332232};

int powKase (int base, int exponente)
{
    int resultado = 1;
    for (int i = 1; i <= exponente; i = i + 1)
    {
        resultado = resultado * base;
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

int intToArray (int numero)
{
  int resultado[6];
  resultado[0] = 0;
  resultado[1] = 0;
  resultado[2] = 0;
  resultado[3] = 0;
  resultado[4] = 0;
  resultado[5] = 0;

  if (numero != 0)
  {
    for (int i = 0; i < 6; i++)
    {
      resultado[i] = numero / powKase(10, (5 - i) );
      numero = numero - powKase(10, (5 - i) ) * resultado[i];
    }
  }
  return resultado;
}

void escribirSalida (int arreglo[])
{
  int resultado[8];
  int numero;
  resultado[0] = 0;
  resultado[1] = 0;
  resultado[2] = 0;
  resultado[3] = 0;
  resultado[4] = 0;
  resultado[5] = 0;
  resultado[6] = 0;
  resultado[7] = 0;
  for(int i = 5; i >= 0; i = i - 1)
  {
    numero = arreglo[i];
    if (numero != 0)
    {
      for (int i = 0; i < 8; i++)
      {
        resultado[i] = numero / powKase(10, (7 - i) );
        numero = numero - powKase(10, (7 - i) ) * resultado[i];
      }
    }
    for (int j = 0; j < 8; j = j + 1)
    {
      dato = resultado[j] - 2;          //Dato
      clockManual = 1;                    //clockManual on
      clockManual = 0;                    //clockManual off
    }
  }
  latch = 1;            //Latch on
  latch = 0;           //Latch off
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
        escribirSalida(intToArray(contador));
        __delay_ms (20);
        contador++;
        if(contador > 999999 )
        {
          contador = 0;
        }
      }
    }
}
