using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Catsland.Core {
    public class PostProcessHDR : PostProcess {

        #region Properties

        [SerialAttribute]
        private readonly CatFloat m_exposure = new CatFloat(0.5f);
        public float Exposure {
            get {
                return m_exposure.GetValue();
            }
            set {
                m_exposure.SetValue(MathHelper.Clamp(value, 0.5f, 2.0f));
            }
        }

        RenderTarget2D m_color;
        RenderTarget2D m_downSample;
        Effect m_effect;

        #endregion

        public PostProcessHDR()
            : base() {
            UpdateBuffer();
            m_effect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>
                ("effect\\HDR");
        }

        public override void UpdateBuffer() {
            m_color = TestAndCreateColorBuffer(m_color);
            m_downSample = TestAndCreateColorBuffer(m_downSample, 0.5f, 0.5f);
        }

        public override void DoRender(int _timeLastFrame) {
            if (!m_enable) {
                SimpleRenderDependency(_timeLastFrame);
                return;
            }
            // get what it needs
            Renderer.SetColorTarget(m_color);
            GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
            SimpleRenderDependency(_timeLastFrame);
            Renderer.CancelColorTarget();
            // render post process
            m_effect.CurrentTechnique = m_effect.Techniques["HDR"];
            
            // pass0 down sample
            Renderer.SetColorTarget(m_downSample);
            m_effect.Parameters["ColorMap"].SetValue((Texture2D)m_color);
            m_effect.CurrentTechnique.Passes[0].Apply();
            RenderQuad();
            Renderer.CancelColorTarget();
            m_effect.Parameters["ColorMap"].SetValue((Texture2D)m_color);
            m_effect.Parameters["DownSampledMap"].SetValue((Texture2D)m_downSample);
            m_effect.Parameters["Exposure"].SetValue(Exposure);
            // pass1
            m_effect.CurrentTechnique.Passes[1].Apply();
            RenderQuad();

        }
    }
}
