#include "Includes.h"


#define _XTAL_FREQ 4000000

#pragma config FOSC = INTOSCIO
#pragma config WDTE = OFF
#pragma config PWRTE = OFF
#pragma config MCLRE = OFF
#pragma config BOREN = ON
#pragma config LVP = OFF
#pragma config CPD = OFF
#pragma config CP = OFF

#define clockManual RA1
#define dato RA0
#define latch RA2

unsigned int dataLength = 0;
unsigned long palabra = 0;
unsigned long x;

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
  for(int i = 0; i < 61; i++)
  {
    dato = ((palabra >> (61 - i)) & 1);       //Dato
    clockManual = 1;                          //clockManual on
    clockManual = 0;                          //clockManual off
  }
  latch = 1;            //Latch on
  latch = 0;           //Latch off
}


void interrupt ISR(void)
{
    
	if(RCIF)  // If UART Rx Interrupt
	{
        x=RCREG;
		if(OERR) // If over run error, then reset the receiver
		{
			CREN = 0;
			CREN = 1;
		}
        if(dataLength > 0)
        {
            RB7=1;
          dataLength--;
          palabra = palabra | (1 << x);
          //palabra = x;
          if(dataLength <= 0) escribirSalida();
        }
        else
        {
            RB6=1;
          palabra = 0;
          dataLength = x;
        }
	}
}

void main()
{
    CMCON = 0x07;
    TRISA = 0b00000000;
    TRISB = 0b00111111;
    RB7=0;
    RB6=0;
	InitUART();							// Initialize UART

	GIE  = 1;  							// Enable global interrupts
    PEIE = 1;  							// Enable Peripheral Interrupts

    resetearSalidas(61);

	while(1)
	{
	}
}
