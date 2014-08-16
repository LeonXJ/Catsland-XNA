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

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            m_debugShape.RelateGameObject = GameObject;
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.BindToScene(scene);
            }
            scene.m_shadowSystem.AddShadowBody(this);
        }

        public override void UnbindFromScene(Scene _scene) {
            base.UnbindFromScene(_scene);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.Destroy(_scene);
            }
            _scene.m_shadowSystem.RemoveShadingBody(this);

        }

        public static string GetMenuNames() {
            return "Shadow|ShadingBase";
        }

        public int GetVerticesNumber() {
            return m_vertices.Count;
        }

        /**
         * @brief get the vertex in local coordinate
         */ 
        public Vector2 GetVertex(int _index) {
            return m_vertices[_index];
        }

        /**
         * @brief get the vertex in world coordinate
         */ 
        public Vector2 GetVertexInWorld(int _index) {
            Vector4 pos = Vector4.Transform(new Vector4(m_vertices[_index].X, m_vertices[_index].Y, 0.0f, 1.0f),
               Matrix.CreateTranslation(m_offset.X, m_offset.Y, 0.0f) * GameObject.AbsTransform);
            return new Vector2(pos.X, pos.Y);
        }

        /**
         * @brief get the transform matrix from local to world
         */ 
        public Matrix GetTransform2World() {
            return Matrix.CreateTranslation(m_offset.X, m_offset.Y, 0.0f) * GameObject.AbsTransform;
        }
    }
}
