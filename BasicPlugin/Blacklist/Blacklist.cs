using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Plugin.BasicPlugin {
    public class Blacklist : CatComponent{

#region Properties

        private HashSet<Prey> m_preys = new HashSet<Prey>();
        public HashSet<Prey> Preys {
            get {
                return m_preys;
            }
        }


#endregion

        public Blacklist(GameObject _gameObject)
            : base(_gameObject) {
            AddToMgr();
        }

        public Blacklist()
            : base() {
            AddToMgr();
        }

        private void AddToMgr() {
            Mgr<Blacklist>.Singleton = this;
        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            
        }

        public void AddToBlacklist(Prey _prey) {
            m_preys.Add(_prey);
        }

        public static new string GetMenuNames() {
            return "Shadow|Blacklist";
        }
    }
}
