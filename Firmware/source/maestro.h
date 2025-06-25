#pragma once
#include <stdint.h>

/* build info variables inserted at pre-build with buildversion tool */

// OTX_Extension_eval("#define buildyear " + new Date().getFullYear() % 100)
#define buildyear 25
// OTX_Extension_eval("#define buildmonth " + (new Date().getMonth() + 1))
#define buildmonth 6
// OTX_Extension_eval("#define buildday " + new Date().getDate())
#define buildday 25
// OTX_Extension_eval("#define buildhour " + new Date().getHours())
#define buildhour 18
// OTX_Extension_eval("#define buildminute " + new Date().getMinutes())
#define buildminute 37
// OTX_Extension_eval("#define buildsecond " + new Date().getSeconds())
#define buildsecond 25
// OTX_Extension_eval( "#define buildnumber " + (${nextLineValue}+1) )
#define buildnumber 7177
          

typedef const struct  __attribute__ ((__packed__))
{
    uint32_t        FirmwareVersion;
	uint32_t		BuildYear				: 6;			/**< core firmware Year of build */
	uint32_t		BuildMonth				: 4;			/**< core firmware Month of build */
	uint32_t		BuildDayOfMonth			: 5;			/**< core firmware Day of build */
	uint32_t		BuildHour				: 5;			/**< core firmware Hour of build (24h mode) */
	uint32_t		BuildMinute				: 6;			/**< core firmware Minute of build */
	uint32_t		BuildSecond				: 6;			/**< core firmware Second of build */
	uint32_t 		BuildNumber;							/**< core firmware incremental build number */
} FirmwareInfo_t;