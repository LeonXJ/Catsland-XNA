using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace HorseRiding {
    public class QTEAnnouncer : CatComponent, IUIDrawable, IQTEAction {
#region Properies

        SpriteFont m_font;
        Keys m_key;
        int m_time;
        bool m_isOn;

#endregion

        public QTEAnnouncer():
            base() {
        }

        public QTEAnnouncer(GameObject _gameObject)
            : base(_gameObject) {

        }

        public override void Initialize(Catsland.Core.Scene scene) {
            base.Initialize(scene);
            m_font = Mgr<CatProject>.Singleton.contentManger.Load<SpriteFont>("font\\keycodeFont");
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            scene._uiRenderer.AddUI(this);
        }

        public void Draw(SpriteBatch _spriteBatch, int timeLastFrame) {

            if (m_isOn) {
                string text = m_key.ToString() + "," + m_time.ToString();
                int screenWidth =
                Mgr<GraphicsDevice>.Singleton.PresentationParameters.BackBufferWidth;
                int screenHeight = Mgr<GraphicsDevice>.Singleton.PresentationParameters.BackBufferHeight;
                _spriteBatch.DrawString(m_font, text,
                    new Vector2(screenWidth, screenHeight) * 0.5f, Color.Black);
            }
        }

        public override void Destroy() {
            Mgr<Scene>.Singleton._uiRenderer.RemoveUI(this);
            base.Destroy();   
        }

        public void OnSuccess() {
            m_isOn = false;
        }

        public void OnFail() {
            m_isOn = false;
        }

        public void Announce(Keys _key, int _time) {
            m_key = _key;
            m_time = _time;
            m_isOn = true;
        }
    }
}
