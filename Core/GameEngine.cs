using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;
using System.Reflection;

/**
 * @file the core of CatsEngine
 *
 * the main game loop of the engine
 *
 * @author LeonXie
 */

namespace Catsland.Core {
    /**
     * @brief the CatsEngine Core
     */
    public class GameEngine : Microsoft.Xna.Framework.Game {

        #region Properties

        private GraphicsDeviceManager m_graphics;
        SpriteBatch _spriteBatch;
        // interface to the editor, not null if in MapEditor mode
        IEditor _editor;
        public IEditor Editor {
            get { return _editor; }
        }
        //bool delayReleaseScene = false;
        // just set for UI testing
        DialogBox _dialogBox;

        private CatConsole m_console;
        public CatConsole CatConsole {
            get {
                return m_console;
            }
        }

        // game mode
        public GameEngineMode _gameEngineMode;
        public enum GameEngineMode {
            InGame,
            MapEditor,
        };
        public InEditorMode _gameInEditorMode;
        public enum InEditorMode {
            Editing,
            Playing
        };

        private bool m_getFocus = false;
        private Scene m_sceneToBeLoad = null;

        private readonly CatFloat m_timeScale = new CatFloat(1.0f);
        public float TimeScale {
            set {
                m_timeScale.SetValue(MathHelper.Max(value, 0));
            }
            get {
                return m_timeScale;
            }
        }
        public CatFloat TimeScaleRef {
            get {
                return m_timeScale;
            }
        }

        private Camera m_mainCamera;
        public Camera MainCamera {
            get {
                return m_mainCamera;
            }
        }


        //private UDPDebugger m_updDebugger;
        UDPConsolePanel m_updConsolePanel;

        private bool showEditFrameInEditorGameMode = false;
        public bool ShowEditFrameInEditorGameMode {
            set {
                showEditFrameInEditorGameMode = value;
            }
            get {
                return showEditFrameInEditorGameMode;
            }
        }

        private bool m_passiveMode = false;

        #endregion

        public GameEngine(IEditor editor = null, bool _enableUDPConsole = true,
            bool _passiveMode = false) {

            // Only create essential variables here 
            // debug and console
            m_passiveMode = _passiveMode;
            m_console = new CatConsole();
            if (_enableUDPConsole) {
                m_updConsolePanel = new UDPConsolePanel(m_console);
                m_updConsolePanel.Start();
            }
            // set game mode
            _editor = editor;
            if (editor != null) {
                _gameEngineMode = GameEngineMode.MapEditor;
                _gameInEditorMode = InEditorMode.Editing;
            }
            else {
                _gameEngineMode = GameEngineMode.InGame;
            }
            // graphics device
            InitGraphicsDevice();
        }

        // this function has to be evoke in GameEngine constructor
        private void InitGraphicsDevice() {
            m_graphics = new GraphicsDeviceManager(this);
            if (_gameEngineMode == GameEngineMode.MapEditor) {
                m_graphics.PreparingDeviceSettings +=
                    new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
                System.Windows.Forms.Control.FromHandle((this.Window.Handle)).VisibleChanged +=
                    new EventHandler(GameEngine_VisibleChanged);
            }
            else if (_gameEngineMode == GameEngineMode.InGame) {
                m_graphics.ToggleFullScreen();
            }
            else {  // unknown mode
                System.Windows.Forms.MessageBox.Show("Unknown game engine mode.", "Vital Error");
                Exit();
            }
#if DEBUG
            this.Window.AllowUserResizing = true;
#endif
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        /**
         * @brief initialize the engine before starting game   
         *
         * initialize() will be invoked automatically by XNA
         */
        protected override void Initialize() {
            base.Initialize();                                                  // it will evoke LoadContent()
            Serialable.InitializeSerializeTypeTable();
            InitSingleton();

            if (_gameEngineMode == GameEngineMode.MapEditor) {
                Editor.GameEngineStarted();
            }
            else if (_gameEngineMode == GameEngineMode.InGame
                && !m_passiveMode) {
                MountInGameProject();
            }
        }

        /**
         * @brief load resource of the game
         *
         * automatically invoked by XNA before starting the game
         */
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            CreatePrimeCamera();
            // Snippet of creating dialogBox
            #region
            /*
            _dialogBox = new DialogBox();
            Mgr<DialogBox>.Singleton = _dialogBox;
            _dialogBox.m_enable = false;
            _dialogBox.m_leftTopTex = Content.Load<Texture2D>(@"image/lefttop");
            _dialogBox.m_topTex = Content.Load<Texture2D>(@"image/top");
            _dialogBox.m_insideTex = Content.Load<Texture2D>(@"image/inside");
            _dialogBox.m_leftTex = Content.Load<Texture2D>(@"image/left");
            _dialogBox.m_font = Content.Load<SpriteFont>(@"font/YaheiFont");
            _dialogBox.m_leftTop = new Point(10, 10);
            _dialogBox.m_rightBottom = new Point(400, 200);
             */
            #endregion
        }

