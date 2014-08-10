using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class DebugShape : Drawable{

#region Properties

        static private int CircleSegment = 16;

        [SerialAttribute]
        private readonly CatVector3 m_position = new CatVector3();
        public Vector3 Position {
            set {
                m_position.SetValue(value);
            }
            get {
                return m_position.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatQuaternion m_rotation = new CatQuaternion();
        public Vector3 Rotation {
            set {
                m_rotation.SetValue(Quaternion.CreateFromYawPitchRoll(
                            MathHelper.ToRadians(value.Y),
                            MathHelper.ToRadians(value.X),
                            MathHelper.ToRadians(value.Z)
                            ));
            }
            get {
                return CatQuaternion.QuaternionToEulerDegreeVector3(m_rotation);
            }
        }

        [SerialAttribute]
        private readonly CatVector3 m_scale = new CatVector3(Vector3.One);
        public Vector3 Scale {
            set {
                m_scale.SetValue(value);
            }
            get {
                return m_scale.GetValue();
            }
        }

        [SerialAttribute(SerialAttribute.AttributePolicy.PolicyReference,
            SerialAttribute.AttributePolicy.PolicyReference)]
        private GameObject m_relateGameObject = null;
        public GameObject RelateGameObject {
            set {
                m_relateGameObject = value;
            }
            get {
                return m_relateGameObject;
            }
        }

        [SerialAttribute]
        private CatColor m_diffuseColor = new CatColor(1.0f, 1.0f, 1.0f, 1.0f);
        public Color DiffuseColor {
            set {
                m_diffuseColor.SetValue(value);
            }
            get {
                return m_diffuseColor;
            }
        }

        private List<Vector2> m_verticeList;
        private VertexPositionColor[] m_vertices;
        private VertexBuffer m_vertexBuffer;

#endregion

        public void Initialize(Scene _scene) {

        }

        public void BindToScene(Scene _scene) {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                if (_scene != null) {
                    _scene._debugDrawableList.AddItem(this);
                }
            }
        }

        public void Destroy(Scene _scene) {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                // add to debug drawable
                _scene._debugDrawableList.RemoveItem(this);
            }
        }

        public void SetVertices(List<Vector2> _vertices) {
            m_verticeList = _vertices;
            UpdateVertex();
        }

        public void SetAsRectangle(Vector2 _size, Vector2 _offset) {
            float half_width = _size.X / 2.0f;
            float half_height = _size.Y / 2.0f;
            List<Vector2> vertex = new List<Vector2>();
            vertex.Add(new Vector2(_offset.X - half_width, _offset.Y + half_height));
            vertex.Add(new Vector2(_offset.X + half_width, _offset.Y + half_height));
            vertex.Add(new Vector2(_offset.X + half_width, _offset.Y - half_height));
            vertex.Add(new Vector2(_offset.X - half_width, _offset.Y - half_height));
            SetVertices(vertex);
        }

        public void SetAsCircle(float _radius, Vector2 _offset) {
            List<Vector2> vertex = new List<Vector2>();
            for (int segment = 0; segment < CircleSegment; ++segment) {
                vertex.Add(new Vector2((float)Math.Cos(2 * segment * MathHelper.Pi / CircleSegment),
                                        (float)Math.Sin(2 * segment * MathHelper.Pi / CircleSegment)) + _offset);
            }
            SetVertices(vertex);
        }

        protected void UpdateVertex() {
            if (m_verticeList.Count > 0) {
                m_vertices = new VertexPositionColor[m_verticeList.Count + 1];
            }
            else {
                m_vertices = null;
            }
            for (int i = 0; i < m_verticeList.Count; ++i) {
                m_vertices[i] = new VertexPositionColor(
                    new Vector3(m_verticeList[i].X, m_verticeList[i].Y, 0.0f),
                    new Color(1.0f, 1.0f, 1.0f, 1.0f));
            }
            if (m_verticeList.Count > 0) {
                m_vertices[m_verticeList.Count] = m_vertices[0];
            }
            m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton, 
                typeof(VertexPositionColor), m_verticeList.Count + 1,
                        BufferUsage.None);
            m_vertexBuffer.SetData<VertexPositionColor>(m_vertices);
        }

        public void Draw(int timeLastFrame) {
            Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
            Effect effect = Mgr<DebugTools>.Singleton.DrawEffect;
            ((BasicEffect)effect).Alpha = 1.0f;
            ((BasicEffect)effect).DiffuseColor = m_diffuseColor.RGB;
            ((BasicEffect)effect).View = Mgr<Camera>.Singleton.View;
            ((BasicEffect)effect).Projection = Mgr<Camera>.Singleton.m_projection;
            ((BasicEffect)effect).VertexColorEnabled = false;

            if (m_relateGameObject == null) {
                ((BasicEffect)effect).World = Matrix.CreateScale(m_scale) *
                    Matrix.CreateFromQuaternion(m_rotation) *
                    Matrix.CreateTranslation(m_position);
            }
            else {
                ((BasicEffect)effect).World = Matrix.CreateScale(m_scale) *
                    Matrix.CreateFromQuaternion(m_rotation) *
                    Matrix.CreateTranslation(m_position) * 
                    m_relateGameObject.AbsTransform;
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.LineStrip,
                    m_vertices,
                    0,
                    m_verticeList.Count);
            }
            
        }

        public float GetDepth() {
            return 0;
        }

        public int CompareTo(object obj) {
            return 1;
        }
  
    }
}
