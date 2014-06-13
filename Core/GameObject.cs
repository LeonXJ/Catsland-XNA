using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Xml;
using System.ComponentModel;

using System.Reflection;
using System.Collections;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    /**
     * @brief GameObject
     * 
     * the CatComponents can be added to GameObject
     * */
    public class GameObject : Serialable, Drawable, ISelectable {
        
        #region Properties

        [SerialAttribute]
        private string m_name = "UntitledGameObject";    // name of gameObject, not need to be unique
        [CategoryAttribute("Basic")]
        public string Name {
            set {
                string oldName = m_name;
                m_name = value;
                // update name dictionary
                Mgr<Scene>.Singleton._gameObjectList.Rename(oldName, this);
                // update editor
                if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                    Mgr<GameEngine>.Singleton.Editor.GameObjectNameChanged();
                }
            }
            get { return m_name; }
        }
        
        #region Transfrom

        // coordinate system here
        //    y
        //    |
        //    |
        //    |_______x
        //   /
        //  /
        // z

        // can only be modify by set function, which only change the value, not reference
        [SerialAttribute]
        private readonly CatVector3 m_position;
        [CategoryAttribute("Location")]
        public Vector3 Position {
            set { m_position.SetValue(value); }
            get { return m_position; }
        }
        public CatVector3 PositionRef {
            get { return m_position; }
        }
        public Vector3 AbsPosition {
            get {
                Vector4 positionV4 = Vector4.Transform(Vector4.UnitW, m_absTransform);
                positionV4 /= positionV4.W;
                return new Vector3(positionV4.X, positionV4.Y, positionV4.Z);
            }
        }               // for convenience, calculate from transform

        [SerialAttribute]
        private readonly CatQuaternion m_rotation;
        [CategoryAttribute("Location")]
        public Vector3 Rotation {
            set {
                m_rotation.SetValue(Quaternion.CreateFromYawPitchRoll(
                            MathHelper.ToRadians(value.Y), 
                            MathHelper.ToRadians(value.X), 
                            MathHelper.ToRadians(value.Z)
                            ));
            }
            get {
                return CatQuaternion.QuaternionToEulerDegreeVector3(m_rotation);
            }
        }
        public CatQuaternion RotationRef {
            get { return m_rotation; }
        }
        public Vector3 AbsRotation {
            get {
                return CatMath.MatrixToEulerAngleVector3(AbsTransform);
            }
        }

        [SerialAttribute]
        private readonly CatVector3 m_scale;
        [CategoryAttribute("Location")]
        public Vector3 Scale {
            set {
                m_scale.SetValue(value);
            }
            get {
                return m_scale.GetValue();
            }
        }
        public CatVector3 ScaleRef {
            get { return m_scale; }
        }
        
        private CatMatrix m_absTransform;
        public Matrix AbsTransform {
            get { return m_absTransform.value; }
        }



        #region Old Transform
        
        public Vector2 PositionOld {
            set {
                m_position.X = value.X;
                m_position.Z = value.Y;
            }
            get {
                return new Vector2(m_position.GetValue().X, m_position.GetValue().Z);
            }
        }
        public float HeightOld {
            set {
                m_position.Y = value;
            }
            get {
                return m_position.Y;
            }
        }

        public Vector2 AbsPositionOld {
            get{
                return new Vector2(AbsPosition.X, AbsPosition.Z);
            }
        }

        public float AbsHeight {
            get {
                Vector4 positionV4 = Vector4.Transform(Vector4.UnitW, m_absTransform);
                return positionV4.Y / positionV4.W;
            }
        }

        #endregion

        #endregion

        [SerialAttribute]
        private Dictionary<string, CatComponent> m_components;

        // debug box
        float markHalfRadis = 0.05f;
        static private VertexPositionColor[] m_vertex;
        static private VertexBuffer m_vertexBuffer;

        // selection box
        private float selectBoxHalfWidth = 0.05f;
        static private VertexPositionColor[] m_selectionVertex;
        static private VertexBuffer _selectionVertexBuffer;
        
        // parent and children
        [SerialAttribute(SerialAttribute.AttributePolicy.PolicyReference,
                         SerialAttribute.AttributePolicy.PolicyNone)]
        private GameObject m_parent;
        [EditorAttribute(typeof(PropertyGridGameObjectSelector),
                         typeof(System.Drawing.Design.UITypeEditor))]
        public GameObject Parent {
            get {
                return m_parent;
            }
            set {
                AttachToGameObject(value);
            }
        }

        private List<GameObject> m_children;
        public List<GameObject> Children {
            get { return m_children; }
        }

        private bool m_isLocked = false;
        public bool Locked {
            set {
                m_isLocked = value;
            }
            get {
                return m_isLocked;
            }
        }

        // parent of the gameObject
        // TODO: deprecate
        string delayBindingParent;

        // editor use
        public bool isExpend = false;

        #endregion

        public GameObject() {
            // assign a random id
            GUID = Guid.NewGuid().ToString();

            m_position = new CatVector3();
            m_rotation = new CatQuaternion();
            m_scale = new CatVector3(Vector3.One);
            m_absTransform = new CatMatrix(Matrix.Identity);

            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                // debug vertex
                if (m_vertex == null) {
                    m_vertex = new VertexPositionColor[4];
                    m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton, typeof(VertexPositionColor),
                        4, BufferUsage.None);
                }
                m_vertex[0] = new VertexPositionColor(new Vector3(-markHalfRadis, 0.0f, 0.0f), Color.White);
                m_vertex[1] = new VertexPositionColor(new Vector3(markHalfRadis, 0.0f, 0.0f), Color.White);
                m_vertex[2] = new VertexPositionColor(new Vector3(0.0f, markHalfRadis, 0.0f), Color.White);
                m_vertex[3] = new VertexPositionColor(new Vector3(0.0f, -markHalfRadis, 0.0f), Color.White);
                m_vertexBuffer.SetData<VertexPositionColor>(m_vertex);
                
                // selection vertex
                if (m_selectionVertex == null) {
                    m_selectionVertex = new VertexPositionColor[4];
                    m_selectionVertex[0] = new VertexPositionColor(new Vector3(-selectBoxHalfWidth, -selectBoxHalfWidth, 0.0f), Color.Blue);
                    m_selectionVertex[1] = new VertexPositionColor(new Vector3(-selectBoxHalfWidth, selectBoxHalfWidth, 0.0f), Color.Blue);
                    m_selectionVertex[2] = new VertexPositionColor(new Vector3(selectBoxHalfWidth, -selectBoxHalfWidth, 0.0f), Color.Blue);
                    m_selectionVertex[3] = new VertexPositionColor(new Vector3(selectBoxHalfWidth, selectBoxHalfWidth, 0.0f), Color.Blue);

                    _selectionVertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton, typeof(VertexPositionColor),
                        4, BufferUsage.None);
                    _selectionVertexBuffer.SetData<VertexPositionColor>(m_selectionVertex);
                }
            }
        }

        /**
         * @brief Bind the gameObject to the scene
         * 
         * @param scene the scene
         * */
        public void BindToScene(Scene scene) {
            // bind to gameObject list
            scene._gameObjectList.SimplyAddReference(this);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                // add to debug drawable
                if (scene != null) {
                    scene._debugDrawableList.AddItem(this);
                }
                // add to selectable list
                if (scene != null) {
                    scene._selectableList.AddItem(this);
                }
            }
            // initialize all components
            if (m_components != null) {
                foreach (KeyValuePair<string, CatComponent> key_value in m_components) {
                    key_value.Value.BindToScene(scene);
                }
            }

            // children
            if (m_children != null) {
                foreach (GameObject child in m_children) {
                    child.BindToScene(scene);
                }
            }
        }

        /**
         * @brief initialize gameObject and its components
         * 
         * should be called after binding to scene
         * 
         * @param scene the scene
         * 
         * @return success?
         * */
        public bool Initialize(Scene scene) {
            // initialize all components
            if (m_components != null) {
                foreach (KeyValuePair<string, CatComponent> key_value in m_components) {
                    key_value.Value.Initialize(scene);
                }
            }

            // initialize children
            if (m_children != null) {
                foreach (GameObject child in m_children) {
                    child.Initialize(scene);
                }
            }
            return true;
        }

        /**
         * @brief update every component in game mode
         * 
         * @param lastTimeFrame the time interval since last frame
         * */
        public void Update(int lastTimeFrame) {
            // update itself
            // update components
            if (m_components != null) {
                foreach (KeyValuePair<string, CatComponent> key_value in m_components) {
                    key_value.Value.Update(lastTimeFrame);
                }
            }

            // calculate absolute position         
            if (m_parent != null) {
                m_absTransform.SetValue(Matrix.CreateFromQuaternion(m_rotation) * Matrix.CreateTranslation(Position)
                    * m_parent.AbsTransform);
            }
            else {
                m_absTransform.SetValue(Matrix.CreateFromQuaternion(m_rotation) * Matrix.CreateTranslation(Position));
            }
            // update children
            if (m_children != null) {
                foreach (GameObject child in m_children) {
                    child.Update(lastTimeFrame);
                }
            }
        }

        /**
         * @brief update every component in editor mode
         * 
         * @param lastTimeFrame the time interval since last frame
         * */
        public void EditorUpdate(int lastTimeFrame) {
            // update itself 
            // update components
            if (m_components != null) {
                foreach (KeyValuePair<string, CatComponent> key_value in m_components) {
                    key_value.Value.EditorUpdate(lastTimeFrame);
                }
            }
            if (m_parent != null) {
                m_absTransform.SetValue(Matrix.CreateScale(m_scale) * 
                    Matrix.CreateFromQuaternion(m_rotation) 
                    * Matrix.CreateTranslation(Position) 
                    * m_parent.AbsTransform);
            }
            else {
                m_absTransform.SetValue(Matrix.CreateScale(m_scale) 
                    * Matrix.CreateFromQuaternion(m_rotation) 
                    * Matrix.CreateTranslation(Position));
            }

            // update children
            if (m_children != null) {
                foreach (GameObject child in m_children) {
                    child.EditorUpdate(lastTimeFrame);
                }
            }
        }

        /**
         * @brief draw the debug box
         * 
         * @param timeLastFrame
         * */
        void Drawable.Draw(int timeLastFrame) {
            // debug information
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                BasicEffect effect = Mgr<DebugTools>.Singleton.DrawEffect;
                effect.View = Mgr<Camera>.Singleton.View;
                effect.Projection = Mgr<Camera>.Singleton.m_projection;
                effect.VertexColorEnabled = false;
                effect.Alpha = 1.0f;
                // new position
                effect.World = AbsTransform;
                    //Matrix.CreateFromQuaternion(AbsRotation) * Matrix.CreateTranslation(m_absPosition);
                
                effect.DiffuseColor = new Vector3(0.0f, 0.8f, 0.0f);
                Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                         PrimitiveType.LineList,
                         m_vertex,
                         0,
                         2);
                }
                // new position
                effect.World = AbsTransform;
                effect.DiffuseColor = new Vector3(0.8f, 0.0f, 0.0f);

                Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineList,
                        m_vertex,
                        0,
                        2);
                }
                
            }
        }

        public override string ToString() {
            return Name;
        }

        /**
         * @brief hard copy this gameObject and its components
         * */
        public GameObject CloneGameObject() {
            GameObject gameObject = new GameObject();
            gameObject.Name = new String(m_name.ToCharArray());
            // guid should not be cloned

            // new position
            gameObject.Position = ((CatVector3)(m_position.ParameterClone()));
            // end of new position

            // children
            if (m_children != null) {
                gameObject.m_children = new List<GameObject>();
                foreach (GameObject child in m_children) {
                    GameObject newGameObject = child.CloneGameObject();
                    newGameObject.AttachToGameObject(gameObject);
                }
            }

            // Components
//             if (m_components != null) {
//                 foreach (KeyValuePair<string, CatComponent> keyValue in m_components) {
//                     gameObject.AddComponent(keyValue.Key, keyValue.Value.CloneComponent(gameObject));
//                 }
//             }
            
            return gameObject;
        }


        public override Type GetThisType() {
            return GetType();
        }
        

