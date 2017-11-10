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
    if(PORTAbits.RA5 == 1)
        {
            break;
        }
    }
}

void contadorDescendente (int masSignificativo, int menosSignificativo)
{
  char j[10];

  /*ABCDEFG*/
  j[0] = 0b11011110;
  j[1] = 0b01010000;
  j[2] = 0b11001101;
  j[3] = 0b11011001;
  j[4] = 0b01010011;
  j[5] = 0b10011011;
  j[6] = 0b10011111;
  j[7] = 0b11010000;
  j[8] = 0b11011111;
  j[9] = 0b11010011;
  for(int i = masSignificativo; i >= 0; i = i - 1)
  {
      PORTA=j[i];
      for (int k = menosSignificativo; k >= 0; k = k - 1)
      {
          PORTB=j[k];
          __delay_ms (1000);

          if(PORTAbits.RA5==1)
          {
              interrumpir ();
          }
      }
      PORTB=j[0];
      if(PORTAbits.RA5==1)
      {
          interrumpir ();
      }
  }
}

void contadorAscendente (int masSignificativo, int menosSignificativo)
{
  char j[10];

  /*ABCDEFG*/
  j[0] = 0b11011110;
  j[1] = 0b01010000;
  j[2] = 0b11001101;
  j[3] = 0b11011001;
  j[4] = 0b01010011;
  j[5] = 0b10011011;
  j[6] = 0b10011111;
  j[7] = 0b11010000;
  j[8] = 0b11011111;
  j[9] = 0b11010011;
  for(int i = masSignificativo; i <= 9; i = i + 1)
  {
      PORTA=j[i];
      for (int k = menosSignificativo; k <= 9; k = k + 1)
      {
          PORTB=j[k];
          __delay_ms (1000);

          if(PORTAbits.RA5==1)
          {
              interrumpir ();
          }
      }
      PORTB=j[0];
      if(PORTAbits.RA5==1)
      {
          interrumpir ();
      }
  }
}

void main ()
{
    CMCON = 07;
    TRISA= 0b00100000;
    TRISB= 0b00100000;

    int numeroUnidad = 0;
    int numeroDecena = 0;

    int ordenDeCuenta = 0;

    int inicioParada=0;
    char j[10];

    /*ABCDEFG*/
    j[0] = 0b11011110;
    j[1] = 0b01010000;
    j[2] = 0b11001101;
    j[3] = 0b11011001;
    j[4] = 0b01010011;
    j[5] = 0b10011011;
    j[6] = 0b10011111;
    j[7] = 0b11010000;
    j[8] = 0b11011111;
    j[9] = 0b11010011;

    PORTA = 0b11011110;
    PORTB = 0b11011110;

    while(1)
    {
        if(PORTAbits.RA5 == 1)
        {
            inicioParada=!inicioParada;
            __delay_ms (1000);
            if (PORTAbits.RA5 == 1)
            {
              while(PORTAbits.RA5 == 1)
              {
                if (PORTBbits.RB5 == 1)
                {
                  numeroDecena = numeroDecena + 1;
                  if(numeroDecena > 9)
                  {
                    numeroDecena = 0;
                  }
                  PORTA = j[numeroDecena];
                  __delay_ms (500);
                }
              }
              inicioParada=!inicioParada;
            }
        }

        if (PORTBbits.RB5 == 1)
        {
          ordenDeCuenta = !ordenDeCuenta;
          __delay_ms (1000);
          if (PORTBbits.RB5 == 1)
          {
            while(PORTBbits.RB5 == 1)
            {
              if (PORTAbits.RA5 == 1)
              {
                numeroUnidad = numeroUnidad + 1;
                if (numeroUnidad > 9)
                {
                  numeroUnidad = 0;
                }
                PORTB = j[numeroUnidad];
                __delay_ms (500);
              }
            }
            ordenDeCuenta = !ordenDeCuenta;
          }
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
