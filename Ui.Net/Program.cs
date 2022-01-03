using System;
using System.Drawing;
using System.Windows.Forms;
using Windows.UI.ViewManagement;
using Microsoft.Toolkit.Forms.UI.XamlHost;

namespace Percentage.Ui.Net
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            using (new Ui.Uwp.App.App())
            {
                var backgroundColor = new UISettings().GetColorValue(UIColorType.Background);
                    backgroundColor.B);
                Application.Run(new Form
                {
                    Controls =
                    {
                        new WindowsXamlHost
                        {
                            Dock = DockStyle.Fill,
                            Child = new Ui.Uwp.SettingsView()
                        }
                    },
                    BackColor = Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G,
                        backgroundColor.B)
                });
            }
        }
    }
}