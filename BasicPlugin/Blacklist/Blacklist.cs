using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Plugin.BasicPlugin {
    public class Blacklist : CatComponent{

        /**
         * @brief a scene-wide blacklist, hunters may search for preys in added to
         *      this blacklist.
         */

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
        }

        public Blacklist()
            : base() {
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            scene.AddSharedObject(typeof(Blacklist).ToString(), this);
        }

        public override void Destroy() {
            base.Destroy();
            Mgr<Scene>.Singleton.RemoveSharedObject(typeof(Blacklist).ToString());
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
