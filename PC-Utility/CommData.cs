using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static CmsisDap_Communicator.DataPacket;
//using static OTX_BLE_App.blePacket;

namespace CmsisDap_Communicator
{
    public class DataPacket
    {
        public enum Command_e : byte
        {
            CMD_IDLE = 0,
            CMD_INFO_STACK,
            CMD_INFO_FIRMWARE,
            CMD_KEYS,
            CMD_ADCVAL,
            CMD_LEDS,
            CMD_EXIT = 0xFF
        }

        public enum stackRegion_e : byte
        { 
            stack_AS    = 1,                                          //!< Australia 923 MHz
            stack_AU    = 2,                                          //!< Japan 915-928 MHz
            stack_CN_L  = 3,                                          //!< China 470-510 MHz
            stack_CN_H  = 4,                                          //!< China 779-787 MHz
            stack_EU_L  = 5,                                          //!< Europe 433 MHz
            stack_EU_H  = 6,                                          //!< Europe 863-870 MHz
            stack_IN    = 7,                                          //!< India 865-867 MHz
            stack_KR    = 8,                                          //!< Korea 920-923 MHz
            stack_US    = 9,                                          //!< North America 902-928 MHz
            stack_RU    = 10,                                         //!< Russia 864-870 MHz
        };    

        public enum buildType_e : byte
        {
            Release     = (byte) 'R',
            Debug       = (byte) 'D'
        }

        public enum stackOption_e : byte
        {
            Secure          = (byte) 'S',
            PSA             = (byte) 'P',
            Configurable    = (byte) 'C',
        }
        public enum stackStage_e : byte
        {
            PreAlpha            = (byte) 'a',
            Alpha               = (byte) 'A',
            PerpetualBeta       = (byte) 'b',
            Beta                = (byte) 'B',
            ReleaseCandidate    = (byte) 'r',
            Releae              = (byte) 'R'
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Header_t
        {
            [FieldOffset(0)]
            public uint Value; // Full 32-bit view

            // Command (8 bits)
            [FieldOffset(0)]
            public Command_e Command;

            // Bitfield (1 + 3 + 1 + 1 + 2 = 8 bits) → as one byte
            [FieldOffset(1)]
            private byte flags;

            // DataLength (2 bytes)
            [FieldOffset(2)]
            public ushort DataLength;

            // Accessors for the bits inside `flags`
            public bool Read
            {
                get => (flags & (1 << 0)) != 0;
                set => flags = (byte)((flags & ~(1 << 0)) | (value ? (1 << 0) : 0));
            }

            public bool SizeInvalid
            {
                get => (flags & (1 << 4)) != 0;
                set => flags = (byte)((flags & ~(1 << 4)) | (value ? (1 << 4) : 0));
            }

            public bool CommandInvalid
            {
                get => (flags & (1 << 5)) != 0;
                set => flags = (byte)((flags & ~(1 << 5)) | (value ? (1 << 5) : 0));
            }
            public bool Reset
            {
                get => (flags & (1 << 6)) != 0;
                set => flags = (byte)((flags & ~(1 << 6)) | (value ? (1 << 6) : 0));
            }
        }

