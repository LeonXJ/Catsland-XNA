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

        private GamePadState m_preGamepadState;

#endregion

        public GamepadInput(GameObject gameObject)
            : base(gameObject){
        }

        public GamepadInput() : base() { }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);

            // TODO
            m_preGamepadState = GamePad.GetState(PlayerIndex.One);
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            CatController catController = (CatController)m_gameObject.
                GetComponent(typeof(CatController).ToString());

            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            catController.m_wantLeft = catController.m_wantLeft || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft);
            catController.m_wantRight = catController.m_wantRight || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight);
            catController.m_wantUp = catController.m_wantUp || gamePadState.IsButtonDown(Buttons.LeftThumbstickUp);
            catController.m_wantDown = catController.m_wantDown || gamePadState.IsButtonDown(Buttons.LeftThumbstickDown);
            catController.m_wantJump = catController.m_wantJump || (!m_preGamepadState.IsButtonDown(Buttons.A)
                && gamePadState.IsButtonDown(Buttons.A));
            catController.m_wantLift = catController.m_wantLift || gamePadState.IsButtonDown(Buttons.A);
            catController.m_wantRun = catController.m_wantRun || gamePadState.IsButtonDown(Buttons.LeftShoulder);
            
            m_preGamepadState = gamePadState;
        }

        public static string GetMenuNames() {
            return "Controller|GamePad Input";
        }
    }
}
