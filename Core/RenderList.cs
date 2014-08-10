using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

/**
 * @file RenderList
 * 
 * @author LeonXie
 * */
namespace Catsland.Core {
    /**
     * @brief game engine will scan thought renderList and render the contents out
     * */
    public class RenderList : Renderer {

        protected RepeatableList<Drawable> m_drawables;

        private ShadowSystem m_shadowRender;
        private RenderTarget2D m_inShadowMap;


        public RenderList() {
            m_drawables = new RepeatableList<Drawable>();
            UpdateBuffer();
        }

        public void AddItem(Drawable _drawable) {
            m_drawables.AddItem(_drawable);
        }

        public void RemoveItem(Drawable _drawable) {
            m_drawables.RemoveItem(_drawable);
        }

        public void ReleaseAll() {
            m_drawables.ReleaseAll();
        }

        public void SetShadowRender(ShadowSystem _shadowSystem) {
            m_shadowRender = _shadowSystem;
        }

        public void UpdateBuffer() {
           /* m_accumulateLight = TestAndCreateColorBuffer(m_accumulateLight);*/
            m_inShadowMap = TestAndCreateColorBuffer(m_inShadowMap);
        }

        // TODO: combine with that in PostProcess
        private RenderTarget2D TestAndCreateColorBuffer(
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

        public override void DoRender(int _timeLastFrame) {
            base.DoRender(_timeLastFrame);
            List<Drawable> drawableList = m_drawables.GetContentList();
            if (drawableList == null) {
                return;
            }
            drawableList.Sort();
            drawableList.Reverse();
            if (m_shadowRender != null) {
                m_shadowRender.DoRender(_timeLastFrame);
                foreach (Drawable drawable in drawableList) {
                    drawable.Draw(_timeLastFrame);
                }
                // render object in shadow
//                 Renderer.SetColorTarget(m_inShadowMap);
//                 GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
//                 graphicsDevice.Clear(Color.White);
//                 int i = 0;
// //                 for (;
// //                     (drawableList[i].GetDepth() > 0.0f && i < drawableList.Count);
//                     for(; i < drawableList.Count; ++i) {
//                     drawableList[i].Draw(_timeLastFrame);
//                 }
//                 Renderer.CancelColorTarget();
//                 // combine inShadowMap and accumulateLight
//                 m_shadowRender.ShadowObject(_timeLastFrame, m_inShadowMap);
//                 // render after
//                 for (; i < drawableList.Count; ++i) {
//                     drawableList[i].Draw(_timeLastFrame);
//                 }
            }
            else {
                foreach (Drawable drawable in drawableList) {
                    drawable.Draw(_timeLastFrame);
                }
            }
        }
    }
}