//         public bool SaveToNode(XmlNode node, XmlDocument doc, Queue<GameObject> toBeSave = null) {
//             TestSave(node, doc);
//             return true;
//         }

        public static GameObject LoadFromNode(XmlNode node, Scene scene=null) {
            XmlElement gameObject = (XmlElement)node;

            // name and guid
            GameObject newGameObject = new GameObject();
            String name = gameObject.GetAttribute("name");
            String guid = gameObject.GetAttribute("guid");
            string parent_guid = gameObject.GetAttribute("parent");

            newGameObject.m_name = name;
            newGameObject.GUID = guid;
            if (parent_guid != "") {
                newGameObject.delayBindingParent = parent_guid;
            }
            // location
            XmlElement position = (XmlElement)gameObject.SelectSingleNode("Position");
            Vector2 postionXY = new Vector2(float.Parse(position.GetAttribute("X")),
                                        float.Parse(position.GetAttribute("Y")));
            float positionHeight = float.Parse(position.GetAttribute("Z"));
            //newGameObject.Position = postionXY;
            //newGameObject.Height = positionHeight;

            // just test, new position
            newGameObject.Position = (new Vector3(postionXY.X, positionHeight, postionXY.Y));
            // end of new position

            // components
            XmlNode components = gameObject.SelectSingleNode("Components");
            foreach (XmlNode component_node in components.ChildNodes) {
                string component_name = component_node.Name;
                CatComponent component = CatComponent.LoadFromNode((XmlElement)component_node, scene, newGameObject);
                newGameObject.AddComponent(component.GetType().Name, component);
            }
            return newGameObject;
        }

        /**
         * @brief destroy this gameObject and its components
         * */
        public void Destroy(Scene scene) {
            // remove children
            if (m_children != null) {
                // make a copy of children for iteration
                GameObject[] tempChildren = m_children.ToArray();
                foreach (GameObject child in tempChildren) {
                    child.Destroy(scene);
                }
            }
            // detach from parent
            DetachFromParent();
            // destroy itself
            scene._gameObjectList.SimplyRemoveReference(this);
            if (m_components != null) {
                foreach (KeyValuePair<string, CatComponent> keyValue in m_components) {
                    keyValue.Value.Destroy();
                }
            }

            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                Mgr<Scene>.Singleton._debugDrawableList.RemoveItem(this);
                Mgr<Scene>.Singleton._selectableList.RemoveItem(this);
            }
        }

        public float GetDepth() {
            // TODO: maybe set Height as a factor is more suitable?
            return AbsPosition.Z;
            //return absPosition.Y;
        }


        /**
         * @brief bind children after loading from file
         * */
        public void ApplyDelayBindingParent(GameObjectList gameObjectList, 
            Dictionary<string, GameObject> tempList) {
            if (delayBindingParent != null) {
                AttachToGameObject(tempList[delayBindingParent]);
                delayBindingParent = null;
            }
        }

        /**
         * @brief used to sort gameObject to perform Painter Algorithm
         * 
         * @param obj compare to other gameObject
         * 
         * @result -1 deeper 0 equal 1 shallow
         * */
        public int CompareTo(object obj) {
            float otherDepth = ((Drawable)obj).GetDepth();
            float thisDepth = GetDepth();
            if (otherDepth > thisDepth) {
                return 1;
            }
            else if (otherDepth < thisDepth) {
                return -1;
            }
            return 0;
        }

        
        /**
         * @brief Detach child from list
         * 
         * @param child the child it want to detach
         * */
        void DetachChild(GameObject child) {
            if (m_children != null) {
                m_children.Remove(child);
            }
        }

        /**
         * @brief detach from parent
         * */
        void DetachFromParent() {
            if (m_parent == null) {
                return;
            }
            // detach from parent
            m_parent.DetachChild(this);
            // change itself
            m_parent = null;
        }

        /**
         * @brief attach to gameObject to be its child
         * 
         * @param _parent the new parent
         * 
         * @result
         * */
        public void AttachToGameObject(GameObject _parent) {
            // avoid to attach to itself
            if (_parent == this) {
                return;
            }
            if (m_parent != null) {
                // detach firstly
                DetachFromParent();
            }
            m_parent = _parent;
            if (m_parent != null) {
                m_parent.AttachChild(this);  
            }
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor
                && Mgr<Scene>.Singleton != null) {
                Mgr<GameEngine>.Singleton.Editor.UpdateGameObjectList(Mgr<Scene>.Singleton._gameObjectList);
            }
            
        }

        /**
         * @brief add child to children
         * 
         * @param child the child gameObject
         * */
        void AttachChild(GameObject child) {
            if (m_children == null) {
                m_children = new List<GameObject>();
            }
            m_children.Add(child);
        }

        public void PostConfigure(Scene scene, Dictionary<string, GameObject> tempList) {
            if (m_components != null) {
                foreach (KeyValuePair<string, CatComponent> keyValue in m_components) {
                    keyValue.Value.PostConfiguration(scene, tempList);
                }
            }
        }

        public List<GameObject> GetGameObjectsByNameFromChildren(string name) {
            if (m_children != null) {
                List<GameObject> result = new List<GameObject>();
                foreach (GameObject gameObject in m_children) {
                    if (gameObject.Name == name) {
                        result.Add(gameObject);
                    }
                }
                return result;
            }
            return null;
        }

        public void DrawSelection() {
            // debug information
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor
                 && m_selectionVertex != null) {
                BasicEffect effect = Mgr<DebugTools>.Singleton.DrawEffect;
                
                effect.View = Mgr<Camera>.Singleton.View;
                effect.Projection = Mgr<Camera>.Singleton.m_projection;
                effect.VertexColorEnabled = false;
                effect.Alpha = 0.4f;
                effect.TextureEnabled = false;
//                 effect.World = Matrix.CreateTranslation(
//                     AbsPosition.X, AbsPosition.Y * Mgr<Scene>.Singleton._yCos + AbsHeight * Mgr<Scene>.Singleton._ySin, 0.0f);
                effect.DiffuseColor = new Vector3(0.3f, 0.0f, 0.0f);
                // new position
                effect.World = m_absTransform; // Matrix.CreateTranslation(m_absPosition.GetValue());

                Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(_selectionVertexBuffer);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.TriangleStrip,
                        m_selectionVertex,
                        0,
                        2);
                }
            }
        }

        public bool IsSelected(float cameraX, float cameraY) {
            // 1) transform to world coordinate, resulting in a ray.
            // 2) test collision between ray and triangle
            Camera camera = Mgr<Camera>.Singleton;
            Vector3 nearPoint = camera.CameraToWorld(new Vector3(cameraX, cameraY, 0.0f));
            Vector3 farPoint = camera.CameraToWorld(new Vector3(cameraX, cameraY, 0.9f));
            Vector3 direction = farPoint - nearPoint;

            Vector4 p0 = Vector4.Transform(new Vector4(-selectBoxHalfWidth, selectBoxHalfWidth, 0.0f, 1.0f),
                            m_absTransform.value);
            Vector4 p1 = Vector4.Transform(new Vector4(-selectBoxHalfWidth, -selectBoxHalfWidth, 0.0f, 1.0f),
                            m_absTransform.value);
            Vector4 p2 = Vector4.Transform(new Vector4(selectBoxHalfWidth, -selectBoxHalfWidth, 0.0f, 1.0f),
                            m_absTransform.value);
            Vector4 p3 = Vector4.Transform(new Vector4(selectBoxHalfWidth, selectBoxHalfWidth, 0.0f, 1.0f),
                            m_absTransform.value);

            Vector3 p0v3= new Vector3(p0.X, p0.Y, p0.Z);
            Vector3 p1v3 = new Vector3(p1.X, p1.Y, p1.Z);
            Vector3 p2v3 = new Vector3(p2.X, p2.Y, p2.Z);
            Vector3 p3v3 = new Vector3(p3.X, p3.Y, p3.Z);

            float u, v, t;
            if(CatMath.IntersectTriangle(nearPoint, direction, p0v3, p1v3, p2v3, out t, out u, out v) == true){
                return true;
            }
            if(CatMath.IntersectTriangle(nearPoint, direction, p2v3, p1v3, p3v3, out t, out u, out v) == true){
                return true;
            }
            return false;
        }

        public GameObject GetGameObject() {
            return this;
        }

        public override void PostDelayBinding() {
            if (m_parent != null) {
                m_parent.AttachChild(this);
            }
        }

        protected override void PostUnserial(XmlNode _node) {
            // add reference to this for components
            if (m_components != null) {
                foreach (KeyValuePair<string, CatComponent> keyValue in m_components) {
                    keyValue.Value.m_gameObject = this;
                }
            }
        }

        protected override void PostClone(Serialable _object) {
            // clone children
            GameObject prototype = _object as GameObject;
            if (prototype.Children != null) {
                foreach (GameObject child in prototype.Children) {
                    GameObject cloneChild = child.DoClone() as GameObject;
                    cloneChild.m_parent = this;
                }
            }
            // set component's reference
            if (m_components != null) {
                foreach (KeyValuePair<string, CatComponent> keyValue in m_components) {
                    keyValue.Value.m_gameObject = this;
                }
            }
        }

        // Only update the absPositions along the path from the root to this
        // gameobject
        public void ForceUpdateAbsTransformation() {
            if (m_parent != null) {
                m_parent.ForceUpdateAbsTransformation();
                m_absTransform.SetValue(Matrix.CreateFromQuaternion(m_rotation) * Matrix.CreateTranslation(Position)
                    * m_parent.AbsTransform);
            }
            else {
                m_absTransform.SetValue(Matrix.CreateFromQuaternion(m_rotation) * Matrix.CreateTranslation(Position));
            }
        }

        public bool IsLocked() {
            return Locked;
        }

        /**
         * the following functions are about components.
         * consider to replace them with uniqueList
         * */
        #region
        /**
         * @brief add components
         * 
         * @param component_name name of the component
         * @param component the component
         * */
        public void AddComponent(string component_name, CatComponent component) {
            if (m_components == null) {
                m_components = new Dictionary<string, CatComponent>();
            }
            m_components.Add(component_name, component);
            component.m_gameObject = this;
        }

        public Dictionary<string, CatComponent> GetComponentList() {
            return m_components;
        }

        public CatComponent GetComponent(string component_name) {
            if (m_components == null) {
                return null;
            }
            else {
                if (m_components.ContainsKey(component_name)) {
                    return m_components[component_name];
                }
                return null;
            }
        }

        public bool RemoveComponent(string component_name) {
            if (m_components == null) {
                return false;
            }
            if (m_components.ContainsKey(component_name)) {
                CatComponent catComponent = m_components[component_name];
                m_components.Remove(component_name);
                catComponent.Destroy();
                return true;
            }
            return false;
        }
        #endregion
    }
}
