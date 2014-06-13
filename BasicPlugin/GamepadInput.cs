using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Catsland.Plugin.BasicPlugin {
    public class GamepadInput : CatComponent {

#region Properties
#endregion

        public GamepadInput(GameObject gameObject)
            : base(gameObject){
        }

        public GamepadInput() : base() { }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);

            // TODO
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            CatController catController = (CatController)m_gameObject.
                GetComponent(typeof(CatController).ToString());

            if (!Enable) {
                catController.m_wantLeft = false;
                catController.m_wantRight = false;
                catController.m_wantUp = false;
                catController.m_wantDown = false;
                catController.m_wantRun = false;
                catController.m_wantJump = false;
                return;
            }

            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            catController.m_wantLeft = gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft);
            catController.m_wantRight = gamePadState.IsButtonDown(Buttons.LeftThumbstickRight);
            catController.m_wantUp = gamePadState.IsButtonDown(Buttons.LeftThumbstickUp);
            catController.m_wantDown = gamePadState.IsButtonDown(Buttons.LeftThumbstickDown);
            catController.m_wantJump = gamePadState.IsButtonDown(Buttons.A);
            catController.m_wantRun = gamePadState.IsButtonDown(Buttons.LeftShoulder);
        }
    }
}