        private void CreatePrimeCamera() {
            if (m_mainCamera == null) {
                m_mainCamera = new Camera();
            }
            float viewWidth = 2.0f;
            float viewHeight = Window.ClientBounds.Height * viewWidth / Window.ClientBounds.Width;
            m_mainCamera.SetProjectionMode(Camera.ProjectionMode.Orthographic);
            m_mainCamera.ViewSize = new Vector2(viewWidth, viewHeight);
            m_mainCamera.ClipDistance = new Vector2(0.1f, 100.0f);
            m_mainCamera.TargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }

        private void MountInGameProject() {
            CatProject project = CatProject.OpenProject(AppDomain.CurrentDomain.BaseDirectory + "project.xml", this);
            if (project == null) {
                System.Windows.Forms.MessageBox.Show("Level file project.xml does not exist.", "Vital Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Exit();
            }
            Mgr<CatProject>.Singleton = project;
            Scene scene = Scene.LoadScene(project.GetSceneFileAddress(project.startupSceneName));
            if (scene == null) {
                System.Windows.Forms.MessageBox.Show("Fail to load startup scene: " + project.startupSceneName, "Vital Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Exit();
            }
            scene.InitializeScene();
            scene.ActivateScene();
            Mgr<Scene>.Singleton = scene;
        }

        private void InitSingleton() {
            Mgr<GameEngine>.Singleton = this;
            Mgr<GraphicsDevice>.Singleton = GraphicsDevice;
            // BasicEffect is for temporary use
            Mgr<BasicEffect>.Singleton = new BasicEffect(GraphicsDevice);
            Mgr<BasicEffect>.Singleton.Name = "BasicEffect";
            Mgr<BasicEffect>.Singleton.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            // TODO: remove this singleton
            Mgr<DebugTools>.Singleton = new DebugTools();
            Mgr<DebugTools>.Singleton.DrawEffect = new BasicEffect(GraphicsDevice);
            // Camera
            Mgr<Camera>.Singleton = m_mainCamera;
        }

        /**
         * @brief logical loop
         *
         * Update() will be invoked automatically be XNA
         *
         * @param gameTime the timestamp provided by XNA
         */
        protected override void Update(GameTime gameTime) {
            // asynchronized release current scene. 
            // this is the right point to release running scene
            int timeInMS = gameTime.ElapsedGameTime.Milliseconds;
            int skewedTimeInMS = (int)(m_timeScale * timeInMS);

            UpdateConsoleAndDebug();
            UpdateSwitchScene();
            if (Mgr<CatProject>.Singleton != null &&
                Mgr<Scene>.Singleton != null) {
                if (_gameEngineMode == GameEngineMode.InGame) {
                    InGameUpdateProcess(skewedTimeInMS);
                }
                else if (_gameInEditorMode == InEditorMode.Editing) {
                    EditorEditUpdateProcess(skewedTimeInMS);
                }
                else if (_gameInEditorMode == InEditorMode.Playing) {
                    EditorGameUpdateProcess(skewedTimeInMS);
                }
                else {
                    System.Windows.Forms.MessageBox.Show("Unknown game engine mode.", "Vital Error");
                    Exit();
                }
            }
            UpdateKeyControl();
            #region deprecated

            // 
            //             GameObjectList gameObjectList = null;
            //             ColliderList colliderList = null;
            //             if (Mgr<Scene>.Singleton != null) {
            //                 gameObjectList = Mgr<Scene>.Singleton._gameObjectList;
            //                 colliderList = Mgr<Scene>.Singleton._colliderList;
            //                 if (_gameEngineMode == GameEngineMode.InGame ||
            //                     _gameInEditorMode == InEditorMode.Playing) {
            //                     Mgr<Scene>.Singleton.GetPhysicsSystem()
            //                         .Update(skewedTimeInMS);
            //                 }
            //             }
            //             // add / remove update gameObject
            //             if (gameObjectList != null) {
            //                 gameObjectList.UpdateAdd();
            //                 gameObjectList.UpdateRemove();
            //             }
            //             if (colliderList != null) {
            //                 colliderList.UpdateRemove();
            //             }
            //             if (_gameEngineMode == GameEngineMode.InGame ||
            //                 _gameInEditorMode == InEditorMode.Playing) {
            //                 // Allows the game to exit
            //                 if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //                     this.Exit();
            //                 KeyboardState keyboardState = Keyboard.GetState();
            //                 if (keyboardState.IsKeyDown(Keys.Escape)) {
            //                     this.Exit();
            //                 }
            //                 if (keyboardState.IsKeyDown(Keys.F11)) {
            //                     m_graphics.ToggleFullScreen();
            //                 }
            // 
            //                 // pose camera
            //                 Mgr<Camera>.Singleton.Update(skewedTimeInMS);
            //                 // motion delegator
            //                 if (Mgr<CatProject>.Singleton != null &&
            //                     Mgr<CatProject>.Singleton.MotionDelegator != null) {
            //                     Mgr<CatProject>.Singleton.MotionDelegator.Update(timeInMS);
            //                 }
            //                 // update gameObjects
            //                 if (gameObjectList != null) {
            //                     gameObjectList.Update(skewedTimeInMS);
            //                 }
            //                 // shadow system
            //                 if (Mgr<Scene>.Singleton.m_shadowSystem != null) {
            //                     Mgr<Scene>.Singleton.m_shadowSystem.Update(skewedTimeInMS);
            //                 }
            //                 // sound manager
            //                 if (Mgr<CatProject>.Singleton.m_soundManager != null) {
            //                     Mgr<CatProject>.Singleton.m_soundManager.
            //                         Update(skewedTimeInMS);
            //                 }
            //             }
            //             else if (_gameInEditorMode == InEditorMode.Editing) {
            //                 // pose camera
            //                 Mgr<Camera>.Singleton.EditorUpdate();
            //                 // update gameObjects
            //                 if (gameObjectList != null) {
            //                     gameObjectList.EditorUpdate(skewedTimeInMS);
            //                 }
            //                 // shadow system
            //                 if (Mgr<Scene>.Singleton != null &&
            //                     Mgr<Scene>.Singleton.m_shadowSystem != null) {
            //                     Mgr<Scene>.Singleton.m_shadowSystem.Update(skewedTimeInMS);
            //                 }
            //             }
            #endregion
            base.Update(gameTime);
        }

        private void InGameUpdateProcess(int _skewedTimeInMS) {
            if (Mgr<Scene>.Singleton.GetPhysicsSystem() != null) {
                Mgr<Scene>.Singleton.GetPhysicsSystem().Update(_skewedTimeInMS);
            }
            if (Mgr<Scene>.Singleton._gameObjectList != null) {
                Scene curScene = Mgr<Scene>.Singleton;
                GameObjectList gameObjectList = Mgr<Scene>.Singleton._gameObjectList;
                gameObjectList.UpdateAdd(curScene);
                gameObjectList.UpdateRemove(curScene);
            }
            if (Mgr<Scene>.Singleton._colliderList != null) {
                Mgr<Scene>.Singleton._colliderList.UpdateRemove();
            }

            if (Mgr<Camera>.Singleton != null) {
                Mgr<Camera>.Singleton.Update(_skewedTimeInMS);
            }
            if (Mgr<CatProject>.Singleton.MotionDelegator != null) {
                Mgr<CatProject>.Singleton.MotionDelegator.Update(_skewedTimeInMS);
            }
            if (Mgr<Scene>.Singleton._gameObjectList != null) {
                Mgr<Scene>.Singleton._gameObjectList.Update(_skewedTimeInMS);
            }
            if (Mgr<Scene>.Singleton.m_shadowSystem != null) {
                Mgr<Scene>.Singleton.m_shadowSystem.Update(_skewedTimeInMS);
            }
            if (Mgr<CatProject>.Singleton.SoundManager != null) {
                Mgr<CatProject>.Singleton.SoundManager.Update(_skewedTimeInMS);
            }
        }

        private void EditorEditUpdateProcess(int _skewedTimeInMS) {
            if (Mgr<Scene>.Singleton._gameObjectList != null) {
                Scene curScene = Mgr<Scene>.Singleton;
                GameObjectList gameObjectList = Mgr<Scene>.Singleton._gameObjectList;
                gameObjectList.UpdateAdd(curScene);
                gameObjectList.UpdateRemove(curScene);
            }
            if (Mgr<Scene>.Singleton._colliderList != null) {
                Mgr<Scene>.Singleton._colliderList.UpdateRemove();
            }
            if (Mgr<Camera>.Singleton != null) {
                Mgr<Camera>.Singleton.EditorUpdate(_skewedTimeInMS);
            }
            if (Mgr<Scene>.Singleton._gameObjectList != null) {
                Mgr<Scene>.Singleton._gameObjectList.EditorUpdate(_skewedTimeInMS);
            }
            if (Mgr<Scene>.Singleton.m_shadowSystem != null) {
                Mgr<Scene>.Singleton.m_shadowSystem.Update(_skewedTimeInMS);
            }
            UpdateEditorEditCommend(_skewedTimeInMS);
        }

        private void EditorGameUpdateProcess(int _skewedTimeInMS) {
            InGameUpdateProcess(_skewedTimeInMS);
        }

        private void UpdateConsoleAndDebug() {
            if (m_console != null) {
                m_console.Update();
            }
            if (m_updConsolePanel != null) {
                m_updConsolePanel.Update();
            }
        }

        private void UpdateSwitchScene() {
            // do unload scene here
            if (m_sceneToBeLoad != null) {
                Scene oldScene = Mgr<Scene>.Singleton;
                if (oldScene != null) {
                    oldScene.Unload();
                }
                m_sceneToBeLoad.ActivateScene();
                m_sceneToBeLoad = null;
                if (_editor != null) {
                    _editor.LoadSceneComplete();
                }
            }
        }

        private void UpdateEditorEditCommend(int _skewedTimeInMS) {
            KeyboardState keyboardState = Keyboard.GetState();
            float hor = 0.0f;
            float ver = 0.0f;
            bool pressed = false;
            if (keyboardState.IsKeyDown(Keys.J)) {
                hor -= 1.0f;
                pressed = true;
            }
            if (keyboardState.IsKeyDown(Keys.L)) {
                hor += 1.0f;
                pressed = true;
            }
            if(keyboardState.IsKeyDown(Keys.I)){
                ver += 1.0f;
                pressed = true;
            }
            if (keyboardState.IsKeyDown(Keys.K)) {
                ver -= 1.0f;
                pressed = true;
            }
            if (_editor != null && pressed) {
                _editor.AdjustSelectedGameObjectPoistion(new Vector2(hor, ver) * _skewedTimeInMS);
            }
        }

        private void UpdateKeyControl() {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
                Exit();
            }
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape)) {
                Exit();
            }
            if (keyboardState.IsKeyDown(Keys.F11)) {
                m_graphics.ToggleFullScreen();
            }
        }

