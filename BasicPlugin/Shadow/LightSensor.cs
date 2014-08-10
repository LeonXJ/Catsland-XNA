using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class LightSensor : CatComponent{

        /**
         * @brief a test component for testing lighting in editor
         */

#region Properties

        private DebugShape m_debugShape;


#endregion

        public LightSensor(GameObject _gameObject)
            :base(_gameObject){
        }

        public LightSensor() : base() { }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {            
                m_debugShape.SetAsCircle(0.2f, Vector2.Zero);
                m_debugShape.RelateGameObject = m_gameObject;
            }
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape = new DebugShape();
                m_debugShape.BindToScene(scene);
            }
        }

        public override void Destroy() {
            base.Destroy();
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.Destroy(Mgr<Scene>.Singleton);
            }
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
        }

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);

            if (Mgr<Scene>.Singleton.m_shadowSystem.IsPointEnlighted(
                new Vector2(m_gameObject.AbsPosition.X, m_gameObject.AbsPosition.Y))) {
                m_debugShape.DiffuseColor = Color.Red;
            }
            else {
                m_debugShape.DiffuseColor = Color.Green;
            }
        
        }

        public static string GetMenuNames() {
            return "Shadow|LightSensor";
        }

    }
}
