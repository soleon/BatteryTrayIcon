using System;

namespace Percentage.Ui.Net
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            using (new Ui.Uwp.App.App())
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}