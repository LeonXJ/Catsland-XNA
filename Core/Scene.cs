using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using System.Xml;
using System.ComponentModel;

/**
 * @file Scene
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    /**
     * @brief scene class, containing gameObject list and other essentials
     * */
    public class Scene : Drawable {
        // name of the scene
        public string _sceneName;

        // the boundary of the scene(in game world)
        public Vector2 _XBound;
        [CategoryAttribute("Scene Size")]
        public Vector2 XBound {
            set {
                _XBound = value;
                UpdateDebugVertex();
            }
            get { return _XBound; }
        }
        public Vector2 _YBound;
        [CategoryAttribute("Scene Size")]
        public Vector2 YBound {
            set {
                _YBound = value;
                UpdateDebugVertex();
            }
            get { return _YBound; }
        }

        public Vector2 _ZBound;
        [CategoryAttribute("Scene Size")]
        public Vector2 ZBound {
            set {
                _ZBound = value;
                UpdateDebugVertex();
            }
            get { return _ZBound; }
        }

        // the angle of view in degree, range [0,90], 0 - side view 90 - top view
        public float _viewAngle;
        [CategoryAttribute("View Angle")]
        public float ViewAngle {
            set { setYAngle(value); }
            get { return _viewAngle; }
        }

        private Camera m_camera;

        // the list of colliders
        public ColliderList _colliderList;
        private PhysicsSystem m_physicsSystem;
        public PhysicsSystem GetPhysicsSystem() {
            return m_physicsSystem;
        }
        // the list of renderable interfaces
        public RenderList _renderList;
        public UIRenderer _uiRenderer;
        // the list of gameObjects
        public GameObjectList _gameObjectList;
        // the list of renderable interfaces which will be rendered only in debug mode
        public DebugDrawableList _debugDrawableList;

        public ShadowSystem m_shadowSystem;
        // the list of selectable rectangles
        public RepeatableList<ISelectable> _selectableList;

        // the vertex of scene debug box
        VertexPositionColor[] m_vertex;
        VertexBuffer m_vertexBuffer;

        // the cos and sin of view angle
        public float _yCos;
        public float _ySin;

        // test
        private PostProcessManager m_postProcessManager;
        public PostProcessManager PostProcessManager {
            get {
                return m_postProcessManager;
            }
        }
        public PostProcessColorAdjustment m_postProcessColorAdjustment;

        [SerialAttribute]
        private PostProcessColorAdjustment m_colorAdjustment;
        [SerialAttribute]
        private PostProcessBloom m_bloom;
        [SerialAttribute]
        private PostProcessVignette m_vignette;
        [SerialAttribute]
        private PostProcessMotionBlur m_motionBlur;

        public Scene() {
        }

        /**
         * @brief set the position of the vertexes of debug box
         * */
        public void UpdateDebugVertex() {
            // debug info: scene bound
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_vertex[0] = m_vertex[4] = new VertexPositionColor(new Vector3(_XBound.X, _YBound.Y * _yCos, 0.0f), Color.White);
                m_vertex[1] = new VertexPositionColor(new Vector3(_XBound.Y, _YBound.Y * _yCos, 0.0f), Color.White);
                m_vertex[2] = new VertexPositionColor(new Vector3(_XBound.Y, _YBound.X * _yCos, 0.0f), Color.White);
                m_vertex[3] = new VertexPositionColor(new Vector3(_XBound.X, _YBound.X * _yCos, 0.0f), Color.White);
            }
        }

        /**
         * @brief set the view angle of the scene
         * 
         * @param the view angle in degree, range [0, 90], 0 - side view 90 - top view
         * 
         * @result
         * */
        public void setYAngle(float angle) {
            _viewAngle = angle;
            _yCos = (float)Math.Cos(angle * MathHelper.Pi / 180.0f);
            _ySin = (float)Math.Sin(angle * MathHelper.Pi / 180.0f);
        }

        /**
         * @brief test if a position is in the scene boundary
         * 
         * @param XYposition
         * @param Zposition
         * 
         * @result in scene?
         * */
        public bool IsInBound(Vector2 XYposition, float Zposition) {
            if (XYposition.X < _XBound.Y && XYposition.X > _XBound.X
                && XYposition.Y < _YBound.Y && XYposition.Y > _YBound.X
                && Zposition < _ZBound.Y && Zposition > _ZBound.X) {
                return true;
            }
            else {
                return false;
            }
        }

        /**
         * @brief get the nearest point in the scene, return original position if in scene
         * 
         * @param XYPosition
         * @param ZPosition
         * 
         * @result the nearest position in the scene
         **/
        public Vector3 GetInBoundPosition(Vector2 XYPosition, float ZPosition) {
            Vector3 result = new Vector3(XYPosition, ZPosition);

            // x
            if (result.X > _XBound.Y) {
                result.X = _XBound.Y;
            }
            else if (result.X < _XBound.X) {
                result.X = _XBound.X;
            }

            // y
            if (result.Y > _YBound.Y) {
                result.Y = _YBound.Y;
            }
            else if (result.Y < _YBound.X) {
                result.Y = _YBound.X;
            }

            // height
            if (result.Z > _ZBound.Y) {
                result.Z = _ZBound.Y;
            }
            else if (result.Z < _ZBound.X) {
                result.Z = _ZBound.X;
            }

            return result;
        }

        /**
         * @brief set this scene to current active scene
         * 
         * we could have more than one scene in memory at a time, but only one active scene 
         * which is being updated and rendered
         * */
        public void ActivateScene() {
            Mgr<Scene>.Singleton = this;
            Mgr<Camera>.Singleton = m_camera;
        }

        /**
         * @brief release the scene
         * */
        public void Unload() {
            // release all dynamic list
            _colliderList.ReleaseAll();
            _renderList.ReleaseAll();
            // TODO: release shadow system
            _gameObjectList.ReleaseAll();
            
            if (Mgr<GameEngine>.Singleton._gameEngineMode ==
                GameEngine.GameEngineMode.MapEditor) {
                _selectableList.ReleaseAll();
            }

        }

        /**
         * @brief draw the debug box of the scene
         * 
         * @param timeLastFrame the time interval
         * */
        public void Draw(int timeLastFrame) {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                UpdateDebugVertex();

                BasicEffect effect = Mgr<DebugTools>.Singleton.DrawEffect;
                effect.View = Mgr<Camera>.Singleton.View;
                effect.Projection = Mgr<Camera>.Singleton.m_projection;
                effect.VertexColorEnabled = false;
                effect.World = Matrix.Identity;
                effect.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                effect.Alpha = 1.0f;

                Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineStrip,
                        m_vertex,
                        0,
                        4);
                }
            }
        }

        public bool SaveScene(string _filename) {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);
            XmlElement scene = doc.CreateElement("Scene");
            doc.AppendChild(scene);

            // scene
            XmlElement basic = doc.CreateElement("SceneBasic");
            scene.AppendChild(basic);
            basic.SetAttribute("viewAngle", "" + _viewAngle);
            //basic.SetAttribute("playerGameObjectName", scenePlayer.Name);

            // Bound
            XmlElement XBound = doc.CreateElement("XBound");
            basic.AppendChild(XBound);
            XBound.SetAttribute("min", "" + _XBound.X);
            XBound.SetAttribute("max", "" + _XBound.Y);

            XmlElement YBound = doc.CreateElement("YBound");
            basic.AppendChild(YBound);
            YBound.SetAttribute("min", "" + _YBound.X);
            YBound.SetAttribute("max", "" + _YBound.Y);

            XmlElement ZBound = doc.CreateElement("ZBound");
            basic.AppendChild(ZBound);
            ZBound.SetAttribute("min", "" + _ZBound.X);
            ZBound.SetAttribute("max", "" + _ZBound.Y);

            // camera
            XmlNode eleCamera = Mgr<Camera>.Singleton.DoSerial(doc);
            scene.AppendChild(eleCamera);

            // postprocess
            XmlNode elePostProcessManager = PostProcessManager.DoSerial(doc);
            scene.AppendChild(elePostProcessManager);

            // gameObjects
            _gameObjectList.SaveToNode(scene, doc);

            doc.Save(_filename);
            return true;
        }

        /**
         * @brief create an empty scene
         * 
         * @param game the game engine
         * 
         * @result an empty scene
         * */
        public static Scene CreateEmptyScene() {
            Scene newScene = new Scene();
            // basic
            newScene.setYAngle(45.0f);
            newScene._XBound = new Vector2(-4.0f, 4.0f);
            newScene._YBound = new Vector2(-0.2f, 0.2f);
            newScene._ZBound = new Vector2(0.0f, 4.0f);

            // working list
            newScene._renderList = new RenderList();
            newScene._colliderList = new ColliderList();
            newScene.m_physicsSystem = new PhysicsSystem();
            newScene.m_shadowSystem = new ShadowSystem();
            newScene._renderList.SetShadowRender(newScene.m_shadowSystem);
            newScene._uiRenderer = new UIRenderer();
            newScene.m_physicsSystem.Initialize();

            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                newScene._debugDrawableList = new DebugDrawableList();
                newScene._debugDrawableList.AddItem(newScene);

                newScene._selectableList = new RepeatableList<ISelectable>();
            }

            // empty list
            newScene._gameObjectList = new GameObjectList();
            newScene.m_postProcessManager = new PostProcessManager();

            return newScene;
        }

        /**
         * @brief create a scene from an XML file
         * 
         * @param node the XML node
         * @param game the game engine
         * 
         * @result scene
         * */
        public static Scene LoadScene(string _filename) // node is the current node
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);
            XmlNode node = doc.SelectSingleNode("Scene");

            Scene newScene = new Scene();

            // working list
            newScene._renderList = new RenderList();
            newScene._colliderList = new ColliderList();
            newScene.m_physicsSystem = new PhysicsSystem();
            newScene.m_shadowSystem = new ShadowSystem();
            newScene._renderList.SetShadowRender(newScene.m_shadowSystem);
            newScene._uiRenderer = new UIRenderer();
            newScene.m_physicsSystem.Initialize();

            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                newScene._debugDrawableList = new DebugDrawableList();
                newScene._debugDrawableList.AddItem(newScene);

                newScene._selectableList = new RepeatableList<ISelectable>();
            }
            // bind to current scene
            //Mgr<Scene>.Singleton = newScene;

            // init scene basic list
            // load and construct scene here
            XmlNode basic = node.SelectSingleNode("SceneBasic");
            XmlElement sceneBasic = (XmlElement)basic;

            newScene.setYAngle(float.Parse(sceneBasic.GetAttribute("viewAngle")));
            String playerGameObjectName = sceneBasic.GetAttribute("playerGameObjectName");

            XmlElement XBound = (XmlElement)sceneBasic.SelectSingleNode("XBound");
            newScene._XBound = new Vector2(float.Parse(XBound.GetAttribute("min")),
                                        float.Parse(XBound.GetAttribute("max")));

            XmlElement YBound = (XmlElement)sceneBasic.SelectSingleNode("YBound");
            newScene._YBound = new Vector2(float.Parse(YBound.GetAttribute("min")),
                                        float.Parse(YBound.GetAttribute("max")));

            XmlElement ZBound = (XmlElement)sceneBasic.SelectSingleNode("ZBound");
            newScene._ZBound = new Vector2(float.Parse(ZBound.GetAttribute("min")),
                                        float.Parse(ZBound.GetAttribute("max")));

            // camera
            XmlNode nodeCamera = node.SelectSingleNode(typeof(Camera).ToString());
            Serialable.BeginSupportingDelayBinding();
            Camera camera = Serialable.DoUnserial((XmlElement)nodeCamera) as Camera;
            Serialable.EndSupportingDelayBinding();
            //Mgr<Camera>.Singleton = camera;
            newScene.m_camera = camera;
            newScene.m_camera.UpdateView();
            newScene.m_camera.UpdateProjection();

            // postprocess manager
            XmlNode nodePostProcessManager =
                node.SelectSingleNode(typeof(PostProcessManager).ToString());
            Serialable.BeginSupportingDelayBinding();
            newScene.m_postProcessManager =
                Serialable.DoUnserial((XmlElement)nodePostProcessManager) as PostProcessManager;
            Serialable.EndSupportingDelayBinding();

            // gameObjects
            XmlNode gameObjects = node.SelectSingleNode("GameObjects");
            newScene._gameObjectList = GameObjectList.LoadFromNode(gameObjects, newScene);

            PostLoadScene(newScene);

            return newScene;
        }

        /**
         * @brief build a scene with code, only for test use
         * 
         * @param the scene object
         * 
         * @result success?
         * */
        public static bool PostLoadScene(Scene scene) {
            PostProcessColorAdjustment colorAdjustment =
                scene.m_postProcessManager.GetPostProcess(typeof(PostProcessColorAdjustment).ToString())
                as PostProcessColorAdjustment;
            PostProcessBloom bloom =
                scene.m_postProcessManager.GetPostProcess(typeof(PostProcessBloom).ToString())
                as PostProcessBloom;
            PostProcessMotionBlur motionBlur =
                 scene.m_postProcessManager.GetPostProcess(typeof(PostProcessMotionBlur).ToString())
                as PostProcessMotionBlur;
            PostProcessVignette vignette =
                 scene.m_postProcessManager.GetPostProcess(typeof(PostProcessVignette).ToString())
                as PostProcessVignette;
            // 
            colorAdjustment.AddDependOn(scene._renderList);
            bloom.AddDependOn(colorAdjustment);
            motionBlur.AddDependOn(bloom);
            vignette.AddDependOn(motionBlur);
            scene.m_postProcessManager.SetEndPostProcess(typeof(PostProcessVignette).ToString());

            //             scene.PostProcessManager.AddPostProcess(typeof(PostProcessColorAdjustment).ToString(),
            //                  scene.m_colorAdjustment);
            //             scene.PostProcessManager.AddPostProcess(typeof(PostProcessBloom).ToString(),
            //                  scene.m_bloom);
            //             scene.PostProcessManager.AddPostProcess(typeof(PostProcessMotionBlur).ToString(),
            //                  scene.m_motionBlur);
            //             scene.PostProcessManager.AddPostProcess(typeof(PostProcessVignette).ToString(),
            //                  scene.m_vignette);
            // TODO: test if the effect files exist
            //             scene.m_motionBlur = new PostProcessMotionBlur();
            //             scene.m_motionBlur.AddDependOn(scene._renderList);
            //             scene.PostProcessManager.AddPostProcess(typeof(PostProcessMotionBlur).ToString(),
            //                 scene.m_motionBlur);
            //             scene.m_colorAdjustment = new PostProcessColorAdjustment();
            //             scene.m_colorAdjustment.AddDependOn(scene.m_motionBlur);
            //             scene.PostProcessManager.AddPostProcess(typeof(PostProcessColorAdjustment).ToString(),
            //                 scene.m_colorAdjustment);
            //             scene.m_vignette = new PostProcessVignette();
            //             scene.m_vignette.AddDependOn(scene.m_colorAdjustment);
            //             scene.PostProcessManager.AddPostProcess(typeof(PostProcessVignette).ToString(),
            //                 scene.m_vignette);
            //             PostProcessHDR hdr = new PostProcessHDR();
            //             hdr.AddDependOn(scene._renderList);
            //             scene.PostProcessManager.AddPostProcess(typeof(PostProcessHDR).ToString(),
            //                 hdr);
            scene.m_bloom = new PostProcessBloom();
            scene.m_bloom.AddDependOn(scene.m_vignette);
            scene.PostProcessManager.AddPostProcess(typeof(PostProcessBloom).ToString(),
                scene.m_bloom);

            return true;
        }

        /**
         * @brief initialize debug box vertex buffer
         * */
        public void InitializeScene() {
            // Scene
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_vertex = new VertexPositionColor[5];
                UpdateDebugVertex();
                m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                    typeof(VertexPositionColor),
                    5,
                    BufferUsage.None);
                m_vertexBuffer.SetData(m_vertex);
            }
        }

        public float GetDepth() {
            return 0;
        }

        public int CompareTo(object obj) {
            return 1;
        }

        public GameObject GetSelectedGameObject(float cameraX, float cameraY) {
            if (_selectableList != null && _selectableList.GetList() != null) {
                GameObject selectedGameObject = null;
                foreach (ISelectable iselectable in _selectableList.GetList()) {
                    if (iselectable.IsSelected(cameraX, cameraY) &&
                        !iselectable.IsLocked() &&
                        (selectedGameObject == null ||
                            iselectable.GetGameObject().GetDepth() > selectedGameObject.GetDepth())) {
                        selectedGameObject = iselectable.GetGameObject();
                    }
                }
                return selectedGameObject;
            }
            return null;
        }

        public bool IsGameObjectSelected(float cameraX, float cameraY, GameObject gameObject) {
            if (gameObject.IsSelected(cameraX, cameraY)) {
                return true;
            }
            ISelectable quadRender = (ISelectable)(gameObject.GetComponent("Catsland.Plugin.BasicPlugin.QuadRender"));
            if (quadRender != null) {
                if (quadRender.IsSelected(cameraX, cameraY)) {
                    return true;
                }
            }
            return false;
        }

        public Vector3 TranslateToFlatCoord(Vector3 position) {
            return new Vector3(position.X, position.Y * _yCos + position.Z * _ySin, 0.0f);
        }
    }
}
