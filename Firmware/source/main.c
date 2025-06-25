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
#include "OnethinxCore01.h"
#include "LoRaWAN_keys.h"
#include "communicator.h"
#include <stdbool.h>
#include <PrintF.h>
#include "maestro.h"

coreConfiguration_t	coreConfig = {
	.Join =
	{
		.KeysPtr = 			&Keys_0,
		.DataRate =			DR_AUTO,
		.Power =			PWR_MAX,
		.MAXTries = 		100,
		.SubBand_1st =     	EU_SUB_BANDS_DEFAULT,
		.SubBand_2nd =     	EU_SUB_BANDS_DEFAULT
	},
	.TX =
	{
		.Confirmed = 		false,
		.DataRate = 		DR_ADR,		// Adaptive Data Rate
		.Power = 			PWR_ADR,	// Adaptive Data Rate
		.FPort = 			1
	},
	.RX =
	{
		.Boost = 			true
	},
	.System =
	{
		.Idle =
		{
			.Mode = 		M0_DeepSleep,
			.BleEcoON =		false,
			.DebugON =		true,
		}
	}
};

sleepConfig_t sleepConfig =
{
	.sleepMode = modeDeepSleep,
	.BleEcoON = false,
	.DebugON = true,
	.sleepCores = coresBoth,
	.wakeUpPin = wakeUpPinHigh(true),
	.wakeUpTime = wakeUpDelay(0, 0, 10, 0), // day, hour, minute, second
};

FirmwareInfo_t FirmwareInfo =
{
    .FirmwareVersion        = 0x00000100,
	.BuildYear	            = buildyear,
	.BuildMonth				= buildmonth,
	.BuildDayOfMonth		= buildday,
	.BuildHour				= buildhour,
	.BuildMinute			= buildminute,
	.BuildSecond			= buildsecond,
	.BuildNumber			= buildnumber,
};

coreStatus_t 	coreStatus;
coreInfo_t 		coreInfo;

volatile uint32_t leds = 0;

int32_t GetADCvoltage()
{
	ADC_StartConvert();
	while (ADC_IsEndConversion(CY_SAR_WAIT_FOR_RESULT) == 0) {}
	int32_t adcResult = ADC_GetResult32(0);
	return ADC_CountsTo_mVolts(0, adcResult);
}

int main(void)
{
	/* enable global interrupts */
	__enable_irq();

	/* initialize radio with parameters in coreConfig */
	coreStatus = LoRaWAN_Init(&coreConfig);
	coreStatus = LoRaWAN_GetInfo(&coreInfo);
	PrintF_Start();
	ADC_Start();
	
	int32_t voltage = GetADCvoltage(CY_SAR_WAIT_FOR_RESULT);
	printf("Reset occured, reading voltage: %ld Volt\n", voltage);

	Communicator();

	Cy_GPIO_Write(LED_R_PORT, LED_R_NUM, 0);
	Cy_GPIO_Write(LED_B_PORT, LED_B_NUM, 1);

	coreStatus = LoRaWAN_Join(M4_NoWait);

	/* Flash LEDs while joining */
	while (LoRaWAN_GetStatus().system.isBusy)
	{
		Cy_GPIO_Inv(LED_R_PORT, LED_R_NUM);
		Cy_GPIO_Inv(LED_B_PORT, LED_B_NUM);
		CyDelay(400);
	}
	if (!LoRaWAN_GetStatus().mac.isJoined)	// Perform reset LoRaWAN join failed.
	{
		*((uint32_t *) 0x40210000) = 0x05FA0000;   // SW RESET M4
		NVIC_SystemReset();
	}

	/* main loop */
	for(;;)
	{
		Cy_GPIO_Write(LED_R_PORT, LED_R_NUM, 0);
		Cy_GPIO_Write(LED_B_PORT, LED_B_NUM, 1);
		int32_t voltage = GetADCvoltage(CY_SAR_WAIT_FOR_RESULT);
		/* Send message over LoRaWAN */
        coreStatus = LoRaWAN_Send((uint8_t *) &voltage, 4, M4_WaitDeepSleep);

		if (LoRaWAN_GetError().errorValue != errorStatus_NoError)
			Cy_GPIO_Write(LED_R_PORT, LED_R_NUM, 1);

		/* Sleep before sending next message, wake up with a button as well */
		LoRaWAN_Sleep(&sleepConfig);
	}
}