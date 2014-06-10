using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HorseRiding {
    public class DebugPlugin : CatComponent, IUIDrawable {

#region Properties

        string m_text = "normal speed";
        SpriteFont m_font;


#endregion

        public DebugPlugin()
            : base() {
        }

        public DebugPlugin(GameObject _gameObject):
            base(_gameObject) {
        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            m_font = Mgr<CatProject>.Singleton.contentManger.Load<SpriteFont>("font\\keycodeFont");
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            scene._uiRenderer.AddUI(this);
        }

        public void Draw(SpriteBatch _spriteBatch, int timeLastFrame) {
//             int screenWidth = 
//                 Mgr<GraphicsDevice>.Singleton.PresentationParameters.BackBufferWidth;
//             int screenHeight = Mgr<GraphicsDevice>.Singleton.PresentationParameters.BackBufferHeight;
//             _spriteBatch.DrawString(m_font, m_text, 
//                 new Vector2(screenWidth, screenHeight) * 0.5f, Color.Black);
            
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            KeyboardState keyboardState = Keyboard.GetState();
//             if (keyboardState.IsKeyDown(Keys.O)) {
//                 Mgr<GameEngine>.Singleton.TimeScale -= 0.01f;
//             }
//             if (keyboardState.IsKeyDown(Keys.P)) {
//                 Mgr<GameEngine>.Singleton.TimeScale += 0.01f;
//             }
//             if (keyboardState.IsKeyDown(Keys.I)) {
//                 Mgr<GameEngine>.Singleton.TimeScale = 1.0f;
//             }
//             if (keyboardState.IsKeyDown(Keys.K)) {
//                 Mgr<GameEngine>.Singleton.TimeScale = 0.1f;
//             }
//             if (keyboardState.IsKeyDown(Keys.N)) {
//                 MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
//                 MovieClip movieClip = motionDelegator.AddMovieClip();
//                 movieClip.AppendMotion(Mgr<GameEngine>.Singleton.TimeScaleRef, new CatFloat(0.1f), 500);
//                 movieClip.Initialize();
//             }

             
             
        }

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);
        }
    }
}
