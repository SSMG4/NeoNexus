using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeoNexus.WinDemo.Services;

namespace NeoNexus.WinDemo
{
    public class MainForm : Form
    {
        private Button btnVersion;
        private Button btnList;
        private Button btnVitaDb;
        private Button btnCbps;
        private TextBox txtOutput;

        public MainForm()
        {
            Text = "NeoNexus - Demo";
            Width = 900;
            Height = 600;

            btnVersion = new Button { Text = "Get Native Version", Left = 10, Top = 10, Width = 160 };
            btnList = new Button { Text = "List PKG Files (demo)", Left = 180, Top = 10, Width = 160 };
            btnVitaDb = new Button { Text = "Fetch VitaDB Homebrews", Left = 350, Top = 10, Width = 200 };
            btnCbps = new Button { Text = "Fetch CBPS CSV", Left = 560, Top = 10, Width = 160 };

            txtOutput = new TextBox { Left = 10, Top = 50, Width = 860, Height = 500, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true };

            btnVersion.Click += (s, e) =>
            {
                var v = NativeMethods.GetVersion();
                txtOutput.Text = $"Native version: {v}";
            };

            btnList.Click += (s, e) =>
            {
                var json = NativeMethods.ListPkgFiles("C:\\path\\to\\dummy.pkg");
                txtOutput.Text = $"PKG files JSON: {json}";
            };

            btnVitaDb.Click += async (s, e) => await OnFetchVitaDb();
            btnCbps.Click += async (s, e) => await OnFetchCbps();

            Controls.Add(btnVersion);
            Controls.Add(btnList);
            Controls.Add(btnVitaDb);
            Controls.Add(btnCbps);
            Controls.Add(txtOutput);
        }

        private async Task OnFetchVitaDb()
        {
            try
            {
                txtOutput.Text = "Fetching VitaDB homebrews...";
                var arr = await VitaDbService.FetchListAsync(VitaDbService.HomebrewsUrl);
                txtOutput.Text = $"Got {arr.Length} entries.\r\n\r\nSample titles:\r\n";
                foreach (var item in arr.Take(10))
                {
                    txtOutput.AppendText("- " + VitaDbService.GetTitle(item) + "\r\n");
                }
            }
            catch (Exception ex)
            {
                txtOutput.Text = "Error fetching VitaDB: " + ex.Message;
            }
        }

        private async Task OnFetchCbps()
        {
            try
            {
                txtOutput.Text = "Downloading CBPS CSV (raw)...";
                var url = "https://raw.githubusercontent.com/Team-CBPS/cbps-db/main/cbpsdb.csv";
                var list = await CbpsService.FetchAndParseCsvAsync(url);
                txtOutput.Text = $"Parsed {list.Count} CBPS entries.\r\n\r\nSample entries:\r\n";
                foreach (var e in list.Take(12))
                {
                    txtOutput.AppendText("- " + e.ToString() + "\r\n");
                }
            }
            catch (Exception ex)
            {
                txtOutput.Text = "Error fetching/parsing CBPS CSV: " + ex.Message;
            }
        }
    }
}
