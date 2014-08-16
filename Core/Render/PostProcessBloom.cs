using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Catsland.Core {
    public class PostProcessBloom : PostProcess {

#region Properties

        [SerialAttribute]
        private readonly CatFloat m_highlightThreshold = new CatFloat(0.8f);
        public float HighlightThreshold {
            set {
                m_highlightThreshold.SetValue(MathHelper.Clamp(value, 0.0f, 1.0f));
            }
            get {
                return m_highlightThreshold.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_blurWidth = new CatFloat(2.0f);
        public float BlurWidth {
            set {
                m_blurWidth.SetValue(MathHelper.Clamp(value, 0.0f, 10.0f));
            }
            get {
                return m_blurWidth.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_sceneIntensity = new CatFloat(0.9f);
        public float SceneIntensity {
            set {
                m_sceneIntensity.SetValue(MathHelper.Clamp(value, 0.0f, 2.0f));
            }
            get {
                return m_sceneIntensity;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_glowIntensity = new CatFloat(0.1f);
        public float GlowIntensity{
            set{
                m_glowIntensity.SetValue(MathHelper.Clamp(value, 0.0f, 2.0f));
            }
            get{
                return m_glowIntensity.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_highlightIntensity = new CatFloat(3.0f);
        public float HighlightIntensity{
            set{
                m_highlightIntensity.SetValue(MathHelper.Clamp(value, 0.0f, 10.0f));
            }
            get{
                return m_highlightIntensity.GetValue();
            }
        }

        private RenderTarget2D m_color;
        private RenderTarget2D m_downsampled;
        private RenderTarget2D m_vBlurred;
        private RenderTarget2D m_hBlurred;
        private Effect m_effect;

#endregion

        public PostProcessBloom()
            : base() {
            UpdateBuffer();
            m_effect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>
                ("effect\\Bloom");
        }

        public override void UpdateBuffer() {
            m_color = TestAndCreateColorBuffer(m_color);
            m_downsampled = TestAndCreateColorBuffer(m_downsampled, 0.25f, 0.25f);
            m_vBlurred = TestAndCreateColorBuffer(m_vBlurred, 0.25f, 0.25f);
            m_hBlurred = TestAndCreateColorBuffer(m_hBlurred, 0.25f, 0.25f);
        }

        public override void DoRender(int _timeLastFrame) {
            GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
            if (!m_enable) {
                SimpleRenderDependency(_timeLastFrame);
                return;
            }
            // get what it needs
            Renderer.SetColorTarget(m_color);
            SimpleRenderDependency(_timeLastFrame);
            Renderer.CancelColorTarget();
            // render post process
            m_effect.CurrentTechnique = m_effect.Techniques["Main"];

            Vector2 screenSize =
                new Vector2(graphicsDevice.PresentationParameters.BackBufferWidth,
                            graphicsDevice.PresentationParameters.BackBufferHeight);
            // pass0 down sample
            Renderer.SetColorTarget(m_downsampled);
            m_effect.Parameters["ColorMap"].SetValue((Texture2D)m_color);
            m_effect.Parameters["gHighlightThreshold"].SetValue(m_highlightThreshold);
            m_effect.Parameters["gScreenSize"].SetValue(screenSize);
            m_effect.Parameters["gBlurWidth"].SetValue(m_blurWidth);
            m_effect.Parameters["gSceneIntensity"].SetValue(m_sceneIntensity);
            m_effect.Parameters["gGlowIntensity"].SetValue(m_glowIntensity);
            m_effect.Parameters["gHighlightIntensity"].SetValue(m_highlightIntensity);

            m_effect.CurrentTechnique.Passes["Downsample"].Apply();
            RenderQuad();
            Renderer.CancelColorTarget();

            // pass1 vblur
            Renderer.SetColorTarget(m_vBlurred);
            m_effect.Parameters["BlurMap"].SetValue((Texture2D)m_downsampled);
            m_effect.CurrentTechnique.Passes["GlowV"].Apply();
            RenderQuad();
            Renderer.CancelColorTarget();

            // pass2 hblur
            Renderer.SetColorTarget(m_hBlurred);
            m_effect.Parameters["BlurMap"].SetValue((Texture2D)m_vBlurred);
            m_effect.CurrentTechnique.Passes["GlowH"].Apply();
            RenderQuad();
            Renderer.CancelColorTarget();

            // final
            m_effect.Parameters["BlurMap"].SetValue((Texture2D)m_hBlurred);
            m_effect.CurrentTechnique.Passes["Final"].Apply();
            RenderQuad();

        }
    }
}
