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
            Application.EnableVisualStyles();
           
            Application.SetCompatibleTextRenderingDefault(false);

            MapEditor mapEditor = new MapEditor();
            mapEditor.Show();
            using (GameEngine game = new GameEngine(mapEditor))
            {
                mapEditor.SetGameEngine(game);	
                game.Run();
            }  
        }
    }
#endif
}

