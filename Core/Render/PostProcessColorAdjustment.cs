using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Catsland.Core {
    public class PostProcessColorAdjustment : PostProcess {

#region Properties

        [SerialAttribute]
        private readonly CatFloat m_saturability = new CatFloat(1.0f);
        public float Saturability {
            get {
                return m_saturability.GetValue();
            }
            set {
                m_saturability.SetValue(MathHelper.Clamp(value, 0.0f, 1.0f));
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_illuminate = new CatFloat(0.0f);
        public float Illuminate {
            get {
                return m_illuminate.GetValue();
            }
            set {
                m_illuminate.SetValue(MathHelper.Clamp(value, -1.0f, 1.0f));
            }
        }
        public CatFloat IllumiateRef {
            get {
                return m_illuminate;
            }
        }

        RenderTarget2D m_color;
        Effect m_effect;

#endregion

        public PostProcessColorAdjustment()
            : base() {
            UpdateBuffer();
            m_effect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>
                ("effect\\ColorAdjustment");
        }

        public void UpdateBuffer(){
            m_color = TestAndCreateColorBuffer(m_color);
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
            m_effect.CurrentTechnique = m_effect.Techniques["ColorAdjustment"];
            m_effect.Parameters["ColorMap"].SetValue((Texture2D)m_color);
            m_effect.Parameters["Saturability"].SetValue(Saturability);
            m_effect.Parameters["Illuminate"].SetValue(Illuminate);
            m_effect.CurrentTechnique.Passes[0].Apply();
            RenderQuad();
        }
    }
}
