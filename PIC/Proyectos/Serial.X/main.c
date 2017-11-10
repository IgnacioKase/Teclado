#include "Includes.h"

#pragma config FOSC = INTOSCIO
#pragma config WDTE = OFF
#pragma config PWRTE = OFF
#pragma config MCLRE = OFF
#pragma config BOREN = ON
#pragma config LVP = OFF
#pragma config CPD = OFF
#pragma config CP = OFF

#define clockManual RB0
#define dato RB1
#define latch RB2

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


void interrupt ISR(void)
{
	if(RCIF)  // If UART Rx Interrupt
	{
		if(OERR) // If over run error, then reset the receiver
		{
			CREN = 0;
			CREN = 1;
		}
        
		SendByteSerially(RCREG);	// Echo back received char
	}
}

void main()
{	
	InitUART();							// Initialize UART
    
    //SendStringSerially("Hello World!");	// Send string on UART

	GIE  = 1;  							// Enable global interrupts
    PEIE = 1;  							// Enable Peripheral Interrupts

	while(1)
	{
	}
}
