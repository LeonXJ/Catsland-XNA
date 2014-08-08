using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Plugin.BasicPlugin {
    public class TestEchoCommand : IConsoleCommand{

        private string m_str;
        
        public string GetCommandName() {
            return "echo";
        }

        public void ParseStringParameter(String _parameters) {
            m_str = _parameters;
        }

        public object Execute() {
            Console.Out.WriteLine(m_str);
            return "Echo says: " + m_str;
        }
    }
}
