
#ifndef __UART_H
#define __UART_H

#ifndef _XTAL_FREQ
	#define _XTAL_FREQ 4000000
#endif

// Comm Setup
#define BAUDRATE 250000  //bps
// 8 bit data mode with one stop bit
// No flow control, no parity bit

//Function Prototypes
void InitUART(void);
void SendByteSerially(unsigned char);
unsigned char ReceiveByteSerially(void);
void SendStringSerially(const unsigned char*);

#endif

