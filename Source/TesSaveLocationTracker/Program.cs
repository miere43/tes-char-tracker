using System;
using System.Windows.Forms;

namespace TesSaveLocationTracker
{
    class Program
    {
        [STAThread]
        static int Main(string[] unused)
        {
            Application.EnableVisualStyles();
            Application.Run(new App.MainForm());

            return 0;
        }
    }
}