        public void DoSwitchScene(string _scenefile) {
            m_sceneToBeLoad = Scene.LoadScene(_scenefile);
            if (m_sceneToBeLoad != null) {
                m_sceneToBeLoad.InitializeScene();
            }
        }

        /**
         * @brief draw thing out
         * 
         * @param gameTime the time interval between adjacent frames 
         * */
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            int timeInMS = gameTime.ElapsedGameTime.Milliseconds;
            if (Mgr<CatProject>.Singleton != null &&
                Mgr<Scene>.Singleton != null) {
                if (_gameEngineMode == GameEngineMode.InGame) {
                    InGameDrawProcess(timeInMS);
                }
                else if (_gameInEditorMode == InEditorMode.Editing) {
                    EditEditDrawProcess(timeInMS);
                }
                else if (_gameInEditorMode == InEditorMode.Playing) {
                    EditorGameDrawProcess(timeInMS);
                }
            }

            // draw UI
            /*
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap,
    DepthStencilState.None, RasterizerState.CullNone);
            _dialogBox.Draw(_spriteBatch);
            _spriteBatch.End();
            */
            if (Mgr<Scene>.Singleton != null && Mgr<Scene>.Singleton._uiRenderer != null) {
                Mgr<Scene>.Singleton._uiRenderer.Draw(gameTime.ElapsedGameTime.Milliseconds);
            }

