// **********************************************************************
//     PSoC6 CMSIS-DAP Programmer - Windows Forms UI Frontend
// ______________________________________________________________________
//
// Copyright (c) 2025 Rolf Nooteboom
// SPDX-License-Identifier: AGPL-3.0-or-later WITH additional terms
//
// Licensed under the GNU Affero General Public License v3.0 or later (AGPL-3.0)
// with the following modifications:
// - This software may be used **only for non-commercial purposes**.
// - All derivative works must be shared under the same license and
//   must be reported back to the original author (Rolf Nooteboom).
// - The original copyright, license, and attribution notices must be retained.
//
// References:
// - CMSIS-DAP: https://arm-software.github.io/CMSIS_5/DAP/html/index.html
// - Infineon PSoC 6 Programming Specification 002-15554 Rev. *O
//
// Description:
// - Provides a UI to interact with CMSIS-DAP compatible devices
// - Allows parsing and flashing of PSoC6 ELF or HEX firmware
// - Handles acquire, erase, program, and verify functions
//
//  Author: Rolf Nooteboom <rolf@nooteboom-elektronica.com>
//  Created: 2025
//
// **********************************************************************

using HidSharp;
using HidSharp.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using static CmsisDap_Communicator.CmsisDap;
using static CmsisDap_Communicator.DataPacket;
using static System.Net.Mime.MediaTypeNames;

namespace CmsisDap_Communicator
{
    public partial class Form1 : Form
    {
        CmsisDap dap = new CmsisDap();
        private DeviceInfo? _selectedDevice = null;
        private CmsisDap.Device? _programmer = null;

        const string thisName = "CMSIS-DAP Communicator 1.0 by Onethinx.com | Rolf Nooteboom";
        Color backColor = Color.FromArgb(32, 32, 32);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int cGrip = 13;  // Grip size

