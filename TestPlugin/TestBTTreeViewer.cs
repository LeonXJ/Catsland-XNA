using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Catsland.Editor;

namespace Catsland.Plugin.TestPlugin {
    public class TestBTTreeViewer : IConsoleCommand{
        private string m_btTreeName;

        public string GetCommandName() {
            return "TestBTTreeViewer";
        }

        public void ParseStringParameter(string _parameters) {
            m_btTreeName = _parameters;
        }

        public object Execute() {
            BTTree btTree = Mgr<CatProject>.Singleton.BTTreeManager.LoadBTTree(m_btTreeName);
            if (btTree != null) {
                Mgr<MapEditor>.Singleton.m_btTreeEditor.OpenBTTree(btTree);
            }
            return "BTTree loaded and opened in viewer";
        }
    }
}
