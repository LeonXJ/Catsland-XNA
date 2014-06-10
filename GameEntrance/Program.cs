using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Catsland.Core;

namespace GameEntrance {
    static class Program {
        static void Main() {
            GameEngine game = new GameEngine();
            game.Run();
        }
    }
}
