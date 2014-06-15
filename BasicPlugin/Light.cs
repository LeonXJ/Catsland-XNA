﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Plugin.BasicPlugin {
    public class Light : CatComponent, Drawable{

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
        protected readonly CatVector2 m_offset = new CatVector2();
        public Vector2 Offset {
            set {
                m_offset.SetValue(value);
                m_debugShape.Position = new Vector3(m_offset.X, m_offset.Y, 0.0f);
            }
            get {
                return m_offset.GetValue();
            }
        }

        protected List<Vector2> m_verticeList;

        protected VertexPositionColor[] m_vertice;
        protected VertexBuffer m_vertexBuffer;


#endregion

        public Light(GameObject _gameObject)
            : base(_gameObject) {

        }

        public Light()
            : base() {

        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            m_debugShape.RelateGameObject = m_gameObject;
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.BindToScene(scene);
            }
            // TODO: Bind light to lightlist
            scene._debugDrawableList.AddItem(this);
        }

        protected void UpdateDrawVertex(){
            if(m_vertice == null && m_verticeList != null){
                m_vertice = new VertexPositionColor[m_verticeList.Count];
                for(int i=0; i<m_verticeList.Count; ++i){
                    m_vertice[i] = new VertexPositionColor(Vector3.Zero, Color
                        .White);
                }
            }

            m_vertice[0].Position = new Vector3(m_verticeList[0].X, m_verticeList[0].Y, 0.0f);
            for(int i=1; i<m_verticeList.Count; ++i){
                int index = (i + 1) / 2;
                if( i % 2 == 0){
                    index = m_verticeList.Count - index;
                }
                m_vertice[i].Position = new Vector3(m_verticeList[index].X, 
                                                    m_verticeList[index].Y,
                                                    0.0f);
            }
            m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                typeof(VertexPositionColor), m_verticeList.Count,
                        BufferUsage.None);
            m_vertexBuffer.SetData<VertexPositionColor>(m_vertice);
        }

        public override void Destroy() {
            base.Destroy();
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.Destroy(Mgr<Scene>.Singleton);
            }
            // remove light from light list
            Mgr<Scene>.Singleton._debugDrawableList.RemoveItem(this);
        }

        public void Draw(int timeLastFrame){
            // TODO: use light material
            if(m_vertice == null){
                return;
            }
            //Mgr<BasicEffect>.Singleton.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
            Effect effect = Mgr<DebugTools>.Singleton.DrawEffect;
            ((BasicEffect)effect).Alpha = 0.2f;
            ((BasicEffect)effect).DiffuseColor = new Vector3(1.0f, 0.0f, 1.0f);
            ((BasicEffect)effect).View = Mgr<Camera>.Singleton.View;
            ((BasicEffect)effect).Projection = Mgr<Camera>.Singleton.m_projection;
            ((BasicEffect)effect).VertexColorEnabled = false;
            ((BasicEffect)effect).World = Matrix.CreateTranslation(new Vector3(m_offset.X, m_offset.Y, 0.0f)) * 
                    m_gameObject.AbsTransform;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleStrip,
                    m_vertice,
                    0,
                    m_verticeList.Count - 2);
            }

        }

        public virtual bool IsPointInLight(Vector2 _point) {
            return false;
        }

        public static string GetMenuNames() {
            return "Shadow|LightBase";
        }

        virtual public Vector2 GetLightDirection(Vector2 _point){
            return -Vector2.UnitY;
        }

        public float GetDepth() {
            return 0;
        }

        public int CompareTo(object obj) {
            return 1;
        }



    }
}
