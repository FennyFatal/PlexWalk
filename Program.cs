using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PlexWalk
{
    static class Program
    {
        static string[] startArgs;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            startArgs = args;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BrowseForm(args));
        }
    }
}
