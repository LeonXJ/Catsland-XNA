﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;

namespace Catsland.Plugin.BasicPlugin {
    class RectangleCollider : ColliderBase {

#region Properties

        [SerialAttribute]
        protected readonly CatVector2 m_size;
        public Vector2 Size {
            set {
                m_size.SetValue(value);
                if (m_body != null) {
                    CreateAndConfigBody();
                }
                UpdateDebugVertex();
            }
            get {
                return m_size.GetValue();
            }
        }

        public bool IsPlatform {
            set {
                if (value) {
                    m_body.UserData = Tag.Platform;
                }
                else {
                    m_body.UserData = null;
                }
            }
            get {
                return m_body.UserData == Tag.Platform;
            }
        }

#endregion

        public RectangleCollider() 
        :base(){
            m_size = new CatVector2(0.5f, 0.5f);
        }

        public RectangleCollider(GameObject _gameObject)
        :base(_gameObject){
            m_size = new CatVector2(0.5f, 0.5f);
        }

        protected override Body CreateBody(PhysicsSystem _physicsSystem){
            return BodyFactory.CreateRectangle(_physicsSystem.GetWorld(),
                                               m_size.X,
                                               m_size.Y,
                                               m_mass, Tag.Platform);
        }

        protected override void UpdateDebugVertex() {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                float halfWidth = m_size.X / 2.0f;
                float halfHeight = m_size.Y / 2.0f;
                if (m_vertex == null) {
                    m_vertex = new VertexPositionColor[5];
                    m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton, typeof(VertexPositionColor),
                       5, BufferUsage.None);
                }
                m_vertex[0] = m_vertex[4] = new VertexPositionColor(new Vector3(-halfWidth, halfHeight, 0.0f), Color.LimeGreen);
                m_vertex[1] = new VertexPositionColor(new Vector3(halfWidth, halfHeight, 0.0f), Color.LimeGreen);
                m_vertex[2] = new VertexPositionColor(new Vector3(halfWidth, -halfHeight, 0.0f), Color.LimeGreen);
                m_vertex[3] = new VertexPositionColor(new Vector3(-halfWidth, -halfHeight, 0.0f), Color.LimeGreen);
                m_vertexBuffer.SetData<VertexPositionColor>(m_vertex);
            }
        }
    }
}
