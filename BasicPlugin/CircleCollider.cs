using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Plugin.BasicPlugin {
    class CircleCollider : ColliderBase {

#region Properties

        [SerialAttribute]
        protected readonly CatFloat m_radius;
        public float Radius {
            set {
                m_radius.SetValue(value);
                if (m_body != null) {
                    CreateAndConfigBody();
                }
                UpdateDebugVertex();
            }
            get {
                return m_radius.GetValue();
            }
        }

        static int DebugCircleSegmentNum = 32;
#endregion

        public CircleCollider()
            : base() {
                m_radius = new CatFloat(0.2f);
        }

        public CircleCollider(GameObject _gameObject)
            : base(_gameObject) {
                m_radius = new CatFloat(0.2f);
        }

        protected override Body CreateBody(PhysicsSystem _physicsSystem) {
            return BodyFactory.CreateCircle(_physicsSystem.GetWorld(),
                                            m_radius,
                                            m_mass);
        }

        protected override void UpdateDebugVertex() {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                if (m_vertex == null) {
                    m_vertex = new VertexPositionColor[DebugCircleSegmentNum + 1];
                    m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton, typeof(VertexPositionColor),
                       DebugCircleSegmentNum+1, BufferUsage.None);
                }
                for (int segment = 0; segment < DebugCircleSegmentNum; ++segment) {
                    m_vertex[segment] = new VertexPositionColor(
                        Radius * new Vector3((float)Math.Cos(2 * segment * MathHelper.Pi / DebugCircleSegmentNum),
                                    (float)Math.Sin(2 * segment * MathHelper.Pi / DebugCircleSegmentNum),
                                    0.0f), 
                        Color.LimeGreen);
                }
                m_vertex[DebugCircleSegmentNum] = m_vertex[0];
                m_vertexBuffer.SetData<VertexPositionColor>(m_vertex);
            }
        }
    }
}
