using System.Windows.Forms;

namespace NeoNexus.WinDemo.Views
{
    public class PkgDetailView : UserControl
    {
        private TextBox txt;

        public PkgDetailView()
        {
            txt = new TextBox { Multiline = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
            Controls.Add(txt);
        }

        public void ShowDetails(string details)
        {
            txt.Text = details;
        }
    }
}
