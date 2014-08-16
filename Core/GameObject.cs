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
                Mgr<Scene>.Singleton._gameObjectList.Rename(oldName, this);
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
        private readonly CatVector3 m_position = new CatVector3(Vector3.Zero);
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
        }              

        [SerialAttribute]
        private readonly CatQuaternion m_rotation = new CatQuaternion(Quaternion.Identity);
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
        private readonly CatVector3 m_scale = new CatVector3(Vector3.One);
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
        public Vector3 AbsScale {
            get {
                return CatMath.GetScale(m_absTransform);
            }
        }
        
        private CatMatrix m_absTransform = new CatMatrix(Matrix.Identity);
        public Matrix AbsTransform {
            get { return m_absTransform.value; }
        }

        #region Old Transform
        [Obsolete]
        public Vector2 PositionOld {
            set {
                m_position.X = value.X;
                m_position.Z = value.Y;
            }
            get {
                return new Vector2(m_position.GetValue().X, m_position.GetValue().Z);
            }
        }
        [Obsolete]
        public float HeightOld {
            set {
                m_position.Y = value;
            }
            get {
                return m_position.Y;
            }
        }
        [Obsolete]
        public Vector2 AbsPositionOld {
            get{
                return new Vector2(AbsPosition.X, AbsPosition.Z);
            }
        }
        [Obsolete]
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

        // child bind to parent in serializing
        private List<GameObject> m_children;
        public List<GameObject> Children {
            get { return m_children; }
        }

        // debug box
        // TODO: replace with debug box
        float markHalfRadis = 0.05f;
        static private VertexPositionColor[] m_vertex;
        static private VertexBuffer m_vertexBuffer;

        // selection box
        private float selectBoxHalfWidth = 0.05f;
        static private VertexPositionColor[] m_selectionVertex;
        static private VertexBuffer _selectionVertexBuffer;
        
        private bool m_isLocked = false;
        public bool Locked {
            set {
                m_isLocked = value;
            }
            get {
                return m_isLocked;
            }
        }
        // editor use
        public bool isExpend = false;

        #endregion

        public GameObject() {
            GUID = Guid.NewGuid().ToString();
            CheckAndUpdateEditorMembers();
        }

        private void CheckAndUpdateEditorMembers() {
            // #TODO: replace this with debug box
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
         * @brief detach from parent
         * */
        public void DetachFromParent() {
            if (m_parent == null) {
                return;
            }
            m_parent.DetachChild(this);
            m_parent = null;
        }

        /**
         * @brief attach/detach to/from parent
         * 
         * @param _parent the new parent. use null for detach
         * 
         * @result
         * */
        public void AttachToGameObject(GameObject _parent) {
            // avoid to attach to itself
            if (_parent == this) {
                return;
            }
            if (m_parent != null) {
                DetachFromParent();
            }
            m_parent = _parent;
            if (m_parent != null) {
                m_parent.AddChild(this);
            }
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor
                && Mgr<Scene>.Singleton != null) {
                Mgr<GameEngine>.Singleton.Editor.UpdateGameObjectList(Mgr<Scene>.Singleton._gameObjectList);
            }
        }

        /**
         * @brief [Called by GameObjectList only] Add the gameObject and its children
         *  to GameObjectList
         */
        public void AddGameObjectTreeToGameObjectList(GameObjectList _gameObjectList) {
            _gameObjectList.AddSingleGameObject(this);
            if (m_children != null) {
                foreach (GameObject child in m_children) {
                    child.AddGameObjectTreeToGameObjectList(_gameObjectList);
                }
            }
        }

        /**
         * @brief [Called by GameObjectList only] Bind the gameObject to the scene
         * 
         * @param scene the scene
         * */
        public void BindToScene(Scene scene) {
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
            // bind all components
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
         * @brief [Called by GameObjectList only] initialize gameObject and its components
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
         * @brief [Called by GameObjectList only] recursively remove self from gameObjectList
         */ 
        public void RemoveGameObjectTreeFromGameObjectList(GameObjectList _gameObjectList) {
            if (m_children != null) {
                GameObject[] tempChildren = m_children.ToArray();
                foreach (GameObject child in tempChildren){
                    child.RemoveGameObjectTreeFromGameObjectList(_gameObjectList);
                }
            }
            DetachFromParent(); 
            _gameObjectList.RemoveSingleGameObject(this);
        }

        /**
         * @brief update every component in game mode
         * 
         * @param lastTimeFrame the time interval since last frame
         * */
        public void Update(int lastTimeFrame, GameObjectList _gameObjectList) {
            if (!ShouldBeUpdated(_gameObjectList)) {
                return;
            }
            if (m_components != null) {
                foreach (KeyValuePair<string, CatComponent> key_value in m_components) {
                    key_value.Value.Update(lastTimeFrame);
                }
            }
            UpdateAbsTranformation(lastTimeFrame);
            if (m_children != null) {
                foreach (GameObject child in m_children) {
                    child.Update(lastTimeFrame, _gameObjectList);
                }
            }
        }

        /**
         * @brief update abs transformation
         **/
        private void UpdateAbsTranformation(int _timeLastFrameInMS) {
            if (m_parent != null) {
                m_absTransform.SetValue(Matrix.CreateFromQuaternion(m_rotation) * Matrix.CreateTranslation(Position)
                    * m_parent.AbsTransform);
            }
            else {
                m_absTransform.SetValue(Matrix.CreateFromQuaternion(m_rotation) * Matrix.CreateTranslation(Position));
            }
        }

        /**
         * @brief decide whether this object should be updated
         **/ 
        private bool ShouldBeUpdated(GameObjectList _gameObjectList) {
            // Don't update the gameObject if it is not in GameObjectList
            // Case: 1. GameObject a.Update() -> AddGameObject(b), b.AttachToGameObject(c)
            //       2. c.Update() -> b.Update()
            // Here, b has not been added to contentList (not been BindToScene and Initialize yet) 
            if (!_gameObjectList.ContainKey(GUID)) {
                return false;
            }
            return true;
        }

        /**
         * @brief update every component in editor mode
         * 
         * @param lastTimeFrame the time interval since last frame
         * */
        public void EditorUpdate(int lastTimeFrame, GameObjectList _gameObjectList) {
            // update itself 
            if (!ShouldBeUpdated(_gameObjectList)) {
                return;
            }
            // update components
            if (m_components != null) {
                foreach (KeyValuePair<string, CatComponent> key_value in m_components) {
                    key_value.Value.EditorUpdate(lastTimeFrame);
                }
            }
            UpdateAbsTranformation(lastTimeFrame);
            if (m_children != null) {
                foreach (GameObject child in m_children) {
                    child.EditorUpdate(lastTimeFrame, _gameObjectList);
                }
            }
        }

        /**
         * @brief draw the debug box
         * 
         * @param timeLastFrame
         * */
        void Drawable.Draw(int timeLastFrame) {
            // TODO: replace this with debug
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

        public override Type GetThisType() {
            return GetType();
        }
             
        /**
         * @brief [Called by GameObjectList] recursively destroy this gameObject and its children
         * */
        public void Destroy(Scene scene) {
            // destroy children
            if (m_children != null) {
                GameObject[] tempChildren = m_children.ToArray();
                foreach (GameObject child in tempChildren) {
                    child.Destroy(scene);
                }
            }
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
            return AbsPosition.Z;
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
        private void DetachChild(GameObject child) {
            if (m_children != null) {
                m_children.Remove(child);
            }
        }

        /**
         * @brief add child to children
         * 
         * @param child the child gameObject
         * */
        private void AddChild(GameObject child) {
            if (child == null) {
                return;
            }
            if (m_children == null) {
                m_children = new List<GameObject>();
            }
            m_children.Add(child);
        }

        /**
         * @brief get GameObject GUIDs with the given name
         **/ 
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
                m_parent.AddChild(this);
            }
        }

        protected override void PostUnserial(XmlNode _node) {
            SetComponentsReference();
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
            SetComponentsReference();
        }

        private void SetComponentsReference() {
            // We don't use delay binding for this field. See the comment in CatComponent
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
         * @brief add components
         * 
         * @param component the component
         * */
        public void AddComponent(CatComponent _component) {
            if (_component != null) {
                string key = _component.GetType().ToString();
                if (m_components == null) {
                    m_components = new Dictionary<string, CatComponent>();
                }
                m_components.Add(key, _component);
                _component.m_gameObject = this;
            }
        }

        public Dictionary<string, CatComponent> GetComponents() {
            return m_components;
        }

        public CatComponent GetComponent(Type _componentType) {
            return GetComponent(_componentType.ToString());
        }

        [Obsolete("Use GetComponent(Type _componentType) instead")]
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

        public bool RemoveComponent(Type _componentType) {
            return RemoveComponent(_componentType.ToString());
        }

        [Obsolete("User RemoveComponent(Type _componentName) instead")]
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

    }
}
