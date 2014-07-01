using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class Prey : CatComponent{


        public Prey(GameObject _gameObject)
            : base(_gameObject) {

        }

        public Prey()
            : base() {

        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            if (Mgr<Blacklist>.Singleton != null) {
                Mgr<Blacklist>.Singleton.AddToBlacklist(this);
            }
        }

        public Vector2 GetPointInWorld() {
            Vector3 absPosition = m_gameObject.AbsPosition;
            return new Vector2(absPosition.X, absPosition.Y);
        }

        public static new string GetMenuNames() {
            return "Shadow|Prey";
        }
    }
}
