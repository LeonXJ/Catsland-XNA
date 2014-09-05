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

        [SerialAttribute]
        private CatFloat m_senseDistance = new CatFloat(0.2f);
        public float SenseDistance{
            set {
                m_senseDistance.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_senseDistance;
            }
        }

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
            if (!m_enable) {
                return;
            }

            m_spotPrey = null;
            Blacklist blacklist = Mgr<Scene>.Singleton.GetSharedObject(typeof(Blacklist).ToString()) 
                as Blacklist;
            ShadowSystem shadowSystem = GameObject.Scene.m_shadowSystem;
            if (blacklist != null) {
                foreach (Prey prey in blacklist.Preys) {
                    Vector2 preyPosition = prey.GetPointInWorld();
                    if (IsPointInOnLight(preyPosition)) {
                        Vector3 myPosition = GameObject.AbsPosition;
                        Vector2 dist = preyPosition - new Vector2(myPosition.X, myPosition.Y);
                        if (dist.LengthSquared() > m_senseDistance * m_senseDistance
                            && shadowSystem != null && !shadowSystem.IsPointEnlighted(preyPosition)){
                            continue;
                        }
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
