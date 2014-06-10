using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public RenderList() {
            m_drawables = new RepeatableList<Drawable>();
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

        public override void DoRender(int _timeLastFrame) {
            base.DoRender(_timeLastFrame);
            List<Drawable> drawableList = m_drawables.GetContentList();
            if (drawableList == null) {
                return;
            }
            drawableList.Sort();
            drawableList.Reverse();
            foreach (Drawable drawable in drawableList) {
                drawable.Draw(_timeLastFrame);
            }


        }
    }
}