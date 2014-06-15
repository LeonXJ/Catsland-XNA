using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class ShadingBody : CatComponent{
    
#region Properties

        protected DebugShape m_debugShape = new DebugShape();

        private int id;
        public int ID {
            set {
                id = value;
            }
            get {
                return id;
            }
        }

        [SerialAttribute]
        private readonly CatVector2 m_offset = new CatVector2();
        public Vector2 Offset {
            set {
                m_offset.SetValue(value);
                m_debugShape.Position = new Vector3(m_offset.X, m_offset.Y, 0.0f);
            }
            get {
                return m_offset.GetValue();
            }
        }

        protected List<Vector2> m_vertices;

#endregion

        public ShadingBody(GameObject _gameObject)
            : base(_gameObject) {

        }

        public ShadingBody()
            : base() {

        }

        public override void Initialize(Catsland.Core.Scene scene) {
            base.Initialize(scene);
            m_debugShape.RelateGameObject = m_gameObject;
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.BindToScene(scene);
            }
        }

        public override void Destroy() {
            base.Destroy();
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.Destroy(Mgr<Scene>.Singleton);
            }
        }

        public static string GetMenuNames() {
            return "Shadow|ShadingBase";
        }

        public int GetVerticesNumber() {
            return m_vertices.Count;
        }

        public Vector2 GetVertex(int _index) {
            return m_vertices[_index];
        }




    }
}
