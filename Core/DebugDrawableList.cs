using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * @file DebugDrawableList
 * 
 * @author LeonXie
 * */
namespace Catsland.Core {
    /**
     * @brief DebugDrawableList
     * */
    public class DebugDrawableList : RepeatableList<Drawable> {
        public void Draw(int timeLastFrame) {
            if (contentList == null) {
                return;
            }
            foreach (Drawable drawable in contentList) {
                drawable.Draw(timeLastFrame);
            }
        }
    }
}