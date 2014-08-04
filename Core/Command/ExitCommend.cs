using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class ExitCommend : IConsoleCommand {
        public object Execute() {
            Mgr<GameEngine>.Singleton.Exit();
            return "Game engine exit.";
        }

        public string GetCommandName() {
            return "exit";
        }

        public void ParseStringParameter(String _parameters) {
        }
    }
}
