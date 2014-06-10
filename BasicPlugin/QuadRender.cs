using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;
using Catsland.Core;
using System.ComponentModel;

namespace Catsland.Plugin.BasicPlugin
{
    public class QuadRender : CatComponent, Drawable, ISelectable
    {
        #region Properties

        [SerialAttribute]
        private CatVector2 m_size; // = new Vector2(0.1f, 0.1f);
        [CategoryAttribute("Location")]
        public Vector2 Size {
            get { 
                return m_size.GetValue(); 
            }
            set {
                m_size.SetValue(value);
                UpdateVertex();
            }
        }

        [SerialAttribute]
        private CatVector2 m_offset;
        [CategoryAttribute("Location")]
        public Vector2 Offset {
            get { 
                return m_offset.GetValue(); 
            }
            set {
                m_offset.SetValue(value);
                UpdateVertex();
            }
        }

        public bool OptimalSize {
            get {
                return false;
            }
            set {
                setToOptimalSize();
            }
        }

        private VertexBuffer m_vertexBuffer;
        public VertexPositionColorTexture[] m_vertex;   // animator need to access

        // TODO: deprecate
        private bool x_mirror;
        public bool XMirror {
            get { return x_mirror; }
            set {
                x_mirror = value;
                UpdateVertex();
            }
        }
        public CatFloat alpha = new CatFloat(1.0f);
        public float Alpha {
            set { alpha.SetValue(value); }
            get { return alpha.GetValue(); }
        }
        public CatVector3 diffuseColor = new CatVector3(Vector3.One);
        public Vector3 DiffuseColor {
            set { diffuseColor.SetValue(value); }
            get { return diffuseColor.GetValue(); }
        }

        #endregion

        public QuadRender() : base() {
            m_size = new CatVector2(0.1f, 0.1f);
            m_offset = new CatVector2();
        }

        public QuadRender(GameObject gameObject)
            : base(gameObject)
        {
            m_size = new CatVector2(0.1f, 0.1f);
            m_offset = new CatVector2();
        }

        public override CatComponent CloneComponent(GameObject gameObject)
        {
            QuadRender quadRender = new QuadRender(gameObject);
            quadRender.Enable = Enable;
            quadRender.Alpha = Alpha;
            quadRender.m_size = new CatVector2(m_size.X, m_size.Y);
            quadRender.m_offset = new CatVector2(m_offset.X, m_offset.Y);

            return quadRender;
        }

        public override void BindToScene(Scene scene)
        {
            base.BindToScene(scene);
            scene._renderList.AddItem(this);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                scene._selectableList.AddItem(this);
            }
            
        }

        /* Before initialize, m_size and m_offset should be set
         */
        public override void Initialize(Scene scene)
        {
            base.Initialize(scene);
            // create mesh
            UpdateVertex();
        }

        public void UpdateVertex()
        {
            if (m_vertex == null) {
                m_vertex = new VertexPositionColorTexture[4];
            }
            m_vertex[0] = new VertexPositionColorTexture(
                new Vector3(m_offset.X - m_size.X / 2.0f, m_offset.Y - m_size.Y / 2.0f, 0.0f),
                new Color(1.0f, 1.0f, 1.0f, 1.0f),
                new Vector2(x_mirror? 1.0f : 0.0f, 1.0f));
            m_vertex[1] = new VertexPositionColorTexture(
                new Vector3(m_offset.X - m_size.X / 2.0f, m_offset.Y + m_size.Y / 2.0f, 0.0f),
                new Color(1.0f, 1.0f, 1.0f, 1.0f),
                new Vector2(x_mirror? 1.0f : 0.0f, 0.0f));
            m_vertex[2] = new VertexPositionColorTexture(
                new Vector3(m_offset.X + m_size.X / 2.0f, m_offset.Y - m_size.Y / 2.0f, 0.0f),
                new Color(0.0f, 1.0f, 1.0f, 1.0f),
                new Vector2(x_mirror? 0.0f : 1.0f, 1.0f));
            m_vertex[3] = new VertexPositionColorTexture(
                new Vector3(m_offset.X + m_size.X / 2.0f, m_offset.Y + m_size.Y / 2.0f, 0.0f),
                new Color(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector2(x_mirror? 0.0f : 1.0f, 0.0f));
            if (m_vertexBuffer == null) {
                m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                                    typeof(VertexPositionColorTexture), 4, BufferUsage.None);
            }
            m_vertexBuffer.SetData<VertexPositionColorTexture>(m_vertex);
        }

