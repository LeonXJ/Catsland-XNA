using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Core {
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
        public Vector2[] GetVertices() {
            return m_vertices.ToArray();
        }

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
            scene.m_shadowSystem.AddShadowBody(this);
        }

        public override void Destroy() {
            base.Destroy();
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.Destroy(Mgr<Scene>.Singleton);
            }
            // TODO: remove from shadow system

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

        public Vector2 GetVertexInWorld(int _index) {
            Vector4 pos = Vector4.Transform(new Vector4(m_vertices[_index].X, m_vertices[_index].Y, 0.0f, 1.0f),
               Matrix.CreateTranslation(m_offset.X, m_offset.Y, 0.0f) * m_gameObject.AbsTransform);
            return new Vector2(pos.X, pos.Y);
        }




    }
}
