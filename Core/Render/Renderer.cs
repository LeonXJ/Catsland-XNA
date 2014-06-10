using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class Renderer : Serialable{

        #region Properties
        private static Stack<RenderTarget2D> m_colorTargetStack =
            new Stack<RenderTarget2D>();

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
    }
}
