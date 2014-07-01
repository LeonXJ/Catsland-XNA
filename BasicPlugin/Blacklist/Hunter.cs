using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class Hunter : SpotLight {

#region Properties

#endregion

        public Hunter(GameObject _gameObject)
            : base(_gameObject) {

        }

        public Hunter()
            : base() {
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

            bool alarm = false;
            if (Mgr<Blacklist>.Singleton != null) {
                foreach (Prey prey in Mgr<Blacklist>.Singleton.Preys) {
                    if (IsPointInLight(prey.GetPointInWorld())) {
                        m_debugShape.DiffuseColor = Color.Red;
                        DiffuseColor = Color.Red;
                        alarm = true;
                    }
                }
            }
            if (!alarm) {
                m_debugShape.DiffuseColor = Color.Green;
                DiffuseColor = Color.Green;
            }
        }

        public static new string GetMenuNames() {
            return "Shadow|Hunter";
        }

    }
}