        public void Draw(int timeLastFrame) {
            if (!Enable) {
                return;
            }

            ModelComponent modelComponent = (ModelComponent)m_gameObject.GetComponent(typeof(ModelComponent).ToString());

            Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
            // configure effect

            Effect effect = null;
            if (modelComponent != null && modelComponent.Model != null) {

                CatMaterial material = modelComponent.GetCatModelInstance().GetMaterial();
                material.SetParameter("World", new CatMatrix(m_gameObject.AbsTransform));
                material.SetParameter("View", new CatMatrix(Mgr<Camera>.Singleton.View));
                material.SetParameter("Projection", new CatMatrix(Mgr<Camera>.Singleton.m_projection));
                effect = material.ApplyMaterial();
            }
            else {
                effect = Mgr<DebugTools>.Singleton.DrawEffect;
                ((BasicEffect)effect).Alpha = 1.0f;
                ((BasicEffect)effect).DiffuseColor = new Vector3(1.0f,0.0f,1.0f);
                ((BasicEffect)effect).View = Mgr<Camera>.Singleton.View;
                ((BasicEffect)effect).Projection = Mgr<Camera>.Singleton.m_projection;
                ((BasicEffect)effect).VertexColorEnabled = false;
                ((BasicEffect)effect).World = m_gameObject.AbsTransform;

            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColorTexture>(
                    PrimitiveType.TriangleStrip, m_vertex, 0, 2);
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc)
        {
            XmlElement quadRender = doc.CreateElement(typeof(QuadRender).Name);
            node.AppendChild(quadRender);

            quadRender.SetAttribute("enable", "" + Enable);
            quadRender.SetAttribute("alpha", "" + Alpha);

            XmlElement size = doc.CreateElement("Size");
            quadRender.AppendChild(size);
            size.SetAttribute("Width", "" + m_size.X);
            size.SetAttribute("Height", "" + m_size.Y);

            XmlElement offset = doc.CreateElement("Offset");
            quadRender.AppendChild(offset);
            offset.SetAttribute("X", "" + m_offset.X);
            offset.SetAttribute("Y", "" + m_offset.Y);

            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject)
        {
            base.ConfigureFromNode(node, scene, gameObject);

            Enable = bool.Parse(node.GetAttribute("enable"));
            alpha.SetValue(float.Parse(node.GetAttribute("alpha")));

            // size
            XmlElement size = (XmlElement)node.SelectSingleNode("Size");
            m_size = new CatVector2(float.Parse(size.GetAttribute("Width")),
                                    float.Parse(size.GetAttribute("Height")));

            // offset
            XmlElement offset = (XmlElement)node.SelectSingleNode("Offset");
            m_offset = new CatVector2(float.Parse(offset.GetAttribute("X")),
                                    float.Parse(offset.GetAttribute("Y")));
        }

        public override void Destroy() {
            Mgr<Scene>.Singleton._renderList.RemoveItem(this);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                Mgr<Scene>.Singleton._selectableList.RemoveItem(this);
            }
            base.Destroy();
        }

        private ModelComponent getModel() {
            return (ModelComponent)m_gameObject.GetComponent(typeof(ModelComponent).Name);
        }


        private void setToOptimalSize() {
            // get model component and image size
            int imageWidth = 0;
            int imageHeight = 0;
            ModelComponent modelComponent = 
                m_gameObject.GetComponent(typeof(ModelComponent).ToString()) as ModelComponent;
            CatModelInstance modelInstance = modelComponent.GetCatModelInstance();
            if (modelInstance == null || modelInstance.GetMaterial() == null) {
                return;
            }
            CatTexture texture = (CatTexture)(modelInstance.GetMaterial().GetParameter("DiffuseMap"));
            
            imageWidth = texture.value.Width;
            imageHeight = texture.value.Height;

            if (modelComponent.Model.GetAnimation() != null) {
                imageWidth /= modelComponent.Model.GetAnimation().m_tiltUV.X;
                imageHeight /= modelComponent.Model.GetAnimation().m_tiltUV.Y;
            }
                  
            // get Camera Min Width
            float cameraMinWidth = Mgr<Camera>.Singleton.ViewSize.X;
            // get Screen Width
            float screenWidth = Mgr<Camera>.Singleton.targetResolutionWidth;
            // calculate optimalWidth
            Size = new Vector2(imageWidth * cameraMinWidth / screenWidth,
                imageHeight * cameraMinWidth / screenWidth);
        }

        public float GetDepth()
        {
            return m_gameObject.AbsPosition.Z;
        }

        public int CompareTo(object obj)
        {
            float otherDepth = ((Drawable)obj).GetDepth();
            float thisDepth = GetDepth();
            if (otherDepth > thisDepth)
            {
                return 1;
            }
            else if (otherDepth < thisDepth)
            {
                return -1;
            }
            return 0;
        }

