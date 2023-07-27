using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonoGame.Forms.DX
{
    public partial class Form1 : Form
    {
        private static int width = Screen.PrimaryScreen.Bounds.Width;
        private static int height = Screen.PrimaryScreen.Bounds.Height;
        public Form1()
        {
            InitializeComponent();
            base.FormBorderStyle = FormBorderStyle.None;
            base.TopMost = true;
            base.Width = width;
            base.Height = height;
        }
        public static void Minimize()
        {
            Form1 form = (Form1)Application.OpenForms["Form1"];
            form.WindowState = FormWindowState.Minimized;
        }
    }
}
