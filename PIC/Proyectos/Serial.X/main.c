#include "Includes.h"

#pragma config FOSC = INTOSCIO
#pragma config WDTE = OFF
#pragma config PWRTE = OFF
#pragma config MCLRE = OFF
#pragma config BOREN = ON
#pragma config LVP = OFF
#pragma config CPD = OFF
#pragma config CP = OFF

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
    
    SendStringSerially("Hello World!");	// Send string on UART

	GIE  = 1;  							// Enable global interrupts
    PEIE = 1;  							// Enable Peripheral Interrupts

	while(1)
	{
	}
}
