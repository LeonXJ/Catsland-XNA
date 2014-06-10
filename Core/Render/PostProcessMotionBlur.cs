using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Catsland.Core {
    public class PostProcessMotionBlur : PostProcess {

        #region Properties
        [SerialAttribute]
        private readonly CatFloat m_blurIntensity = new CatFloat(0.2f);
        public float BlurIntensity {
            set {
                m_blurIntensity.SetValue(MathHelper.Clamp(value, 0.0f, 1.0f));
            }
            get {
                return m_blurIntensity.GetValue();
            }
        }
        public CatFloat BlurIntensityRef {
            get {
                return m_blurIntensity;
            }
        }

        Effect m_effect;
        RenderTarget2D m_color;
        RenderTarget2D m_accColorRead;
        RenderTarget2D m_accColorWrite;

        #endregion

        public PostProcessMotionBlur()
            : base() {
            UpdateBuffer();
            m_effect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>
                ("effect\\MotionBlur");
        }

        public override void UpdateBuffer() {
            m_color = TestAndCreateColorBuffer(m_color);
            m_accColorRead = TestAndCreateColorBuffer(m_accColorRead);
            ClearRenderTargetContent(m_accColorRead);
            m_accColorWrite = TestAndCreateColorBuffer(m_accColorWrite);
        }

        private void ClearRenderTargetContent(RenderTarget2D _renderTarget) {
            GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
            Renderer.SetColorTarget(m_accColorRead);
            graphicsDevice.Clear(new Color(0.0f,0.0f,0.0f,1.0f));
            Renderer.CancelColorTarget();
        }

        public override void DoRender(int _timeLastFrame) {
            base.DoRender(_timeLastFrame);
            if (!m_enable) {
                SimpleRenderDependency(_timeLastFrame);
                return;
            }
            Renderer.SetColorTarget(m_color);
            SimpleRenderDependency(_timeLastFrame);
            Renderer.CancelColorTarget();

            GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
            m_effect.CurrentTechnique = m_effect.Techniques["Main"];
            // write to acc
            Renderer.SetColorTarget(m_accColorWrite);
            m_effect.Parameters["ColorMap"].SetValue((Texture2D)m_color);
            m_effect.Parameters["AccColorMap"].SetValue((Texture2D)m_accColorRead);
            m_effect.Parameters["gBlurIntensity"].SetValue(m_blurIntensity);
            m_effect.CurrentTechnique.Passes["Blur"].Apply();
            graphicsDevice.Clear(Color.Black);
            RenderQuad();
            Renderer.CancelColorTarget();
            // output
            graphicsDevice.Clear(Color.Black);
            m_effect.Parameters["ColorMap"].SetValue((Texture2D)m_accColorWrite);
            m_effect.CurrentTechnique.Passes["Final"].Apply();
            RenderQuad();
            // swap
            RenderTarget2D tmp = m_accColorRead;
            m_accColorRead = m_accColorWrite;
            m_accColorWrite = tmp;
        }
    }
}
