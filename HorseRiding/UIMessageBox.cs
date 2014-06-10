using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HorseRiding {
    public class UIMessageBox : CatComponent, IUIDrawable {

#region Properties

        [SerialAttribute]
        private string m_message = "";
        public string Message {
            set {
                m_message = value;
            }
            get {
                return m_message;
            }
        }

        [SerialAttribute]
        private readonly CatColor m_color = new CatColor();
        public Color Color {
            set {
                m_color.SetValue(value);
            }
            get {
                return m_color;
            }
        }

        private SpriteFont m_font;
        private bool m_isOnShow = false;
        private IQTEAction m_action = null;

#endregion

        public UIMessageBox() : base() { }
        public UIMessageBox(GameObject _gameObject):
            base(_gameObject) {

        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            m_font = Mgr<CatProject>.Singleton.contentManger.Load<SpriteFont>("font\\Vivaldi");
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            scene._uiRenderer.AddUI(this);
        }

        public void DoShow(IQTEAction _action) {
            m_action = _action;
            m_isOnShow = true;
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            if (m_isOnShow) {
                KeyboardState ks = Keyboard.GetState();
                if (ks.IsKeyDown(Keys.Space)) {
                    m_isOnShow = false;
                    if (m_action != null) {
                        m_action.OnSuccess();
                    }
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch, int timeLastFrame) {

            if (m_isOnShow){
                int screenWidth =
                Mgr<GraphicsDevice>.Singleton.PresentationParameters.BackBufferWidth;
                int screenHeight = Mgr<GraphicsDevice>.Singleton.PresentationParameters.BackBufferHeight;
                _spriteBatch.DrawString(m_font, m_message + "\n Press any key to continue",
                    new Vector2(100, screenHeight * 0.5f), m_color);
            }
        }
    }
}
