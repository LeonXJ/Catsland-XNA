using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Catsland.Core;

namespace GameEntrance {
    static class Program {
        static void Main(string[] args) {
            bool enableConsole = true;
            bool passiveMode = false;
            foreach (string arg in args) {
                if (arg == "passive") {
                    passiveMode = true;
                }
                else if (arg == "noconsole") {
                    enableConsole = false;
                }
            }
            GameEngine game = new GameEngine(null, enableConsole, passiveMode);
            game.Run();
        }
    }
}
