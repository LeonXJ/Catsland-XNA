using System;

/**
 * @file Program
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
#if WINDOWS || XBOX
    /**
     * @brief here is the entrance of the main game
     * 
     * [STAThread] should be added for editor
     * */

    static class Program {
        [STAThread]
        static void Main(string[] args) {
            using (GameEngine game = new GameEngine()) {
                game.Run();
            }
        }
    }
#endif
}