        bool ISelectable.IsSelected(float cameraX, float cameraY) {
            // TODO: make it validate for 3D world
            // 1) transform to world coordinate, resulting in a ray.
            // 2) test collision between ray and triangle
            Camera camera = Mgr<Camera>.Singleton;
            Vector3 nearPoint = camera.CameraToWorld(new Vector3(cameraX, cameraY, 0.0f));
            Vector3 farPoint = camera.CameraToWorld(new Vector3(cameraX, cameraY, 0.9f));
            Vector3 direction = farPoint - nearPoint;

            Vector4 p0 = Vector4.Transform(new Vector4(-m_size.X / 2.0f + m_offset.X, 
                                                        m_size.Y / 2.0f + m_offset.Y, 
                                                        0.0f, 
                                                        1.0f),
                                           m_gameObject.AbsTransform);
            Vector4 p1 = Vector4.Transform(new Vector4(-m_size.X / 2.0f + m_offset.X, 
                                                       -m_size.Y / 2.0f + m_offset.Y, 
                                                       0.0f,
                                                       1.0f),
                                           m_gameObject.AbsTransform);
            Vector4 p2 = Vector4.Transform(new Vector4(m_size.X / 2.0f + m_offset.X, 
                                                       m_size.Y / 2.0f + m_offset.Y, 
                                                       0.0f,
                                                       1.0f),
                                           m_gameObject.AbsTransform);
            Vector4 p3 = Vector4.Transform(new Vector4(m_size.X / 2.0f + m_offset.X, 
                                                       -m_size.Y / 2.0f + m_offset.Y, 
                                                       0.0f,
                                                       1.0f),
                                           m_gameObject.AbsTransform);
            Vector3 p0v3 = new Vector3(p0.X, p0.Y, p0.Z);
            Vector3 p1v3 = new Vector3(p1.X, p1.Y, p1.Z);
            Vector3 p2v3 = new Vector3(p2.X, p2.Y, p2.Z);
            Vector3 p3v3 = new Vector3(p3.X, p3.Y, p3.Z);

            float u, v, t;
            if (CatMath.IntersectTriangle(nearPoint, direction, p0v3, p1v3, p2v3, out t, out u, out v) == true) {
                return true;
            }
            if (CatMath.IntersectTriangle(nearPoint, direction, p2v3, p1v3, p3v3, out t, out u, out v) == true) {
                return true;
            }
            return false;
        }

        GameObject ISelectable.GetGameObject() {
            return m_gameObject;
        }

        public static string GetMenuNames() {
            return "Render|QuadRender";
        }

        void ISelectable.DrawSelection() {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor
                && m_vertex != null) {
                BasicEffect effect = Mgr<DebugTools>.Singleton.DrawEffect;
                effect.View = Mgr<Camera>.Singleton.View;
                effect.Projection = Mgr<Camera>.Singleton.m_projection;
                effect.VertexColorEnabled = false;
                effect.Alpha = 0.4f;
                effect.TextureEnabled = false;

                effect.World = m_gameObject.AbsTransform;// Matrix.CreateTranslation(m_gameObject.AbsPosition);

                effect.DiffuseColor = new Vector3(0.3f, 0.0f, 0.0f);

                Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColorTexture>(
                        PrimitiveType.TriangleStrip, m_vertex, 0, 2);
                }
            }
        }

        public bool IsLocked() {
            return m_gameObject.Locked;
        }
    }

    public class QuadRenderGameObject : IEditorScript {

        public void RunScript() {

            GameObject newGameObject = new GameObject();
            newGameObject.PositionOld = Vector2.Zero;
            newGameObject.HeightOld = 0.0f;
            // new position
            newGameObject.Position = (new CatVector3(Vector3.Zero));
            // end of new position

            ModelComponent modelComponent = new ModelComponent(newGameObject);
            modelComponent.BindToScene(Mgr<Scene>.Singleton);
            modelComponent.Initialize(Mgr<Scene>.Singleton);
            newGameObject.AddComponent(typeof(ModelComponent).Name, modelComponent);

            QuadRender quadRender = new QuadRender(newGameObject);
            quadRender.BindToScene(Mgr<Scene>.Singleton);
            quadRender.Initialize(Mgr<Scene>.Singleton);
            newGameObject.AddComponent(typeof(QuadRender).Name, quadRender);


            Mgr<Scene>.Singleton._gameObjectList.AddItem(newGameObject.GUID, newGameObject);
            Mgr<Scene>.Singleton._debugDrawableList.AddItem(newGameObject);
        }

        public static string GetMenuNames() {
            return "Create GameObject|With Model and QuadRender";
        }
    }
}
