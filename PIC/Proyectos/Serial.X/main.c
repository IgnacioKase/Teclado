#include <xc.h>

//#define _XTAL_FREQ 4000000
#define BAUDRATE 19200
#define _XTAL_FREQ 20000000


/*__CONFIG(FOSC_HS & WDTE_OFF & PWRTE_ON & MCLRE_ON & BOREN_ON 
		& LVP_OFF & CPD_OFF & CP_OFF);
//#pragma config FOSC = INTOSCIO*/
#pragma config FOSC = HS
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
#define led RA3
#define pulsadorReset RA4

unsigned int palabras[4] = {0, 0, 0, 0};
unsigned int dataLength = 0, lectura = 0, indice;
unsigned short trama = 0;
unsigned int buffer = 0;

void InitUART(void);
void SendByteSerially(unsigned char);
unsigned char ReceiveByteSerially(void);
void SendStringSerially(const unsigned char*);
void interrupt ISR(void);
void resetearSalidas (int cantidadSalidas);
void escribirSalida (void);
void resetDatos(void);

void main()
{
    CMCON = 0x07;
    TRISA = 0b00010000;
    led = 0;
    
	InitUART();							// Initialize UART

	GIE  = 1;  							// Enable global interrupts
    PEIE = 1;  							// Enable Peripheral Interrupts

    resetearSalidas(64);

	while(1)
	{
        if(pulsadorReset)
        {
            __delay_ms(1000);
            if(!pulsadorReset) break;
            GIE = 0;
            GIE = 1;
            dataLength = 0;
            for(int k = 0; k < 4; k++)
            {
                led = 1;
                __delay_ms(250);
                palabras[k] = 0;
                led = 0;
            }
            buffer = 0;
            trama = 0;
            resetearSalidas(64); 
            __delay_ms(4000);
        }
	}
}

void interrupt ISR(void)
{
	if(RCIF)
	{
        lectura = ReceiveByteSerially();
        led = !led;
        if (lectura == 0xfe) trama = 0;
        if(trama)
        {
           if(dataLength > 0)
            {
               dataLength--;
               if(buffer == 0) 
               {
                    buffer = lectura;
               }
               else
               {
                    indice = buffer/16;
                    if(lectura)
                        palabras[ indice] |= ( 1 << (buffer - (indice * 16)));
                    else
                        palabras[ indice] &= (!( 1 << (buffer - (indice * 16))));
                    buffer = 0;
                    if( dataLength <= 0 ) escribirSalida();
               }
            }
            else
            {
                dataLength = lectura;
            } 
        }  
        if (lectura == 0xff) trama = 1;
	}
}

void escribirSalida ()
{
    for(int k = 3; k >= 0; k--)
    {
        for(int i = 0; i < 16; i++)
        {
          dato = ((palabras[k] >> (15 - i)) & 1);
          clockManual = 1;
          clockManual = 0;
        }
    }
    
  latch = 1;
  latch = 0;
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

void InitUART(void)
{	
	SPBRG = ((_XTAL_FREQ/16)/BAUDRATE) - 1;
	BRGH  = 1;                   	// Fast baudrate
	SYNC  = 0;						// Asynchronous
	SPEN  = 1;						// Enable serial port pins
	CREN  = 1;						// Enable reception
	SREN  = 0;						// No effect
	TXIE  = 0;						// Disable tx interrupts
	RCIE  = 1;						// Enable rx interrupts
	TX9   = 0;						// 8-bit transmission
	RX9   = 0;						// 8-bit reception
	TXEN  = 0;						// Reset transmitter
	TXEN  = 1;						// Enable the transmitter
}

unsigned char ReceiveByteSerially(void)
{
	if(OERR)
	{
		CREN = 0;
		CREN = 1;
	}
	return RCREG;
}