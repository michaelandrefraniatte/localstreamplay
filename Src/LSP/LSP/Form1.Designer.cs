
namespace LSP
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.LabelServerIP = new System.Windows.Forms.Label();
            this.TextBoxServerIP = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.fastColoredTextBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
            this.autocompleteMenu1 = new AutocompleteMenuNS.AutocompleteMenu();
            ((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(230, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 104;
            this.label2.Text = "Height";
            // 
            // textBox2
            // 
            this.autocompleteMenu1.SetAutocompleteMenu(this.textBox2, null);
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.Color.DimGray;
            this.textBox2.Location = new System.Drawing.Point(303, 59);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(93, 20);
            this.textBox2.TabIndex = 103;
            this.textBox2.Text = "768";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 102;
            this.label1.Text = "Width";
            // 
            // textBox1
            // 
            this.autocompleteMenu1.SetAutocompleteMenu(this.textBox1, null);
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.DimGray;
            this.textBox1.Location = new System.Drawing.Point(106, 59);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(94, 20);
            this.textBox1.TabIndex = 101;
            this.textBox1.Text = "1360";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(328, 146);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(68, 23);
            this.button6.TabIndex = 100;
            this.button6.Text = "Save As";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(254, 146);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(68, 23);
            this.button5.TabIndex = 99;
            this.button5.Text = "Save";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(180, 146);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(68, 23);
            this.button4.TabIndex = 98;
            this.button4.Text = "Open";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(106, 146);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(68, 23);
            this.button3.TabIndex = 97;
            this.button3.Text = "Test";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(32, 146);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(68, 23);
            this.button2.TabIndex = 96;
            this.button2.Text = "Run";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // LabelServerIP
            // 
            this.LabelServerIP.AutoSize = true;
            this.LabelServerIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelServerIP.Location = new System.Drawing.Point(29, 26);
            this.LabelServerIP.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelServerIP.Name = "LabelServerIP";
            this.LabelServerIP.Size = new System.Drawing.Size(58, 13);
            this.LabelServerIP.TabIndex = 93;
            this.LabelServerIP.Text = "IP Address";
            // 
            // TextBoxServerIP
            // 
            this.autocompleteMenu1.SetAutocompleteMenu(this.TextBoxServerIP, null);
            this.TextBoxServerIP.BackColor = System.Drawing.Color.White;
            this.TextBoxServerIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxServerIP.ForeColor = System.Drawing.Color.DimGray;
            this.TextBoxServerIP.Location = new System.Drawing.Point(106, 23);
            this.TextBoxServerIP.Margin = new System.Windows.Forms.Padding(2);
            this.TextBoxServerIP.Name = "TextBoxServerIP";
            this.TextBoxServerIP.Size = new System.Drawing.Size(290, 20);
            this.TextBoxServerIP.TabIndex = 92;
            this.TextBoxServerIP.Text = "10.0.0.13";
            this.TextBoxServerIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(32, 96);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(364, 39);
            this.button1.TabIndex = 94;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "script_16x16.png");
            this.imageList1.Images.SetKeyName(1, "app_16x16.png");
            this.imageList1.Images.SetKeyName(2, "1302166543_virtualbox.png");
            // 
            // fastColoredTextBox1
            // 
            this.fastColoredTextBox1.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.autocompleteMenu1.SetAutocompleteMenu(this.fastColoredTextBox1, this.autocompleteMenu1);
            this.fastColoredTextBox1.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;]+);\r\n^\\s*(case|default)\\s*[^:]" +
    "*(?<range>:)\\s*(?<range>[^;]+);\r\n";
            this.fastColoredTextBox1.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.fastColoredTextBox1.BackBrush = null;
            this.fastColoredTextBox1.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy2;
            this.fastColoredTextBox1.CharHeight = 14;
            this.fastColoredTextBox1.CharWidth = 8;
            this.fastColoredTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fastColoredTextBox1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fastColoredTextBox1.IsReplaceMode = false;
            this.fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.CSharp;
            this.fastColoredTextBox1.LeftBracket = '(';
            this.fastColoredTextBox1.LeftBracket2 = '{';
            this.fastColoredTextBox1.Location = new System.Drawing.Point(32, 189);
            this.fastColoredTextBox1.Name = "fastColoredTextBox1";
            this.fastColoredTextBox1.Paddings = new System.Windows.Forms.Padding(0);
            this.fastColoredTextBox1.RightBracket = ')';
            this.fastColoredTextBox1.RightBracket2 = '}';
            this.fastColoredTextBox1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fastColoredTextBox1.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fastColoredTextBox1.ServiceColors")));
            this.fastColoredTextBox1.Size = new System.Drawing.Size(364, 319);
            this.fastColoredTextBox1.TabIndex = 105;
            this.fastColoredTextBox1.Zoom = 100;
            this.fastColoredTextBox1.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fastColoredTextBox1_TextChanged);
            this.fastColoredTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.fastColoredTextBox1_KeyUp);
            // 
            // autocompleteMenu1
            // 
            this.autocompleteMenu1.AllowsTabKey = true;
            this.autocompleteMenu1.Colors = ((AutocompleteMenuNS.Colors)(resources.GetObject("autocompleteMenu1.Colors")));
            this.autocompleteMenu1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.autocompleteMenu1.ImageList = null;
            this.autocompleteMenu1.Items = new string[] {
        "kmevent",
        "sendinput",
        "keys12345",
        "keys54321",
        "wd",
        "wu",
        "valchanged",
        "Scale",
        "hostscreenwidth",
        "hostscreenheight",
        "width",
        "height",
        "MouseHookX",
        "MouseHookY",
        "MouseHookButtonX1",
        "MouseHookButtonX2",
        "MouseHookWheelUp",
        "MouseHookWheelDown",
        "MouseHookRightButton",
        "MouseHookLeftButton",
        "MouseHookMiddleButton",
        "MouseHookXButton",
        "Key_LBUTTON",
        "Key_RBUTTON",
        "Key_CANCEL",
        "Key_MBUTTON",
        "Key_XBUTTON1",
        "Key_XBUTTON2",
        "Key_BACK",
        "Key_Tab",
        "Key_CLEAR",
        "Key_Return",
        "Key_SHIFT",
        "Key_CONTROL",
        "Key_MENU",
        "Key_PAUSE",
        "Key_CAPITAL",
        "Key_KANA",
        "Key_HANGEUL",
        "Key_HANGUL",
        "Key_JUNJA",
        "Key_FINAL",
        "Key_HANJA",
        "Key_KANJI",
        "Key_Escape",
        "Key_CONVERT",
        "Key_NONCONVERT",
        "Key_ACCEPT",
        "Key_MODECHANGE",
        "Key_Space",
        "Key_PRIOR",
        "Key_NEXT",
        "Key_END",
        "Key_HOME",
        "Key_LEFT",
        "Key_UP",
        "Key_RIGHT",
        "Key_DOWN",
        "Key_SELECT",
        "Key_PRINT",
        "Key_EXECUTE",
        "Key_SNAPSHOT",
        "Key_INSERT",
        "Key_DELETE",
        "Key_HELP",
        "Key_APOSTROPHE",
        "Key_0",
        "Key_1",
        "Key_2",
        "Key_3",
        "Key_4",
        "Key_5",
        "Key_6",
        "Key_7",
        "Key_8",
        "Key_9",
        "Key_A",
        "Key_B",
        "Key_C",
        "Key_D",
        "Key_E",
        "Key_F",
        "Key_G",
        "Key_H",
        "Key_I",
        "Key_J",
        "Key_K",
        "Key_L",
        "Key_M",
        "Key_N",
        "Key_O",
        "Key_P",
        "Key_Q",
        "Key_R",
        "Key_S",
        "Key_T",
        "Key_U",
        "Key_V",
        "Key_W",
        "Key_X",
        "Key_Y",
        "Key_Z",
        "Key_LWIN",
        "Key_RWIN",
        "Key_APPS",
        "Key_SLEEP",
        "Key_NUMPAD0",
        "Key_NUMPAD1",
        "Key_NUMPAD2",
        "Key_NUMPAD3",
        "Key_NUMPAD4",
        "Key_NUMPAD5",
        "Key_NUMPAD6",
        "Key_NUMPAD7",
        "Key_NUMPAD8",
        "Key_NUMPAD9",
        "Key_MULTIPLY",
        "Key_ADD",
        "Key_SEPARATOR",
        "Key_SUBTRACT",
        "Key_DECIMAL",
        "Key_DIVIDE",
        "Key_F1",
        "Key_F2",
        "Key_F3",
        "Key_F4",
        "Key_F5",
        "Key_F6",
        "Key_F7",
        "Key_F8",
        "Key_F9",
        "Key_F10",
        "Key_F11",
        "Key_F12",
        "Key_F13",
        "Key_F14",
        "Key_F15",
        "Key_F16",
        "Key_F17",
        "Key_F18",
        "Key_F19",
        "Key_F20",
        "Key_F21",
        "Key_F22",
        "Key_F23",
        "Key_F24",
        "Key_NUMLOCK",
        "Key_SCROLL",
        "Key_LeftShift",
        "Key_RightShift",
        "Key_LeftControl",
        "Key_RightControl",
        "Key_LMENU",
        "Key_RMENU",
        "Key_BROWSER_BACK",
        "Key_BROWSER_FORWARD",
        "Key_BROWSER_REFRESH",
        "Key_BROWSER_STOP",
        "Key_BROWSER_SEARCH",
        "Key_BROWSER_FAVORITES",
        "Key_BROWSER_HOME",
        "Key_VOLUME_MUTE",
        "Key_VOLUME_DOWN",
        "Key_VOLUME_UP",
        "Key_MEDIA_NEXT_TRACK",
        "Key_MEDIA_PREV_TRACK",
        "Key_MEDIA_STOP",
        "Key_MEDIA_PLAY_PAUSE",
        "Key_LAUNCH_MAIL",
        "Key_LAUNCH_MEDIA_SELECT",
        "Key_LAUNCH_APP1",
        "Key_LAUNCH_APP2",
        "Key_OEM_1",
        "Key_OEM_PLUS",
        "Key_OEM_COMMA",
        "Key_OEM_MINUS",
        "Key_OEM_PERIOD",
        "Key_OEM_2",
        "Key_OEM_3",
        "Key_OEM_4",
        "Key_OEM_5",
        "Key_OEM_6",
        "Key_OEM_7",
        "Key_OEM_8",
        "Key_OEM_102",
        "Key_PROCESSKEY",
        "Key_PACKET",
        "Key_ATTN",
        "Key_CRSEL",
        "Key_EXSEL",
        "Key_EREOF",
        "Key_PLAY",
        "Key_ZOOM",
        "Key_NONAME",
        "Key_PA1",
        "Key_OEM_CLEAR",
        "ButtonAPressed",
        "ButtonBPressed",
        "ButtonXPressed",
        "ButtonYPressed",
        "ButtonStartPressed",
        "ButtonBackPressed",
        "ButtonDownPressed",
        "ButtonUpPressed",
        "ButtonLeftPressed",
        "ButtonRightPressed",
        "ButtonShoulderLeftPressed",
        "ButtonShoulderRightPressed",
        "ThumbpadLeftPressed",
        "ThumbpadRightPressed",
        "TriggerLeftPosition",
        "TriggerRightPosition",
        "ThumbLeftX",
        "ThumbLeftY",
        "ThumbRightX",
        "ThumbRightY",
        "pollcount",
        "sleeptime",
        "getstate",
        "KeyboardMouseDriverType",
        "MouseMoveX",
        "MouseMoveY",
        "MouseAbsX",
        "MouseAbsY",
        "MouseDesktopX",
        "MouseDesktopY",
        "SendLeftClick",
        "SendRightClick",
        "SendMiddleClick",
        "SendWheelUp",
        "SendWheelDown",
        "SendLeft",
        "SendRight",
        "SendUp",
        "SendDown",
        "SendLButton",
        "SendRButton",
        "SendCancel",
        "SendMBUTTON",
        "SendXBUTTON1",
        "SendXBUTTON2",
        "SendBack",
        "SendTab",
        "SendClear",
        "SendReturn",
        "SendSHIFT",
        "SendCONTROL",
        "SendMENU",
        "SendPAUSE",
        "SendCAPITAL",
        "SendKANA",
        "SendHANGEUL",
        "SendHANGUL",
        "SendJUNJA",
        "SendFINAL",
        "SendHANJA",
        "SendKANJI",
        "SendEscape",
        "SendCONVERT",
        "SendNONCONVERT",
        "SendACCEPT",
        "SendMODECHANGE",
        "SendSpace",
        "SendPRIOR",
        "SendNEXT",
        "SendEND",
        "SendHOME",
        "SendLEFT",
        "SendUP",
        "SendRIGHT",
        "SendDOWN",
        "SendSELECT",
        "SendPRINT",
        "SendEXECUTE",
        "SendSNAPSHOT",
        "SendINSERT",
        "SendDELETE",
        "SendHELP",
        "SendAPOSTROPHE",
        "Send0",
        "Send1",
        "Send2",
        "Send3",
        "Send4",
        "Send5",
        "Send6",
        "Send7",
        "Send8",
        "Send9",
        "SendA",
        "SendB",
        "SendC",
        "SendD",
        "SendE",
        "SendF",
        "SendG",
        "SendH",
        "SendI",
        "SendJ",
        "SendK",
        "SendL",
        "SendM",
        "SendN",
        "SendO",
        "SendP",
        "SendQ",
        "SendR",
        "SendS",
        "SendT",
        "SendU",
        "SendV",
        "SendW",
        "SendX",
        "SendY",
        "SendZ",
        "SendLWIN",
        "SendRWIN",
        "SendAPPS",
        "SendSLEEP",
        "SendNUMPAD0",
        "SendNUMPAD1",
        "SendNUMPAD2",
        "SendNUMPAD3",
        "SendNUMPAD4",
        "SendNUMPAD5",
        "SendNUMPAD6",
        "SendNUMPAD7",
        "SendNUMPAD8",
        "SendNUMPAD9",
        "SendMULTIPLY",
        "SendADD",
        "SendSEPARATOR",
        "SendSUBTRACT",
        "SendDECIMAL",
        "SendDIVIDE",
        "SendF1",
        "SendF2",
        "SendF3",
        "SendF4",
        "SendF5",
        "SendF6",
        "SendF7",
        "SendF8",
        "SendF9",
        "SendF10",
        "SendF11",
        "SendF12",
        "SendF13",
        "SendF14",
        "SendF15",
        "SendF16",
        "SendF17",
        "SendF18",
        "SendF19",
        "SendF20",
        "SendF21",
        "SendF22",
        "SendF23",
        "SendF24",
        "SendNUMLOCK",
        "SendSCROLL",
        "SendLeftShift",
        "SendRightShift",
        "SendLeftControl",
        "SendRightControl",
        "SendLMENU",
        "SendRMENU",
        "back",
        "start",
        "A",
        "B",
        "X",
        "Y",
        "up",
        "left",
        "down",
        "right",
        "leftstick",
        "rightstick",
        "leftbumper",
        "rightbumper",
        "lefttrigger",
        "righttrigger",
        "leftstickx",
        "leftsticky",
        "rightstickx",
        "rightsticky"};
            this.autocompleteMenu1.TargetControlWrapper = null;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(428, 533);
            this.Controls.Add(this.fastColoredTextBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.LabelServerIP);
            this.Controls.Add(this.TextBoxServerIP);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Local Stream Play";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label LabelServerIP;
        private System.Windows.Forms.TextBox TextBoxServerIP;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ImageList imageList1;
        private AutocompleteMenuNS.AutocompleteMenu autocompleteMenu1;
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBox1;
    }
}

