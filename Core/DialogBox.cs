using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/**
 * @file DialogBox
 * 
 * UI dialog box
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {

    /**
     * @brief UI dialog box
     * 
     * [left top   ][top    ][right top   ]
     * [left       ][content][right       ]
     * [left bottom][bottom ][right bottom]
     * 
     * */
    public class DialogBox {
        public Texture2D m_leftTopTex;
        public Texture2D m_topTex;
        public Texture2D m_insideTex;
        public Texture2D m_leftTex;
        public SpriteFont m_font;
        public String m_text;
        public Point m_leftTop;
        public Point m_rightBottom;

        public bool m_enable;

        /**
         * @brief draw the dialog box
         * 
         * called by engine
         * 
         * @param spriteBatch created by Engine
         * */
        public void Draw(SpriteBatch spriteBatch) {
            if (!m_enable) {
                return;
            }

            // leftTop
            int width = m_rightBottom.X - m_leftTop.X;
            int height = m_rightBottom.Y - m_leftTop.Y;
            spriteBatch.Draw(m_leftTopTex,
                new Rectangle(m_leftTop.X, m_leftTop.Y, m_leftTopTex.Width, m_leftTopTex.Height),
                Color.White);
            // top
            spriteBatch.Draw(m_topTex,
                new Rectangle(m_leftTop.X + m_leftTopTex.Width, m_leftTop.Y, width - 2 * m_leftTopTex.Width, m_leftTopTex.Height),
                    Color.White);
            // rightTop
            spriteBatch.Draw(m_leftTopTex,
                new Rectangle(m_rightBottom.X - m_leftTopTex.Width, m_leftTop.Y, m_leftTopTex.Width, m_leftTopTex.Height),
                null,
                Color.White,
                0.0f,
                new Vector2(0.0f, 0.0f),
                SpriteEffects.FlipHorizontally,
                0.0f);
            // left

            spriteBatch.Draw(m_leftTex,
                new Rectangle(m_leftTop.X, m_leftTop.Y + m_leftTopTex.Height, m_leftTopTex.Width, height - 2 * m_leftTopTex.Height),
                null,
                Color.White,
                0.0f,
                new Vector2(0.0f, 0.0f),
                SpriteEffects.None,
                0.0f);

            // inside
            spriteBatch.Draw(m_insideTex,
                new Rectangle(m_leftTop.X + m_leftTopTex.Width, m_leftTop.Y + m_leftTopTex.Height,
                    width - 2 * m_leftTopTex.Width, height - 2 * m_leftTopTex.Height),
                    null,
                    Color.White,
                    0.0f,
                    new Vector2(0.0f, 0.0f),
                    SpriteEffects.None,
                    0.1f);

            // right
            spriteBatch.Draw(m_leftTex,
                new Rectangle(m_rightBottom.X - m_leftTopTex.Width, m_leftTop.Y + m_leftTopTex.Height,
                    m_leftTopTex.Width, height - 2 * m_leftTopTex.Height),
                null,
                Color.White,
                0.0f,
                new Vector2(0.0f, 0.0f),
                SpriteEffects.FlipHorizontally,
                0.0f);

            // leftBottom
            spriteBatch.Draw(m_leftTopTex,
                new Rectangle(m_leftTop.X, m_rightBottom.Y - m_leftTopTex.Height, m_leftTopTex.Width, m_leftTopTex.Height),
                null,
                Color.White,
                0.0f,
                new Vector2(0.0f, 0.0f),
                SpriteEffects.FlipVertically,
                0.0f);

            // bottom
            spriteBatch.Draw(m_topTex,
                new Rectangle(m_leftTop.X + m_leftTopTex.Width, m_rightBottom.Y - m_leftTopTex.Height,
                    width - 2 * m_leftTopTex.Width, m_leftTopTex.Height),
                    null,
                    Color.White,
                    0.0f,
                    new Vector2(0.0f, 0.0f),
                    SpriteEffects.FlipVertically,
                    0.0f);

            // rightBottom
            spriteBatch.Draw(m_leftTopTex,
                new Rectangle(m_rightBottom.X - m_leftTopTex.Width, m_rightBottom.Y - m_leftTopTex.Height,
                    m_leftTopTex.Width, m_leftTopTex.Height),
                    null,
                    Color.White,
                    (float)MathHelper.Pi,
                    new Vector2(m_leftTopTex.Width, m_leftTopTex.Height),
                    SpriteEffects.None,
                    0.0f);

            // content
            spriteBatch.DrawString(m_font, m_text,
                new Vector2(m_leftTop.X + m_leftTopTex.Width, m_leftTop.Y + m_leftTopTex.Height),
                Color.White, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
