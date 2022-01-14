using System;
using System.Threading;
using System.Windows.Forms;

namespace WinPods.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Mutex.TryOpenExisting("0409B046-DF3B-4430-9817-B1197617607D", out _))
            {
                _ = new Mutex(false, "0409B046-DF3B-4430-9817-B1197617607D");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainService());
            }
        }
    }
}
