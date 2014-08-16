using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Common;
using Catsland.Plugin.BasicPlugin;

namespace HorseRiding {
    public class StaticSensor : CatComponent, Drawable {

#region Properties

        [SerialAttribute]
        protected readonly CatVector2 m_offset = new CatVector2();
        public Vector2 Offset {
            set {
                m_offset.SetValue(value);
                UpdateSensor();
                UpdateDebugVertex();
            }
            get {
                return m_offset.GetValue();
            }
        }

        [SerialAttribute]
        protected readonly CatVector2 m_size = new CatVector2(0.1f, 0.1f);
        public Vector2 Size {
            set {
                float width = MathHelper.Max(value.X, 0.0f);
                float height = MathHelper.Max(value.Y, 0.0f);
                m_size.SetValue(new Vector2(width, height));
                UpdateSensor();
                UpdateDebugVertex();
            }
            get {
                return m_size.GetValue();
            }
        }

        protected int m_touchCount = 0;
        public bool IsTriggered {
            get {
                return m_touchCount > 0;
            }
        }

        private Body m_body;

        private VertexPositionColor[] m_vertex;
        private VertexBuffer m_vertexBuffer;

#endregion

        public StaticSensor() : base() {
        }

        public StaticSensor(GameObject _gameObject):
            base(_gameObject) {

        }

        public override void Initialize(Catsland.Core.Scene scene) {
            base.Initialize(scene);
            UpdateSensor();
            UpdateDebugVertex();
            MoveBodyToGameObject();
        }

        public override void BindToScene(Scene scene) {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                // add to debug drawable
                if (scene != null) {
                    scene._debugDrawableList.AddItem(this);
                }
            }
        }

        public override void UnbindFromScene(Scene _scene) {
            base.UnbindFromScene(_scene);
            DeleteBody();
        }

        protected void DeleteBody() {
            PhysicsSystem physicsSystem = Mgr<Scene>.Singleton.GetPhysicsSystem();
            if (m_body != null) {
                physicsSystem.GetWorld().RemoveBody(m_body);
                m_body = null;
            }
        }

        public void UpdateSensor() {
            PhysicsSystem physicsSystem = Mgr<Scene>.Singleton.GetPhysicsSystem();
            DeleteBody();
            m_body = BodyFactory.CreateRectangle(physicsSystem.GetWorld(),
                                    m_size.X, m_size.Y, 1.0f);
            m_body.BodyType = BodyType.Static;
            m_body.IsSensor = true;
            m_body.OnCollision += OnCollision;
            m_body.OnSeparation += OnSeparation;
        }

        public void UpdateDebugVertex() {
            if (m_vertex == null) {
                m_vertex = new VertexPositionColor[5];
                for (int i = 0; i < 5; ++i) {
                    m_vertex[i] = new VertexPositionColor();
                }
            }
            float halfWidth = m_size.X / 2.0f;
            float halfHeight = m_size.Y / 2.0f;
            m_vertex[0].Position = new Vector3(halfWidth, halfHeight, 0.0f);
            m_vertex[1].Position = new Vector3(halfWidth, -halfHeight, 0.0f);
            m_vertex[2].Position = new Vector3(-halfWidth, -halfHeight, 0.0f);
            m_vertex[3].Position = new Vector3(-halfWidth, halfHeight, 0.0f);
            m_vertex[4].Position = new Vector3(halfWidth, halfHeight, 0.0f);
            if (m_vertexBuffer == null) {
                m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                                    typeof(VertexPositionColor),
                                    5,
                                    BufferUsage.None);
            }
            m_vertexBuffer.SetData<VertexPositionColor>(m_vertex);
        }

        protected bool OnCollision(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {
            // TODO: add mask
            ++m_touchCount;
            return Enter(_fixtureA, _fixtureB, _contact);
        }

        protected void OnSeparation(Fixture _fixtureA, Fixture _fixtureB) {
            --m_touchCount;
            Exit(_fixtureA, _fixtureB);
        }

        virtual protected bool Enter(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {
            return true;
        }
        virtual protected void Exit(Fixture _fixtureA, Fixture _fixtureB) { }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            MoveBodyToGameObject();
        }

        private void MoveBodyToGameObject() {
            m_gameObject.ForceUpdateAbsTransformation();
            Vector3 absRotate = CatMath.MatrixToEulerAngleVector3(m_gameObject.AbsTransform);
            if (m_body != null) {
                m_body.SetTransform(new Vector2(m_gameObject.AbsPosition.X,
                                                                 m_gameObject.AbsPosition.Y),
                                                                                absRotate.Z);
            }
            
        }

        public void Draw(int timeLastFrame) {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                BasicEffect effect = Mgr<DebugTools>.Singleton.DrawEffect;
                effect.View = Mgr<Camera>.Singleton.View;
                effect.Projection = Mgr<Camera>.Singleton.m_projection;
                effect.VertexColorEnabled = false;
                if (!m_enable) {
                    effect.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
                }
                else {
                    if (IsTriggered) {
                        effect.DiffuseColor = new Vector3(0.9f, 0.0f, 0.0f);
                    }
                    else {
                        effect.DiffuseColor = new Vector3(0.0f, 0.9f, 0.0f);
                    }
                }
                effect.Alpha = 1.0f;

                if (m_body == null) {
                    return;
                }

                Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                    Transform transform;
                    m_body.GetTransform(out transform);
                    Vector2 position = transform.p + m_offset;
                    Matrix matPosition = Matrix.CreateTranslation(new Vector3(
                                                                   position.X,
                                                                   position.Y,
                                                                   0));
                    effect.World = matPosition;
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineStrip,
                        m_vertex,
                        0,
                        4);
                }
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
