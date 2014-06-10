using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class UIRenderer {

#region Properties

        private List<IUIDrawable> m_drawables = new List<IUIDrawable>();
        private SpriteBatch m_spriteBatch;

#endregion

        public UIRenderer() {
            m_spriteBatch = new SpriteBatch(Mgr<GraphicsDevice>.Singleton);
        }

        public void AddUI(IUIDrawable _drawable) {
            m_drawables.Add(_drawable);
        }

        public void RemoveUI(IUIDrawable _drawable) {
            m_drawables.Remove(_drawable);
        }

        public void Draw(int _timeInMS) {
            m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap,
    DepthStencilState.None, RasterizerState.CullNone);
            foreach (IUIDrawable drawable in m_drawables) {
                drawable.Draw(m_spriteBatch, _timeInMS);
            }
            m_spriteBatch.End();
        }
    }
}
