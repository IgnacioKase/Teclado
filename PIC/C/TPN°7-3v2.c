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

void interrumpir ()
{
    __delay_ms (1500);
    while (1)
    {
    if(PORTBbits.RB0 == 1)
        {
            break;
        }
    }
}

int powKase (int base, int exponente)
{
    int resultado = 1;
    for (int i = 1; i <= exponente; i = i + 1)
    {
        resultado = resultado * base;
    }
    return resultado;
}

void segundoInterrumpido(int cantidad)
{
  for (int i = 1; i <= cantidad; i = i + 1)
  {
    __delay_ms (1000/cantidad);
    if(PORTBbits.RB0==1)
    {
        interrumpir ();
    }
  }
}

void paraleloSerie (int masSignificativo, int menosSignificativo)
{
  int j[10];

  /*ABCDEFG*/
  j[0] = 0b11111100;
  j[1] = 0b01100000;
  j[2] = 0b11011010;
  j[3] = 0b11110010;
  j[4] = 0b01100110;
  j[5] = 0b10110110;
  j[6] = 0b10111110;
  j[7] = 0b11100110;
  j[8] = 0b11111110;
  j[9] = 0b11100110;

  int palabraBinaria = 0b000000000000000000000000;
  int palabraBinaria = palabraBinaria | j[menosSignificativo];
  int palabraBinaria = palabraBinaria | (j[masSignificativo] << 8);

  int dato;
  for (int i = 23; i > 0; i = i + 1)
  {
    dato = palabraBinaria - powKase(2, i);
    if (dato >=0)
    {
      dato = 1;
    }
    else
    {
      dato = 0;
    }
    PORTBbits.RB2 = 0;
    PORTBbits.RB1 = 1;
    PORTBbits.RB1 = 0;
  }
}

void contadorDescendente (int masSignificativo, int menosSignificativo)
{
  int unidadActual = menosSignificativo;
  int decenaActual = masSignificativo;
  for(int i = decenaActual; i >= 0; i = i - 1)
  {
      for (int k = unidadActual; k >= 0; k = k - 1)
      {
        unidadActual = k;
        paraleloSerie (decenaActual, unidadActual);
        segundoInterrumpido (20);
      }
      decenaActual = i;
      if(PORTBbits.RB0==1)
      {
          interrumpir ();
      }
  }
}

void contadorAscendente (int masSignificativo, int menosSignificativo)
{
  int unidadActual = menosSignificativo;
  int decenaActual = masSignificativo;
  for(int i = decenaActual; i >= 9; i = i + 1)
  {
      for (int k = unidadActual; k <= 9; k = k + 1)
      {
        unidadActual = k;
        paraleloSerie (decenaActual, unidadActual);
        segundoInterrumpido (20);
      }
      decenaActual = i;
      if(PORTBbits.RB0==1)
      {
          interrumpir ();
      }
  }
}

int contadorNatural (int numero)
{
  numero = numero + 1;
  if (numero > 9)
  {
    numero = 0;
  }
  return numero;
}

void main ()
{
    TRISB = 0b01000001;

    int numeroUnidad = 0;
    int numeroDecena = 0;

    int ordenDeCuenta = 0;

    int inicioParada=0;

    while(1)
    {
      if (PORTBbits.RB0 == 1)  //inicioParada
      {
        inicioParada = !inicioParada;
        __delay_ms (200);
      }
      if (PORTBbits.RB6 == 1) //Ascendente
      {
        ordenDeCuenta = !ordenDeCuenta;
        __delay_ms (200);
      }
      if (PORTBbits.RB5 == 1)  //Decena
      {
        numeroDecena = contadorNatural (numeroDecena);
        __delay_ms (200);
      }
      if (PORTBbits.RB4 == 1) //Unidad
      {
        numeroUnidad = contadorNatural (numeroUnidad);
        __delay_ms (200);
      }
      if(inicioParada==1)
      {
          if(ordenDeCuenta)
          {
            contadorAscendente (numeroDecena, numeroUnidad);
          }
          else
          {
            contadorDescendente (numeroDecena, numeroUnidad);
          }
      }
    }
}
