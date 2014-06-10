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
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        // interface to the editor, not null if in MapEditor mode
        IEditor _editor;
        public IEditor Editor {
            get { return _editor; }
        }
        bool delayReleaseScene = false;
        // just set for UI testing
        DialogBox _dialogBox;
        // game mode
        public GameEngineMode _gameEngineMode;
        public enum GameEngineMode {
            InGame,
            MapEditor
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

        public GameEngine(IEditor editor = null) {
            _graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";

            // set gameEngineMode
            _gameEngineMode = GameEngineMode.InGame;
            if (editor != null) {
                // editor mode
                _editor = editor;
                _gameEngineMode = GameEngineMode.MapEditor;
                _gameInEditorMode = InEditorMode.Editing;

                // set handlers for window size change events
                _graphics.PreparingDeviceSettings +=
                    new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
                System.Windows.Forms.Control.FromHandle((this.Window.Handle)).VisibleChanged +=
                    new EventHandler(GameEngine_VisibleChanged);
            }
            else {
                _graphics.ToggleFullScreen();
            }

            // serial type
            Serialable.InitializeSerializeTypeTable();

            // Load CatComponent classes and TriggerInvoker classes
            //TypeManager typeManager = new TypeManager();
            // change the filepath when publish
            //typeManager.Load_Plugins(@"./plugin");
            // load editor scripts
            //if (_gameEngineMode == GameEngineMode.MapEditor) {
            //    typeManager.Load_EditorScripts(@"./plugin");
            //}
            //Mgr<TypeManager>.Singleton = typeManager;
        }

        /**
         * @brief initialize the engine before starting game
         *
         * initialize() will be invoked automatically by XNA
         */
        protected override void Initialize() {
            base.Initialize();

            

            // disable depth test. draw sprites from back to front
            



            // set window size change handler
            // do not allow change window size in release mode
#if DEBUG
            this.Window.AllowUserResizing = true;
#endif
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        /**
         * @brief load resource of the game
         *
         * automatically invoked by XNA before starting the game
         */
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Init singleton
            Mgr<GameEngine>.Singleton = this;
            Mgr<GraphicsDevice>.Singleton = GraphicsDevice;
            //TODO: should be removed
            Mgr<BasicEffect>.Singleton = new BasicEffect(GraphicsDevice);
            Mgr<BasicEffect>.Singleton.Name = "BasicEffect";
            Mgr<BasicEffect>.Singleton.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Mgr<DebugTools>.Singleton = new DebugTools();
            Mgr<DebugTools>.Singleton.DrawEffect = new BasicEffect(GraphicsDevice);

            // Camera
            // TODO: camera should be loaded from map file
            #region
            Camera camera = new Camera();

            float viewWidth = 2.0f;
            float viewHeight = Window.ClientBounds.Height * viewWidth / Window.ClientBounds.Width;
            camera.SetProjectionMode(Camera.ProjectionMode.Orthographic);
            camera.ViewSize = new Vector2(viewWidth, viewHeight);
            camera.ClipDistance = new Vector2(0.1f, 100.0f);
            camera.TargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
            Mgr<Camera>.Singleton = camera;
            #endregion
            // UI
            // TODO: UI should be loaded from map file
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


            if (_gameEngineMode == GameEngineMode.InGame) {
                CatProject project = CatProject.OpenProject(AppDomain.CurrentDomain.BaseDirectory + "project.xml", this);
                if (project == null) {
                    System.Windows.Forms.MessageBox.Show("Level file project.xml does not exist.", "Crash",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    Exit();
                    return;
                }
                
                //CatProject project = CatProject.OpenProject(@"E:\workspace\catsland_china\test\BirthdayParty\project.xml", this);
                Mgr<CatProject>.Singleton = project;
                Scene scene = Scene.LoadScene(project.GetSceneFileAddress(project.startupSceneName));
                scene.InitializeScene();
                scene.ActivateScene();
            }
            else {
                // editor mode, create an empty scene
                /*
                CatProject project = CatProject.CreateEmptyProject("UntitleProject", System.Environment.CurrentDirectory + "\\", this);
                Editor.SetCurrentProject(project, "UntitleProject");
                Mgr<CatProject>.Singleton = project;
                project.typeManager.Load_EditorScripts(project.projectRoot + "\\" + project.pluginDirectory);
                project.typeManager.Load_Plugins(project.projectRoot + "\\" + project.pluginDirectory);
                Mgr<TypeManager>.Singleton = project.typeManager;

                Scene scene = Scene.LoadScene(project.projectRoot + "\\" + project.sceneDirectory + "\\" + project.currentSceneName + ".xml");
                scene.InitializeScene();
                scene.ActivateScene();
                Editor.BindToScene(scene);
                 */
                Mgr<CatProject>.Singleton = null;
                Editor.GameEngineStarted();
            }

         

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
            
            if (delayReleaseScene) {
                Mgr<Scene>.Singleton.Unload();
                delayReleaseScene = false;
            }

            UpdateSwitchScene();

            GameObjectList gameObjectList = null;
            ColliderList colliderList = null;
            if (Mgr<Scene>.Singleton != null) {
                gameObjectList = Mgr<Scene>.Singleton._gameObjectList;
                colliderList = Mgr<Scene>.Singleton._colliderList;
                if (_gameEngineMode == GameEngineMode.InGame ||
                    _gameInEditorMode == InEditorMode.Playing) {
                    Mgr<Scene>.Singleton.GetPhysicsSystem()
                        .Update(skewedTimeInMS);
                }
            }
            // add / remove update gameObject
            if (gameObjectList != null) {
                gameObjectList.UpdateAdd();
                gameObjectList.UpdateRemove();
            }
            if (colliderList != null) {
                colliderList.UpdateRemove();
            }
            if (_gameEngineMode == GameEngineMode.InGame ||
                _gameInEditorMode == InEditorMode.Playing) {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Escape)) {
                    this.Exit();
                }
                if (keyboardState.IsKeyDown(Keys.F11)) {
                    _graphics.ToggleFullScreen();
                }

                // pose camera
                Mgr<Camera>.Singleton.Update(skewedTimeInMS);
                // motion delegator
                if (Mgr<CatProject>.Singleton != null &&
                    Mgr<CatProject>.Singleton.MotionDelegator != null) {
                    Mgr<CatProject>.Singleton.MotionDelegator.Update(timeInMS);
                }
                // update gameObjects
                if (gameObjectList != null) {
                    gameObjectList.Update(skewedTimeInMS);
                }
                // sound manager
                if (Mgr<CatProject>.Singleton.m_soundManager != null) {
                    Mgr<CatProject>.Singleton.m_soundManager.
                        Update(skewedTimeInMS);
                }

            }
            else if (_gameInEditorMode == InEditorMode.Editing) {
                // pose camera
                Mgr<Camera>.Singleton.EditorUpdate();
                // update gameObjects
                if (gameObjectList != null) {
                    gameObjectList.EditorUpdate(skewedTimeInMS);
                }
            }
            base.Update(gameTime);
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

            // draw render list
            RenderList renderList = null;
            DebugDrawableList debugDrawableList = null;
            //PostProcessColorAdjustment postProcessColorAdjustment = null;
            PostProcess endPostProcess = null;
            if (Mgr<Scene>.Singleton != null) {
                renderList = Mgr<Scene>.Singleton._renderList;
                debugDrawableList = Mgr<Scene>.Singleton._debugDrawableList;
                //postProcessColorAdjustment = Mgr<Scene>.Singleton.m_postProcessColorAdjustment;
                endPostProcess = Mgr<Scene>.Singleton.PostProcessManager.GetEndProcess();
            }

            if (endPostProcess != null) {
                endPostProcess.DoRender(gameTime.ElapsedGameTime.Milliseconds);
            }
            else if (renderList != null) {
                renderList.DoRender(gameTime.ElapsedGameTime.Milliseconds);
            }
            
            // draw debug info
            if (_gameEngineMode == GameEngineMode.MapEditor 
                && _gameInEditorMode == InEditorMode.Editing 
                && debugDrawableList != null) {

                DepthStencilState dss = new DepthStencilState();
                dss.DepthBufferEnable = false;
                GraphicsDevice.DepthStencilState = dss;

                SamplerState sampleState = new SamplerState();
                sampleState.Filter = TextureFilter.Anisotropic;
                sampleState.AddressU = TextureAddressMode.Clamp;
                sampleState.AddressV = TextureAddressMode.Clamp;
                GraphicsDevice.SamplerStates[0] = sampleState;

//                 RasterizerState rasterizerState = new RasterizerState();
//                 rasterizerState.MultiSampleAntiAlias = true;
//                 GraphicsDevice.RasterizerState = rasterizerState;

                BlendState blendState = new BlendState();
                blendState.AlphaSourceBlend = Blend.SourceAlpha;
                blendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
                blendState.ColorBlendFunction = BlendFunction.Add;
                GraphicsDevice.BlendState = blendState;

                GameObject selected = _editor.GetSelectedGameObject();
                if(selected != null){
                    selected.DrawSelection();
                    CatComponent quadRender = selected.
                        GetComponent("Catsland.Plugin.BasicPlugin.QuadRender");
                    if (quadRender != null) {
                        ((ISelectable)quadRender).DrawSelection();
                    }
                }

                debugDrawableList.Draw(gameTime.ElapsedGameTime.Milliseconds);
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

        /**
         * @brief release resource here
         * */
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
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
            if (Mgr<Scene>.Singleton != null) {
                Mgr<Scene>.Singleton.PostProcessManager.UpdateBuffers();
            }
           
        }

        /**
         * @brief window size change handler
         * 
         * for editor mode
         * */
        public void GameEngine_SizeChanged() {
            if (_editor != null) {
                // get render size from editor
                Point size = _editor.GetRenderAreaSize();
                _graphics.PreferredBackBufferWidth = size.X;
                _graphics.PreferredBackBufferHeight = size.Y;
                _graphics.ApplyChanges();
                // size of view coordinates
                float viewWidth = 2.0f;
                float viewHeight = size.Y * viewWidth / size.X;
                // set camera matrix
                if (Mgr<Camera>.Singleton != null) {
                    Mgr<Camera>.Singleton.ViewSize = new Vector2(viewWidth, viewHeight);  
                }
                Mgr<Scene>.Singleton.PostProcessManager.UpdateBuffers();
            }
            else {
                // TODO: replace with error log system
                Console.Out.WriteLine("No editor found.");
            }
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

        /**
         * @brief window size change handler
         * 
         * for game mode
         * */
        void Window_ClientSizeChanged(object sender, EventArgs e) {
            float viewWidth = 2.0f;
            float viewHeight = Window.ClientBounds.Height * viewWidth / Window.ClientBounds.Width;
            Mgr<Camera>.Singleton.ViewSize = new Vector2(viewWidth, viewHeight);
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            Mgr<Scene>.Singleton.PostProcessManager.UpdateBuffers();
        }

        /**
         * @brief release a scene async
         * */
        public void DelayReleaseScene() {
            delayReleaseScene = true;
        }

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
