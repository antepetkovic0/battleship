using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka.Actor;

namespace Game
{
    static class GlobalContext
    {
        public static MainMenuForm MainMenuForm { get; set; }
        public static GameForm GameForm { get; set; }

        public static string Name { get; set; }
    }

    static class Program
    {
        public static ActorSystem System { get; set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainMenuForm mainMenuForm = new MainMenuForm();
            GlobalContext.MainMenuForm = mainMenuForm;

            Application.Run(mainMenuForm);
        }
    }
}