            base.Draw(gameTime);
        }

        private void InGameDrawProcess(int _timeInMS) {
            DrawGameScene(_timeInMS);
        }

        private void EditEditDrawProcess(int _timeInMS) {
            DrawGameScene(_timeInMS);
            DrawEditor(_timeInMS);
        }

        private void EditorGameDrawProcess(int _timeInMS) {
            DrawGameScene(_timeInMS);
            if (showEditFrameInEditorGameMode) {
                DrawEditor(_timeInMS);
            }
        }

        private void DrawGameScene(int _timeInMS) {
            PostProcess endProcess = null;
            if (Mgr<Scene>.Singleton.PostProcessManager != null) {
                endProcess = Mgr<Scene>.Singleton.PostProcessManager.GetEndProcess();
            }
            if (endProcess != null) {
                endProcess.DoRender(_timeInMS);
            }
            else if (Mgr<Scene>.Singleton._renderList != null) {
                Mgr<Scene>.Singleton._renderList.DoRender(_timeInMS);
            }
        }

        private void DrawEditor(int _timeInMS) {

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            GraphicsDevice.DepthStencilState = dss;

            SamplerState sampleState = new SamplerState();
            sampleState.Filter = TextureFilter.Anisotropic;
            sampleState.AddressU = TextureAddressMode.Clamp;
            sampleState.AddressV = TextureAddressMode.Clamp;
            GraphicsDevice.SamplerStates[0] = sampleState;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            GameObject selected = _editor.GetSelectedGameObject();
            if (selected != null) {
                selected.DrawSelection();
                CatComponent quadRender = selected.
                    GetComponent("Catsland.Plugin.BasicPlugin.QuadRender");
                if (quadRender != null) {
                    ((ISelectable)quadRender).DrawSelection();
                }
            }
            if (Mgr<Scene>.Singleton._debugDrawableList != null) {
                Mgr<Scene>.Singleton._debugDrawableList.Draw(_timeInMS);
            }
        }

        /**
         * @brief release resource here
         * */
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here

        }

        protected override void EndRun() {
            base.EndRun();
            //             if (m_updDebugger != null) {
            //                 m_updDebugger.Stop();
            //             }
            if (m_updConsolePanel != null) {
                m_updConsolePanel.Stop();
            }
        }

        /**
         * @brief window size change handler
         * 
         * for game mode
         * */
        void Window_ClientSizeChanged(object sender, EventArgs e) {
            OnDisplaySizeChanged(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        /**
         * @brief window size change handler
         * 
         * for editor mode
         * */
        public void GameEngine_SizeChanged() {
            if (_editor != null) {
                Point size = _editor.GetRenderAreaSize();
                OnDisplaySizeChanged(size.X, size.Y);
            }
            else {
                Console.Out.WriteLine("No editor found.");
            }
        }

        private void OnDisplaySizeChanged(int _width, int _height) {
            m_graphics.PreferredBackBufferWidth = _width;
            m_graphics.PreferredBackBufferHeight = _height;
            m_graphics.ApplyChanges();
            if (Mgr<Camera>.Singleton != null) {
                Mgr<Camera>.Singleton.SetViewSizeByReservingWidth(_width, _height);
            }
            UpdateRenderBuffers();
        }

        /**
         * @brief editor window prepare handler
         * 
         * @param sender
         * @param e
         * */
        // editor window prepare handler
        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e) {
            // get render size from editor
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = _editor.GetRenderAreaHandle();
            Point size = _editor.GetRenderAreaSize();
            // update render device
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = size.X;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = size.Y;
            UpdateRenderBuffers();
        }

        /**
         * @brief window visible change handler
         * 
         * for editor mode
         * */
        private void GameEngine_VisibleChanged(object sender, EventArgs e) {
            if (System.Windows.Forms.Control.FromHandle((this.Window.Handle)).Visible == true) {
                System.Windows.Forms.Control.FromHandle((this.Window.Handle)).Visible = false;
            }
        }

        private void UpdateRenderBuffers() {
            if(Mgr<Scene>.Singleton != null){
                if (Mgr<Scene>.Singleton.PostProcessManager != null) {
                    Mgr<Scene>.Singleton.PostProcessManager.UpdateBuffers();
                }
                if (Mgr<Scene>.Singleton._renderList != null) {
                    Mgr<Scene>.Singleton._renderList.UpdateBuffer();
                }
                if (Mgr<Scene>.Singleton.m_shadowSystem != null) {
                    Mgr<Scene>.Singleton.m_shadowSystem.UpdateBuffer();
                }
            }
        }

        /**
         * @brief release a scene async
         * */
        //         public void DelayReleaseScene() {
        //             delayReleaseScene = true;
        //         }

        public void GetFocus() {
            m_getFocus = true;
        }

        public void LostFocus() {
            m_getFocus = false;
        }

        public bool IsGetFocus() {
            return m_getFocus;
        }
    }
}
