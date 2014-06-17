using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Catsland.Core {
    public class Renderer : Serialable{

        #region Properties
        private static Stack<RenderTarget2D> m_colorTargetStack =
            new Stack<RenderTarget2D>();

        protected static VertexPositionTexture[] vertex;
        protected static VertexBuffer vertexBuffer;


        #endregion

        virtual public void DoRender(int _timeLastFrame){}

        // push
        public static void SetColorTarget(RenderTarget2D _colorTarget) {
            m_colorTargetStack.Push(_colorTarget);
            Mgr<GraphicsDevice>.Singleton.SetRenderTarget(_colorTarget);
        }

        //pop
        public static void CancelColorTarget() {
            if (m_colorTargetStack.Count > 0) {
                m_colorTargetStack.Pop();
            }
            if (m_colorTargetStack.Count > 0) {
                Mgr<GraphicsDevice>.Singleton.SetRenderTarget(m_colorTargetStack.Peek());
            }
            else {
                Mgr<GraphicsDevice>.Singleton.SetRenderTarget(null);
            }
        }

        protected void CreateQuad() {
            if (vertex != null) {
                return;
            }
            GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            float widthOffset = 1.0f / width;
            float heightOffset = 1.0f / height;
            vertex = new VertexPositionTexture[4];
            vertex[0] = new VertexPositionTexture(
                new Vector3(-1.0f, -1.0f + heightOffset, 1.0f),
                new Vector2(0, 1));
            vertex[1] = new VertexPositionTexture(
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector2(0, 0));
            vertex[2] = new VertexPositionTexture(
                new Vector3(1.0f - widthOffset, -1.0f + heightOffset, 1.0f),
                new Vector2(1, 1));
            vertex[3] = new VertexPositionTexture(
                new Vector3(1.0f - widthOffset, 1.0f, 1.0f),
                new Vector2(1, 0));
            vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                typeof(VertexPositionTexture), 4, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionTexture>(vertex);
        }

        protected void RenderQuad() {
            CreateQuad();
            Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(vertexBuffer);
            Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleStrip, vertex, 0, 2);
        }
    }
}
