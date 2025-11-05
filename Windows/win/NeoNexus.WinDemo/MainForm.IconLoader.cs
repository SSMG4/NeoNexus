using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NeoNexus.WinDemo
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        private void TryLoadProjectPngIcon()
        {
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var iconPath = Path.Combine(baseDir, "Assets", "icons", "icon0.png");
                if (!File.Exists(iconPath)) return;

                using var bmp = new Bitmap(iconPath);
                IntPtr hIcon = bmp.GetHicon();
                try
                {
                    Icon ico = Icon.FromHandle(hIcon);
                    this.Icon = (Icon)ico.Clone(); // clone so original handle can be destroyed
                }
                finally
                {
                    DestroyIcon(hIcon);
                }
            }
            catch
            {
                // best-effort, don't crash the app if icon load fails
            }
        }

        public MainForm()
        {
            InitializeComponent();
            TryLoadProjectPngIcon();
        }
    }
}
