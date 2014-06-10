using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Catsland.Core {
    public class PostProcessVignette : PostProcess {
#region Properties

        [SerialAttribute]
        private readonly CatVector2 m_radius = new CatVector2(0.0f, 0.9f);
        public Vector2 Radius {
            get {
                return m_radius.GetValue();
            }
            set {
                m_radius.SetValue(
                    new Vector2(MathHelper.Clamp(value.X, 0.0f, value.Y), value.Y));
            }
        }
        public CatVector2 RadiusRef {
            get {
                return m_radius;
            }
        }

        private RenderTarget2D m_color;
        private Effect m_effect;

#endregion

        public PostProcessVignette()
            : base() {
            UpdateBuffer();
            m_effect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>
                ("effect\\Vignette");
        }

        public override void UpdateBuffer() {
            m_color = TestAndCreateColorBuffer(m_color);
        }

        public override void DoRender(int _timeLastFrame) {
            base.DoRender(_timeLastFrame);
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
            m_effect.CurrentTechnique = m_effect.Techniques["Vignette"];
            m_effect.Parameters["ColorMap"].SetValue((Texture2D)m_color);
            m_effect.Parameters["InnerRadius"].SetValue(Radius.X);
            m_effect.Parameters["OuterRadius"].SetValue(Radius.Y);
            m_effect.CurrentTechnique.Passes[0].Apply();
            RenderQuad();
        
        }
    }
}