        // Show GripSize
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, Color.SteelBlue, rc);
            Color FrameColor = this.Focused ? Color.SteelBlue : Color.SlateGray;
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(0x0B, 0x32, 0x80), 1), new Rectangle(0, 0, Width - 1, Height - 1));
        }

        // Resize form
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17;
                    // HTBOTTOMRIGHT
                    return;
                }
            }
            base.WndProc(ref m);
        }

        public Form1()
        {
            InitializeComponent();
            UIExtension.StatusTextBox = tbStatus;
            UIExtension.GroupBox = gbCommunicate;
            UIExtension.ProgressBar = pnlProgress;
            UIExtension.cbProgrammer = cbProgs;
            btScanUSB_Click(this, new EventArgs());
            cbProgs.DrawMode = DrawMode.OwnerDrawFixed;
            cbProgs.DrawItem += (sender, e) =>
            {
                if (e.Index < 0) return;

                e.DrawBackground();
                var dev = (DeviceInfo)cbProgs.Items[e.Index]!;
                using var brush = new SolidBrush(dev.IsUnverified ? Color.Red : this.ForeColor);
                e.Graphics.DrawString(dev.ToString(), e.Font!, brush, e.Bounds);
                e.DrawFocusRectangle();
            };
            this.Text = thisName;
            lblTop.Text = thisName;

        }

        private void btScanUSB_Click(object sender, EventArgs e)
        {
            try
            {
                tbStatus.Clear();
                var devices = dap.Enumerate();
                if (devices.Count == 0)
                {
                    UIExtension.ToError("\r\nNo CMSIS-DAP device found.");
                    return;
                }
                cbProgs.DataSource = devices;
                UIExtension.ToStatus("\r\nDevices found:");
                foreach (var dev in devices)
                {
                    UIExtension.ToStatus($"\r\n{dev}", dev.IsUnverified ? Color.Red : this.ForeColor);
                }
            }
            catch { }
        }

        private CmsisDap.Device OpenSelectedProg()
        {
            if (!(UIExtension.GetSelectedProgrammer() is DeviceInfo selectedDevice))
                throw new Exception("No Valid Programmer Selected.");
            if (_programmer != null && selectedDevice == _selectedDevice) return _programmer;
            _selectedDevice = selectedDevice;
            _programmer = dap.Open(selectedDevice);
            if (_programmer == null)
                throw new Exception("No Valid Programmer Selected.");

            UIExtension.ToStatus($"\r\nProgrammer Capabilities: 0x{_programmer.Capabilities:X2}");
            UIExtension.ToStatus($"\r\nProgrammer VendorName: {_programmer.VendorName}");
            UIExtension.ToStatus($"\r\nProgrammer ProductName: {_programmer.ProductName}");
            UIExtension.ToStatus($"\r\nProgrammer SerialNumber: {_programmer.SerialNumber}");
            UIExtension.ToStatus($"\r\nProgrammer ProtocolVersion: {_programmer.ProtocolVersion}");
            UIExtension.ToStatus($"\r\nProgrammer TargetDeviceVendor: {_programmer.TargetDeviceVendor}");
            UIExtension.ToStatus($"\r\nProgrammer TargetDeviceName: {_programmer.TargetDeviceName}");
            UIExtension.ToStatus($"\r\nProgrammer TargetBoardVendor: {_programmer.TargetBoardVendor}");
            UIExtension.ToStatus($"\r\nProgrammer TargetBoardName: {_programmer.TargetBoardName}");
            UIExtension.ToStatus($"\r\nProgrammer FirmwareVersion: {_programmer.FirmwareVersion}");

            return _programmer;
        }

        void Execute(string name, Action action)
        {
            Task.Run(() =>
            {
                try
                {
                    DateTime startAction = DateTime.Now;
                    UIExtension.ToStatus($"\r\n\r\nStart of {name.ToLower()}: {startAction:G}");
                    UIExtension.Progress(0, 100);
                    action();
                    UIExtension.ToStatus($"\r\n{name} successfully.");

                    TimeSpan elapsed = DateTime.Now - startAction;
                    UIExtension.ToStatus($"\r\nTotal time: {elapsed.Seconds}.{elapsed.Milliseconds:D3} seconds");
                }
                catch (Exception ex)
                {
                    UIExtension.ToError($"\r\n{name} FAILED: {ex.Message}");
                }
                UIExtension.Progress(0, 0);
            });
        }

        private void btSetKeys_Click(object sender, EventArgs e)
        {
            Execute("Set Keys", () => { Com_SetKeys(); });
        }

        private void btReadKeys_Click(object sender, EventArgs e)
        {
            Execute("Read Keys", () => { Com_ReadKeys(); });
        }

        private void btReadStack_Click(object sender, EventArgs e)
        {
            Execute("Read Stack Info", () => { Com_ReadStack(); });
        }

        private void btReadFwInfo_Click(object sender, EventArgs e)
        {
            Execute("Read Firmware Info", () => { Com_ReadFwInfo(); });
        }

        private void btReadAdcVal_Click(object sender, EventArgs e)
        {
            Execute("Read ADC Value", () => { Com_ReadAdcVal(); });
        }

        private void btReadLed_Click(object sender, EventArgs e)
        {
            Execute("Set LED status", () => { Com_ReadLeds(); });
        }

        private void btSetLeds_Click(object sender, EventArgs e)
        {
            Execute("Set LEDs", () => { Com_SetLeds(); });
        }


        private void btExit_Click(object sender, EventArgs e)
        {
            Execute("Exit", () => { Com_Exit(); });
        }

        void Com_SetKeys()
        {
            UIExtension.ToStatus("\r\nWriting LoRaWAN keys...");
            var LoRaWAN_keys = new LoRaWAN_keys_t();
            var OTAA_10x_keys = new OTAA_10x_t();
            OTAA_10x_keys.DevEui = HexToBytes(tbLoRaDevEUI.Text, 8);
            OTAA_10x_keys.AppEui = HexToBytes(tbLoRaAppEUI.Text, 8);
            OTAA_10x_keys.AppKey = HexToBytes(tbLoRaAppKey.Text, 16);
            LoRaWAN_keys.keyData = StructToData(OTAA_10x_keys);
            byte[] CommData = StructToData(LoRaWAN_keys);
            WriteData(CommData, Command_e.CMD_KEYS);
        }

        void Com_ReadKeys()
        {
            UIExtension.ToStatus("\r\nReading LoRaWAN keys...");
            var rawdata = ReadData(Command_e.CMD_KEYS, GetStructSize<LoRaWAN_keys_t>());
            var LoRaWAN_keys = new LoRaWAN_keys_t();
            DataToStruct(rawdata, ref LoRaWAN_keys);
            OTAA_10x_t OTAA_10x_keys = new OTAA_10x_t();
            DataToStruct(LoRaWAN_keys.keyData, ref OTAA_10x_keys);
            this.ThreadSafe(delegate
            {
                tbLoRaDevEUI.Text = BytesToHex(OTAA_10x_keys.DevEui);
                tbLoRaAppEUI.Text = BytesToHex(OTAA_10x_keys.AppEui);
                tbLoRaAppKey.Text = BytesToHex(OTAA_10x_keys.AppKey);
                string info = "\r\n" + $"""
                     DevEUI  : 0x{tbLoRaDevEUI.Text}
                     AppEUI  : 0x{tbLoRaAppEUI.Text}
                     AppKey  : 0x{tbLoRaAppKey.Text}       
                    """;
                UIExtension.ToStatus(info);
            });

        }

        void Com_ReadStack()
        {
            UIExtension.ToStatus("\r\nReading LoRaWAN Stack info...");
            var rawdata = ReadData(Command_e.CMD_INFO_STACK, GetStructSize<coreInfo_t>());
            var coreInfo = new coreInfo_t();
            DataToStruct(rawdata, ref coreInfo);
            string info = "\r\n" + $"""
                 Firmware version : {coreInfo.StackVersion >> 24:X2}.{coreInfo.StackVersion >> 16:X2}.{coreInfo.StackVersion >> 8:X2}.{coreInfo.StackVersion & 0xFF:X2}
                 Build date       : {coreInfo.BuildDayOfMonth:D2}-{coreInfo.BuildMonth:D2}-{coreInfo.BuildYear:D2}, {coreInfo.BuildHour:D2}:{coreInfo.BuildMinute:D2}:{coreInfo.BuildSecond:D2}
                 Build number     : {coreInfo.BuildNumber}
                 DevEUI           : {BytesToHex(coreInfo.DevEUI)}
                 Code name        : {coreInfo.CodeName}
                 Build type       : {(buildType_e)coreInfo.BuildType}
                 Stack region     : {(stackRegion_e)coreInfo.StackRegion}
                 Stack option     : {(stackOption_e)coreInfo.StackOption}
                 Stack stage      : {(stackStage_e)coreInfo.StackStage}
                """;
            UIExtension.ToStatus(info);
        }
        void Com_ReadFwInfo()
        {
            UIExtension.ToStatus("\r\nReading Firmware info...");
            var rawdata = ReadData(Command_e.CMD_INFO_FIRMWARE, GetStructSize<FirmwareInfo_t>());
            var FirmwareInfo = new FirmwareInfo_t();
            DataToStruct(rawdata, ref FirmwareInfo);
            string info = "\r\n" + $"""
                Firmware version : {FirmwareInfo.FirmwareVersion >> 24:X2}.{FirmwareInfo.FirmwareVersion >> 16:X2}.{FirmwareInfo.FirmwareVersion >> 8:X2}.{FirmwareInfo.FirmwareVersion & 0xFF:X2}
                Build date       : {FirmwareInfo.BuildDayOfMonth:D2}-{FirmwareInfo.BuildMonth:D2}-{FirmwareInfo.BuildYear:D2}, {FirmwareInfo.BuildHour:D2}:{FirmwareInfo.BuildMinute:D2}:{FirmwareInfo.BuildSecond:D2}
                Build number     : {FirmwareInfo.BuildNumber}       
            """;
            UIExtension.ToStatus(info);
        }
        void Com_ReadAdcVal()
        {
            UIExtension.ToStatus("\r\nReading ADC Value...");
            double voltage = (double)ReadInt32(Command_e.CMD_ADCVAL) / 1000;
            UIExtension.ToStatus($"\r\nADC Voltage: {voltage:0.0000} V");
        }
        void Com_ReadLeds()
        {
            UIExtension.ToStatus("\r\nReading LED status...");
            var LEDstate = ReadInt32(Command_e.CMD_LEDS);
            this.ThreadSafe(delegate
            {
                cbLedRed.Checked = (LEDstate & 0x00000001) > 0;
                cbLedBlue.Checked = (LEDstate & 0x00000100) > 0;
                UIExtension.ToStatus("\r\n");
                UIExtension.ToStatus(" ⚫ ", Color.Red);
                UIExtension.ToStatus("Red LED  : ");
                if (cbLedRed.Checked)
                    UIExtension.ToStatus("🟢 ON", Color.Red);
                else
                    UIExtension.ToStatus("\U0001f7e2 OFF", Color.Black);

                UIExtension.ToStatus("\r\n");
                UIExtension.ToStatus(" ⚫ ", Color.Blue);
                UIExtension.ToStatus("Blue LED : ");
                if (cbLedBlue.Checked)
                    UIExtension.ToStatus("🟢 ON", Color.Blue);
                else
                    UIExtension.ToStatus("🟢 OFF", Color.Black);

            });
        }
        void Com_SetLeds()
        {
            UIExtension.ToStatus("\r\nSetting LED status...");
            var CommData = (cbLedRed.Checked ? 0x00000001 : 0) | (cbLedBlue.Checked ? 0x00000100 : 0);
            WriteInt32((uint)CommData, Command_e.CMD_LEDS);
        }

        void Com_Exit()
        {
            UIExtension.ToStatus("\r\nExiting...");
            WriteInt32(0, Command_e.CMD_EXIT);
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            Execute("Reset", () =>
            {
                Reset();
            });
        }
        void Reset()
        {
            var Device = OpenSelectedProg();
            UIExtension.ToStatus("\r\nConnecting target...");

            Psoc6Programmer Programmer = new Psoc6Programmer(Device!, PSoC6Family.PSOC6ABLE2, SWJ_Interface.SWD, 4000000);
            Programmer.ToggleXRES();
        }
        private void btAcquire_Click(object sender, EventArgs e)
        {
            Execute("Acquire", () =>
            {
                Acquire();
            });
        }

        private void WaitResponse(Psoc6Programmer Programmer)
        {
            Header_t header = new Header_t();
            int timeOut = 100;
            do
            {
                header.Value = Programmer.ReadIO(0x08038000);
                if (header.Command == Command_e.CMD_IDLE) break;
                Thread.Sleep(1);
            } while (--timeOut > 0);
            if (timeOut == 0) throw new InvalidOperationException("Read timeout: No response from target, check firmware and CPU execution state.");
            if (header.CommandInvalid) throw new InvalidOperationException("Error, target response: Invalid Command.");
            if (header.SizeInvalid) throw new InvalidOperationException("Error, target response: Invalid Data Length.");
            if (header.Reset) throw new InvalidOperationException("Error, target response: Received Reset.");

        }

        private void WriteData(byte[] data, Command_e Command)
        {
            var Device = OpenSelectedProg();
            Psoc6Programmer Programmer = new Psoc6Programmer(Device!, PSoC6Family.PSOC6ABLE2, SWJ_Interface.SWD, 4000000);
            Programmer.Attach(AP_e.AP_CM4);
            Programmer.TransferBlock(0x08038004, data, 0, data.Length);
            Header_t header = new Header_t() { Command = Command, DataLength = (ushort)data.Length };
            // Write header after data transfer to ensure data is present at PSoC before parsing
            Programmer.WriteIO(0x08038000, header.Value);
            WaitResponse(Programmer);
        }

        private void WriteInt32(UInt32 data, Command_e Command)
        {
            var Device = OpenSelectedProg();
            Psoc6Programmer Programmer = new Psoc6Programmer(Device!, PSoC6Family.PSOC6ABLE2, SWJ_Interface.SWD, 4000000);
            Programmer.Attach(AP_e.AP_CM4);
            Programmer.WriteIO(0x08038004, data);
            Header_t header = new Header_t() { Command = Command, DataLength = 4 };
            // Write header after data transfer to ensure data is present at PSoC before parsing
            Programmer.WriteIO(0x08038000, header.Value);
            WaitResponse(Programmer);
        }

        private byte[] ReadData(Command_e Command, int length)
        {
            var Device = OpenSelectedProg();
            Psoc6Programmer Programmer = new Psoc6Programmer(Device!, PSoC6Family.PSOC6ABLE2, SWJ_Interface.SWD, 4000000);
            Programmer.Attach(AP_e.AP_CM4);
            Header_t header = new Header_t() { Command = Command, Read = true, DataLength = (ushort)length };
            Programmer.WriteIO(0x08038000, header.Value);
            WaitResponse(Programmer);
            return Programmer.TransferBlockRead(0x08038004, 0, length);
        }

        private UInt32 ReadInt32(Command_e Command)
        {
            var Device = OpenSelectedProg();
            Psoc6Programmer Programmer = new Psoc6Programmer(Device!, PSoC6Family.PSOC6ABLE2, SWJ_Interface.SWD, 4000000);
            Programmer.Attach(AP_e.AP_CM4);
            Header_t header = new Header_t() { Command = Command, Read = true, DataLength = 4 };
            Programmer.WriteIO(0x08038000, header.Value);
            WaitResponse(Programmer);
            return Programmer.ReadIO(0x08038004);
        }

        /// <summary>
        /// Acquires the target and identifies silicon using SROM Call.
        /// </summary>
        void Acquire()
        {
            var Device = OpenSelectedProg();
            // -----------------------------
            // Connect / Acquire the target
            // -----------------------------
            UIExtension.ToStatus("\r\nAcquiring target...");

            Psoc6Programmer Programmer = new Psoc6Programmer(Device!, PSoC6Family.PSOC6ABLE2, SWJ_Interface.SWD, 4000000);
            Programmer.Acquire(AcquireMode.ACQ_RESET, false, AP_e.AP_CM4);

            UIExtension.ToStatus("\r\nTarget acquired successfully.");

            // -----------------------------
            // Read PSoC6 Info
            // -----------------------------
            UIExtension.ToStatus("\r\nReading PSoC6 Info...");

            Programmer.GetSiliconInfo(out ushort FamilyId, out ushort SiliconId, out byte RevisionId, out byte ProtectionState);
            string familyName = PSoC6Family.FromFamilyId(FamilyId).Name;
            string protection = Enum.IsDefined(typeof(ProtectionState_e), ProtectionState)
                ? ((ProtectionState_e)ProtectionState).ToString()
                : $"UNKNOWN (0x{ProtectionState:X2})";

            string info = "\r\n" + $"""
                ------------------------------------------------------
                 PSoC6 Device Info
                ------------------------------------------------------
                 Family ID     : 0x{FamilyId:X4}  ({familyName})
                 Silicon ID    : 0x{SiliconId:X4}
                 Revision ID   : 0x{RevisionId:X2}
                 Protection    : 0x{ProtectionState:X2}    ({protection})
                ------------------------------------------------------
                """;

            UIExtension.ToStatus(info);
        }

        private void tbHEX_TextChanged(object? sender, EventArgs e)
        {
            var tb = (TextBox)sender!;
            int selStart = tb.SelectionStart;

            // Remove non-hex characters and dashes
            string raw = new string(tb.Text
                .Where(c => Uri.IsHexDigit(c))
                .ToArray());

            // Format: insert dash every 2 chars
            var formatted = string.Join("-", Enumerable.Range(0, raw.Length / 2 + (raw.Length % 2))
                                                       .Select(i => raw.Skip(i * 2).Take(2))
                                                       .Where(g => g.Any())
                                                       .Select(g => new string(g.ToArray())));

            tb.TextChanged -= tbHEX_TextChanged; // prevent recursion
            tb.Text = formatted.ToUpperInvariant();

            // Restore the cursor position smartly
            tb.SelectionStart = Math.Min(selStart + (tb.Text.Length - raw.Length), tb.Text.Length);
            tb.TextChanged += tbHEX_TextChanged;
        }

        private void tbHEX_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) // Allow backspace
                return;

            if (!Uri.IsHexDigit(e.KeyChar)) // Only allow hex characters (0-9, a-f, A-F)
                e.Handled = true;
        }

        public static byte[] HexToBytes(string hexString, int targetLength)
        {
            byte[] bytes = hexString
                .Split(new[] { '-', ':', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(b => Convert.ToByte(b, 16))
                .ToArray();
            if (bytes.Length < targetLength)
            {
                Array.Resize(ref bytes, targetLength);
            }
            return bytes;
        }
        public static string BytesToHex(byte[] data)
        {
            return string.Join("-", data.Select(b => b.ToString("X2")));
        }

        private void pnlTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pnlMinimize_MouseEnter(object sender, EventArgs e)
        {
            pnlMinimize.BackColor = Color.FromArgb(0x3f, 0x3f, 0x41);
            lblMinimize.ForeColor = Color.FromArgb(255, 0, 0);
        }
        private void pnlMinimize_MouseLeave(object sender, EventArgs e)
        {
            pnlMinimize.BackColor = Color.FromArgb(45, 45, 48);
            lblMinimize.ForeColor = Color.FromArgb(224, 224, 224);
        }
        private void pnlMinimize_Click(object sender, EventArgs e)
        {
            pnlMinimize.BackColor = Color.FromArgb(45, 45, 48);
            this.WindowState = FormWindowState.Minimized;
        }

        private void pnlExit_MouseEnter(object sender, EventArgs e)
        {
            pnlExit.BackColor = Color.FromArgb(0x3f, 0x3f, 0x41);
            lblExit.ForeColor = Color.FromArgb(255, 0, 0);
        }
        private void pnlExit_MouseLeave(object sender, EventArgs e)
        {
            pnlExit.BackColor = Color.FromArgb(45, 45, 48);
            lblExit.ForeColor = Color.FromArgb(224, 224, 224);
        }
        private void pnlExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private FormWindowState previousWindowState;
        private Rectangle previousBounds;
        private void pnlSmall_Click(object sender, EventArgs e)
        {
            //this.Size = new Size(100, 100);
            //this.WindowState = FormWindowState.Maximized;
            if (this.WindowState != FormWindowState.Maximized)
            {
                // Save current bounds and state
                previousWindowState = this.WindowState;
                previousBounds = this.Bounds;

                // Maximize the form
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                // Restore previous state and bounds
                this.WindowState = previousWindowState;

                // Only restore size/position if returning to normal
                if (previousWindowState == FormWindowState.Normal)
                {
                    this.Bounds = previousBounds;
                }
            }
        }
        private void pnlSmall_MouseEnter(object sender, EventArgs e)
        {
            pnlSmall.BackColor = Color.FromArgb(0x3f, 0x3f, 0x41);
            lblSmall.ForeColor = Color.FromArgb(255, 0, 0);
        }
        private void pnlSmall_MouseLeave(object sender, EventArgs e)
        {
            pnlSmall.BackColor = Color.FromArgb(45, 45, 48);
            lblSmall.ForeColor = Color.FromArgb(224, 224, 224);
        }


        private void Main_Resize(object sender, EventArgs e)
        {
            this.Refresh();
            this.Invalidate();
        }

    }

    public class ComboItem
    {
        public string Text { get; set; }
        public Color Color { get; set; }
        public ComboItem(string text, Color color)
        {
            Text = text;
            Color = color;
        }
        public override string ToString() => Text; // for .Text binding, etc.
    }

    public class ProgressPanel : Panel
    {
        private int min = 0;
        private int max = 100;
        private int value = 0;
        public int Minimum
        {
            get => min;
            set { min = value; Invalidate(); }
        }
        public int Maximum
        {
            get => max;
            set { max = value; Invalidate(); }
        }
        public int Value
        {
            get => this.value;
            set
            {
                this.value = Math.Max(min, Math.Min(max, value));
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float ratio = (float)(Value - Minimum) / (Maximum - Minimum);
            int fillWidth = (int)(Width * ratio);

            using var fillBrush = new SolidBrush(this.ForeColor);
            using var backBrush = new SolidBrush(this.BackColor);
            //using var borderPen = new Pen(Color.Black);

            e.Graphics.FillRectangle(fillBrush, 0, 0, fillWidth, Height);
            e.Graphics.FillRectangle(backBrush, fillWidth, 0, Width - fillWidth, Height);
           // e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
        }
    }


    /// <summary>
    /// Extension methods UI interface.
    /// </summary>
    public static class UIExtension
    {
       // public static TextBox? StatusTextBox { get; set; }
        public static RichTextBox? StatusTextBox { get; set; }
        public static ProgressPanel? ProgressBar { get; set; }
        public static GroupBox? GroupBox { get; set; }
        public static ComboBox? cbProgrammer { get; set; }

        public static void ThreadSafe(this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)
                control.Invoke(action);
            else
                action();
        }
        public static void ToStatus(string line, Color color)
        {
            if (StatusTextBox == null) Console.Write(line);
            else
            {
                StatusTextBox.ThreadSafe(() =>
                {
                    StatusTextBox.SelectionLength = 0;
                    StatusTextBox.SelectionStart = StatusTextBox.Text.Length;
                    StatusTextBox.SelectionColor = color;
                    StatusTextBox.AppendText(line.Replace("\n", "\n "));
                    StatusTextBox.SelectionStart = StatusTextBox.Text.Length;
                    StatusTextBox.ScrollToCaret();
                });
            }
        }

        public static void ToStatus(string line)
        {
            if (StatusTextBox == null) Console.Write(line);
            else
            {
                StatusTextBox.ThreadSafe(() =>
                {
                    StatusTextBox.SelectionLength = 0;
                    StatusTextBox.SelectionStart = StatusTextBox.Text.Length;
                    StatusTextBox.SelectionColor = StatusTextBox.ForeColor;
                    StatusTextBox.AppendText(line.Replace("\n", "\n "));
                    StatusTextBox.SelectionStart = StatusTextBox.Text.Length;
                    StatusTextBox.ScrollToCaret();
                });
            }
        }
        public static void ToError(string line)
        {
            ToStatus(line, Color.Red);
        }
        public static void ToStatus(string id, byte[] response)
        {
            string hexString = "0x" + BitConverter.ToString(response).Replace("-", "");
            ToStatus("\r\n" + id + hexString);
        }
        public static void Progress(uint value, uint max)
        {
            if (max > 0xFFFFFF)
            {
                max >>= 8;
                value >>= 8;
            }
            Progress((int)value, (int)max);
        }
        public static void Progress(int value, int max)
        {
            if (ProgressBar == null) return;
            ProgressBar.ThreadSafe(() =>
            {
                if (max != 0)
                {
                    ProgressBar.Maximum = max;
                    ProgressBar.Value = value;
                    GroupBox!.Enabled = false;
                }
                else
                {
                    ProgressBar.Maximum = 1;
                    ProgressBar.Value = 1;
                    GroupBox!.Enabled = true;
                }

            });
        }
        public static object? GetSelectedProgrammer()
        {
            object? selectedProg = null;
            cbProgrammer!.ThreadSafe(() =>
            {
                selectedProg = cbProgrammer!.SelectedItem;
            });
            return selectedProg;

        }
    }


}
