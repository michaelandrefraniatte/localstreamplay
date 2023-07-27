using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Diagnostics;
using NAudio.Wave;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WebSocketSharp;
using SharpDX.XInput;
using FastColoredTextBoxNS;
using Range = FastColoredTextBoxNS.Range;
using System.Drawing;
using NAudio.CoreAudioApi;

namespace LSP
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
        }
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
		public static extern uint TimeBeginPeriod(uint ms);
		[DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
		public static extern uint TimeEndPeriod(uint ms);
		[DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
		public static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        [DllImport("advapi32.dll")]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);
        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(out int x, out int y);
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int X, int Y);
        public static System.Collections.Generic.List<double> time = new System.Collections.Generic.List<double>();
        public static int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        public static int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        public static uint CurrentResolution = 0;
        public static string controlport = "64000", audioport = "62000", ip;
        public static bool closed = false;
        public static bool running = false, runningscript = false;
        public static string screenadress, audioadress;
        public static MouseHook mouseHook = new MouseHook();
        public static KeyboardHook keyboardHook = new KeyboardHook();
        public static double MouseHookX, MouseHookY;
        public static int MouseHookWheel, MouseDesktopHookX, MouseDesktopHookY, MouseHookButtonX, MouseHookTime, mousehookwheelcount, mousehookbuttoncount;
        public static bool MouseHookLeftButton, MouseHookRightButton, MouseHookLeftDoubleClick, MouseHookRightDoubleClick, MouseHookMiddleButton, MouseHookXButton, mousehookwheelbool, mousehookbuttonbool, MouseHookButtonX1, MouseHookButtonX2, MouseHookWheelUp, MouseHookWheelDown;
        public static int vkCode, scanCode, mousehookx, mousehooky, tempmousehookx, tempmousehooky, axisx, axisy;
        public static bool KeyboardHookButtonDown, KeyboardHookButtonUp;
        public static bool Getstate = false;
        public static string openFilePath = null;
        public static bool justSaved = true;
        DialogResult result;
        ContextMenu contextMenu = new ContextMenu();
        MenuItem menuItem; 
        private static Range range;
        private static Style InputStyle = new TextStyle(Brushes.Blue, null, System.Drawing.FontStyle.Regular), OutputStyle = new TextStyle(Brushes.Orange, null, System.Drawing.FontStyle.Regular);
        public string filename = "";
        public static string stringscript = "";
        public static bool minmax;
        public static int hostwidth = 0, hostheight = 0;
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			running = false;
            runningscript = false;
            closed = true;
            try
            {
                LSPControl.Disconnect();
            }
            catch { }
            try
            {
                LSPAudio.Disconnect();
            }
            catch { }
            mouseHook.Hook -= new MouseHook.MouseHookCallback(MouseHook_Hook);
            mouseHook.Uninstall();
            keyboardHook.Hook -= new KeyboardHook.KeyboardHookCallback(KeyboardHook_Hook);
            keyboardHook.Uninstall();
            if (TextBoxServerIP.Text != "" & textBox1.Text != "" & textBox2.Text != "")
            {
                using (StreamWriter createdfile = new StreamWriter("tempsave"))
                {
                    createdfile.WriteLine(TextBoxServerIP.Text);
                    createdfile.WriteLine(textBox1.Text);
                    createdfile.WriteLine(textBox2.Text);
                }
            }
			try
			{
				using (Process p = Process.GetCurrentProcess())
				{
					p.Kill();
				}
			}
			catch { }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!justSaved)
            {
                result = MessageBox.Show("Content will be lost! Are you sure?", "Exit", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            mouseHook.Hook += new MouseHook.MouseHookCallback(MouseHook_Hook);
            mouseHook.Install();
            keyboardHook.Hook += new KeyboardHook.KeyboardHookCallback(KeyboardHook_Hook);
            keyboardHook.Install();
            this.Location = new System.Drawing.Point(0, 0);
            if (File.Exists("tempsave"))
            {
                using (StreamReader file = new StreamReader("tempsave"))
                {
                    TextBoxServerIP.Text = file.ReadLine();
                    textBox1.Text = file.ReadLine();
                    textBox2.Text = file.ReadLine();
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            menuItem = new MenuItem("Cut");
            contextMenu.MenuItems.Add(menuItem);
            menuItem.Select += new EventHandler(changeCursor);
            menuItem.Click += new EventHandler(cutAction);
            menuItem = new MenuItem("Copy");
            contextMenu.MenuItems.Add(menuItem);
            menuItem.Select += new EventHandler(changeCursor);
            menuItem.Click += new EventHandler(copyAction);
            menuItem = new MenuItem("Paste");
            contextMenu.MenuItems.Add(menuItem);
            menuItem.Select += new EventHandler(changeCursor);
            menuItem.Click += new EventHandler(pasteAction);
            fastColoredTextBox1.ContextMenu = contextMenu;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            stringscript = fastColoredTextBox1.Text.Replace("\r\n", " ");
            LSPEmulate.TestControl();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (running)
                {
                    running = false;
                    button1.Text = "Start";
                    System.Threading.Thread.Sleep(100);
                    LSPControl.Disconnect();
                    LSPAudio.Disconnect();
                }
                else
                {
                    button1.Text = "Stop";
                    running = true;
                    ip = TextBoxServerIP.Text;
                    hostwidth = Convert.ToInt32(textBox1.Text);
                    hostheight = Convert.ToInt32(textBox2.Text);
                    Task.Run(() => LSPControl.Connect());
                    Task.Run(() => LSPAudio.Connect());
                }
            }
            catch { }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (!justSaved)
            {
                result = MessageBox.Show("Content will be lost! Are you sure?", "Open", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    OpenFileDialog op = new OpenFileDialog();
                    op.Filter = "Text Document(*.txt)|*.txt|All Files(*.*)|";
                    if (op.ShowDialog() == DialogResult.OK)
                    {
                        fastColoredTextBox1.OpenFile(op.FileName, Encoding.UTF8);
                        justSaved = true;
                        filename = op.FileName;
                        openFilePath = op.FileName;
                    }
                }
            }
            else
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "Text Document(*.txt)|*.txt|All Files(*.*)|";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    fastColoredTextBox1.OpenFile(op.FileName, Encoding.UTF8);
                    justSaved = true;
                    filename = op.FileName;
                    openFilePath = op.FileName;
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (openFilePath == null)
            {
                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "Text Document(*.txt)|*.txt|All Files(*.*)|";
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    fastColoredTextBox1.SaveToFile(sf.FileName, Encoding.UTF8);
                    justSaved = true;
                    filename = sf.FileName;
                    openFilePath = sf.FileName;
                    this.Text = sf.FileName;
                }
            }
            else
            {
                fastColoredTextBox1.SaveToFile(openFilePath, Encoding.UTF8);
                justSaved = true;
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text Document(*.txt)|*.txt|All Files(*.*)|";
            if (sf.ShowDialog() == DialogResult.OK)
            {
                fastColoredTextBox1.SaveToFile(sf.FileName, Encoding.UTF8);
                justSaved = true;
                filename = sf.FileName;
                openFilePath = sf.FileName;
                this.Text = sf.FileName;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (runningscript)
            {
                runningscript = false;
                fastColoredTextBox1.ReadOnly = false;
                fastColoredTextBox1.Enabled = true;
                button2.Text = "Start";
            }
            else
            {
                runningscript = true;
                fastColoredTextBox1.ReadOnly = true;
                fastColoredTextBox1.Enabled = false;
                button2.Text = "Stop";
                stringscript = fastColoredTextBox1.Text.Replace("\r\n", " ");
                Task.Run(() => taskEmulate());
            }
        }
        private void taskEmulate()
        {
            LSPEmulate.Connect();
            LSPEmulate.GetControl();
        }
        private void fileText_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                fastColoredTextBox1.ContextMenu = contextMenu;
            }
        }
        private void fileText_TextChanged(object sender, EventArgs e)
        {
            justSaved = false;
        }
        private void changeCursor(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void cutAction(object sender, EventArgs e)
        {
            fastColoredTextBox1.Cut();
        }
        private void copyAction(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.SelectedText != "")
            {
                Clipboard.SetText(fastColoredTextBox1.SelectedText);
            }
        }
        private void pasteAction(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                fastColoredTextBox1.SelectedText = Clipboard.GetText(TextDataFormat.Text).ToString();
            }
        }
        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                range = (sender as FastColoredTextBox).Range; range.SetStyle(InputStyle, new Regex(@"keys12345"));
                range.SetStyle(InputStyle, new Regex(@"keys54321"));
                range.SetStyle(InputStyle, new Regex(@"wd"));
                range.SetStyle(InputStyle, new Regex(@"wu"));
                range.SetStyle(InputStyle, new Regex(@"valchanged"));
                range.SetStyle(InputStyle, new Regex(@"Scale"));
                range.SetStyle(InputStyle, new Regex(@"hostscreenwidth"));
                range.SetStyle(InputStyle, new Regex(@"hostscreenheight"));
                range.SetStyle(InputStyle, new Regex(@"width"));
                range.SetStyle(InputStyle, new Regex(@"height"));
                range.SetStyle(InputStyle, new Regex(@"MouseHookX"));
                range.SetStyle(InputStyle, new Regex(@"MouseHookY"));
                range.SetStyle(InputStyle, new Regex(@"MouseHookButtonX1"));
                range.SetStyle(InputStyle, new Regex(@"MouseHookButtonX2"));
                range.SetStyle(InputStyle, new Regex(@"MouseHookWheelUp"));
                range.SetStyle(InputStyle, new Regex(@"MouseHookWheelDown"));
                range.SetStyle(InputStyle, new Regex(@"MouseHookRightButton"));
                range.SetStyle(InputStyle, new Regex(@"MouseHookLeftButton"));
                range.SetStyle(InputStyle, new Regex(@"MouseHookMiddleButton"));
                range.SetStyle(InputStyle, new Regex(@"MouseHookXButton"));
                range.SetStyle(InputStyle, new Regex(@"Key_LBUTTON"));
                range.SetStyle(InputStyle, new Regex(@"Key_RBUTTON"));
                range.SetStyle(InputStyle, new Regex(@"Key_CANCEL"));
                range.SetStyle(InputStyle, new Regex(@"Key_MBUTTON"));
                range.SetStyle(InputStyle, new Regex(@"Key_XBUTTON1"));
                range.SetStyle(InputStyle, new Regex(@"Key_XBUTTON2"));
                range.SetStyle(InputStyle, new Regex(@"Key_BACK"));
                range.SetStyle(InputStyle, new Regex(@"Key_Tab"));
                range.SetStyle(InputStyle, new Regex(@"Key_CLEAR"));
                range.SetStyle(InputStyle, new Regex(@"Key_Return"));
                range.SetStyle(InputStyle, new Regex(@"Key_SHIFT"));
                range.SetStyle(InputStyle, new Regex(@"Key_CONTROL"));
                range.SetStyle(InputStyle, new Regex(@"Key_MENU"));
                range.SetStyle(InputStyle, new Regex(@"Key_PAUSE"));
                range.SetStyle(InputStyle, new Regex(@"Key_CAPITAL"));
                range.SetStyle(InputStyle, new Regex(@"Key_KANA"));
                range.SetStyle(InputStyle, new Regex(@"Key_HANGEUL"));
                range.SetStyle(InputStyle, new Regex(@"Key_HANGUL"));
                range.SetStyle(InputStyle, new Regex(@"Key_JUNJA"));
                range.SetStyle(InputStyle, new Regex(@"Key_FINAL"));
                range.SetStyle(InputStyle, new Regex(@"Key_HANJA"));
                range.SetStyle(InputStyle, new Regex(@"Key_KANJI"));
                range.SetStyle(InputStyle, new Regex(@"Key_Escape"));
                range.SetStyle(InputStyle, new Regex(@"Key_CONVERT"));
                range.SetStyle(InputStyle, new Regex(@"Key_NONCONVERT"));
                range.SetStyle(InputStyle, new Regex(@"Key_ACCEPT"));
                range.SetStyle(InputStyle, new Regex(@"Key_MODECHANGE"));
                range.SetStyle(InputStyle, new Regex(@"Key_Space"));
                range.SetStyle(InputStyle, new Regex(@"Key_PRIOR"));
                range.SetStyle(InputStyle, new Regex(@"Key_NEXT"));
                range.SetStyle(InputStyle, new Regex(@"Key_END"));
                range.SetStyle(InputStyle, new Regex(@"Key_HOME"));
                range.SetStyle(InputStyle, new Regex(@"Key_LEFT"));
                range.SetStyle(InputStyle, new Regex(@"Key_UP"));
                range.SetStyle(InputStyle, new Regex(@"Key_RIGHT"));
                range.SetStyle(InputStyle, new Regex(@"Key_DOWN"));
                range.SetStyle(InputStyle, new Regex(@"Key_SELECT"));
                range.SetStyle(InputStyle, new Regex(@"Key_PRINT"));
                range.SetStyle(InputStyle, new Regex(@"Key_EXECUTE"));
                range.SetStyle(InputStyle, new Regex(@"Key_SNAPSHOT"));
                range.SetStyle(InputStyle, new Regex(@"Key_INSERT"));
                range.SetStyle(InputStyle, new Regex(@"Key_DELETE"));
                range.SetStyle(InputStyle, new Regex(@"Key_HELP"));
                range.SetStyle(InputStyle, new Regex(@"Key_APOSTROPHE"));
                range.SetStyle(InputStyle, new Regex(@"Key_0"));
                range.SetStyle(InputStyle, new Regex(@"Key_1"));
                range.SetStyle(InputStyle, new Regex(@"Key_2"));
                range.SetStyle(InputStyle, new Regex(@"Key_3"));
                range.SetStyle(InputStyle, new Regex(@"Key_4"));
                range.SetStyle(InputStyle, new Regex(@"Key_5"));
                range.SetStyle(InputStyle, new Regex(@"Key_6"));
                range.SetStyle(InputStyle, new Regex(@"Key_7"));
                range.SetStyle(InputStyle, new Regex(@"Key_8"));
                range.SetStyle(InputStyle, new Regex(@"Key_9"));
                range.SetStyle(InputStyle, new Regex(@"Key_A"));
                range.SetStyle(InputStyle, new Regex(@"Key_B"));
                range.SetStyle(InputStyle, new Regex(@"Key_C"));
                range.SetStyle(InputStyle, new Regex(@"Key_D"));
                range.SetStyle(InputStyle, new Regex(@"Key_E"));
                range.SetStyle(InputStyle, new Regex(@"Key_F"));
                range.SetStyle(InputStyle, new Regex(@"Key_G"));
                range.SetStyle(InputStyle, new Regex(@"Key_H"));
                range.SetStyle(InputStyle, new Regex(@"Key_I"));
                range.SetStyle(InputStyle, new Regex(@"Key_J"));
                range.SetStyle(InputStyle, new Regex(@"Key_K"));
                range.SetStyle(InputStyle, new Regex(@"Key_L"));
                range.SetStyle(InputStyle, new Regex(@"Key_M"));
                range.SetStyle(InputStyle, new Regex(@"Key_N"));
                range.SetStyle(InputStyle, new Regex(@"Key_O"));
                range.SetStyle(InputStyle, new Regex(@"Key_P"));
                range.SetStyle(InputStyle, new Regex(@"Key_Q"));
                range.SetStyle(InputStyle, new Regex(@"Key_R"));
                range.SetStyle(InputStyle, new Regex(@"Key_S"));
                range.SetStyle(InputStyle, new Regex(@"Key_T"));
                range.SetStyle(InputStyle, new Regex(@"Key_U"));
                range.SetStyle(InputStyle, new Regex(@"Key_V"));
                range.SetStyle(InputStyle, new Regex(@"Key_W"));
                range.SetStyle(InputStyle, new Regex(@"Key_X"));
                range.SetStyle(InputStyle, new Regex(@"Key_Y"));
                range.SetStyle(InputStyle, new Regex(@"Key_Z"));
                range.SetStyle(InputStyle, new Regex(@"Key_LWIN"));
                range.SetStyle(InputStyle, new Regex(@"Key_RWIN"));
                range.SetStyle(InputStyle, new Regex(@"Key_APPS"));
                range.SetStyle(InputStyle, new Regex(@"Key_SLEEP"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMPAD0"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMPAD1"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMPAD2"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMPAD3"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMPAD4"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMPAD5"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMPAD6"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMPAD7"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMPAD8"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMPAD9"));
                range.SetStyle(InputStyle, new Regex(@"Key_MULTIPLY"));
                range.SetStyle(InputStyle, new Regex(@"Key_ADD"));
                range.SetStyle(InputStyle, new Regex(@"Key_SEPARATOR"));
                range.SetStyle(InputStyle, new Regex(@"Key_SUBTRACT"));
                range.SetStyle(InputStyle, new Regex(@"Key_DECIMAL"));
                range.SetStyle(InputStyle, new Regex(@"Key_DIVIDE"));
                range.SetStyle(InputStyle, new Regex(@"Key_F1"));
                range.SetStyle(InputStyle, new Regex(@"Key_F2"));
                range.SetStyle(InputStyle, new Regex(@"Key_F3"));
                range.SetStyle(InputStyle, new Regex(@"Key_F4"));
                range.SetStyle(InputStyle, new Regex(@"Key_F5"));
                range.SetStyle(InputStyle, new Regex(@"Key_F6"));
                range.SetStyle(InputStyle, new Regex(@"Key_F7"));
                range.SetStyle(InputStyle, new Regex(@"Key_F8"));
                range.SetStyle(InputStyle, new Regex(@"Key_F9"));
                range.SetStyle(InputStyle, new Regex(@"Key_F10"));
                range.SetStyle(InputStyle, new Regex(@"Key_F11"));
                range.SetStyle(InputStyle, new Regex(@"Key_F12"));
                range.SetStyle(InputStyle, new Regex(@"Key_F13"));
                range.SetStyle(InputStyle, new Regex(@"Key_F14"));
                range.SetStyle(InputStyle, new Regex(@"Key_F15"));
                range.SetStyle(InputStyle, new Regex(@"Key_F16"));
                range.SetStyle(InputStyle, new Regex(@"Key_F17"));
                range.SetStyle(InputStyle, new Regex(@"Key_F18"));
                range.SetStyle(InputStyle, new Regex(@"Key_F19"));
                range.SetStyle(InputStyle, new Regex(@"Key_F20"));
                range.SetStyle(InputStyle, new Regex(@"Key_F21"));
                range.SetStyle(InputStyle, new Regex(@"Key_F22"));
                range.SetStyle(InputStyle, new Regex(@"Key_F23"));
                range.SetStyle(InputStyle, new Regex(@"Key_F24"));
                range.SetStyle(InputStyle, new Regex(@"Key_NUMLOCK"));
                range.SetStyle(InputStyle, new Regex(@"Key_SCROLL"));
                range.SetStyle(InputStyle, new Regex(@"Key_LeftShift"));
                range.SetStyle(InputStyle, new Regex(@"Key_RightShift"));
                range.SetStyle(InputStyle, new Regex(@"Key_LeftControl"));
                range.SetStyle(InputStyle, new Regex(@"Key_RightControl"));
                range.SetStyle(InputStyle, new Regex(@"Key_LMENU"));
                range.SetStyle(InputStyle, new Regex(@"Key_RMENU"));
                range.SetStyle(InputStyle, new Regex(@"Key_BROWSER_BACK"));
                range.SetStyle(InputStyle, new Regex(@"Key_BROWSER_FORWARD"));
                range.SetStyle(InputStyle, new Regex(@"Key_BROWSER_REFRESH"));
                range.SetStyle(InputStyle, new Regex(@"Key_BROWSER_STOP"));
                range.SetStyle(InputStyle, new Regex(@"Key_BROWSER_SEARCH"));
                range.SetStyle(InputStyle, new Regex(@"Key_BROWSER_FAVORITES"));
                range.SetStyle(InputStyle, new Regex(@"Key_BROWSER_HOME"));
                range.SetStyle(InputStyle, new Regex(@"Key_VOLUME_MUTE"));
                range.SetStyle(InputStyle, new Regex(@"Key_VOLUME_DOWN"));
                range.SetStyle(InputStyle, new Regex(@"Key_VOLUME_UP"));
                range.SetStyle(InputStyle, new Regex(@"Key_MEDIA_NEXT_TRACK"));
                range.SetStyle(InputStyle, new Regex(@"Key_MEDIA_PREV_TRACK"));
                range.SetStyle(InputStyle, new Regex(@"Key_MEDIA_STOP"));
                range.SetStyle(InputStyle, new Regex(@"Key_MEDIA_PLAY_PAUSE"));
                range.SetStyle(InputStyle, new Regex(@"Key_LAUNCH_MAIL"));
                range.SetStyle(InputStyle, new Regex(@"Key_LAUNCH_MEDIA_SELECT"));
                range.SetStyle(InputStyle, new Regex(@"Key_LAUNCH_APP1"));
                range.SetStyle(InputStyle, new Regex(@"Key_LAUNCH_APP2"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_1"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_PLUS"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_COMMA"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_MINUS"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_PERIOD"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_2"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_3"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_4"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_5"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_6"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_7"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_8"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_102"));
                range.SetStyle(InputStyle, new Regex(@"Key_PROCESSKEY"));
                range.SetStyle(InputStyle, new Regex(@"Key_PACKET"));
                range.SetStyle(InputStyle, new Regex(@"Key_ATTN"));
                range.SetStyle(InputStyle, new Regex(@"Key_CRSEL"));
                range.SetStyle(InputStyle, new Regex(@"Key_EXSEL"));
                range.SetStyle(InputStyle, new Regex(@"Key_EREOF"));
                range.SetStyle(InputStyle, new Regex(@"Key_PLAY"));
                range.SetStyle(InputStyle, new Regex(@"Key_ZOOM"));
                range.SetStyle(InputStyle, new Regex(@"Key_NONAME"));
                range.SetStyle(InputStyle, new Regex(@"Key_PA1"));
                range.SetStyle(InputStyle, new Regex(@"Key_OEM_CLEAR"));
                range.SetStyle(InputStyle, new Regex(@"ButtonAPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonBPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonXPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonYPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonStartPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonBackPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonDownPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonUpPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonLeftPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonRightPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonShoulderLeftPressed"));
                range.SetStyle(InputStyle, new Regex(@"ButtonShoulderRightPressed"));
                range.SetStyle(InputStyle, new Regex(@"ThumbpadLeftPressed"));
                range.SetStyle(InputStyle, new Regex(@"ThumbpadRightPressed"));
                range.SetStyle(InputStyle, new Regex(@"TriggerLeftPosition"));
                range.SetStyle(InputStyle, new Regex(@"TriggerRightPosition"));
                range.SetStyle(InputStyle, new Regex(@"ThumbLeftX"));
                range.SetStyle(InputStyle, new Regex(@"ThumbLeftY"));
                range.SetStyle(InputStyle, new Regex(@"ThumbRightX"));
                range.SetStyle(InputStyle, new Regex(@"ThumbRightY"));
                range.SetStyle(InputStyle, new Regex(@"pollcount"));
                range.SetStyle(InputStyle, new Regex(@"sleeptime"));
                range.SetStyle(InputStyle, new Regex(@"getstate"));
                range.SetStyle(OutputStyle, new Regex(@"KeyboardMouseDriverType"));
                range.SetStyle(OutputStyle, new Regex(@"MouseMoveX"));
                range.SetStyle(OutputStyle, new Regex(@"MouseMoveY"));
                range.SetStyle(OutputStyle, new Regex(@"MouseAbsX"));
                range.SetStyle(OutputStyle, new Regex(@"MouseAbsY"));
                range.SetStyle(OutputStyle, new Regex(@"MouseDesktopX"));
                range.SetStyle(OutputStyle, new Regex(@"MouseDesktopY"));
                range.SetStyle(OutputStyle, new Regex(@"SendLeftClick"));
                range.SetStyle(OutputStyle, new Regex(@"SendRightClick"));
                range.SetStyle(OutputStyle, new Regex(@"SendMiddleClick"));
                range.SetStyle(OutputStyle, new Regex(@"SendWheelUp"));
                range.SetStyle(OutputStyle, new Regex(@"SendWheelDown"));
                range.SetStyle(OutputStyle, new Regex(@"SendLeft"));
                range.SetStyle(OutputStyle, new Regex(@"SendRight"));
                range.SetStyle(OutputStyle, new Regex(@"SendUp"));
                range.SetStyle(OutputStyle, new Regex(@"SendDown"));
                range.SetStyle(OutputStyle, new Regex(@"SendLButton"));
                range.SetStyle(OutputStyle, new Regex(@"SendRButton"));
                range.SetStyle(OutputStyle, new Regex(@"SendCancel"));
                range.SetStyle(OutputStyle, new Regex(@"SendMBUTTON"));
                range.SetStyle(OutputStyle, new Regex(@"SendXBUTTON1"));
                range.SetStyle(OutputStyle, new Regex(@"SendXBUTTON2"));
                range.SetStyle(OutputStyle, new Regex(@"SendBack"));
                range.SetStyle(OutputStyle, new Regex(@"SendTab"));
                range.SetStyle(OutputStyle, new Regex(@"SendClear"));
                range.SetStyle(OutputStyle, new Regex(@"SendReturn"));
                range.SetStyle(OutputStyle, new Regex(@"SendSHIFT"));
                range.SetStyle(OutputStyle, new Regex(@"SendCONTROL"));
                range.SetStyle(OutputStyle, new Regex(@"SendMENU"));
                range.SetStyle(OutputStyle, new Regex(@"SendPAUSE"));
                range.SetStyle(OutputStyle, new Regex(@"SendCAPITAL"));
                range.SetStyle(OutputStyle, new Regex(@"SendKANA"));
                range.SetStyle(OutputStyle, new Regex(@"SendHANGEUL"));
                range.SetStyle(OutputStyle, new Regex(@"SendHANGUL"));
                range.SetStyle(OutputStyle, new Regex(@"SendJUNJA"));
                range.SetStyle(OutputStyle, new Regex(@"SendFINAL"));
                range.SetStyle(OutputStyle, new Regex(@"SendHANJA"));
                range.SetStyle(OutputStyle, new Regex(@"SendKANJI"));
                range.SetStyle(OutputStyle, new Regex(@"SendEscape"));
                range.SetStyle(OutputStyle, new Regex(@"SendCONVERT"));
                range.SetStyle(OutputStyle, new Regex(@"SendNONCONVERT"));
                range.SetStyle(OutputStyle, new Regex(@"SendACCEPT"));
                range.SetStyle(OutputStyle, new Regex(@"SendMODECHANGE"));
                range.SetStyle(OutputStyle, new Regex(@"SendSpace"));
                range.SetStyle(OutputStyle, new Regex(@"SendPRIOR"));
                range.SetStyle(OutputStyle, new Regex(@"SendNEXT"));
                range.SetStyle(OutputStyle, new Regex(@"SendEND"));
                range.SetStyle(OutputStyle, new Regex(@"SendHOME"));
                range.SetStyle(OutputStyle, new Regex(@"SendLEFT"));
                range.SetStyle(OutputStyle, new Regex(@"SendUP"));
                range.SetStyle(OutputStyle, new Regex(@"SendRIGHT"));
                range.SetStyle(OutputStyle, new Regex(@"SendDOWN"));
                range.SetStyle(OutputStyle, new Regex(@"SendSELECT"));
                range.SetStyle(OutputStyle, new Regex(@"SendPRINT"));
                range.SetStyle(OutputStyle, new Regex(@"SendEXECUTE"));
                range.SetStyle(OutputStyle, new Regex(@"SendSNAPSHOT"));
                range.SetStyle(OutputStyle, new Regex(@"SendINSERT"));
                range.SetStyle(OutputStyle, new Regex(@"SendDELETE"));
                range.SetStyle(OutputStyle, new Regex(@"SendHELP"));
                range.SetStyle(OutputStyle, new Regex(@"SendAPOSTROPHE"));
                range.SetStyle(OutputStyle, new Regex(@"Send0"));
                range.SetStyle(OutputStyle, new Regex(@"Send1"));
                range.SetStyle(OutputStyle, new Regex(@"Send2"));
                range.SetStyle(OutputStyle, new Regex(@"Send3"));
                range.SetStyle(OutputStyle, new Regex(@"Send4"));
                range.SetStyle(OutputStyle, new Regex(@"Send5"));
                range.SetStyle(OutputStyle, new Regex(@"Send6"));
                range.SetStyle(OutputStyle, new Regex(@"Send7"));
                range.SetStyle(OutputStyle, new Regex(@"Send8"));
                range.SetStyle(OutputStyle, new Regex(@"Send9"));
                range.SetStyle(OutputStyle, new Regex(@"SendA"));
                range.SetStyle(OutputStyle, new Regex(@"SendB"));
                range.SetStyle(OutputStyle, new Regex(@"SendC"));
                range.SetStyle(OutputStyle, new Regex(@"SendD"));
                range.SetStyle(OutputStyle, new Regex(@"SendE"));
                range.SetStyle(OutputStyle, new Regex(@"SendF"));
                range.SetStyle(OutputStyle, new Regex(@"SendG"));
                range.SetStyle(OutputStyle, new Regex(@"SendH"));
                range.SetStyle(OutputStyle, new Regex(@"SendI"));
                range.SetStyle(OutputStyle, new Regex(@"SendJ"));
                range.SetStyle(OutputStyle, new Regex(@"SendK"));
                range.SetStyle(OutputStyle, new Regex(@"SendL"));
                range.SetStyle(OutputStyle, new Regex(@"SendM"));
                range.SetStyle(OutputStyle, new Regex(@"SendN"));
                range.SetStyle(OutputStyle, new Regex(@"SendO"));
                range.SetStyle(OutputStyle, new Regex(@"SendP"));
                range.SetStyle(OutputStyle, new Regex(@"SendQ"));
                range.SetStyle(OutputStyle, new Regex(@"SendR"));
                range.SetStyle(OutputStyle, new Regex(@"SendS"));
                range.SetStyle(OutputStyle, new Regex(@"SendT"));
                range.SetStyle(OutputStyle, new Regex(@"SendU"));
                range.SetStyle(OutputStyle, new Regex(@"SendV"));
                range.SetStyle(OutputStyle, new Regex(@"SendW"));
                range.SetStyle(OutputStyle, new Regex(@"SendX"));
                range.SetStyle(OutputStyle, new Regex(@"SendY"));
                range.SetStyle(OutputStyle, new Regex(@"SendZ"));
                range.SetStyle(OutputStyle, new Regex(@"SendLWIN"));
                range.SetStyle(OutputStyle, new Regex(@"SendRWIN"));
                range.SetStyle(OutputStyle, new Regex(@"SendAPPS"));
                range.SetStyle(OutputStyle, new Regex(@"SendSLEEP"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMPAD0"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMPAD1"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMPAD2"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMPAD3"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMPAD4"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMPAD5"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMPAD6"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMPAD7"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMPAD8"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMPAD9"));
                range.SetStyle(OutputStyle, new Regex(@"SendMULTIPLY"));
                range.SetStyle(OutputStyle, new Regex(@"SendADD"));
                range.SetStyle(OutputStyle, new Regex(@"SendSEPARATOR"));
                range.SetStyle(OutputStyle, new Regex(@"SendSUBTRACT"));
                range.SetStyle(OutputStyle, new Regex(@"SendDECIMAL"));
                range.SetStyle(OutputStyle, new Regex(@"SendDIVIDE"));
                range.SetStyle(OutputStyle, new Regex(@"SendF1"));
                range.SetStyle(OutputStyle, new Regex(@"SendF2"));
                range.SetStyle(OutputStyle, new Regex(@"SendF3"));
                range.SetStyle(OutputStyle, new Regex(@"SendF4"));
                range.SetStyle(OutputStyle, new Regex(@"SendF5"));
                range.SetStyle(OutputStyle, new Regex(@"SendF6"));
                range.SetStyle(OutputStyle, new Regex(@"SendF7"));
                range.SetStyle(OutputStyle, new Regex(@"SendF8"));
                range.SetStyle(OutputStyle, new Regex(@"SendF9"));
                range.SetStyle(OutputStyle, new Regex(@"SendF10"));
                range.SetStyle(OutputStyle, new Regex(@"SendF11"));
                range.SetStyle(OutputStyle, new Regex(@"SendF12"));
                range.SetStyle(OutputStyle, new Regex(@"SendF13"));
                range.SetStyle(OutputStyle, new Regex(@"SendF14"));
                range.SetStyle(OutputStyle, new Regex(@"SendF15"));
                range.SetStyle(OutputStyle, new Regex(@"SendF16"));
                range.SetStyle(OutputStyle, new Regex(@"SendF17"));
                range.SetStyle(OutputStyle, new Regex(@"SendF18"));
                range.SetStyle(OutputStyle, new Regex(@"SendF19"));
                range.SetStyle(OutputStyle, new Regex(@"SendF20"));
                range.SetStyle(OutputStyle, new Regex(@"SendF21"));
                range.SetStyle(OutputStyle, new Regex(@"SendF22"));
                range.SetStyle(OutputStyle, new Regex(@"SendF23"));
                range.SetStyle(OutputStyle, new Regex(@"SendF24"));
                range.SetStyle(OutputStyle, new Regex(@"SendNUMLOCK"));
                range.SetStyle(OutputStyle, new Regex(@"SendSCROLL"));
                range.SetStyle(OutputStyle, new Regex(@"SendLeftShift"));
                range.SetStyle(OutputStyle, new Regex(@"SendRightShift"));
                range.SetStyle(OutputStyle, new Regex(@"SendLeftControl"));
                range.SetStyle(OutputStyle, new Regex(@"SendRightControl"));
                range.SetStyle(OutputStyle, new Regex(@"SendLMENU"));
                range.SetStyle(OutputStyle, new Regex(@"SendRMENU"));
                range.SetStyle(OutputStyle, new Regex(@"back"));
                range.SetStyle(OutputStyle, new Regex(@"start"));
                range.SetStyle(OutputStyle, new Regex(@"A"));
                range.SetStyle(OutputStyle, new Regex(@"B"));
                range.SetStyle(OutputStyle, new Regex(@"X"));
                range.SetStyle(OutputStyle, new Regex(@"Y"));
                range.SetStyle(OutputStyle, new Regex(@"up"));
                range.SetStyle(OutputStyle, new Regex(@"left"));
                range.SetStyle(OutputStyle, new Regex(@"down"));
                range.SetStyle(OutputStyle, new Regex(@"right"));
                range.SetStyle(OutputStyle, new Regex(@"leftstick"));
                range.SetStyle(OutputStyle, new Regex(@"rightstick"));
                range.SetStyle(OutputStyle, new Regex(@"leftbumper"));
                range.SetStyle(OutputStyle, new Regex(@"rightbumper"));
                range.SetStyle(OutputStyle, new Regex(@"lefttrigger"));
                range.SetStyle(OutputStyle, new Regex(@"righttrigger"));
                range.SetStyle(OutputStyle, new Regex(@"leftstickx"));
                range.SetStyle(OutputStyle, new Regex(@"leftsticky"));
                range.SetStyle(OutputStyle, new Regex(@"rightstickx"));
                range.SetStyle(OutputStyle, new Regex(@"rightsticky"));
            }
            catch { }
        }
        private void fastColoredTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            justSaved = false;
        }
        public static void MouseHook_Hook(MouseHook.MSLLHOOKSTRUCT mouseStruct) { }
        public static void KeyboardHook_Hook(KeyboardHook.KBDLLHOOKSTRUCT keyboardStruct) { }
        public static void MouseHookProcessButtons()
        {
            if (Getstate)
            {
                SetCursorPos(mousehookx <= 0 ? 0 : width, mousehooky <= 0 ? 0 : height);
                if (time.Count() <= 60)
                    time.Add(MouseHookTime + mousehookx + mousehooky);
                else
                {
                    time.Add(MouseHookTime + mousehookx + mousehooky);
                    time.RemoveAt(0);
                }
                if (time.All(x => x == time.First()))
                {
                    tempmousehookx = 0;
                    tempmousehooky = 0;
                }
                else
                {
                    tempmousehookx = mousehookx;
                    tempmousehooky = mousehooky;
                }
                MouseHookX = tempmousehookx;
                MouseHookY = tempmousehooky;
                if (MouseHookX >= 1024f)
                    MouseHookX = 1024f;
                if (MouseHookX <= -1024f)
                    MouseHookX = -1024f;
                if (MouseHookY >= 1024f)
                    MouseHookY = 1024f;
                if (MouseHookY <= -1024f)
                    MouseHookY = -1024f;
            }
            else
            {
                MouseHookX = mousehookx;
                MouseHookY = mousehooky;
            }
            if (MouseHookWheel != 0)
                mousehookwheelbool = true;
            if (mousehookwheelbool)
                mousehookwheelcount++;
            if (mousehookwheelcount >= 80)
            {
                mousehookwheelbool = false;
                MouseHook.MouseHookWheel = 0;
                MouseHookWheel = 0;
                mousehookwheelcount = 0;
            }
            if (MouseHookButtonX != 0)
                mousehookbuttonbool = true;
            if (mousehookbuttonbool)
                mousehookbuttoncount++;
            if (mousehookbuttoncount >= 80)
            {
                mousehookbuttonbool = false;
                MouseHook.MouseHookButtonX = 0;
                MouseHookButtonX = 0;
                mousehookbuttoncount = 0;
            }
            if (MouseHookButtonX == 65536)
                MouseHookButtonX1 = true;
            else
                MouseHookButtonX1 = false;
            if (MouseHookButtonX == 131072)
                MouseHookButtonX2 = true;
            else
                MouseHookButtonX2 = false;
            if (MouseHookWheel == 7864320)
                MouseHookWheelUp = true;
            else
                MouseHookWheelUp = false;
            if (MouseHookWheel == -7864320)
                MouseHookWheelDown = true;
            else
                MouseHookWheelDown = false;
        }
        public const int S_LBUTTON = (int)0x0;
        public const int S_RBUTTON = 0;
        public const int S_CANCEL = 70;
        public const int S_MBUTTON = 0;
        public const int S_XBUTTON1 = 0;
        public const int S_XBUTTON2 = 0;
        public const int S_BACK = 14;
        public const int S_Tab = 15;
        public const int S_CLEAR = 76;
        public const int S_Return = 28;
        public const int S_SHIFT = 42;
        public const int S_CONTROL = 29;
        public const int S_MENU = 56;
        public const int S_PAUSE = 0;
        public const int S_CAPITAL = 58;
        public const int S_KANA = 0;
        public const int S_HANGEUL = 0;
        public const int S_HANGUL = 0;
        public const int S_JUNJA = 0;
        public const int S_FINAL = 0;
        public const int S_HANJA = 0;
        public const int S_KANJI = 0;
        public const int S_Escape = 1;
        public const int S_CONVERT = 0;
        public const int S_NONCONVERT = 0;
        public const int S_ACCEPT = 0;
        public const int S_MODECHANGE = 0;
        public const int S_Space = 57;
        public const int S_PRIOR = 73;
        public const int S_NEXT = 81;
        public const int S_END = 79;
        public const int S_HOME = 71;
        public const int S_LEFT = 75;
        public const int S_UP = 72;
        public const int S_RIGHT = 77;
        public const int S_DOWN = 80;
        public const int S_SELECT = 0;
        public const int S_PRINT = 0;
        public const int S_EXECUTE = 0;
        public const int S_SNAPSHOT = 84;
        public const int S_INSERT = 82;
        public const int S_DELETE = 83;
        public const int S_HELP = 99;
        public const int S_APOSTROPHE = 41;
        public const int S_0 = 11;
        public const int S_1 = 2;
        public const int S_2 = 3;
        public const int S_3 = 4;
        public const int S_4 = 5;
        public const int S_5 = 6;
        public const int S_6 = 7;
        public const int S_7 = 8;
        public const int S_8 = 9;
        public const int S_9 = 10;
        public const int S_A = 16;
        public const int S_B = 48;
        public const int S_C = 46;
        public const int S_D = 32;
        public const int S_E = 18;
        public const int S_F = 33;
        public const int S_G = 34;
        public const int S_H = 35;
        public const int S_I = 23;
        public const int S_J = 32;
        public const int S_K = 37;
        public const int S_L = 38;
        public const int S_M = 39;
        public const int S_N = 49;
        public const int S_O = 24;
        public const int S_P = 25;
        public const int S_Q = 30;
        public const int S_R = 19;
        public const int S_S = 31;
        public const int S_T = 20;
        public const int S_U = 22;
        public const int S_V = 47;
        public const int S_W = 44;
        public const int S_X = 45;
        public const int S_Y = 21;
        public const int S_Z = 17;
        public const int S_LWIN = 91;
        public const int S_RWIN = 92;
        public const int S_APPS = 93;
        public const int S_SLEEP = 95;
        public const int S_NUMPAD0 = 82;
        public const int S_NUMPAD1 = 79;
        public const int S_NUMPAD2 = 80;
        public const int S_NUMPAD3 = 81;
        public const int S_NUMPAD4 = 75;
        public const int S_NUMPAD5 = 76;
        public const int S_NUMPAD6 = 77;
        public const int S_NUMPAD7 = 71;
        public const int S_NUMPAD8 = 72;
        public const int S_NUMPAD9 = 73;
        public const int S_MULTIPLY = 55;
        public const int S_ADD = 78;
        public const int S_SEPARATOR = 0;
        public const int S_SUBTRACT = 74;
        public const int S_DECIMAL = 83;
        public const int S_DIVIDE = 53;
        public const int S_F1 = 59;
        public const int S_F2 = 60;
        public const int S_F3 = 61;
        public const int S_F4 = 62;
        public const int S_F5 = 63;
        public const int S_F6 = 64;
        public const int S_F7 = 65;
        public const int S_F8 = 66;
        public const int S_F9 = 67;
        public const int S_F10 = 68;
        public const int S_F11 = 87;
        public const int S_F12 = 88;
        public const int S_F13 = 100;
        public const int S_F14 = 101;
        public const int S_F15 = 102;
        public const int S_F16 = 103;
        public const int S_F17 = 104;
        public const int S_F18 = 105;
        public const int S_F19 = 106;
        public const int S_F20 = 107;
        public const int S_F21 = 108;
        public const int S_F22 = 109;
        public const int S_F23 = 110;
        public const int S_F24 = 118;
        public const int S_NUMLOCK = 69;
        public const int S_SCROLL = 70;
        public const int S_LeftShift = 42;
        public const int S_RightShift = 54;
        public const int S_LeftControl = 29;
        public const int S_RightControl = 29;
        public const int S_LMENU = 56;
        public const int S_RMENU = 56;
        public const int S_BROWSER_BACK = 106;
        public const int S_BROWSER_FORWARD = 105;
        public const int S_BROWSER_REFRESH = 103;
        public const int S_BROWSER_STOP = 104;
        public const int S_BROWSER_SEARCH = 101;
        public const int S_BROWSER_FAVORITES = 102;
        public const int S_BROWSER_HOME = 50;
        public const int S_VOLUME_MUTE = 32;
        public const int S_VOLUME_DOWN = 46;
        public const int S_VOLUME_UP = 48;
        public const int S_MEDIA_NEXT_TRACK = 25;
        public const int S_MEDIA_PREV_TRACK = 16;
        public const int S_MEDIA_STOP = 36;
        public const int S_MEDIA_PLAY_PAUSE = 34;
        public const int S_LAUNCH_MAIL = 108;
        public const int S_LAUNCH_MEDIA_SELECT = 109;
        public const int S_LAUNCH_APP1 = 107;
        public const int S_LAUNCH_APP2 = 33;
        public const int S_OEM_1 = 27;
        public const int S_OEM_PLUS = 13;
        public const int S_OEM_COMMA = 50;
        public const int S_OEM_MINUS = 0;
        public const int S_OEM_PERIOD = 51;
        public const int S_OEM_2 = 52;
        public const int S_OEM_3 = 40;
        public const int S_OEM_4 = 12;
        public const int S_OEM_5 = 43;
        public const int S_OEM_6 = 26;
        public const int S_OEM_7 = 41;
        public const int S_OEM_8 = 53;
        public const int S_OEM_102 = 86;
        public const int S_PROCESSKEY = 0;
        public const int S_PACKET = 0;
        public const int S_ATTN = 0;
        public const int S_CRSEL = 0;
        public const int S_EXSEL = 0;
        public const int S_EREOF = 93;
        public const int S_PLAY = 0;
        public const int S_ZOOM = 98;
        public const int S_NONAME = 0;
        public const int S_PA1 = 0;
        public const int S_OEM_CLEAR = 0;
        public static bool Key_LBUTTON;
        public static bool Key_RBUTTON;
        public static bool Key_CANCEL;
        public static bool Key_MBUTTON;
        public static bool Key_XBUTTON1;
        public static bool Key_XBUTTON2;
        public static bool Key_BACK;
        public static bool Key_Tab;
        public static bool Key_CLEAR;
        public static bool Key_Return;
        public static bool Key_SHIFT;
        public static bool Key_CONTROL;
        public static bool Key_MENU;
        public static bool Key_PAUSE;
        public static bool Key_CAPITAL;
        public static bool Key_KANA;
        public static bool Key_HANGEUL;
        public static bool Key_HANGUL;
        public static bool Key_JUNJA;
        public static bool Key_FINAL;
        public static bool Key_HANJA;
        public static bool Key_KANJI;
        public static bool Key_Escape;
        public static bool Key_CONVERT;
        public static bool Key_NONCONVERT;
        public static bool Key_ACCEPT;
        public static bool Key_MODECHANGE;
        public static bool Key_Space;
        public static bool Key_PRIOR;
        public static bool Key_NEXT;
        public static bool Key_END;
        public static bool Key_HOME;
        public static bool Key_LEFT;
        public static bool Key_UP;
        public static bool Key_RIGHT;
        public static bool Key_DOWN;
        public static bool Key_SELECT;
        public static bool Key_PRINT;
        public static bool Key_EXECUTE;
        public static bool Key_SNAPSHOT;
        public static bool Key_INSERT;
        public static bool Key_DELETE;
        public static bool Key_HELP;
        public static bool Key_APOSTROPHE;
        public static bool Key_0;
        public static bool Key_1;
        public static bool Key_2;
        public static bool Key_3;
        public static bool Key_4;
        public static bool Key_5;
        public static bool Key_6;
        public static bool Key_7;
        public static bool Key_8;
        public static bool Key_9;
        public static bool Key_A;
        public static bool Key_B;
        public static bool Key_C;
        public static bool Key_D;
        public static bool Key_E;
        public static bool Key_F;
        public static bool Key_G;
        public static bool Key_H;
        public static bool Key_I;
        public static bool Key_J;
        public static bool Key_K;
        public static bool Key_L;
        public static bool Key_M;
        public static bool Key_N;
        public static bool Key_O;
        public static bool Key_P;
        public static bool Key_Q;
        public static bool Key_R;
        public static bool Key_S;
        public static bool Key_T;
        public static bool Key_U;
        public static bool Key_V;
        public static bool Key_W;
        public static bool Key_X;
        public static bool Key_Y;
        public static bool Key_Z;
        public static bool Key_LWIN;
        public static bool Key_RWIN;
        public static bool Key_APPS;
        public static bool Key_SLEEP;
        public static bool Key_NUMPAD0;
        public static bool Key_NUMPAD1;
        public static bool Key_NUMPAD2;
        public static bool Key_NUMPAD3;
        public static bool Key_NUMPAD4;
        public static bool Key_NUMPAD5;
        public static bool Key_NUMPAD6;
        public static bool Key_NUMPAD7;
        public static bool Key_NUMPAD8;
        public static bool Key_NUMPAD9;
        public static bool Key_MULTIPLY;
        public static bool Key_ADD;
        public static bool Key_SEPARATOR;
        public static bool Key_SUBTRACT;
        public static bool Key_DECIMAL;
        public static bool Key_DIVIDE;
        public static bool Key_F1;
        public static bool Key_F2;
        public static bool Key_F3;
        public static bool Key_F4;
        public static bool Key_F5;
        public static bool Key_F6;
        public static bool Key_F7;
        public static bool Key_F8;
        public static bool Key_F9;
        public static bool Key_F10;
        public static bool Key_F11;
        public static bool Key_F12;
        public static bool Key_F13;
        public static bool Key_F14;
        public static bool Key_F15;
        public static bool Key_F16;
        public static bool Key_F17;
        public static bool Key_F18;
        public static bool Key_F19;
        public static bool Key_F20;
        public static bool Key_F21;
        public static bool Key_F22;
        public static bool Key_F23;
        public static bool Key_F24;
        public static bool Key_NUMLOCK;
        public static bool Key_SCROLL;
        public static bool Key_LeftShift;
        public static bool Key_RightShift;
        public static bool Key_LeftControl;
        public static bool Key_RightControl;
        public static bool Key_LMENU;
        public static bool Key_RMENU;
        public static bool Key_BROWSER_BACK;
        public static bool Key_BROWSER_FORWARD;
        public static bool Key_BROWSER_REFRESH;
        public static bool Key_BROWSER_STOP;
        public static bool Key_BROWSER_SEARCH;
        public static bool Key_BROWSER_FAVORITES;
        public static bool Key_BROWSER_HOME;
        public static bool Key_VOLUME_MUTE;
        public static bool Key_VOLUME_DOWN;
        public static bool Key_VOLUME_UP;
        public static bool Key_MEDIA_NEXT_TRACK;
        public static bool Key_MEDIA_PREV_TRACK;
        public static bool Key_MEDIA_STOP;
        public static bool Key_MEDIA_PLAY_PAUSE;
        public static bool Key_LAUNCH_MAIL;
        public static bool Key_LAUNCH_MEDIA_SELECT;
        public static bool Key_LAUNCH_APP1;
        public static bool Key_LAUNCH_APP2;
        public static bool Key_OEM_1;
        public static bool Key_OEM_PLUS;
        public static bool Key_OEM_COMMA;
        public static bool Key_OEM_MINUS;
        public static bool Key_OEM_PERIOD;
        public static bool Key_OEM_2;
        public static bool Key_OEM_3;
        public static bool Key_OEM_4;
        public static bool Key_OEM_5;
        public static bool Key_OEM_6;
        public static bool Key_OEM_7;
        public static bool Key_OEM_8;
        public static bool Key_OEM_102;
        public static bool Key_PROCESSKEY;
        public static bool Key_PACKET;
        public static bool Key_ATTN;
        public static bool Key_CRSEL;
        public static bool Key_EXSEL;
        public static bool Key_EREOF;
        public static bool Key_PLAY;
        public static bool Key_ZOOM;
        public static bool Key_NONAME;
        public static bool Key_PA1;
        public static bool Key_OEM_CLEAR;
        public static void KeyboardHookProcessButtons()
        {
            if (KeyboardHookButtonDown)
            {
                if (scanCode == S_LBUTTON)
                    Key_LBUTTON = true;
                if (scanCode == S_RBUTTON)
                    Key_RBUTTON = true;
                if (scanCode == S_CANCEL)
                    Key_CANCEL = true;
                if (scanCode == S_MBUTTON)
                    Key_MBUTTON = true;
                if (scanCode == S_XBUTTON1)
                    Key_XBUTTON1 = true;
                if (scanCode == S_XBUTTON2)
                    Key_XBUTTON2 = true;
                if (scanCode == S_BACK)
                    Key_BACK = true;
                if (scanCode == S_Tab)
                    Key_Tab = true;
                if (scanCode == S_CLEAR)
                    Key_CLEAR = true;
                if (scanCode == S_Return)
                    Key_Return = true;
                if (scanCode == S_SHIFT)
                    Key_SHIFT = true;
                if (scanCode == S_CONTROL)
                    Key_CONTROL = true;
                if (scanCode == S_MENU)
                    Key_MENU = true;
                if (scanCode == S_PAUSE)
                    Key_PAUSE = true;
                if (scanCode == S_CAPITAL)
                    Key_CAPITAL = true;
                if (scanCode == S_KANA)
                    Key_KANA = true;
                if (scanCode == S_HANGEUL)
                    Key_HANGEUL = true;
                if (scanCode == S_HANGUL)
                    Key_HANGUL = true;
                if (scanCode == S_JUNJA)
                    Key_JUNJA = true;
                if (scanCode == S_FINAL)
                    Key_FINAL = true;
                if (scanCode == S_HANJA)
                    Key_HANJA = true;
                if (scanCode == S_KANJI)
                    Key_KANJI = true;
                if (scanCode == S_Escape)
                    Key_Escape = true;
                if (scanCode == S_CONVERT)
                    Key_CONVERT = true;
                if (scanCode == S_NONCONVERT)
                    Key_NONCONVERT = true;
                if (scanCode == S_ACCEPT)
                    Key_ACCEPT = true;
                if (scanCode == S_MODECHANGE)
                    Key_MODECHANGE = true;
                if (scanCode == S_Space)
                    Key_Space = true;
                if (scanCode == S_PRIOR)
                    Key_PRIOR = true;
                if (scanCode == S_NEXT)
                    Key_NEXT = true;
                if (scanCode == S_END)
                    Key_END = true;
                if (scanCode == S_HOME)
                    Key_HOME = true;
                if (scanCode == S_LEFT)
                    Key_LEFT = true;
                if (scanCode == S_UP)
                    Key_UP = true;
                if (scanCode == S_RIGHT)
                    Key_RIGHT = true;
                if (scanCode == S_DOWN)
                    Key_DOWN = true;
                if (scanCode == S_SELECT)
                    Key_SELECT = true;
                if (scanCode == S_PRINT)
                    Key_PRINT = true;
                if (scanCode == S_EXECUTE)
                    Key_EXECUTE = true;
                if (scanCode == S_SNAPSHOT)
                    Key_SNAPSHOT = true;
                if (scanCode == S_INSERT)
                    Key_INSERT = true;
                if (scanCode == S_DELETE)
                    Key_DELETE = true;
                if (scanCode == S_HELP)
                    Key_HELP = true;
                if (scanCode == S_APOSTROPHE)
                    Key_APOSTROPHE = true;
                if (scanCode == S_0)
                    Key_0 = true;
                if (scanCode == S_1)
                    Key_1 = true;
                if (scanCode == S_2)
                    Key_2 = true;
                if (scanCode == S_3)
                    Key_3 = true;
                if (scanCode == S_4)
                    Key_4 = true;
                if (scanCode == S_5)
                    Key_5 = true;
                if (scanCode == S_6)
                    Key_6 = true;
                if (scanCode == S_7)
                    Key_7 = true;
                if (scanCode == S_8)
                    Key_8 = true;
                if (scanCode == S_9)
                    Key_9 = true;
                if (scanCode == S_A)
                    Key_A = true;
                if (scanCode == S_B)
                    Key_B = true;
                if (scanCode == S_C)
                    Key_C = true;
                if (scanCode == S_D)
                    Key_D = true;
                if (scanCode == S_E)
                    Key_E = true;
                if (scanCode == S_F)
                    Key_F = true;
                if (scanCode == S_G)
                    Key_G = true;
                if (scanCode == S_H)
                    Key_H = true;
                if (scanCode == S_I)
                    Key_I = true;
                if (scanCode == S_J)
                    Key_J = true;
                if (scanCode == S_K)
                    Key_K = true;
                if (scanCode == S_L)
                    Key_L = true;
                if (scanCode == S_M)
                    Key_M = true;
                if (scanCode == S_N)
                    Key_N = true;
                if (scanCode == S_O)
                    Key_O = true;
                if (scanCode == S_P)
                    Key_P = true;
                if (scanCode == S_Q)
                    Key_Q = true;
                if (scanCode == S_R)
                    Key_R = true;
                if (scanCode == S_S)
                    Key_S = true;
                if (scanCode == S_T)
                    Key_T = true;
                if (scanCode == S_U)
                    Key_U = true;
                if (scanCode == S_V)
                    Key_V = true;
                if (scanCode == S_W)
                    Key_W = true;
                if (scanCode == S_X)
                    Key_X = true;
                if (scanCode == S_Y)
                    Key_Y = true;
                if (scanCode == S_Z)
                    Key_Z = true;
                if (scanCode == S_LWIN)
                    Key_LWIN = true;
                if (scanCode == S_RWIN)
                    Key_RWIN = true;
                if (scanCode == S_APPS)
                    Key_APPS = true;
                if (scanCode == S_SLEEP)
                    Key_SLEEP = true;
                if (scanCode == S_NUMPAD0)
                    Key_NUMPAD0 = true;
                if (scanCode == S_NUMPAD1)
                    Key_NUMPAD1 = true;
                if (scanCode == S_NUMPAD2)
                    Key_NUMPAD2 = true;
                if (scanCode == S_NUMPAD3)
                    Key_NUMPAD3 = true;
                if (scanCode == S_NUMPAD4)
                    Key_NUMPAD4 = true;
                if (scanCode == S_NUMPAD5)
                    Key_NUMPAD5 = true;
                if (scanCode == S_NUMPAD6)
                    Key_NUMPAD6 = true;
                if (scanCode == S_NUMPAD7)
                    Key_NUMPAD7 = true;
                if (scanCode == S_NUMPAD8)
                    Key_NUMPAD8 = true;
                if (scanCode == S_NUMPAD9)
                    Key_NUMPAD9 = true;
                if (scanCode == S_MULTIPLY)
                    Key_MULTIPLY = true;
                if (scanCode == S_ADD)
                    Key_ADD = true;
                if (scanCode == S_SEPARATOR)
                    Key_SEPARATOR = true;
                if (scanCode == S_SUBTRACT)
                    Key_SUBTRACT = true;
                if (scanCode == S_DECIMAL)
                    Key_DECIMAL = true;
                if (scanCode == S_DIVIDE)
                    Key_DIVIDE = true;
                if (scanCode == S_F1)
                    Key_F1 = true;
                if (scanCode == S_F2)
                    Key_F2 = true;
                if (scanCode == S_F3)
                    Key_F3 = true;
                if (scanCode == S_F4)
                    Key_F4 = true;
                if (scanCode == S_F5)
                    Key_F5 = true;
                if (scanCode == S_F6)
                    Key_F6 = true;
                if (scanCode == S_F7)
                    Key_F7 = true;
                if (scanCode == S_F8)
                    Key_F8 = true;
                if (scanCode == S_F9)
                    Key_F9 = true;
                if (scanCode == S_F10)
                    Key_F10 = true;
                if (scanCode == S_F11)
                    Key_F11 = true;
                if (scanCode == S_F12)
                    Key_F12 = true;
                if (scanCode == S_F13)
                    Key_F13 = true;
                if (scanCode == S_F14)
                    Key_F14 = true;
                if (scanCode == S_F15)
                    Key_F15 = true;
                if (scanCode == S_F16)
                    Key_F16 = true;
                if (scanCode == S_F17)
                    Key_F17 = true;
                if (scanCode == S_F18)
                    Key_F18 = true;
                if (scanCode == S_F19)
                    Key_F19 = true;
                if (scanCode == S_F20)
                    Key_F20 = true;
                if (scanCode == S_F21)
                    Key_F21 = true;
                if (scanCode == S_F22)
                    Key_F22 = true;
                if (scanCode == S_F23)
                    Key_F23 = true;
                if (scanCode == S_F24)
                    Key_F24 = true;
                if (scanCode == S_NUMLOCK)
                    Key_NUMLOCK = true;
                if (scanCode == S_SCROLL)
                    Key_SCROLL = true;
                if (scanCode == S_LeftShift)
                    Key_LeftShift = true;
                if (scanCode == S_RightShift)
                    Key_RightShift = true;
                if (scanCode == S_LeftControl)
                    Key_LeftControl = true;
                if (scanCode == S_RightControl)
                    Key_RightControl = true;
                if (scanCode == S_LMENU)
                    Key_LMENU = true;
                if (scanCode == S_RMENU)
                    Key_RMENU = true;
                if (scanCode == S_BROWSER_BACK)
                    Key_BROWSER_BACK = true;
                if (scanCode == S_BROWSER_FORWARD)
                    Key_BROWSER_FORWARD = true;
                if (scanCode == S_BROWSER_REFRESH)
                    Key_BROWSER_REFRESH = true;
                if (scanCode == S_BROWSER_STOP)
                    Key_BROWSER_STOP = true;
                if (scanCode == S_BROWSER_SEARCH)
                    Key_BROWSER_SEARCH = true;
                if (scanCode == S_BROWSER_FAVORITES)
                    Key_BROWSER_FAVORITES = true;
                if (scanCode == S_BROWSER_HOME)
                    Key_BROWSER_HOME = true;
                if (scanCode == S_VOLUME_MUTE)
                    Key_VOLUME_MUTE = true;
                if (scanCode == S_VOLUME_DOWN)
                    Key_VOLUME_DOWN = true;
                if (scanCode == S_VOLUME_UP)
                    Key_VOLUME_UP = true;
                if (scanCode == S_MEDIA_NEXT_TRACK)
                    Key_MEDIA_NEXT_TRACK = true;
                if (scanCode == S_MEDIA_PREV_TRACK)
                    Key_MEDIA_PREV_TRACK = true;
                if (scanCode == S_MEDIA_STOP)
                    Key_MEDIA_STOP = true;
                if (scanCode == S_MEDIA_PLAY_PAUSE)
                    Key_MEDIA_PLAY_PAUSE = true;
                if (scanCode == S_LAUNCH_MAIL)
                    Key_LAUNCH_MAIL = true;
                if (scanCode == S_LAUNCH_MEDIA_SELECT)
                    Key_LAUNCH_MEDIA_SELECT = true;
                if (scanCode == S_LAUNCH_APP1)
                    Key_LAUNCH_APP1 = true;
                if (scanCode == S_LAUNCH_APP2)
                    Key_LAUNCH_APP2 = true;
                if (scanCode == S_OEM_1)
                    Key_OEM_1 = true;
                if (scanCode == S_OEM_PLUS)
                    Key_OEM_PLUS = true;
                if (scanCode == S_OEM_COMMA)
                    Key_OEM_COMMA = true;
                if (scanCode == S_OEM_MINUS)
                    Key_OEM_MINUS = true;
                if (scanCode == S_OEM_PERIOD)
                    Key_OEM_PERIOD = true;
                if (scanCode == S_OEM_2)
                    Key_OEM_2 = true;
                if (scanCode == S_OEM_3)
                    Key_OEM_3 = true;
                if (scanCode == S_OEM_4)
                    Key_OEM_4 = true;
                if (scanCode == S_OEM_5)
                    Key_OEM_5 = true;
                if (scanCode == S_OEM_6)
                    Key_OEM_6 = true;
                if (scanCode == S_OEM_7)
                    Key_OEM_7 = true;
                if (scanCode == S_OEM_8)
                    Key_OEM_8 = true;
                if (scanCode == S_OEM_102)
                    Key_OEM_102 = true;
                if (scanCode == S_PROCESSKEY)
                    Key_PROCESSKEY = true;
                if (scanCode == S_PACKET)
                    Key_PACKET = true;
                if (scanCode == S_ATTN)
                    Key_ATTN = true;
                if (scanCode == S_CRSEL)
                    Key_CRSEL = true;
                if (scanCode == S_EXSEL)
                    Key_EXSEL = true;
                if (scanCode == S_EREOF)
                    Key_EREOF = true;
                if (scanCode == S_PLAY)
                    Key_PLAY = true;
                if (scanCode == S_ZOOM)
                    Key_ZOOM = true;
                if (scanCode == S_NONAME)
                    Key_NONAME = true;
                if (scanCode == S_PA1)
                    Key_PA1 = true;
                if (scanCode == S_OEM_CLEAR)
                    Key_OEM_CLEAR = true;
            }
            if (KeyboardHookButtonUp)
            {
                if (scanCode == S_LBUTTON)
                    Key_LBUTTON = false;
                if (scanCode == S_RBUTTON)
                    Key_RBUTTON = false;
                if (scanCode == S_CANCEL)
                    Key_CANCEL = false;
                if (scanCode == S_MBUTTON)
                    Key_MBUTTON = false;
                if (scanCode == S_XBUTTON1)
                    Key_XBUTTON1 = false;
                if (scanCode == S_XBUTTON2)
                    Key_XBUTTON2 = false;
                if (scanCode == S_BACK)
                    Key_BACK = false;
                if (scanCode == S_Tab)
                    Key_Tab = false;
                if (scanCode == S_CLEAR)
                    Key_CLEAR = false;
                if (scanCode == S_Return)
                    Key_Return = false;
                if (scanCode == S_SHIFT)
                    Key_SHIFT = false;
                if (scanCode == S_CONTROL)
                    Key_CONTROL = false;
                if (scanCode == S_MENU)
                    Key_MENU = false;
                if (scanCode == S_PAUSE)
                    Key_PAUSE = false;
                if (scanCode == S_CAPITAL)
                    Key_CAPITAL = false;
                if (scanCode == S_KANA)
                    Key_KANA = false;
                if (scanCode == S_HANGEUL)
                    Key_HANGEUL = false;
                if (scanCode == S_HANGUL)
                    Key_HANGUL = false;
                if (scanCode == S_JUNJA)
                    Key_JUNJA = false;
                if (scanCode == S_FINAL)
                    Key_FINAL = false;
                if (scanCode == S_HANJA)
                    Key_HANJA = false;
                if (scanCode == S_KANJI)
                    Key_KANJI = false;
                if (scanCode == S_Escape)
                    Key_Escape = false;
                if (scanCode == S_CONVERT)
                    Key_CONVERT = false;
                if (scanCode == S_NONCONVERT)
                    Key_NONCONVERT = false;
                if (scanCode == S_ACCEPT)
                    Key_ACCEPT = false;
                if (scanCode == S_MODECHANGE)
                    Key_MODECHANGE = false;
                if (scanCode == S_Space)
                    Key_Space = false;
                if (scanCode == S_PRIOR)
                    Key_PRIOR = false;
                if (scanCode == S_NEXT)
                    Key_NEXT = false;
                if (scanCode == S_END)
                    Key_END = false;
                if (scanCode == S_HOME)
                    Key_HOME = false;
                if (scanCode == S_LEFT)
                    Key_LEFT = false;
                if (scanCode == S_UP)
                    Key_UP = false;
                if (scanCode == S_RIGHT)
                    Key_RIGHT = false;
                if (scanCode == S_DOWN)
                    Key_DOWN = false;
                if (scanCode == S_SELECT)
                    Key_SELECT = false;
                if (scanCode == S_PRINT)
                    Key_PRINT = false;
                if (scanCode == S_EXECUTE)
                    Key_EXECUTE = false;
                if (scanCode == S_SNAPSHOT)
                    Key_SNAPSHOT = false;
                if (scanCode == S_INSERT)
                    Key_INSERT = false;
                if (scanCode == S_DELETE)
                    Key_DELETE = false;
                if (scanCode == S_HELP)
                    Key_HELP = false;
                if (scanCode == S_APOSTROPHE)
                    Key_APOSTROPHE = false;
                if (scanCode == S_0)
                    Key_0 = false;
                if (scanCode == S_1)
                    Key_1 = false;
                if (scanCode == S_2)
                    Key_2 = false;
                if (scanCode == S_3)
                    Key_3 = false;
                if (scanCode == S_4)
                    Key_4 = false;
                if (scanCode == S_5)
                    Key_5 = false;
                if (scanCode == S_6)
                    Key_6 = false;
                if (scanCode == S_7)
                    Key_7 = false;
                if (scanCode == S_8)
                    Key_8 = false;
                if (scanCode == S_9)
                    Key_9 = false;
                if (scanCode == S_A)
                    Key_A = false;
                if (scanCode == S_B)
                    Key_B = false;
                if (scanCode == S_C)
                    Key_C = false;
                if (scanCode == S_D)
                    Key_D = false;
                if (scanCode == S_E)
                    Key_E = false;
                if (scanCode == S_F)
                    Key_F = false;
                if (scanCode == S_G)
                    Key_G = false;
                if (scanCode == S_H)
                    Key_H = false;
                if (scanCode == S_I)
                    Key_I = false;
                if (scanCode == S_J)
                    Key_J = false;
                if (scanCode == S_K)
                    Key_K = false;
                if (scanCode == S_L)
                    Key_L = false;
                if (scanCode == S_M)
                    Key_M = false;
                if (scanCode == S_N)
                    Key_N = false;
                if (scanCode == S_O)
                    Key_O = false;
                if (scanCode == S_P)
                    Key_P = false;
                if (scanCode == S_Q)
                    Key_Q = false;
                if (scanCode == S_R)
                    Key_R = false;
                if (scanCode == S_S)
                    Key_S = false;
                if (scanCode == S_T)
                    Key_T = false;
                if (scanCode == S_U)
                    Key_U = false;
                if (scanCode == S_V)
                    Key_V = false;
                if (scanCode == S_W)
                    Key_W = false;
                if (scanCode == S_X)
                    Key_X = false;
                if (scanCode == S_Y)
                    Key_Y = false;
                if (scanCode == S_Z)
                    Key_Z = false;
                if (scanCode == S_LWIN)
                    Key_LWIN = false;
                if (scanCode == S_RWIN)
                    Key_RWIN = false;
                if (scanCode == S_APPS)
                    Key_APPS = false;
                if (scanCode == S_SLEEP)
                    Key_SLEEP = false;
                if (scanCode == S_NUMPAD0)
                    Key_NUMPAD0 = false;
                if (scanCode == S_NUMPAD1)
                    Key_NUMPAD1 = false;
                if (scanCode == S_NUMPAD2)
                    Key_NUMPAD2 = false;
                if (scanCode == S_NUMPAD3)
                    Key_NUMPAD3 = false;
                if (scanCode == S_NUMPAD4)
                    Key_NUMPAD4 = false;
                if (scanCode == S_NUMPAD5)
                    Key_NUMPAD5 = false;
                if (scanCode == S_NUMPAD6)
                    Key_NUMPAD6 = false;
                if (scanCode == S_NUMPAD7)
                    Key_NUMPAD7 = false;
                if (scanCode == S_NUMPAD8)
                    Key_NUMPAD8 = false;
                if (scanCode == S_NUMPAD9)
                    Key_NUMPAD9 = false;
                if (scanCode == S_MULTIPLY)
                    Key_MULTIPLY = false;
                if (scanCode == S_ADD)
                    Key_ADD = false;
                if (scanCode == S_SEPARATOR)
                    Key_SEPARATOR = false;
                if (scanCode == S_SUBTRACT)
                    Key_SUBTRACT = false;
                if (scanCode == S_DECIMAL)
                    Key_DECIMAL = false;
                if (scanCode == S_DIVIDE)
                    Key_DIVIDE = false;
                if (scanCode == S_F1)
                    Key_F1 = false;
                if (scanCode == S_F2)
                    Key_F2 = false;
                if (scanCode == S_F3)
                    Key_F3 = false;
                if (scanCode == S_F4)
                    Key_F4 = false;
                if (scanCode == S_F5)
                    Key_F5 = false;
                if (scanCode == S_F6)
                    Key_F6 = false;
                if (scanCode == S_F7)
                    Key_F7 = false;
                if (scanCode == S_F8)
                    Key_F8 = false;
                if (scanCode == S_F9)
                    Key_F9 = false;
                if (scanCode == S_F10)
                    Key_F10 = false;
                if (scanCode == S_F11)
                    Key_F11 = false;
                if (scanCode == S_F12)
                    Key_F12 = false;
                if (scanCode == S_F13)
                    Key_F13 = false;
                if (scanCode == S_F14)
                    Key_F14 = false;
                if (scanCode == S_F15)
                    Key_F15 = false;
                if (scanCode == S_F16)
                    Key_F16 = false;
                if (scanCode == S_F17)
                    Key_F17 = false;
                if (scanCode == S_F18)
                    Key_F18 = false;
                if (scanCode == S_F19)
                    Key_F19 = false;
                if (scanCode == S_F20)
                    Key_F20 = false;
                if (scanCode == S_F21)
                    Key_F21 = false;
                if (scanCode == S_F22)
                    Key_F22 = false;
                if (scanCode == S_F23)
                    Key_F23 = false;
                if (scanCode == S_F24)
                    Key_F24 = false;
                if (scanCode == S_NUMLOCK)
                    Key_NUMLOCK = false;
                if (scanCode == S_SCROLL)
                    Key_SCROLL = false;
                if (scanCode == S_LeftShift)
                    Key_LeftShift = false;
                if (scanCode == S_RightShift)
                    Key_RightShift = false;
                if (scanCode == S_LeftControl)
                    Key_LeftControl = false;
                if (scanCode == S_RightControl)
                    Key_RightControl = false;
                if (scanCode == S_LMENU)
                    Key_LMENU = false;
                if (scanCode == S_RMENU)
                    Key_RMENU = false;
                if (scanCode == S_BROWSER_BACK)
                    Key_BROWSER_BACK = false;
                if (scanCode == S_BROWSER_FORWARD)
                    Key_BROWSER_FORWARD = false;
                if (scanCode == S_BROWSER_REFRESH)
                    Key_BROWSER_REFRESH = false;
                if (scanCode == S_BROWSER_STOP)
                    Key_BROWSER_STOP = false;
                if (scanCode == S_BROWSER_SEARCH)
                    Key_BROWSER_SEARCH = false;
                if (scanCode == S_BROWSER_FAVORITES)
                    Key_BROWSER_FAVORITES = false;
                if (scanCode == S_BROWSER_HOME)
                    Key_BROWSER_HOME = false;
                if (scanCode == S_VOLUME_MUTE)
                    Key_VOLUME_MUTE = false;
                if (scanCode == S_VOLUME_DOWN)
                    Key_VOLUME_DOWN = false;
                if (scanCode == S_VOLUME_UP)
                    Key_VOLUME_UP = false;
                if (scanCode == S_MEDIA_NEXT_TRACK)
                    Key_MEDIA_NEXT_TRACK = false;
                if (scanCode == S_MEDIA_PREV_TRACK)
                    Key_MEDIA_PREV_TRACK = false;
                if (scanCode == S_MEDIA_STOP)
                    Key_MEDIA_STOP = false;
                if (scanCode == S_MEDIA_PLAY_PAUSE)
                    Key_MEDIA_PLAY_PAUSE = false;
                if (scanCode == S_LAUNCH_MAIL)
                    Key_LAUNCH_MAIL = false;
                if (scanCode == S_LAUNCH_MEDIA_SELECT)
                    Key_LAUNCH_MEDIA_SELECT = false;
                if (scanCode == S_LAUNCH_APP1)
                    Key_LAUNCH_APP1 = false;
                if (scanCode == S_LAUNCH_APP2)
                    Key_LAUNCH_APP2 = false;
                if (scanCode == S_OEM_1)
                    Key_OEM_1 = false;
                if (scanCode == S_OEM_PLUS)
                    Key_OEM_PLUS = false;
                if (scanCode == S_OEM_COMMA)
                    Key_OEM_COMMA = false;
                if (scanCode == S_OEM_MINUS)
                    Key_OEM_MINUS = false;
                if (scanCode == S_OEM_PERIOD)
                    Key_OEM_PERIOD = false;
                if (scanCode == S_OEM_2)
                    Key_OEM_2 = false;
                if (scanCode == S_OEM_3)
                    Key_OEM_3 = false;
                if (scanCode == S_OEM_4)
                    Key_OEM_4 = false;
                if (scanCode == S_OEM_5)
                    Key_OEM_5 = false;
                if (scanCode == S_OEM_6)
                    Key_OEM_6 = false;
                if (scanCode == S_OEM_7)
                    Key_OEM_7 = false;
                if (scanCode == S_OEM_8)
                    Key_OEM_8 = false;
                if (scanCode == S_OEM_102)
                    Key_OEM_102 = false;
                if (scanCode == S_PROCESSKEY)
                    Key_PROCESSKEY = false;
                if (scanCode == S_PACKET)
                    Key_PACKET = false;
                if (scanCode == S_ATTN)
                    Key_ATTN = false;
                if (scanCode == S_CRSEL)
                    Key_CRSEL = false;
                if (scanCode == S_EXSEL)
                    Key_EXSEL = false;
                if (scanCode == S_EREOF)
                    Key_EREOF = false;
                if (scanCode == S_PLAY)
                    Key_PLAY = false;
                if (scanCode == S_ZOOM)
                    Key_ZOOM = false;
                if (scanCode == S_NONAME)
                    Key_NONAME = false;
                if (scanCode == S_PA1)
                    Key_PA1 = false;
                if (scanCode == S_OEM_CLEAR)
                    Key_OEM_CLEAR = false;
            }
        }
    }
	public class LSPControl
    {
        private static IPEndPoint ipEnd;
        private static Socket client;
        public static string ip;
        public static int port;
        public static string control;
        public static void Connect()
        {
            ip = Form1.ip;
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            while (!client.Connected)
            {
                try
                {
                    port = Convert.ToInt32(Form1.controlport);
                    ipEnd = new IPEndPoint(IPAddress.Parse(ip), port);
                    client.Blocking = true;
                    client.Connect(ipEnd);
                }
                catch { }
                System.Threading.Thread.Sleep(1);
            }
            Task.Run(() => taskSend());
        }
        public static void taskSend()
        {
            while (Form1.running)
            {
                try
                {
                    byte[] clientData;
                    clientData = controlToByteArray(control);
                    client.Send(clientData);
                }
                catch { }
                System.Threading.Thread.Sleep(15);
            }
        }
        public static void Disconnect()
        {
            client.Close();
        }
        public static byte[] controlToByteArray(string control)
        {
            byte[] data = Encoding.ASCII.GetBytes(control);
            return data;
        }
    }
    public class LSPAudio
    {
        public static string ip;
        public static string port;
        public static WebSocket wsc;
        public static BufferedWaveProvider src;
        public static WasapiOut soundOut;
        public static void Connect()
        {
            System.Threading.Thread.Sleep(2000);
            ip = Form1.ip;
            port = Form1.audioport;
            String connectionString = "ws://" + ip + ":" + port + "/Audio";
            wsc = new WebSocket(connectionString);
            wsc.OnMessage += Ws_OnMessage;
            while (!wsc.IsAlive)
            {
                try
                {
                    wsc.Connect();
                    wsc.Send("Hello from client");
                }
                catch { }
                System.Threading.Thread.Sleep(1);
            }
            var enumerator = new MMDeviceEnumerator();
            MMDevice wasapi = null;
            foreach (var mmdevice in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                wasapi = mmdevice;
                break;
            }
            soundOut = new WasapiOut(wasapi, AudioClientShareMode.Exclusive, false, 1);
            src = new BufferedWaveProvider(soundOut.OutputWaveFormat);
            soundOut.Init(src);
            soundOut.Play();
        }
        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            byte[] Data = TrimEnd(e.RawData);
            if (Data.Length > 0)
                src.AddSamples(Data, 0, Data.Length);
        }
        public static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);
            Array.Resize(ref array, lastIndex + 1);
            return array;
        }
        public static void Disconnect()
        {
            wsc.Close();
            soundOut.Stop();
        }
    }
    public class LSPEmulate
    {
        private static Type program;
        private static object obj;
        private static Assembly assembly;
        private static System.CodeDom.Compiler.CompilerResults results;
        private static Microsoft.CSharp.CSharpCodeProvider provider;
        private static System.CodeDom.Compiler.CompilerParameters parameters;
        private static string code = @"
                using System;
                using System.Runtime.InteropServices;
                namespace StringToCode
                {
                    public class FooClass 
                    { 
                        public int pollcount = 0, sleeptime = 1;
                        public bool getstate = false;
                        string KeyboardMouseDriverType = """"; double MouseMoveX; double MouseMoveY; double MouseAbsX; double MouseAbsY; double MouseDesktopX; double MouseDesktopY; bool SendLeftClick; bool SendRightClick; bool SendMiddleClick; bool SendWheelUp; bool SendWheelDown; bool SendLeft; bool SendRight; bool SendUp; bool SendDown; bool SendLButton; bool SendRButton; bool SendCancel; bool SendMBUTTON; bool SendXBUTTON1; bool SendXBUTTON2; bool SendBack; bool SendTab; bool SendClear; bool SendReturn; bool SendSHIFT; bool SendCONTROL; bool SendMENU; bool SendPAUSE; bool SendCAPITAL; bool SendKANA; bool SendHANGEUL; bool SendHANGUL; bool SendJUNJA; bool SendFINAL; bool SendHANJA; bool SendKANJI; bool SendEscape; bool SendCONVERT; bool SendNONCONVERT; bool SendACCEPT; bool SendMODECHANGE; bool SendSpace; bool SendPRIOR; bool SendNEXT; bool SendEND; bool SendHOME; bool SendLEFT; bool SendUP; bool SendRIGHT; bool SendDOWN; bool SendSELECT; bool SendPRINT; bool SendEXECUTE; bool SendSNAPSHOT; bool SendINSERT; bool SendDELETE; bool SendHELP; bool SendAPOSTROPHE; bool Send0; bool Send1; bool Send2; bool Send3; bool Send4; bool Send5; bool Send6; bool Send7; bool Send8; bool Send9; bool SendA; bool SendB; bool SendC; bool SendD; bool SendE; bool SendF; bool SendG; bool SendH; bool SendI; bool SendJ; bool SendK; bool SendL; bool SendM; bool SendN; bool SendO; bool SendP; bool SendQ; bool SendR; bool SendS; bool SendT; bool SendU; bool SendV; bool SendW; bool SendX; bool SendY; bool SendZ; bool SendLWIN; bool SendRWIN; bool SendAPPS; bool SendSLEEP; bool SendNUMPAD0; bool SendNUMPAD1; bool SendNUMPAD2; bool SendNUMPAD3; bool SendNUMPAD4; bool SendNUMPAD5; bool SendNUMPAD6; bool SendNUMPAD7; bool SendNUMPAD8; bool SendNUMPAD9; bool SendMULTIPLY; bool SendADD; bool SendSEPARATOR; bool SendSUBTRACT; bool SendDECIMAL; bool SendDIVIDE; bool SendF1; bool SendF2; bool SendF3; bool SendF4; bool SendF5; bool SendF6; bool SendF7; bool SendF8; bool SendF9; bool SendF10; bool SendF11; bool SendF12; bool SendF13; bool SendF14; bool SendF15; bool SendF16; bool SendF17; bool SendF18; bool SendF19; bool SendF20; bool SendF21; bool SendF22; bool SendF23; bool SendF24; bool SendNUMLOCK; bool SendSCROLL; bool SendLeftShift; bool SendRightShift; bool SendLeftControl; bool SendRightControl; bool SendLMENU; bool SendRMENU; 
                        bool back; bool start; bool A; bool B; bool X; bool Y; bool up; bool left; bool down; bool right; bool leftstick; bool rightstick; bool leftbumper; bool rightbumper; bool lefttrigger; bool righttrigger; double leftstickx; double leftsticky; double rightstickx; double rightsticky; 
                        int keys12345, keys54321;
                        public static int[] wd = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
                        public static int[] wu = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
                        public static void valchanged(int n, bool val)
                        {
                            if (val)
                            {
                                if (wd[n] <= 1)
                                {
                                    wd[n] = wd[n] + 1;
                                }
                                wu[n] = 0;
                            }
                            else
                            {
                                if (wu[n] <= 1)
                                {
                                    wu[n] = wu[n] + 1;
                                }
                                wd[n] = 0;
                            }
                        }
                        public object[] Main(int hostscreenwidth, int hostscreenheight, int width, int height, double MouseHookX, double MouseHookY, bool MouseHookButtonX1, bool MouseHookButtonX2, bool MouseHookWheelUp, bool MouseHookWheelDown, bool MouseHookRightButton, bool MouseHookLeftButton, bool MouseHookMiddleButton, bool MouseHookXButton, bool Key_LBUTTON, bool Key_RBUTTON, bool Key_CANCEL, bool Key_MBUTTON, bool Key_XBUTTON1, bool Key_XBUTTON2, bool Key_BACK, bool Key_Tab, bool Key_CLEAR, bool Key_Return, bool Key_SHIFT, bool Key_CONTROL, bool Key_MENU, bool Key_PAUSE, bool Key_CAPITAL, bool Key_KANA, bool Key_HANGEUL, bool Key_HANGUL, bool Key_JUNJA, bool Key_FINAL, bool Key_HANJA, bool Key_KANJI, bool Key_Escape, bool Key_CONVERT, bool Key_NONCONVERT, bool Key_ACCEPT, bool Key_MODECHANGE, bool Key_Space, bool Key_PRIOR, bool Key_NEXT, bool Key_END, bool Key_HOME, bool Key_LEFT, bool Key_UP, bool Key_RIGHT, bool Key_DOWN, bool Key_SELECT, bool Key_PRINT, bool Key_EXECUTE, bool Key_SNAPSHOT, bool Key_INSERT, bool Key_DELETE, bool Key_HELP, bool Key_APOSTROPHE, bool Key_0, bool Key_1, bool Key_2, bool Key_3, bool Key_4, bool Key_5, bool Key_6, bool Key_7, bool Key_8, bool Key_9, bool Key_A, bool Key_B, bool Key_C, bool Key_D, bool Key_E, bool Key_F, bool Key_G, bool Key_H, bool Key_I, bool Key_J, bool Key_K, bool Key_L, bool Key_M, bool Key_N, bool Key_O, bool Key_P, bool Key_Q, bool Key_R, bool Key_S, bool Key_T, bool Key_U, bool Key_V, bool Key_W, bool Key_X, bool Key_Y, bool Key_Z, bool Key_LWIN, bool Key_RWIN, bool Key_APPS, bool Key_SLEEP, bool Key_NUMPAD0, bool Key_NUMPAD1, bool Key_NUMPAD2, bool Key_NUMPAD3, bool Key_NUMPAD4, bool Key_NUMPAD5, bool Key_NUMPAD6, bool Key_NUMPAD7, bool Key_NUMPAD8, bool Key_NUMPAD9, bool Key_MULTIPLY, bool Key_ADD, bool Key_SEPARATOR, bool Key_SUBTRACT, bool Key_DECIMAL, bool Key_DIVIDE, bool Key_F1, bool Key_F2, bool Key_F3, bool Key_F4, bool Key_F5, bool Key_F6, bool Key_F7, bool Key_F8, bool Key_F9, bool Key_F10, bool Key_F11, bool Key_F12, bool Key_F13, bool Key_F14, bool Key_F15, bool Key_F16, bool Key_F17, bool Key_F18, bool Key_F19, bool Key_F20, bool Key_F21, bool Key_F22, bool Key_F23, bool Key_F24, bool Key_NUMLOCK, bool Key_SCROLL, bool Key_LeftShift, bool Key_RightShift, bool Key_LeftControl, bool Key_RightControl, bool Key_LMENU, bool Key_RMENU, bool Key_BROWSER_BACK, bool Key_BROWSER_FORWARD, bool Key_BROWSER_REFRESH, bool Key_BROWSER_STOP, bool Key_BROWSER_SEARCH, bool Key_BROWSER_FAVORITES, bool Key_BROWSER_HOME, bool Key_VOLUME_MUTE, bool Key_VOLUME_DOWN, bool Key_VOLUME_UP, bool Key_MEDIA_NEXT_TRACK, bool Key_MEDIA_PREV_TRACK, bool Key_MEDIA_STOP, bool Key_MEDIA_PLAY_PAUSE, bool Key_LAUNCH_MAIL, bool Key_LAUNCH_MEDIA_SELECT, bool Key_LAUNCH_APP1, bool Key_LAUNCH_APP2, bool Key_OEM_1, bool Key_OEM_PLUS, bool Key_OEM_COMMA, bool Key_OEM_MINUS, bool Key_OEM_PERIOD, bool Key_OEM_2, bool Key_OEM_3, bool Key_OEM_4, bool Key_OEM_5, bool Key_OEM_6, bool Key_OEM_7, bool Key_OEM_8, bool Key_OEM_102, bool Key_PROCESSKEY, bool Key_PACKET, bool Key_ATTN, bool Key_CRSEL, bool Key_EXSEL, bool Key_EREOF, bool Key_PLAY, bool Key_ZOOM, bool Key_NONAME, bool Key_PA1, bool Key_OEM_CLEAR, bool ButtonAPressed, bool ButtonBPressed, bool ButtonXPressed, bool ButtonYPressed, bool ButtonStartPressed, bool ButtonBackPressed, bool ButtonDownPressed, bool ButtonUpPressed, bool ButtonLeftPressed, bool ButtonRightPressed, bool ButtonShoulderLeftPressed, bool ButtonShoulderRightPressed, bool ThumbpadLeftPressed, bool ThumbpadRightPressed, double TriggerLeftPosition, double TriggerRightPosition, double ThumbLeftX, double ThumbLeftY, double ThumbRightX, double ThumbRightY)
                        {
                            funct_driver
                            return new object[] { hostscreenwidth, hostscreenheight, sleeptime, KeyboardMouseDriverType, MouseMoveX, MouseMoveY, MouseAbsX, MouseAbsY, MouseDesktopX, MouseDesktopY, SendLeftClick, SendRightClick, SendMiddleClick, SendWheelUp, SendWheelDown, SendLeft, SendRight, SendUp, SendDown, SendLButton, SendRButton, SendCancel, SendMBUTTON, SendXBUTTON1, SendXBUTTON2, SendBack, SendTab, SendClear, SendReturn, SendSHIFT, SendCONTROL, SendMENU, SendPAUSE, SendCAPITAL, SendKANA, SendHANGEUL, SendHANGUL, SendJUNJA, SendFINAL, SendHANJA, SendKANJI, SendEscape, SendCONVERT, SendNONCONVERT, SendACCEPT, SendMODECHANGE, SendSpace, SendPRIOR, SendNEXT, SendEND, SendHOME, SendLEFT, SendUP, SendRIGHT, SendDOWN, SendSELECT, SendPRINT, SendEXECUTE, SendSNAPSHOT, SendINSERT, SendDELETE, SendHELP, SendAPOSTROPHE, Send0, Send1, Send2, Send3, Send4, Send5, Send6, Send7, Send8, Send9, SendA, SendB, SendC, SendD, SendE, SendF, SendG, SendH, SendI, SendJ, SendK, SendL, SendM, SendN, SendO, SendP, SendQ, SendR, SendS, SendT, SendU, SendV, SendW, SendX, SendY, SendZ, SendLWIN, SendRWIN, SendAPPS, SendSLEEP, SendNUMPAD0, SendNUMPAD1, SendNUMPAD2, SendNUMPAD3, SendNUMPAD4, SendNUMPAD5, SendNUMPAD6, SendNUMPAD7, SendNUMPAD8, SendNUMPAD9, SendMULTIPLY, SendADD, SendSEPARATOR, SendSUBTRACT, SendDECIMAL, SendDIVIDE, SendF1, SendF2, SendF3, SendF4, SendF5, SendF6, SendF7, SendF8, SendF9, SendF10, SendF11, SendF12, SendF13, SendF14, SendF15, SendF16, SendF17, SendF18, SendF19, SendF20, SendF21, SendF22, SendF23, SendF24, SendNUMLOCK, SendSCROLL, SendLeftShift, SendRightShift, SendLeftControl, SendRightControl, SendLMENU, SendRMENU, back, start, A, B, X, Y, up, left, down, right, leftstick, rightstick, leftbumper, rightbumper, lefttrigger, righttrigger, leftstickx, leftsticky, rightstickx, rightsticky };
                        }
                        public double Scale(double value, double min, double max, double minScale, double maxScale)
                        {
                            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
                            return scaled;
                        }
                    }
                }";
        private static int sleeptime;
        private static object[] val;
        private static Controller[] controller = new Controller[] { null };
        private static State state;
        public static bool iscontrollerconnected = false;
        public static bool Controller1ButtonAPressed = false;
        public static bool Controller1ButtonBPressed = false;
        public static bool Controller1ButtonXPressed = false;
        public static bool Controller1ButtonYPressed = false;
        public static bool Controller1ButtonStartPressed = false;
        public static bool Controller1ButtonBackPressed = false;
        public static bool Controller1ButtonDownPressed = false;
        public static bool Controller1ButtonUpPressed = false;
        public static bool Controller1ButtonLeftPressed = false;
        public static bool Controller1ButtonRightPressed = false;
        public static bool Controller1ButtonShoulderLeftPressed = false;
        public static bool Controller1ButtonShoulderRightPressed = false;
        public static bool Controller1ThumbpadLeftPressed = false;
        public static bool Controller1ThumbpadRightPressed = false;
        public static double Controller1TriggerLeftPosition = 0;
        public static double Controller1TriggerRightPosition = 0;
        public static double Controller1ThumbLeftX = 0;
        public static double Controller1ThumbLeftY = 0;
        public static double Controller1ThumbRightX = 0;
        public static double Controller1ThumbRightY = 0;
        public static int hostscreenwidth;
        public static int hostscreenheight;
        public static int[] wd = { 2 };
        public static int[] wu = { 2 };
        public static void valchanged(int n, bool val)
        {
            if (val)
            {
                if (wd[n] <= 1)
                {
                    wd[n] = wd[n] + 1;
                }
                wu[n] = 0;
            }
            else
            {
                if (wu[n] <= 1)
                {
                    wu[n] = wu[n] + 1;
                }
                wd[n] = 0;
            }
        }
        public static void Connect()
        {
            try
            {
                controller = new Controller[] { null };
                var controllers = new[] { new Controller(UserIndex.One) };
                foreach (var selectControler in controllers)
                {
                    if (selectControler.IsConnected)
                    {
                        controller[0] = selectControler;
                        break;
                    }
                }
                if (controller[0] == null)
                {
                    iscontrollerconnected = false;
                }
                else
                {
                    iscontrollerconnected = true;
                }
            }
            catch { }
        }
        public static void TestControl()
        {
            string finalcode = code.Replace("funct_driver", Form1.stringscript);
            parameters = new System.CodeDom.Compiler.CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            provider = new Microsoft.CSharp.CSharpCodeProvider();
            results = provider.CompileAssemblyFromSource(parameters, finalcode);
            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}) : {1}", error.ErrorNumber, error.ErrorText));
                }
                MessageBox.Show("Script Error :\n\r" + sb.ToString());
                return;
            }
            else
            {
                MessageBox.Show("Script ok.");
            }
        }
        public static void GetControl()
        {
            string finalcode = code.Replace("funct_driver", Form1.stringscript);
            parameters = new System.CodeDom.Compiler.CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            provider = new Microsoft.CSharp.CSharpCodeProvider();
            results = provider.CompileAssemblyFromSource(parameters, finalcode);
            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}) : {1}", error.ErrorNumber, error.ErrorText));
                }
                MessageBox.Show("Script Error :\n\r" + sb.ToString());
                return;
            }
            assembly = results.CompiledAssembly;
            program = assembly.GetType("StringToCode.FooClass");
            obj = Activator.CreateInstance(program);
            Form1.Getstate = false;
            sleeptime = 1;
            while (Form1.runningscript)
            {
                if (iscontrollerconnected)
                {
                    ControllerProcess();
                }
                Form1.KeyboardHookProcessButtons();
                valchanged(0, Form1.Key_ADD);
                if (wd[0] == 1 & !Form1.Getstate)
                {
                    Form1.width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                    Form1.height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                    Form1.Getstate = true;
                }
                else
                {
                    if (wd[0] == 1 & Form1.Getstate)
                    {
                        Form1.Getstate = false;
                    }
                }
                Form1.MouseHookProcessButtons();
                val = (object[])program.InvokeMember("Main", BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, new object[] { Form1.hostwidth, Form1.hostheight, Form1.width, Form1.height, Form1.MouseHookX, Form1.MouseHookY, Form1.MouseHookButtonX1, Form1.MouseHookButtonX2, Form1.MouseHookWheelUp, Form1.MouseHookWheelDown, Form1.MouseHookRightButton, Form1.MouseHookLeftButton, Form1.MouseHookMiddleButton, Form1.MouseHookXButton, Form1.Key_LBUTTON, Form1.Key_RBUTTON, Form1.Key_CANCEL, Form1.Key_MBUTTON, Form1.Key_XBUTTON1, Form1.Key_XBUTTON2, Form1.Key_BACK, Form1.Key_Tab, Form1.Key_CLEAR, Form1.Key_Return, Form1.Key_SHIFT, Form1.Key_CONTROL, Form1.Key_MENU, Form1.Key_PAUSE, Form1.Key_CAPITAL, Form1.Key_KANA, Form1.Key_HANGEUL, Form1.Key_HANGUL, Form1.Key_JUNJA, Form1.Key_FINAL, Form1.Key_HANJA, Form1.Key_KANJI, Form1.Key_Escape, Form1.Key_CONVERT, Form1.Key_NONCONVERT, Form1.Key_ACCEPT, Form1.Key_MODECHANGE, Form1.Key_Space, Form1.Key_PRIOR, Form1.Key_NEXT, Form1.Key_END, Form1.Key_HOME, Form1.Key_LEFT, Form1.Key_UP, Form1.Key_RIGHT, Form1.Key_DOWN, Form1.Key_SELECT, Form1.Key_PRINT, Form1.Key_EXECUTE, Form1.Key_SNAPSHOT, Form1.Key_INSERT, Form1.Key_DELETE, Form1.Key_HELP, Form1.Key_APOSTROPHE, Form1.Key_0, Form1.Key_1, Form1.Key_2, Form1.Key_3, Form1.Key_4, Form1.Key_5, Form1.Key_6, Form1.Key_7, Form1.Key_8, Form1.Key_9, Form1.Key_A, Form1.Key_B, Form1.Key_C, Form1.Key_D, Form1.Key_E, Form1.Key_F, Form1.Key_G, Form1.Key_H, Form1.Key_I, Form1.Key_J, Form1.Key_K, Form1.Key_L, Form1.Key_M, Form1.Key_N, Form1.Key_O, Form1.Key_P, Form1.Key_Q, Form1.Key_R, Form1.Key_S, Form1.Key_T, Form1.Key_U, Form1.Key_V, Form1.Key_W, Form1.Key_X, Form1.Key_Y, Form1.Key_Z, Form1.Key_LWIN, Form1.Key_RWIN, Form1.Key_APPS, Form1.Key_SLEEP, Form1.Key_NUMPAD0, Form1.Key_NUMPAD1, Form1.Key_NUMPAD2, Form1.Key_NUMPAD3, Form1.Key_NUMPAD4, Form1.Key_NUMPAD5, Form1.Key_NUMPAD6, Form1.Key_NUMPAD7, Form1.Key_NUMPAD8, Form1.Key_NUMPAD9, Form1.Key_MULTIPLY, Form1.Key_ADD, Form1.Key_SEPARATOR, Form1.Key_SUBTRACT, Form1.Key_DECIMAL, Form1.Key_DIVIDE, Form1.Key_F1, Form1.Key_F2, Form1.Key_F3, Form1.Key_F4, Form1.Key_F5, Form1.Key_F6, Form1.Key_F7, Form1.Key_F8, Form1.Key_F9, Form1.Key_F10, Form1.Key_F11, Form1.Key_F12, Form1.Key_F13, Form1.Key_F14, Form1.Key_F15, Form1.Key_F16, Form1.Key_F17, Form1.Key_F18, Form1.Key_F19, Form1.Key_F20, Form1.Key_F21, Form1.Key_F22, Form1.Key_F23, Form1.Key_F24, Form1.Key_NUMLOCK, Form1.Key_SCROLL, Form1.Key_LeftShift, Form1.Key_RightShift, Form1.Key_LeftControl, Form1.Key_RightControl, Form1.Key_LMENU, Form1.Key_RMENU, Form1.Key_BROWSER_BACK, Form1.Key_BROWSER_FORWARD, Form1.Key_BROWSER_REFRESH, Form1.Key_BROWSER_STOP, Form1.Key_BROWSER_SEARCH, Form1.Key_BROWSER_FAVORITES, Form1.Key_BROWSER_HOME, Form1.Key_VOLUME_MUTE, Form1.Key_VOLUME_DOWN, Form1.Key_VOLUME_UP, Form1.Key_MEDIA_NEXT_TRACK, Form1.Key_MEDIA_PREV_TRACK, Form1.Key_MEDIA_STOP, Form1.Key_MEDIA_PLAY_PAUSE, Form1.Key_LAUNCH_MAIL, Form1.Key_LAUNCH_MEDIA_SELECT, Form1.Key_LAUNCH_APP1, Form1.Key_LAUNCH_APP2, Form1.Key_OEM_1, Form1.Key_OEM_PLUS, Form1.Key_OEM_COMMA, Form1.Key_OEM_MINUS, Form1.Key_OEM_PERIOD, Form1.Key_OEM_2, Form1.Key_OEM_3, Form1.Key_OEM_4, Form1.Key_OEM_5, Form1.Key_OEM_6, Form1.Key_OEM_7, Form1.Key_OEM_8, Form1.Key_OEM_102, Form1.Key_PROCESSKEY, Form1.Key_PACKET, Form1.Key_ATTN, Form1.Key_CRSEL, Form1.Key_EXSEL, Form1.Key_EREOF, Form1.Key_PLAY, Form1.Key_ZOOM, Form1.Key_NONAME, Form1.Key_PA1, Form1.Key_OEM_CLEAR, Controller1ButtonAPressed, Controller1ButtonBPressed, Controller1ButtonXPressed, Controller1ButtonYPressed, Controller1ButtonStartPressed, Controller1ButtonBackPressed, Controller1ButtonDownPressed, Controller1ButtonUpPressed, Controller1ButtonLeftPressed, Controller1ButtonRightPressed, Controller1ButtonShoulderLeftPressed, Controller1ButtonShoulderRightPressed, Controller1ThumbpadLeftPressed, Controller1ThumbpadRightPressed, Controller1TriggerLeftPosition, Controller1TriggerRightPosition, Controller1ThumbLeftX, Controller1ThumbLeftY, Controller1ThumbRightX, Controller1ThumbRightY });
                string hostscreenwidth = val[0].ToString(); string hostscreenheight = val[1].ToString(); sleeptime = (int)val[2];
                string KeyboardMouseDriverType = val[3].ToString(); string MouseMoveX = val[4].ToString(); string MouseMoveY = val[5].ToString(); string MouseAbsX = val[6].ToString(); string MouseAbsY = val[7].ToString(); string MouseDesktopX = val[8].ToString(); string MouseDesktopY = val[9].ToString(); string SendLeftClick = val[10].ToString(); string SendRightClick = val[11].ToString(); string SendMiddleClick = val[12].ToString(); string SendWheelUp = val[13].ToString(); string SendWheelDown = val[14].ToString(); string SendLeft = val[15].ToString(); string SendRight = val[16].ToString(); string SendUp = val[17].ToString(); string SendDown = val[18].ToString(); string SendLButton = val[19].ToString(); string SendRButton = val[20].ToString(); string SendCancel = val[21].ToString(); string SendMBUTTON = val[22].ToString(); string SendXBUTTON1 = val[23].ToString(); string SendXBUTTON2 = val[24].ToString(); string SendBack = val[25].ToString(); string SendTab = val[26].ToString(); string SendClear = val[27].ToString(); string SendReturn = val[28].ToString(); string SendSHIFT = val[29].ToString(); string SendCONTROL = val[30].ToString(); string SendMENU = val[31].ToString(); string SendPAUSE = val[32].ToString(); string SendCAPITAL = val[33].ToString(); string SendKANA = val[34].ToString(); string SendHANGEUL = val[35].ToString(); string SendHANGUL = val[36].ToString(); string SendJUNJA = val[37].ToString(); string SendFINAL = val[38].ToString(); string SendHANJA = val[39].ToString(); string SendKANJI = val[40].ToString(); string SendEscape = val[41].ToString(); string SendCONVERT = val[42].ToString(); string SendNONCONVERT = val[43].ToString(); string SendACCEPT = val[44].ToString(); string SendMODECHANGE = val[45].ToString(); string SendSpace = val[46].ToString(); string SendPRIOR = val[47].ToString(); string SendNEXT = val[48].ToString(); string SendEND = val[49].ToString(); string SendHOME = val[50].ToString(); string SendLEFT = val[51].ToString(); string SendUP = val[52].ToString(); string SendRIGHT = val[53].ToString(); string SendDOWN = val[54].ToString(); string SendSELECT = val[55].ToString(); string SendPRINT = val[56].ToString(); string SendEXECUTE = val[57].ToString(); string SendSNAPSHOT = val[58].ToString(); string SendINSERT = val[59].ToString(); string SendDELETE = val[60].ToString(); string SendHELP = val[61].ToString(); string SendAPOSTROPHE = val[62].ToString(); string Send0 = val[63].ToString(); string Send1 = val[64].ToString(); string Send2 = val[65].ToString(); string Send3 = val[66].ToString(); string Send4 = val[67].ToString(); string Send5 = val[68].ToString(); string Send6 = val[69].ToString(); string Send7 = val[70].ToString(); string Send8 = val[71].ToString(); string Send9 = val[72].ToString(); string SendA = val[73].ToString(); string SendB = val[74].ToString(); string SendC = val[75].ToString(); string SendD = val[76].ToString(); string SendE = val[77].ToString(); string SendF = val[78].ToString(); string SendG = val[79].ToString(); string SendH = val[80].ToString(); string SendI = val[81].ToString(); string SendJ = val[82].ToString(); string SendK = val[83].ToString(); string SendL = val[84].ToString(); string SendM = val[85].ToString(); string SendN = val[86].ToString(); string SendO = val[87].ToString(); string SendP = val[88].ToString(); string SendQ = val[89].ToString(); string SendR = val[90].ToString(); string SendS = val[91].ToString(); string SendT = val[92].ToString(); string SendU = val[93].ToString(); string SendV = val[94].ToString(); string SendW = val[95].ToString(); string SendX = val[96].ToString(); string SendY = val[97].ToString(); string SendZ = val[98].ToString(); string SendLWIN = val[99].ToString(); string SendRWIN = val[100].ToString(); string SendAPPS = val[101].ToString(); string SendSLEEP = val[102].ToString(); string SendNUMPAD0 = val[103].ToString(); string SendNUMPAD1 = val[104].ToString(); string SendNUMPAD2 = val[105].ToString(); string SendNUMPAD3 = val[106].ToString(); string SendNUMPAD4 = val[107].ToString(); string SendNUMPAD5 = val[108].ToString(); string SendNUMPAD6 = val[109].ToString(); string SendNUMPAD7 = val[110].ToString(); string SendNUMPAD8 = val[111].ToString(); string SendNUMPAD9 = val[112].ToString(); string SendMULTIPLY = val[113].ToString(); string SendADD = val[114].ToString(); string SendSEPARATOR = val[115].ToString(); string SendSUBTRACT = val[116].ToString(); string SendDECIMAL = val[117].ToString(); string SendDIVIDE = val[118].ToString(); string SendF1 = val[119].ToString(); string SendF2 = val[120].ToString(); string SendF3 = val[121].ToString(); string SendF4 = val[122].ToString(); string SendF5 = val[123].ToString(); string SendF6 = val[124].ToString(); string SendF7 = val[125].ToString(); string SendF8 = val[126].ToString(); string SendF9 = val[127].ToString(); string SendF10 = val[128].ToString(); string SendF11 = val[129].ToString(); string SendF12 = val[130].ToString(); string SendF13 = val[131].ToString(); string SendF14 = val[132].ToString(); string SendF15 = val[133].ToString(); string SendF16 = val[134].ToString(); string SendF17 = val[135].ToString(); string SendF18 = val[136].ToString(); string SendF19 = val[137].ToString(); string SendF20 = val[138].ToString(); string SendF21 = val[139].ToString(); string SendF22 = val[140].ToString(); string SendF23 = val[141].ToString(); string SendF24 = val[142].ToString(); string SendNUMLOCK = val[143].ToString(); string SendSCROLL = val[144].ToString(); string SendLeftShift = val[145].ToString(); string SendRightShift = val[146].ToString(); string SendLeftControl = val[147].ToString(); string SendRightControl = val[148].ToString(); string SendLMENU = val[149].ToString(); string SendRMENU = val[150].ToString();
                string back = val[151].ToString(); string start = val[152].ToString(); string A = val[153].ToString(); string B = val[154].ToString(); string X = val[155].ToString(); string Y = val[156].ToString(); string up = val[157].ToString(); string left = val[158].ToString(); string down = val[159].ToString(); string right = val[160].ToString(); string leftstick = val[161].ToString(); string rightstick = val[162].ToString(); string leftbumper = val[163].ToString(); string rightbumper = val[164].ToString(); string lefttrigger = val[165].ToString(); string righttrigger = val[166].ToString(); string leftstickx = val[167].ToString(); string leftsticky = val[168].ToString(); string rightstickx = val[169].ToString(); string rightsticky = val[170].ToString();
                LSPControl.control = hostscreenwidth + "," + hostscreenheight + "," + sleeptime.ToString() + "," + KeyboardMouseDriverType + "," + MouseMoveX + "," + MouseMoveY + "," + MouseAbsX + "," + MouseAbsY + "," + MouseDesktopX + "," + MouseDesktopY + "," + SendLeftClick + "," + SendRightClick + "," + SendMiddleClick + "," + SendWheelUp + "," + SendWheelDown + "," + SendLeft + "," + SendRight + "," + SendUp + "," + SendDown + "," + SendLButton + "," + SendRButton + "," + SendCancel + "," + SendMBUTTON + "," + SendXBUTTON1 + "," + SendXBUTTON2 + "," + SendBack + "," + SendTab + "," + SendClear + "," + SendReturn + "," + SendSHIFT + "," + SendCONTROL + "," + SendMENU + "," + SendPAUSE + "," + SendCAPITAL + "," + SendKANA + "," + SendHANGEUL + "," + SendHANGUL + "," + SendJUNJA + "," + SendFINAL + "," + SendHANJA + "," + SendKANJI + "," + SendEscape + "," + SendCONVERT + "," + SendNONCONVERT + "," + SendACCEPT + "," + SendMODECHANGE + "," + SendSpace + "," + SendPRIOR + "," + SendNEXT + "," + SendEND + "," + SendHOME + "," + SendLEFT + "," + SendUP + "," + SendRIGHT + "," + SendDOWN + "," + SendSELECT + "," + SendPRINT + "," + SendEXECUTE + "," + SendSNAPSHOT + "," + SendINSERT + "," + SendDELETE + "," + SendHELP + "," + SendAPOSTROPHE + "," + Send0 + "," + Send1 + "," + Send2 + "," + Send3 + "," + Send4 + "," + Send5 + "," + Send6 + "," + Send7 + "," + Send8 + "," + Send9 + "," + SendA + "," + SendB + "," + SendC + "," + SendD + "," + SendE + "," + SendF + "," + SendG + "," + SendH + "," + SendI + "," + SendJ + "," + SendK + "," + SendL + "," + SendM + "," + SendN + "," + SendO + "," + SendP + "," + SendQ + "," + SendR + "," + SendS + "," + SendT + "," + SendU + "," + SendV + "," + SendW + "," + SendX + "," + SendY + "," + SendZ + "," + SendLWIN + "," + SendRWIN + "," + SendAPPS + "," + SendSLEEP + "," + SendNUMPAD0 + "," + SendNUMPAD1 + "," + SendNUMPAD2 + "," + SendNUMPAD3 + "," + SendNUMPAD4 + "," + SendNUMPAD5 + "," + SendNUMPAD6 + "," + SendNUMPAD7 + "," + SendNUMPAD8 + "," + SendNUMPAD9 + "," + SendMULTIPLY + "," + SendADD + "," + SendSEPARATOR + "," + SendSUBTRACT + "," + SendDECIMAL + "," + SendDIVIDE + "," + SendF1 + "," + SendF2 + "," + SendF3 + "," + SendF4 + "," + SendF5 + "," + SendF6 + "," + SendF7 + "," + SendF8 + "," + SendF9 + "," + SendF10 + "," + SendF11 + "," + SendF12 + "," + SendF13 + "," + SendF14 + "," + SendF15 + "," + SendF16 + "," + SendF17 + "," + SendF18 + "," + SendF19 + "," + SendF20 + "," + SendF21 + "," + SendF22 + "," + SendF23 + "," + SendF24 + "," + SendNUMLOCK + "," + SendSCROLL + "," + SendLeftShift + "," + SendRightShift + "," + SendLeftControl + "," + SendRightControl + "," + SendLMENU + "," + SendRMENU + "," + back + "," + start + "," + A + "," + B + "," + X + "," + Y + "," + up + "," + left + "," + down + "," + right + "," + leftstick + "," + rightstick + "," + leftbumper + "," + rightbumper + "," + lefttrigger + "," + righttrigger + "," + leftstickx + "," + leftsticky + "," + rightstickx + "," + rightsticky + ",end";
                System.Threading.Thread.Sleep(sleeptime);
            }
            iscontrollerconnected = false;
        }
        private static void ControllerProcess()
        {
            state = controller[0].GetState();
            if (state.Gamepad.Buttons.ToString().Contains("A"))
                Controller1ButtonAPressed = true;
            else
                Controller1ButtonAPressed = false;
            if (state.Gamepad.Buttons.ToString().EndsWith("B") | state.Gamepad.Buttons.ToString().Contains("B, "))
                Controller1ButtonBPressed = true;
            else
                Controller1ButtonBPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("X"))
                Controller1ButtonXPressed = true;
            else
                Controller1ButtonXPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("Y"))
                Controller1ButtonYPressed = true;
            else
                Controller1ButtonYPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("Start"))
                Controller1ButtonStartPressed = true;
            else
                Controller1ButtonStartPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("Back"))
                Controller1ButtonBackPressed = true;
            else
                Controller1ButtonBackPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("DPadDown"))
                Controller1ButtonDownPressed = true;
            else
                Controller1ButtonDownPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("DPadUp"))
                Controller1ButtonUpPressed = true;
            else
                Controller1ButtonUpPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("DPadLeft"))
                Controller1ButtonLeftPressed = true;
            else
                Controller1ButtonLeftPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("DPadRight"))
                Controller1ButtonRightPressed = true;
            else
                Controller1ButtonRightPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("LeftShoulder"))
                Controller1ButtonShoulderLeftPressed = true;
            else
                Controller1ButtonShoulderLeftPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("RightShoulder"))
                Controller1ButtonShoulderRightPressed = true;
            else
                Controller1ButtonShoulderRightPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("LeftThumb"))
                Controller1ThumbpadLeftPressed = true;
            else
                Controller1ThumbpadLeftPressed = false;
            if (state.Gamepad.Buttons.ToString().Contains("RightThumb"))
                Controller1ThumbpadRightPressed = true;
            else
                Controller1ThumbpadRightPressed = false;
            Controller1TriggerLeftPosition = state.Gamepad.LeftTrigger;
            Controller1TriggerRightPosition = state.Gamepad.RightTrigger;
            Controller1ThumbLeftX = state.Gamepad.LeftThumbX;
            Controller1ThumbLeftY = state.Gamepad.LeftThumbY;
            Controller1ThumbRightX = state.Gamepad.RightThumbX;
            Controller1ThumbRightY = state.Gamepad.RightThumbY;
        }
    }
    public class MouseHook
    {
        public static int MouseHookWheel, MouseHookButtonX, MouseDesktopHookX, MouseDesktopHookY, mousehookx, mousehooky, MouseHookTime;
        public static bool MouseHookLeftButton, MouseHookRightButton, MouseHookLeftDoubleClick, MouseHookRightDoubleClick, MouseHookMiddleButton, MouseHookXButton;
        public static int mousehookwheelcount, mousehookbuttoncount;
        public static bool mousehookwheelbool, mousehookbuttonbool;
        public static bool MouseHookButtonX1, MouseHookButtonX2, MouseHookWheelUp, MouseHookWheelDown;
        public delegate IntPtr MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
        public MouseHookHandler hookHandler;
        public MSLLHOOKSTRUCT mouseStruct;
        public delegate void MouseHookCallback(MSLLHOOKSTRUCT mouseStruct);
        public event MouseHookCallback Hook;
        public IntPtr hookID = IntPtr.Zero;
        public static int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        public static int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        public static System.Collections.Generic.List<double> time = new System.Collections.Generic.List<double>();
        public void Install()
        {
            hookHandler = HookFunc;
            hookID = SetHook(hookHandler);
        }
        public void Uninstall()
        {
            if (hookID == IntPtr.Zero)
                return;
            UnhookWindowsHookEx(hookID);
            hookID = IntPtr.Zero;
        }
        ~MouseHook()
        {
            Uninstall();
        }
        public IntPtr SetHook(MouseHookHandler proc)
        {
            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(module.ModuleName), 0);
        }
        public IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            mouseStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            if (MouseHook.MouseMessages.WM_RBUTTONDOWN == (MouseHook.MouseMessages)wParam)
                MouseHookRightButton = true;
            if (MouseHook.MouseMessages.WM_RBUTTONUP == (MouseHook.MouseMessages)wParam)
                MouseHookRightButton = false;
            if (MouseHook.MouseMessages.WM_LBUTTONDOWN == (MouseHook.MouseMessages)wParam)
                MouseHookLeftButton = true;
            if (MouseHook.MouseMessages.WM_LBUTTONUP == (MouseHook.MouseMessages)wParam)
                MouseHookLeftButton = false;
            if (MouseHook.MouseMessages.WM_MBUTTONDOWN == (MouseHook.MouseMessages)wParam)
                MouseHookMiddleButton = true;
            if (MouseHook.MouseMessages.WM_MBUTTONUP == (MouseHook.MouseMessages)wParam)
                MouseHookMiddleButton = false;
            if (MouseHook.MouseMessages.WM_XBUTTONDOWN == (MouseHook.MouseMessages)wParam)
                MouseHookXButton = true;
            if (MouseHook.MouseMessages.WM_XBUTTONUP == (MouseHook.MouseMessages)wParam)
                MouseHookXButton = false;
            if (MouseHook.MouseMessages.WM_LBUTTONDBLCLK == (MouseHook.MouseMessages)wParam)
                MouseHookLeftDoubleClick = true;
            else
                MouseHookLeftDoubleClick = false;
            if (MouseHook.MouseMessages.WM_RBUTTONDBLCLK == (MouseHook.MouseMessages)wParam)
                MouseHookRightDoubleClick = true;
            else
                MouseHookRightDoubleClick = false;
            if (MouseHook.MouseMessages.WM_MOUSEWHEEL == (MouseHook.MouseMessages)wParam)
                MouseHookWheel = (int)mouseStruct.mouseData;
            else
                MouseHookWheel = 0;
            if (MouseHook.MouseMessages.WM_XBUTTONDOWN == (MouseHook.MouseMessages)wParam)
                MouseHookButtonX = (int)mouseStruct.mouseData;
            else
                MouseHookButtonX = 0;
            if (Form1.Getstate)
            {
                GetCursorPos(out MouseDesktopHookX, out MouseDesktopHookY);
                mousehookx = (mouseStruct.pt.x - MouseDesktopHookX) * 15;
                mousehooky = (mouseStruct.pt.y - MouseDesktopHookY) * 30;
            }
            else
            {
                mousehookx = mouseStruct.pt.x;
                mousehooky = mouseStruct.pt.y;
            }
            Form1.mousehookx = mousehookx;
            Form1.mousehooky = mousehooky;
            MouseHookTime = (int)mouseStruct.time;
            Form1.MouseHookTime = MouseHookTime;
            Form1.MouseHookRightButton = MouseHookRightButton;
            Form1.MouseHookLeftButton = MouseHookLeftButton;
            Form1.MouseHookMiddleButton = MouseHookMiddleButton;
            Form1.MouseHookXButton = MouseHookXButton;
            Form1.MouseHookLeftDoubleClick = MouseHookLeftDoubleClick;
            Form1.MouseHookRightDoubleClick = MouseHookRightDoubleClick;
            Form1.MouseHookWheel = MouseHookWheel;
            Form1.MouseHookButtonX = MouseHookButtonX;
            Hook((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
        public const int WH_MOUSE_LL = 14;
        public enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, MouseHookHandler lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(out int x, out int y);
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int X, int Y);
    }
    public class KeyboardHook
    {
        public static bool KeyboardHookButtonDown, KeyboardHookButtonUp;
        public delegate IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
        public KeyboardHookHandler hookHandler;
        public KBDLLHOOKSTRUCT keyboardStruct;
        public delegate void KeyboardHookCallback(KBDLLHOOKSTRUCT keyboardStruct);
        public event KeyboardHookCallback Hook;
        public IntPtr hookID = IntPtr.Zero;
        public static int scanCode, vkCode;
        public void Install()
        {
            hookHandler = HookFunc;
            hookID = SetHook(hookHandler);
        }
        public void Uninstall()
        {
            if (hookID == IntPtr.Zero)
                return;
            UnhookWindowsHookEx(hookID);
            hookID = IntPtr.Zero;
        }
        ~KeyboardHook()
        {
            Uninstall();
        }
        public IntPtr SetHook(KeyboardHookHandler proc)
        {
            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(module.ModuleName), 0);
        }
        public IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            keyboardStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            if (KeyboardHook.KeyboardMessages.WM_KEYDOWN == (KeyboardHook.KeyboardMessages)wParam)
                KeyboardHookButtonDown = true;
            else
                KeyboardHookButtonDown = false;
            if (KeyboardHook.KeyboardMessages.WM_KEYUP == (KeyboardHook.KeyboardMessages)wParam)
                KeyboardHookButtonUp = true;
            else
                KeyboardHookButtonUp = false;
            Form1.KeyboardHookButtonDown = KeyboardHookButtonDown;
            Form1.KeyboardHookButtonUp = KeyboardHookButtonUp;
            Form1.vkCode = (int)keyboardStruct.vkCode;
            Form1.scanCode = (int)keyboardStruct.scanCode;
            Hook((KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT)));
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        public const int WH_KEYBOARD_LL = 13;
        public enum KeyboardMessages
        {
            WM_ACTIVATE = 0x0006,
            WM_APPCOMMAND = 0x0319,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x010,
            WM_HOTKEY = 0x0312,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_KILLFOCUS = 0x0008,
            WM_SETFOCUS = 0x0007,
            WM_SYSDEADCHAR = 0x0107,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_UNICHAR = 0x0109
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookHandler lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}