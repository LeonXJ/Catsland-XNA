using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class Prey : CatComponent{

        /**
         * @brief prey is added to blacklist by itself. Hunters may search for
         *      preys
         */

        public Prey(GameObject _gameObject)
            : base(_gameObject) {

        }

        public Prey()
            : base() {

        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            // prey needs to be added into Blacklist which is added to scene's
            //  sharedObjectList in BindToScene phase, thus we bind and init prey in
            //  initialize
        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            Blacklist blacklist = scene.GetSharedObject(typeof(Blacklist).ToString())
                                    as Blacklist;
            if (blacklist != null) {
                blacklist.AddToBlacklist(this);
            }
        }

        public override void UnbindFromScene(Scene _scene) {
            base.UnbindFromScene(_scene);
            Blacklist blacklist = _scene.GetSharedObject(typeof(Blacklist).ToString())
                                    as Blacklist;
            if (blacklist != null) {
                blacklist.RemoveFromBlacklist(this);
            }
        }

        public Vector2 GetPointInWorld() {
            Vector3 absPosition = m_gameObject.AbsPosition;
            return new Vector2(absPosition.X, absPosition.Y);
        }

        public static string GetMenuNames() {
            return "Shadow|Prey";
        }
    }
}
