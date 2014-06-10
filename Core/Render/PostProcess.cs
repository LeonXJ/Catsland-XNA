using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Catsland.Core {
    public class PostProcess : Renderer {

#region Properties

        [SerialAttribute]
        protected CatBool m_enable = new CatBool(true);
        public bool Enable {
            set {
                m_enable.SetValue(value);
            }
            get {
                return m_enable;
            }
        }

        protected List<Renderer> m_dependRenderer = new List<Renderer>();
        protected VertexPositionTexture[] m_vertex;
        protected VertexBuffer m_vertexBuffer;

#endregion

        protected PostProcess() {
            CreateQuad();
        }

        virtual public void UpdateBuffer() {

        }

        public void AddDependOn(Renderer _renderer) {
            m_dependRenderer.Add(_renderer);
        }

        protected void SimpleRenderDependency(int _timeLastFrame) {
            GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
            graphicsDevice.Clear(Color.Black);
            foreach (Renderer renderer in m_dependRenderer) {
                renderer.DoRender(_timeLastFrame);
            }
            return;
        }

        protected void CreateQuad() {
            GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            float widthOffset = 1.0f / width;
            float heightOffset = 1.0f / height;
            m_vertex = new VertexPositionTexture[4];
            m_vertex[0] = new VertexPositionTexture(
                new Vector3(-1.0f, -1.0f + heightOffset, 1.0f),
                new Vector2(0, 1));
            m_vertex[1] = new VertexPositionTexture(
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector2(0, 0));
            m_vertex[2] = new VertexPositionTexture(
                new Vector3(1.0f - widthOffset, -1.0f + heightOffset, 1.0f),
                new Vector2(1, 1));
            m_vertex[3] = new VertexPositionTexture(
                new Vector3(1.0f - widthOffset, 1.0f, 1.0f),
                new Vector2(1, 0));
            m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                typeof(VertexPositionTexture), 4, BufferUsage.None);
            m_vertexBuffer.SetData<VertexPositionTexture>(m_vertex);
        }

        protected void RenderQuad() {
            Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
            Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleStrip, m_vertex, 0, 2);
        }

        protected RenderTarget2D TestAndCreateColorBuffer(
            RenderTarget2D _oldRenderTarget,
            float _widthRatio = 1.0f, float _heightRatio = 1.0f) {

            GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
            if (_oldRenderTarget != null) {
                //_oldRenderTarget.Dispose();
            }
            return new RenderTarget2D(
                graphicsDevice,
                (int)(graphicsDevice.PresentationParameters.BackBufferWidth * _widthRatio),
                (int)(graphicsDevice.PresentationParameters.BackBufferHeight * _heightRatio));
        }
    }
}
