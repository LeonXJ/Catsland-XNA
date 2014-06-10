using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;

namespace Catsland.Plugin.BasicPlugin {
    public class SensorAttachment : Drawable {
    
#region Properties

        [SerialAttribute]
        private readonly CatVector2 m_offset = new CatVector2();
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
        private readonly CatVector2 m_size = new CatVector2(0.1f, 0.1f);
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

        [SerialAttribute]
        private bool m_enable = true;
        
        private int m_touchCount = 0;
        public bool IsTriggered {
            get {
                return m_touchCount > 0;
            }
        }

        private Body m_body;
        private Fixture m_fixture;

        private VertexPositionColor []m_vertex;
        private VertexBuffer m_vertexBuffer;

#endregion

        public SensorAttachment(Body _body) {
            m_body = _body;
            UpdateSensor();
            UpdateDebugVertex();
        }

        public void BindToScene(Scene scene) {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                // add to debug drawable
                if (scene != null) {
                    scene._debugDrawableList.AddItem(this);
                }
            }
        }

        ~SensorAttachment() {
            if (m_fixture != null) {
                PhysicsSystem physicsSystem = Mgr<Scene>.Singleton.GetPhysicsSystem();
                m_body.DestroyFixture(m_fixture);
            }
        }

        public void UpdateSensor() {
            if (m_fixture != null) {
                PhysicsSystem physicsSystem = Mgr<Scene>.Singleton.GetPhysicsSystem();
                m_body.DestroyFixture(m_fixture);
            }
            m_fixture = FixtureFactory.AttachRectangle(m_size.X, m_size.Y, 0.1f,
                m_offset, m_body);
            m_fixture.IsSensor = true;
            m_fixture.OnCollision += OnCollision;
            m_fixture.OnSeparation += OnSeparation;
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
            m_vertex[0].Position = new Vector3( halfWidth,  halfHeight, 0.0f);
            m_vertex[1].Position = new Vector3( halfWidth, -halfHeight, 0.0f);
            m_vertex[2].Position = new Vector3(-halfWidth, -halfHeight, 0.0f);
            m_vertex[3].Position = new Vector3(-halfWidth,  halfHeight, 0.0f);
            m_vertex[4].Position = new Vector3( halfWidth,  halfHeight, 0.0f);
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

        virtual protected bool Enter(Fixture _fixtureA, Fixture _fixtureB, Contact _contact){
            return true;
        }
        virtual protected void Exit(Fixture _fixtureA, Fixture _fixtureB){}

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
