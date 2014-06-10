using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catsland.Plugin.BasicPlugin;

namespace HorseRiding {
    public class CameraSpaceRender : CatComponent, Drawable {

        #region Properties

        [SerialAttribute]
        private readonly CatVector2 m_position = new CatVector2(0.0f, 0.0f);
        public Vector2 Position {
            set {
                m_position.SetValue(value);
            }
            get {
                return m_position.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatVector2 m_size = new CatVector2(0.5f, 0.5f);
        public Vector2 Size {
            set {
                m_size.SetValue(new Vector2(
                    MathHelper.Max(value.X, 0.0f),
                    MathHelper.Max(value.Y, 0.0f)));
                UpdateVertex();
            }
            get {
                return m_size.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatVector2 m_offset = new CatVector2(0.0f, 0.0f);
        public Vector2 Offset {
            set {
                m_offset.SetValue(value);
            }
            get {
                return m_offset.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_depth = new CatFloat(0.0f);
        public float Depth {
            set {
                m_depth.SetValue(MathHelper.Clamp(value, 0.0f, 1.0f));
            }
            get {
                return m_depth.GetValue();
            }
        }

        private VertexPositionTexture[] m_vertex;
        private VertexBuffer m_vertexBuffer;

        #endregion

        public CameraSpaceRender()
            : base() {

        }

        public CameraSpaceRender(GameObject _gameObject)
            : base(_gameObject) {

        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            scene._renderList.AddItem(this);
//             if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
//                 scene._selectableList.AddItem(this);
//             }
        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            UpdateVertex();
        }

        public override void Destroy() {
            base.Destroy();
            Mgr<Scene>.Singleton._renderList.RemoveItem(this);
        }

        public void UpdateVertex() {
            if (m_vertex == null) {
                m_vertex = new VertexPositionTexture[4];
            }
            float halfWidth = m_size.X / 2.0f;
            float halfHeight = m_size.Y / 2.0f;
            m_vertex[0] = new VertexPositionTexture(
                new Vector3(halfWidth, halfHeight, 0), new Vector2(1, 0));
            m_vertex[1] = new VertexPositionTexture(
                new Vector3(halfWidth, -halfHeight, 0), new Vector2(1, 1));
            m_vertex[2] = new VertexPositionTexture(
                new Vector3(-halfWidth, halfHeight, 0), new Vector2(0, 0));
            m_vertex[3] = new VertexPositionTexture(
                new Vector3(-halfWidth, -halfHeight, 0), new Vector2(0, 1));
            if (m_vertexBuffer == null) {
                m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                    typeof(VertexPositionTexture), 4, BufferUsage.None);
                m_vertexBuffer.SetData<VertexPositionTexture>(m_vertex);
            }
        }

        public void Draw(int timeLastFrame) {
            if (!Enable) {
                return;
            }

            ModelComponent modelComponent = m_gameObject.GetComponent(
                typeof(ModelComponent).ToString()) as ModelComponent;
            GraphicsDevice gd = Mgr<GraphicsDevice>.Singleton;
            gd.SetVertexBuffer(m_vertexBuffer);
            Effect effect = null;
            Vector3 finalPosition = new Vector3(m_position.X - m_offset.X,
                                                m_position.Y - m_offset.Y,
                                                m_depth);
            if (modelComponent != null && modelComponent.Model != null) {
                CatMaterial material = modelComponent.GetCatModelInstance().GetMaterial();
                material.SetParameter("World", new CatMatrix(Matrix.CreateTranslation(
                    finalPosition)));
                material.SetParameter("View", new CatMatrix(Matrix.Identity));
                material.SetParameter("Projection", new CatMatrix(Matrix.Identity));
                effect = material.ApplyMaterial();
            }
            else {
                effect = Mgr<DebugTools>.Singleton.DrawEffect;
                ((BasicEffect)effect).Alpha = 1.0f;
                ((BasicEffect)effect).DiffuseColor = new Vector3(1.0f, 0.0f, 1.0f);
                ((BasicEffect)effect).View = Matrix.Identity;
                ((BasicEffect)effect).Projection = Matrix.Identity;
                ((BasicEffect)effect).VertexColorEnabled = false;
                ((BasicEffect)effect).World = Matrix.CreateTranslation(finalPosition);
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                gd.DrawUserPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleStrip, m_vertex, 0, 2);
            }
        }

        public float GetDepth() {
            return m_depth.GetValue();
        }

        public int CompareTo(object obj) {
            float otherDepth = ((Drawable)obj).GetDepth();
            float thisDepth = GetDepth();
            if (otherDepth > thisDepth) {
                return 1;
            }
            else if (otherDepth < thisDepth) {
                return -1;
            }
            return 0;
        }

        public static string GetMenuNames() {
            return "Render|ScreenSpaceRender";
        }
    }
}
