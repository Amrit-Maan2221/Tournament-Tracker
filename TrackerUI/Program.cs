using System;
using System.Windows.Forms;

namespace TrackerUI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Add Connections
            TrackerLibrary.GlobalConfig.InitializeConnection(TrackerLibrary.DatabaseType.TextFile);

            //Application.Run(new CreateTournamentForm());
            Application.Run(new TournamentDashbordForm());
        }
    }
}
