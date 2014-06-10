using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using System.Xml;
using Catsland.Core;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class KeyboardInput : CatComponent {
        [CategoryAttribute("Input")]
        public Keys Left { set; get; }
        [CategoryAttribute("Input")]
        public Keys Right { set; get; }
        [CategoryAttribute("Input")]
        public Keys Up { set; get; }
        [CategoryAttribute("Input")]
        public Keys Down { set; get; }
        [CategoryAttribute("Input")]
        public Keys Defence { set; get; }
        [CategoryAttribute("Input")]
        public Keys Attack { set; get; }
        [CategoryAttribute("Input")]
        public Keys Run { set; get; }
        [CategoryAttribute("Input")]
        public Keys Jump { set; get; }
        [CategoryAttribute("Input")]
        public Keys Use { set; get; }


        private KeyboardState oldKeyboardState;

        public KeyboardInput(GameObject gameObject)
            : base(gameObject) {

        }

        public KeyboardInput()
            : base() {

        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            KeyboardInput keyboardInput = new KeyboardInput(gameObject);
            return keyboardInput;
        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);

            Left = Keys.Left;
            Right = Keys.Right;
            Up = Keys.Up;
            Down = Keys.Down;

            Defence = Keys.None;
            Attack = Keys.None;
            Jump = Keys.Space;
            Run = Keys.LeftShift;

            Use = Keys.None;
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            
//             CharacterController characterController = (CharacterController)m_gameObject
//                 .GetComponent(typeof(CharacterController).ToString());
//             if (characterController == null) {
//                 Console.WriteLine("Error! KeyboardInput Component needs CharacterController.");
//                 return;
//             }

            CatController characterController = (CatController)m_gameObject.
                GetComponent(typeof(CatController).ToString());

            if (!Enable) {
                characterController.m_wantLeft = false;
                characterController.m_wantRight = false;
//                 characterController.m_wantUp = false;
//                 characterController.m_wantDown = false;
                 characterController.m_wantRun = false;
                 characterController.m_wantJump = false;
//                 characterController.m_wantDefence = false;
//                 characterController.m_wantAttack = false;
//                 characterController.m_wantTalk = false;
                return;
            }

            KeyboardState keyboardState = Keyboard.GetState();
            characterController.m_wantLeft = keyboardState.IsKeyDown(Left);
            characterController.m_wantRight = keyboardState.IsKeyDown(Right);
//             characterController.m_wantUp = keyboardState.IsKeyDown(Up);
//             characterController.m_wantDown = keyboardState.IsKeyDown(Down);
            characterController.m_wantRun = keyboardState.IsKeyDown(Run);
             characterController.m_wantJump = keyboardState.IsKeyDown(Jump);
//             characterController.m_wantDefence = keyboardState.IsKeyDown(Defence);
//             characterController.m_wantAttack = keyboardState.IsKeyDown(Attack);
//             characterController.m_wantTalk = keyboardState.IsKeyDown(Use);

            if (keyboardState.IsKeyDown(Keys.C)) {
                if (oldKeyboardState != null && !oldKeyboardState.IsKeyDown(Keys.C)) {
/*                    characterController.InvertCatStatus();*/
                }
            }

            // TODO: just test sound
            if (keyboardState.IsKeyDown(Keys.M)) {
                if (oldKeyboardState != null && !oldKeyboardState.IsKeyDown(Keys.M)) {
                    Mgr<CatProject>.Singleton.m_soundManager.PlayMusic("music\\highsea");
                }
            }
            if (keyboardState.IsKeyDown(Keys.N)) {
                if (oldKeyboardState != null && !oldKeyboardState.IsKeyDown(Keys.N)) {
                    Mgr<CatProject>.Singleton.m_soundManager.PlayMusic("music\\buc");
                }
            }
            if (keyboardState.IsKeyDown(Keys.B)) {
                if (oldKeyboardState != null && !oldKeyboardState.IsKeyDown(Keys.B)) {
                    Mgr<CatProject>.Singleton.m_soundManager.PlayMusic("music\\fight");
                }
            }
            if (keyboardState.IsKeyDown(Keys.RightControl)) {
                if (oldKeyboardState != null && !oldKeyboardState.IsKeyDown(Keys.RightControl)) {
                    Mgr<CatProject>.Singleton.m_soundManager.PlaySound("sound\\m4a1",
                        new Vector3(0.0f, 0.0f, 1.0f));
                }                
            }
            // test post process
            if (keyboardState.IsKeyDown(Keys.D2)) {
                PostProcessColorAdjustment colorAdjustment = Mgr<Scene>.Singleton.PostProcessManager.
                    GetPostProcess(typeof(PostProcessColorAdjustment).ToString()) 
                    as PostProcessColorAdjustment;
                if (colorAdjustment != null) {
                    colorAdjustment.Saturability += 0.01f;
                }
            }
            if (keyboardState.IsKeyDown(Keys.D1)) {
                PostProcessColorAdjustment colorAdjustment = Mgr<Scene>.Singleton.PostProcessManager.
                    GetPostProcess(typeof(PostProcessColorAdjustment).ToString())
                    as PostProcessColorAdjustment;
                if (colorAdjustment != null) {
                    colorAdjustment.Saturability -= 0.01f;
                }
            }
            if (keyboardState.IsKeyDown(Keys.D4)) {
                PostProcessColorAdjustment colorAdjustment = Mgr<Scene>.Singleton.PostProcessManager.
                    GetPostProcess(typeof(PostProcessColorAdjustment).ToString())
                    as PostProcessColorAdjustment;
                if (colorAdjustment != null) {
                    colorAdjustment.Illuminate += 0.01f;
                }
            }
            if (keyboardState.IsKeyDown(Keys.D3)) {
                PostProcessColorAdjustment colorAdjustment = Mgr<Scene>.Singleton.PostProcessManager.
                    GetPostProcess(typeof(PostProcessColorAdjustment).ToString())
                    as PostProcessColorAdjustment;
                if (colorAdjustment != null) {
                    colorAdjustment.Illuminate -= 0.01f;
                }
            }
            // HDR
            if (keyboardState.IsKeyDown(Keys.D5)) {
                PostProcessHDR hdr = Mgr<Scene>.Singleton.PostProcessManager.
                    GetPostProcess(typeof(PostProcessHDR).ToString())
                    as PostProcessHDR;
                if (hdr != null) {
                    hdr.Exposure -= 0.1f;
                }
            }
            if (keyboardState.IsKeyDown(Keys.D6)) {
                PostProcessHDR hdr = Mgr<Scene>.Singleton.PostProcessManager.
                    GetPostProcess(typeof(PostProcessHDR).ToString())
                    as PostProcessHDR;
                if (hdr != null) {
                    hdr.Exposure += 0.1f;
                }
            }
            // Vignette
            // HDR
            if (keyboardState.IsKeyDown(Keys.D7)) {
                PostProcessVignette vignette = Mgr<Scene>.Singleton.PostProcessManager.
                    GetPostProcess(typeof(PostProcessVignette).ToString())
                    as PostProcessVignette;
                if (vignette != null) {
                    vignette.Radius = new Vector2(vignette.Radius.X - 0.01f,
                                                  vignette.Radius.Y);
                }
            }
            if (keyboardState.IsKeyDown(Keys.D8)) {
                PostProcessVignette vignette = Mgr<Scene>.Singleton.PostProcessManager.
                    GetPostProcess(typeof(PostProcessVignette).ToString())
                    as PostProcessVignette;
                if (vignette != null) {
                    vignette.Radius = new Vector2(vignette.Radius.X + 0.01f,
                                                  vignette.Radius.Y);
                }
            }


            oldKeyboardState = keyboardState;  
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement keyboardInput = doc.CreateElement(typeof(KeyboardInput).Name);
            node.AppendChild(keyboardInput);

            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            base.ConfigureFromNode(node, scene, gameObject);
            return;
        }

        public static string GetMenuNames() {
            return "Controller|Keyboard Input";
        }
    }
}
