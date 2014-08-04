using System;
using Catsland.Core;
using System.Windows.Forms;

namespace Catsland.Editor
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        [STAThread]
        static void Main(string[] args)
        {
            Entrance.Run(args);
        }
    }

    public class Entrance{
        static public void Run(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool inTest = false;
            if (args.Length > 0 && args[0] == "test") {
                inTest = true;
            }

            MapEditor mapEditor = new MapEditor(inTest);
            mapEditor.Show();
            using (GameEngine game = new GameEngine(mapEditor)) {
                mapEditor.SetGameEngine(game);
                game.Run();
            }  
        }
    }
#endif
}

