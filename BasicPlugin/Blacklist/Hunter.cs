using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class Hunter : SpotLight {

        /**
         * @brief Hunter searches for preys in the blacklist.
         */ 

#region Properties

        private Prey m_spotPrey = null;
        public Prey LastSpot {
            get {
                return m_spotPrey;
            }
        }

#endregion

        public Hunter(GameObject _gameObject)
            : base(_gameObject) {

        }

        public Hunter()
            : base() {
        }

        public bool SpotAny() {
            return m_spotPrey != null;
        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            Enlight = false;
            RenderLight = true;
            DiffuseColor = Color.Green;
        }

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);

            Update(timeLastFrame);
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            m_spotPrey = null;
            Blacklist blacklist = Mgr<Scene>.Singleton.GetSharedObject(typeof(Blacklist).ToString()) 
                as Blacklist;
            if (blacklist != null) {
                foreach (Prey prey in blacklist.Preys) {
                    if (IsPointInOnLight(prey.GetPointInWorld())) {
                        m_debugShape.DiffuseColor = Color.Red;
                        DiffuseColor = Color.Red;
                        m_spotPrey = prey;
                    }
                }
            }
            if (!SpotAny()) {
                m_debugShape.DiffuseColor = Color.Green;
                DiffuseColor = Color.Green;
            }
        }

        public static new string GetMenuNames() {
            return "Shadow|Hunter";
        }

    }
}
