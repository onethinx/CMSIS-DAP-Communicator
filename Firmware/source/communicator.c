/********************************************************************************
 *    ___             _   _     _             
 *   / _ \ _ __   ___| |_| |__ (_)_ __ __  __ 
 *  | | | | '_ \ / _ \ __| '_ \| | '_ \\ \/ / 
 *  | |_| | | | |  __/ |_| | | | | | | |>  <  
 *   \___/|_| |_|\___|\__|_| |_|_|_| |_/_/\_\ 
 *
 ********************************************************************************
 *   CMSIS-DAP Communicator – Onethinx OTX-18 Interface Tool
 *
 *   © 2019–2025 Onethinx BV <info@onethinx.com> | Rolf Nooteboom
 *
 *   Description:
 *   This utility facilitates communication between a host PC and the
 *   Onethinx OTX-18 LoRaWAN® module via CMSIS-DAP (HID).
 *
 *   Architecture:
 *     [    PC Application (Windows)    ]
 *               ↓ HID ↑
 *     [ CMSIS-DAP Programmer (USB-HID) ]
 *               ↓ SWD ↑
 *     [     OTX-18 Module (PSoC 6)     ]
 *
 *   Functionality:
 *     - Memory read/write access (while CPU is running)
 *     - Key reading and provisioning
 *     - Exchange settings and states
 * 
 *   Bonus: Drop-in PrintF.c/h enables printf output over a user-defined UART (UART_HW)
 *
 *   Repository:
 *     https://github.com/onethinx/OTX-CMSIS-DAP-Communicator
 *
 *   License:
 *     MIT License – See LICENSE file for full text.
 *
 *   Author:
 *     Onethinx Team – https://onethinx.com
 *
 ********************************************************************************/

 #include "project.h"
#include "communicator.h"
#include "OnethinxCore01.h"
#include "maestro.h"
#include "PrintF.h"

extern coreStatus_t 	    coreStatus;
extern coreInfo_t 		    coreInfo;
extern FirmwareInfo_t       FirmwareInfo;
extern volatile uint32_t    leds;
extern LoRaWAN_keys_t       Keys_0;
extern int32_t GetADCvoltage();

#define MY_REG (*(volatile uint32_t*) 0x08038000) 

typedef enum __attribute__ ((__packed__)) 
{
	CMD_IDLE = 0,
	CMD_INFO_STACK,
	CMD_INFO_FIRMWARE,
	CMD_KEYS,
	CMD_ADCVAL,
	CMD_LEDS,
	CMD_EXIT = 0xFF
} Command_e;

typedef struct __attribute__ ((__packed__)) 
{
	union
	{
		uint32_t Value;
		struct __attribute__ ((__packed__)) 
		{
			Command_e	Command     	: 8;  	// 1 byte
			uint8_t     Read        	: 1;  	// 1 bit
			uint8_t                 	: 3;  	// padding
			uint8_t 	SizeInvalid		: 1;  	// 1 bit
			uint8_t 	CommandInvalid	: 1;  	// 1 bit
			uint8_t     Reset       	: 1;  	// 1 bit
			uint8_t                 	: 1;  	// padding
			uint16_t    DataLength;       		// 2 bytes
		};
	} Header;                        			// total: 4 bytes
	uint8_t Data[124];              			// payload
} CommData_t;


void PrintHexDump(const char* header, const void* data, int16_t size)
{
    printf("%s: ", header);
    const uint8_t* bytes = (const uint8_t*)data;
	int16_t i = 0;
    for (; i < size - 1; i++) printf("%02X-", ((const uint8_t*)bytes)[i]);
    printf("%02X\n", ((const uint8_t*)bytes)[i]);
}

void Communicator(void)
{
	volatile CommData_t * CommData = (volatile CommData_t*)0x08038000;
	CommData->Header.Value = 0x00004000;	// Reset

	while (true)
	{
		if (CommData->Header.Command != CMD_IDLE)
		{
			//printf("Received packet, length %d bytes", CommData->Header.DataLength);
			//printf("\nCommheader: %08X\n", CommData->Header);
			//PrintHexDump("\n  data",  (const void *) CommData->Data, CommData->Header.DataLength);
			uint16_t dataCnt = 0;
			if (CommData->Header.Read)
			{
				switch (CommData->Header.Command)
				{
					case CMD_INFO_STACK:
					{
						* (uint32_t *) &CommData->Data = (uint32_t) coreStatus.system.version;
						for (dataCnt = 0; dataCnt < sizeof(coreInfo); dataCnt++) CommData->Data[dataCnt + 4] = ((uint8_t *) &coreInfo)[dataCnt];
						dataCnt += 4;
					}
					break;
					case CMD_INFO_FIRMWARE:
					{
						for (dataCnt = 0; dataCnt < sizeof(FirmwareInfo); dataCnt++) CommData->Data[dataCnt] = ((uint8_t *) &FirmwareInfo)[dataCnt];
					}
					break;
					case CMD_KEYS:
					{
						for (dataCnt = 0; dataCnt < sizeof(Keys_0); dataCnt++) CommData->Data[dataCnt] = ((uint8_t *) &Keys_0)[dataCnt];
					}
					break;

					case CMD_ADCVAL:
					{
						* (int32_t *) &CommData->Data = GetADCvoltage(CY_SAR_WAIT_FOR_RESULT);
						dataCnt = 4;
					}
					break;
					case CMD_LEDS:
					{
						* (uint32_t *) &CommData->Data = leds;
						dataCnt = 4;
					}
					break;
					default:
					{
						CommData->Header.CommandInvalid = true;
					}
					break;
				}
			}
			else	// Write Function
			{
				switch (CommData->Header.Command)
				{
					case CMD_KEYS:
					{
						for (dataCnt = 0; dataCnt < sizeof(Keys_0); dataCnt++) ((uint8_t *) &Keys_0)[dataCnt] = CommData->Data[dataCnt];
					}
					break;
					case CMD_LEDS:
					{
						leds = * (uint32_t *) &CommData->Data;
						Cy_GPIO_Write(LED_R_PORT, LED_R_NUM, (leds & 0x00000001) != 0);
						Cy_GPIO_Write(LED_B_PORT, LED_B_NUM, (leds & 0x00000100) != 0);
						dataCnt = 4;
					}
					break;
					case CMD_EXIT:
						CommData->Header.Command = CMD_IDLE;
						return;
					default:
					{
						CommData->Header.CommandInvalid = true;
					}
					break;
				}

			}
			CommData->Header.SizeInvalid = CommData->Header.DataLength != dataCnt;
			CommData->Header.DataLength = dataCnt;
			CommData->Header.Command = CMD_IDLE;
		}
	}
}

/* [] END OF FILE */
