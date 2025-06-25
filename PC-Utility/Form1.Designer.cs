namespace CmsisDap_Communicator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tbStatus = new RichTextBox();
            ofd = new OpenFileDialog();
            btScanUSB = new Button();
            cbProgs = new ComboBox();
            groupBox1 = new GroupBox();
            gbCommunicate = new GroupBox();
            btReadLed = new Button();
            btReadAdcVal = new Button();
            btReadKeys = new Button();
            btReadFwInfo = new Button();
            btReadStack = new Button();
            cbLedBlue = new CheckBox();
            cbLedRed = new CheckBox();
            btSetLeds = new Button();
            tbLoRaAppKey = new TextBox();
            tbLoRaAppEUI = new TextBox();
            tbLoRaDevEUI = new TextBox();
            btSetKeys = new Button();
            btAcquire = new Button();
            btReset = new Button();
            gbOptional = new GroupBox();
            btExit = new Button();
            pnlSmall = new Panel();
            lblSmall = new Label();
            pnlMinimize = new Panel();
            lblMinimize = new Label();
            pnlExit = new Panel();
            lblExit = new Label();
            pnlTop = new Panel();
            lbFileName = new Label();
            lblTop = new Label();
            pictureBox1 = new PictureBox();
            pnlProgress = new ProgressPanel();
            groupBox1.SuspendLayout();
            gbCommunicate.SuspendLayout();
            gbOptional.SuspendLayout();
            pnlSmall.SuspendLayout();
            pnlMinimize.SuspendLayout();
            pnlExit.SuspendLayout();
            pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tbStatus
            // 
            tbStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbStatus.BackColor = Color.FromArgb(60, 60, 60);
            tbStatus.BorderStyle = BorderStyle.None;
            tbStatus.ForeColor = Color.Aqua;
            tbStatus.Location = new Point(8, 254);
            tbStatus.Name = "tbStatus";
            tbStatus.Size = new Size(984, 448);
            tbStatus.TabIndex = 3;
            tbStatus.Text = "";
            // 
            // btScanUSB
            // 
            btScanUSB.BackColor = Color.SteelBlue;
            btScanUSB.FlatStyle = FlatStyle.Popup;
            btScanUSB.Location = new Point(6, 21);
            btScanUSB.Name = "btScanUSB";
            btScanUSB.Size = new Size(134, 25);
            btScanUSB.TabIndex = 0;
            btScanUSB.Text = "Scan USB";
            btScanUSB.UseVisualStyleBackColor = false;
            btScanUSB.Click += btScanUSB_Click;
            // 
            // cbProgs
            // 
            cbProgs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbProgs.BackColor = Color.FromArgb(60, 60, 60);
            cbProgs.DropDownWidth = 400;
            cbProgs.FlatStyle = FlatStyle.Popup;
            cbProgs.ForeColor = Color.Aqua;
            cbProgs.FormattingEnabled = true;
            cbProgs.Location = new Point(146, 22);
            cbProgs.Name = "cbProgs";
            cbProgs.Size = new Size(829, 23);
            cbProgs.TabIndex = 1;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.BackColor = Color.Transparent;
            groupBox1.Controls.Add(btScanUSB);
            groupBox1.Controls.Add(cbProgs);
            groupBox1.ForeColor = Color.Aqua;
            groupBox1.Location = new Point(8, 38);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(984, 56);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Select programmer";
            // 
            // gbCommunicate
            // 
            gbCommunicate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbCommunicate.BackColor = Color.Transparent;
            gbCommunicate.Controls.Add(btReadLed);
            gbCommunicate.Controls.Add(btReadAdcVal);
            gbCommunicate.Controls.Add(btReadKeys);
            gbCommunicate.Controls.Add(btReadFwInfo);
            gbCommunicate.Controls.Add(btReadStack);
            gbCommunicate.Controls.Add(cbLedBlue);
            gbCommunicate.Controls.Add(cbLedRed);
            gbCommunicate.Controls.Add(btSetLeds);
            gbCommunicate.Controls.Add(tbLoRaAppKey);
            gbCommunicate.Controls.Add(tbLoRaAppEUI);
            gbCommunicate.Controls.Add(tbLoRaDevEUI);
            gbCommunicate.Controls.Add(btSetKeys);
            gbCommunicate.ForeColor = Color.Aqua;
            gbCommunicate.Location = new Point(8, 100);
            gbCommunicate.Name = "gbCommunicate";
            gbCommunicate.Size = new Size(984, 86);
            gbCommunicate.TabIndex = 1;
            gbCommunicate.TabStop = false;
            gbCommunicate.Text = "Communicate";
            // 
            // btReadLed
            // 
            btReadLed.BackColor = Color.SteelBlue;
            btReadLed.FlatStyle = FlatStyle.Popup;
            btReadLed.Location = new Point(566, 52);
            btReadLed.Name = "btReadLed";
            btReadLed.Size = new Size(134, 25);
            btReadLed.TabIndex = 8;
            btReadLed.Text = "Read LED Status";
            btReadLed.UseVisualStyleBackColor = false;
            btReadLed.Click += btReadLed_Click;
            // 
            // btReadAdcVal
            // 
            btReadAdcVal.BackColor = Color.SteelBlue;
            btReadAdcVal.FlatStyle = FlatStyle.Popup;
            btReadAdcVal.Location = new Point(426, 52);
            btReadAdcVal.Name = "btReadAdcVal";
            btReadAdcVal.Size = new Size(134, 25);
            btReadAdcVal.TabIndex = 7;
            btReadAdcVal.Text = "Read ADC Value";
            btReadAdcVal.UseVisualStyleBackColor = false;
            btReadAdcVal.Click += btReadAdcVal_Click;
            // 
            // btReadKeys
            // 
            btReadKeys.BackColor = Color.SteelBlue;
            btReadKeys.FlatStyle = FlatStyle.Popup;
            btReadKeys.Location = new Point(6, 52);
            btReadKeys.Name = "btReadKeys";
            btReadKeys.Size = new Size(134, 25);
            btReadKeys.TabIndex = 4;
            btReadKeys.Text = "Read LoRaWAN Keys";
            btReadKeys.UseVisualStyleBackColor = false;
            btReadKeys.Click += btReadKeys_Click;
            // 
            // btReadFwInfo
            // 
            btReadFwInfo.BackColor = Color.SteelBlue;
            btReadFwInfo.FlatStyle = FlatStyle.Popup;
            btReadFwInfo.Location = new Point(286, 52);
            btReadFwInfo.Name = "btReadFwInfo";
            btReadFwInfo.Size = new Size(134, 25);
            btReadFwInfo.TabIndex = 6;
            btReadFwInfo.Text = "Read FW Info";
            btReadFwInfo.UseVisualStyleBackColor = false;
            btReadFwInfo.Click += btReadFwInfo_Click;
            // 
            // btReadStack
            // 
            btReadStack.BackColor = Color.SteelBlue;
            btReadStack.FlatStyle = FlatStyle.Popup;
            btReadStack.Location = new Point(146, 52);
            btReadStack.Name = "btReadStack";
            btReadStack.Size = new Size(134, 25);
            btReadStack.TabIndex = 5;
            btReadStack.Text = "Read Stack Info";
            btReadStack.UseVisualStyleBackColor = false;
            btReadStack.Click += btReadStack_Click;
            // 
            // cbLedBlue
            // 
            cbLedBlue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbLedBlue.AutoSize = true;
            cbLedBlue.Location = new Point(893, 63);
            cbLedBlue.Name = "cbLedBlue";
            cbLedBlue.RightToLeft = RightToLeft.Yes;
            cbLedBlue.Size = new Size(82, 19);
            cbLedBlue.TabIndex = 11;
            cbLedBlue.Text = "Blue LED";
            cbLedBlue.UseVisualStyleBackColor = true;
            // 
            // cbLedRed
            // 
            cbLedRed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbLedRed.AutoSize = true;
            cbLedRed.Location = new Point(900, 47);
            cbLedRed.Name = "cbLedRed";
            cbLedRed.RightToLeft = RightToLeft.Yes;
            cbLedRed.Size = new Size(75, 19);
            cbLedRed.TabIndex = 10;
            cbLedRed.Text = "Red LED";
            cbLedRed.UseVisualStyleBackColor = true;
            // 
            // btSetLeds
            // 
            btSetLeds.BackColor = Color.SteelBlue;
            btSetLeds.FlatStyle = FlatStyle.Popup;
            btSetLeds.Location = new Point(706, 52);
            btSetLeds.Name = "btSetLeds";
            btSetLeds.Size = new Size(134, 25);
            btSetLeds.TabIndex = 9;
            btSetLeds.Text = "Set Leds";
            btSetLeds.UseVisualStyleBackColor = false;
            btSetLeds.Click += btSetLeds_Click;
            // 
            // tbLoRaAppKey
            // 
            tbLoRaAppKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbLoRaAppKey.BackColor = Color.FromArgb(60, 60, 60);
            tbLoRaAppKey.BorderStyle = BorderStyle.FixedSingle;
            tbLoRaAppKey.ForeColor = Color.Aqua;
            tbLoRaAppKey.Location = new Point(567, 22);
            tbLoRaAppKey.MaxLength = 47;
            tbLoRaAppKey.Name = "tbLoRaAppKey";
            tbLoRaAppKey.PlaceholderText = "AppKey ";
            tbLoRaAppKey.Size = new Size(409, 23);
            tbLoRaAppKey.TabIndex = 3;
            tbLoRaAppKey.TextChanged += tbHEX_TextChanged;
            tbLoRaAppKey.KeyPress += tbHEX_KeyPress;
            // 
            // tbLoRaAppEUI
            // 
            tbLoRaAppEUI.BackColor = Color.FromArgb(60, 60, 60);
            tbLoRaAppEUI.BorderStyle = BorderStyle.FixedSingle;
            tbLoRaAppEUI.ForeColor = Color.Aqua;
            tbLoRaAppEUI.Location = new Point(357, 22);
            tbLoRaAppEUI.MaxLength = 23;
            tbLoRaAppEUI.Name = "tbLoRaAppEUI";
            tbLoRaAppEUI.PlaceholderText = "JoinEUI / AppEUI";
            tbLoRaAppEUI.Size = new Size(202, 23);
            tbLoRaAppEUI.TabIndex = 2;
            tbLoRaAppEUI.TextChanged += tbHEX_TextChanged;
            tbLoRaAppEUI.KeyPress += tbHEX_KeyPress;
            // 
            // tbLoRaDevEUI
            // 
            tbLoRaDevEUI.BackColor = Color.FromArgb(60, 60, 60);
            tbLoRaDevEUI.BorderStyle = BorderStyle.FixedSingle;
            tbLoRaDevEUI.ForeColor = Color.Aqua;
            tbLoRaDevEUI.Location = new Point(147, 22);
            tbLoRaDevEUI.MaxLength = 23;
            tbLoRaDevEUI.Name = "tbLoRaDevEUI";
            tbLoRaDevEUI.PlaceholderText = "DevEUI";
            tbLoRaDevEUI.Size = new Size(202, 23);
            tbLoRaDevEUI.TabIndex = 1;
            tbLoRaDevEUI.TextChanged += tbHEX_TextChanged;
            tbLoRaDevEUI.KeyPress += tbHEX_KeyPress;
            // 
            // btSetKeys
            // 
            btSetKeys.BackColor = Color.SteelBlue;
            btSetKeys.FlatStyle = FlatStyle.Popup;
            btSetKeys.Location = new Point(6, 21);
            btSetKeys.Name = "btSetKeys";
            btSetKeys.Size = new Size(134, 25);
            btSetKeys.TabIndex = 0;
            btSetKeys.Text = "Set LoRaWAN Keys";
            btSetKeys.UseVisualStyleBackColor = false;
            btSetKeys.Click += btSetKeys_Click;
            // 
            // btAcquire
            // 
            btAcquire.BackColor = Color.SteelBlue;
            btAcquire.FlatStyle = FlatStyle.Popup;
            btAcquire.Location = new Point(6, 22);
            btAcquire.Name = "btAcquire";
            btAcquire.Size = new Size(134, 25);
            btAcquire.TabIndex = 0;
            btAcquire.Text = "Acquire";
            btAcquire.UseVisualStyleBackColor = false;
            btAcquire.Click += btAcquire_Click;
            // 
            // btReset
            // 
            btReset.BackColor = Color.SteelBlue;
            btReset.FlatStyle = FlatStyle.Popup;
            btReset.Location = new Point(146, 22);
            btReset.Name = "btReset";
            btReset.Size = new Size(134, 25);
            btReset.TabIndex = 1;
            btReset.Text = "Reset";
            btReset.UseVisualStyleBackColor = false;
            btReset.Click += btReset_Click;
            // 
            // gbOptional
            // 
            gbOptional.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbOptional.BackColor = Color.Transparent;
            gbOptional.Controls.Add(btExit);
            gbOptional.Controls.Add(btAcquire);
            gbOptional.Controls.Add(btReset);
            gbOptional.ForeColor = Color.Aqua;
            gbOptional.Location = new Point(8, 192);
            gbOptional.Name = "gbOptional";
            gbOptional.Size = new Size(984, 56);
            gbOptional.TabIndex = 2;
            gbOptional.TabStop = false;
            gbOptional.Text = "Optional functions (not needed for communication)";
            // 
            // btExit
            // 
            btExit.BackColor = Color.SteelBlue;
            btExit.FlatStyle = FlatStyle.Popup;
            btExit.Location = new Point(286, 22);
            btExit.Name = "btExit";
            btExit.Size = new Size(134, 25);
            btExit.TabIndex = 2;
            btExit.Text = "Exit";
            btExit.UseVisualStyleBackColor = false;
            btExit.Click += btExit_Click;
            // 
            // pnlSmall
            // 
            pnlSmall.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlSmall.BackColor = Color.FromArgb(45, 45, 48);
            pnlSmall.Controls.Add(lblSmall);
            pnlSmall.Location = new Point(920, 1);
            pnlSmall.Margin = new Padding(4, 3, 4, 3);
            pnlSmall.Name = "pnlSmall";
            pnlSmall.Size = new Size(40, 32);
            pnlSmall.TabIndex = 162;
            pnlSmall.Click += pnlSmall_Click;
            pnlSmall.MouseEnter += pnlSmall_MouseEnter;
            pnlSmall.MouseLeave += pnlSmall_MouseLeave;
            // 
            // lblSmall
            // 
            lblSmall.AutoSize = true;
            lblSmall.Font = new Font("Wingdings", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            lblSmall.ForeColor = Color.WhiteSmoke;
            lblSmall.Location = new Point(10, 9);
            lblSmall.Margin = new Padding(4, 0, 4, 0);
            lblSmall.Name = "lblSmall";
            lblSmall.Size = new Size(20, 16);
            lblSmall.TabIndex = 0;
            lblSmall.Text = "o";
            lblSmall.Click += pnlSmall_Click;
            lblSmall.MouseEnter += pnlSmall_MouseEnter;
            lblSmall.MouseLeave += pnlSmall_MouseLeave;
            // 
            // pnlMinimize
            // 
            pnlMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlMinimize.BackColor = Color.FromArgb(45, 45, 48);
            pnlMinimize.Controls.Add(lblMinimize);
            pnlMinimize.Location = new Point(880, 1);
            pnlMinimize.Margin = new Padding(4, 3, 4, 3);
            pnlMinimize.Name = "pnlMinimize";
            pnlMinimize.Size = new Size(40, 32);
            pnlMinimize.TabIndex = 161;
            pnlMinimize.Click += pnlMinimize_Click;
            pnlMinimize.MouseEnter += pnlMinimize_MouseEnter;
            pnlMinimize.MouseLeave += pnlMinimize_MouseLeave;
            // 
            // lblMinimize
            // 
            lblMinimize.AutoSize = true;
            lblMinimize.Font = new Font("Webdings", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            lblMinimize.ForeColor = Color.WhiteSmoke;
            lblMinimize.Location = new Point(8, 5);
            lblMinimize.Margin = new Padding(4, 0, 4, 0);
            lblMinimize.Name = "lblMinimize";
            lblMinimize.Size = new Size(24, 20);
            lblMinimize.TabIndex = 0;
            lblMinimize.Text = "0";
            lblMinimize.Click += pnlMinimize_Click;
            lblMinimize.MouseEnter += pnlMinimize_MouseEnter;
            lblMinimize.MouseLeave += pnlMinimize_MouseLeave;
            // 
            // pnlExit
            // 
            pnlExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlExit.BackColor = Color.FromArgb(45, 45, 48);
            pnlExit.Controls.Add(lblExit);
            pnlExit.Location = new Point(960, 1);
            pnlExit.Margin = new Padding(4, 3, 4, 3);
            pnlExit.Name = "pnlExit";
            pnlExit.Size = new Size(40, 32);
            pnlExit.TabIndex = 160;
            pnlExit.Click += pnlExit_Click;
            pnlExit.MouseEnter += pnlExit_MouseEnter;
            pnlExit.MouseLeave += pnlExit_MouseLeave;
            // 
            // lblExit
            // 
            lblExit.AutoSize = true;
            lblExit.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblExit.ForeColor = Color.FromArgb(224, 224, 224);
            lblExit.Location = new Point(12, 8);
            lblExit.Margin = new Padding(4, 0, 4, 0);
            lblExit.Name = "lblExit";
            lblExit.Size = new Size(16, 15);
            lblExit.TabIndex = 1;
            lblExit.Text = "X";
            lblExit.Click += pnlExit_Click;
            lblExit.MouseEnter += pnlExit_MouseEnter;
            lblExit.MouseLeave += pnlExit_MouseLeave;
            // 
            // pnlTop
            // 
            pnlTop.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlTop.BackColor = Color.FromArgb(45, 45, 48);
            pnlTop.Controls.Add(lbFileName);
            pnlTop.Controls.Add(lblTop);
            pnlTop.Controls.Add(pictureBox1);
            pnlTop.Location = new Point(1, 1);
            pnlTop.Margin = new Padding(4, 3, 4, 3);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(879, 32);
            pnlTop.TabIndex = 159;
            pnlTop.MouseDown += pnlTop_MouseDown;
            // 
            // lbFileName
            // 
            lbFileName.AutoSize = true;
            lbFileName.Font = new Font("Verdana", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbFileName.ForeColor = Color.Turquoise;
            lbFileName.Location = new Point(66, 8);
            lbFileName.Margin = new Padding(4, 0, 4, 0);
            lbFileName.Name = "lbFileName";
            lbFileName.RightToLeft = RightToLeft.No;
            lbFileName.Size = new Size(0, 14);
            lbFileName.TabIndex = 5;
            // 
            // lblTop
            // 
            lblTop.AutoSize = true;
            lblTop.Font = new Font("Verdana", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTop.ForeColor = Color.Turquoise;
            lblTop.Location = new Point(44, 8);
            lblTop.Margin = new Padding(4, 0, 4, 0);
            lblTop.Name = "lblTop";
            lblTop.RightToLeft = RightToLeft.Yes;
            lblTop.Size = new Size(12, 14);
            lblTop.TabIndex = 4;
            lblTop.Text = "-";
            lblTop.MouseDown += pnlTop_MouseDown;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(9, 2);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(28, 28);
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // pnlProgress
            // 
            pnlProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlProgress.BackColor = Color.FromArgb(60, 60, 60);
            pnlProgress.ForeColor = Color.MediumAquamarine;
            pnlProgress.Location = new Point(8, 709);
            pnlProgress.Margin = new Padding(4, 3, 4, 3);
            pnlProgress.Maximum = 100;
            pnlProgress.Minimum = 0;
            pnlProgress.Name = "pnlProgress";
            pnlProgress.Size = new Size(984, 14);
            pnlProgress.TabIndex = 163;
            pnlProgress.Value = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(20, 20, 20);
            ClientSize = new Size(1000, 732);
            Controls.Add(pnlProgress);
            Controls.Add(pnlSmall);
            Controls.Add(pnlMinimize);
            Controls.Add(pnlExit);
            Controls.Add(pnlTop);
            Controls.Add(gbOptional);
            Controls.Add(gbCommunicate);
            Controls.Add(groupBox1);
            Controls.Add(tbStatus);
            Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.Aqua;
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(950, 400);
            Name = "Form1";
            Text = "PSoC6 CMSIS-DAP Programmer V 1.0 (c) Rolf Nooteboom";
            Resize += Main_Resize;
            groupBox1.ResumeLayout(false);
            gbCommunicate.ResumeLayout(false);
            gbCommunicate.PerformLayout();
            gbOptional.ResumeLayout(false);
            pnlSmall.ResumeLayout(false);
            pnlSmall.PerformLayout();
            pnlMinimize.ResumeLayout(false);
            pnlMinimize.PerformLayout();
            pnlExit.ResumeLayout(false);
            pnlExit.PerformLayout();
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public RichTextBox tbStatus;
        private OpenFileDialog ofd;
        private Button btScanUSB;
        private ComboBox cbProgs;
        private GroupBox groupBox1;
        private GroupBox gbCommunicate;
        private TextBox tbLoRaDevEUI;
        private Button btSetKeys;
        private Button btAcquire;
        private Button btReset;
        private TextBox tbLoRaAppKey;
        private TextBox tbLoRaAppEUI;
        private Button btSetLeds;
        private CheckBox cbLedBlue;
        private CheckBox cbLedRed;
        private Button btReadStack;
        private Button btReadFwInfo;
        private Button btReadLed;
        private Button btReadAdcVal;
        private Button btReadKeys;
        private GroupBox gbOptional;
        private Panel pnlSmall;
        private Label lblSmall;
        private Panel pnlMinimize;
        private Label lblMinimize;
        private Panel pnlExit;
        private Label lblExit;
        private Panel pnlTop;
        private Label lbFileName;
        private Label lblTop;
        private PictureBox pictureBox1;
        private ProgressPanel pnlProgress;
        private Button btExit;
    }
}
