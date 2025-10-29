using System.Windows.Forms;

namespace NeoNexus.WinDemo.Views
{
    public class HomebrewListView : UserControl
    {
        private ListView list;

        public HomebrewListView()
        {
            list = new ListView { Dock = DockStyle.Fill, View = View.Details };
            list.Columns.Add("Title", 300);
            list.Columns.Add("Type", 100);
            Controls.Add(list);
        }

        public void SetItems(System.Collections.Generic.IEnumerable<string> items)
        {
            list.Items.Clear();
            foreach (var it in items)
            {
                list.Items.Add(new ListViewItem(new[] { it, "" }));
            }
        }
    }
}
