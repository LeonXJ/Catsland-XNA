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