        public enum keyType_e : byte
        {
            ABP_10x_key = 0x01,
            OTAA_10x_key = 0x02,
            OTAA_11x_key = 0x03,
            PreStored_key = 0xF0,
            UserStored_key = 0xF1,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct OTAA_10x_t
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] DevEui;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] AppEui;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] AppKey;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ABP_10x_t
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] DevEui;
            public uint DevAddr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] NwkSkey;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] AppSkey;
        }

        // Container type that holds the header and a raw key data array.
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LoRaWAN_keys_t
        {
            public ushort flags; // 16-bit header

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] keyData;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] reserved;
            // Optionally, you can add properties to expose the header fields.
            public keyType_e KeyType
            {
                get { return (keyType_e)(flags & 0xFF); }
                set { flags = (ushort)((flags & 0xFF00) | (byte)value); }
            }

            public byte Reserved
            {
                get { return (byte)((flags >> 8) & 0x7F); }
                set { flags = (ushort)((flags & 0x00FF) | ((value & 0x7F) << 8)); }
            }

            public bool PublicNetwork
            {
                get { return (flags & 0x8000) != 0; }
                set { flags = value ? (ushort)(flags | 0x8000) : (ushort)(flags & 0x7FFF); }
            }
        }



        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FirmwareInfo_t
        {
            public uint FirmwareVersion;

            private uint buildBits;

            public byte BuildYear
            {
                get => (byte)(buildBits & 0x3F); // 6 bits
                set => buildBits = (buildBits & ~0x3Fu) | ((uint)value & 0x3F);
            }

            public byte BuildMonth
            {
                get => (byte)((buildBits >> 6) & 0x0F); // 4 bits
                set => buildBits = (buildBits & ~(0x0Fu << 6)) | (((uint)value & 0x0F) << 6);
            }

            public byte BuildDayOfMonth
            {
                get => (byte)((buildBits >> 10) & 0x1F); // 5 bits
                set => buildBits = (buildBits & ~(0x1Fu << 10)) | (((uint)value & 0x1F) << 10);
            }

            public byte BuildHour
            {
                get => (byte)((buildBits >> 15) & 0x1F); // 5 bits
                set => buildBits = (buildBits & ~(0x1Fu << 15)) | (((uint)value & 0x1F) << 15);
            }

            public byte BuildMinute
            {
                get => (byte)((buildBits >> 20) & 0x3F); // 6 bits
                set => buildBits = (buildBits & ~(0x3Fu << 20)) | (((uint)value & 0x3F) << 20);
            }

            public byte BuildSecond
            {
                get => (byte)((buildBits >> 26) & 0x3F); // 6 bits
                set => buildBits = (buildBits & ~(0x3Fu << 26)) | (((uint)value & 0x3F) << 26);
            }

            public uint BuildNumber;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct coreInfo_t
        {
            public uint StackVersion; // Added StackVersion before coreInfo in firmware

            private uint buildBits; // Holds buildYear through buildSecond (6+4+5+5+6+6 = 32 bits)

            public byte BuildYear
            {
                get => (byte)(buildBits & 0x3F); // 6 bits
                set => buildBits = (buildBits & ~0x3Fu) | (uint)(value & 0x3F);
            }

            public byte BuildMonth
            {
                get => (byte)((buildBits >> 6) & 0x0F); // 4 bits
                set => buildBits = (buildBits & ~(0x0Fu << 6)) | ((uint)(value & 0x0F) << 6);
            }

            public byte BuildDayOfMonth
            {
                get => (byte)((buildBits >> 10) & 0x1F); // 5 bits
                set => buildBits = (buildBits & ~(0x1Fu << 10)) | ((uint)(value & 0x1F) << 10);
            }

            public byte BuildHour
            {
                get => (byte)((buildBits >> 15) & 0x1F); // 5 bits
                set => buildBits = (buildBits & ~(0x1Fu << 15)) | ((uint)(value & 0x1F) << 15);
            }

            public byte BuildMinute
            {
                get => (byte)((buildBits >> 20) & 0x3F); // 6 bits
                set => buildBits = (buildBits & ~(0x3Fu << 20)) | ((uint)(value & 0x3F) << 20);
            }

            public byte BuildSecond
            {
                get => (byte)((buildBits >> 26) & 0x3F); // 6 bits
                set => buildBits = (buildBits & ~(0x3Fu << 26)) | ((uint)(value & 0x3F) << 26);
            }

            public uint BuildNumber;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] DevEUI;

            public buildType_e BuildType;

            public stackRegion_e StackRegion;

            public stackOption_e StackOption;

            public stackStage_e StackStage;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string CodeName;
        }

        // Represents the dateTime_t union.
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct dateTime_t
        {
            // The entire 32-bit value.
            public uint value;

            // Properties to extract each bit-field.
            public uint Second
            {
                get { return value & 0x3F; } // 6 bits
                set { this.value = (this.value & ~0x3Fu) | (value & 0x3F); }
            }
            public uint Minute
            {
                get { return (value >> 6) & 0x3F; } // 6 bits
                set { this.value = (this.value & ~(0x3Fu << 6)) | ((value & 0x3F) << 6); }
            }
            public uint Hour
            {
                get { return (value >> 12) & 0x1F; } // 5 bits
                set { this.value = (this.value & ~(0x1Fu << 12)) | ((value & 0x1F) << 12); }
            }
            public uint DayOfMonth
            {
                get { return (value >> 17) & 0x1F; } // 5 bits
                set { this.value = (this.value & ~(0x1Fu << 17)) | ((value & 0x1F) << 17); }
            }
            public uint Month
            {
                get { return (value >> 22) & 0xF; } // 4 bits
                set { this.value = (this.value & ~(0xFu << 22)) | ((value & 0xF) << 22); }
            }
            public uint Year
            {
                get { return (value >> 26) & 0x3F; } // 6 bits
                set { this.value = (this.value & ~(0x3Fu << 26)) | ((value & 0x3F) << 26); }
            }
        }

        // Represents the wakeUpTime_t structure with a union.
        // Total size is 1 byte for flags plus 4 bytes for the union = 5 bytes.
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct wakeUpTime_t
        {
            // The first byte contains the bit fields: enabled (bit0) and isDateTime (bit1).
            [FieldOffset(0)]
            private int flags;

            // The union of either dateTime_t or separate byte fields.
            // They start at offset 4.
            [FieldOffset(4)]
            public dateTime_t dateTime; // 4 bytes (if isDateTime is true)

            // Alternative representation (if isDateTime is false) – delays.
            [FieldOffset(4)]
            public byte days;
            [FieldOffset(5)]
            public byte hours;
            [FieldOffset(6)]
            public byte minutes;
            [FieldOffset(7)]
            public byte seconds;

            // Properties to access the bit fields.
            public bool Enabled
            {
                get { return (flags & 0x01) != 0; }
                set { if (value) flags |= 0x01; else flags &= 0xFE; }
            }
            public bool IsDateTime
            {
                get { return (flags & 0x02) != 0; }
                set { if (value) flags |= 0x02; else flags &= 0xFD; }
            }
        }




        public static UInt32 WriteCommand(Command_e Command, int length)
        {
            return (UInt32)((int)Command | (length & 0xFFFF) << 16);
        }

        public static UInt32 ReadCommand(Command_e Command, int length)
        {
            return (UInt32)((int)Command | 0x100 | (length & 0xFFFF) << 16);
        }

        public static void DataToStruct<T>(byte[] CommData, ref T structure) where T : struct
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
            try
            {
                Marshal.Copy(CommData, 0, ptr, Math.Min(Marshal.SizeOf<T>(), CommData.Length));
                structure = Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }


        public static byte[] StructToData<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] data = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.StructureToPtr(structure, ptr, true);
                Marshal.Copy(ptr, data, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return data;
        }
        public static int GetStructSize<T>() where T : struct
        {
            return Marshal.SizeOf<T>();
        }

    }
}
