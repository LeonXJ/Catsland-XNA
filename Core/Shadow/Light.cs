using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class Light : CatComponent, Drawable{

#region Properties

        protected DebugShape m_debugShape = new DebugShape();

        private int id;
        public int ID {
            set {
                id = value;
            }
            get {
                return id;
            }
        }

        [SerialAttribute]
        protected readonly CatVector2 m_offset = new CatVector2();
        public Vector2 Offset {
            set {
                m_offset.SetValue(value);
                m_debugShape.Position = new Vector3(m_offset.X, m_offset.Y, 0.0f);
            }
            get {
                return m_offset.GetValue();
            }
        }

        [SerialAttribute]
        protected readonly CatColor m_diffuseColor = new CatColor(0.4f, 0.4f, 0.4f, 0.4f);
        public Color DiffuseColor {
            set {
                m_diffuseColor.SetValue(value);
            }
            get {
                return m_diffuseColor;
            }
        }

        // ---- Configure of RenderLight and Enlight -------
        //        Light Type        | RenderLight | Enlight
        //       normal light       |    true     |   true
        // visible field of vision  |    true     |  false
        // invisible field of vision|    false    |  false
        //          ?????           |    false    |   true

        // switch of the light
        [SerialAttribute]
        protected readonly CatBool m_isLightOn = new CatBool(true);
        public bool IsLightOn {
            set {
                m_isLightOn.SetValue(value);
            }
            get {
                return m_isLightOn;
            }
        }
        
        // render the light?
        [SerialAttribute]
        protected readonly CatBool m_renderLight = new CatBool(true);
        public bool RenderLight {
            set {
                m_renderLight.SetValue(value);
            }
            get {
                return m_renderLight;
            }
        }

        // should the light enlights objects?
        [SerialAttribute]
        protected readonly CatBool m_enlight = new CatBool(true);
        public bool Enlight {
            set {
                m_enlight.SetValue(value);
            }
            get {
                return m_enlight;
            }
        }

        protected List<Vector2> m_verticeList;
        protected VertexPositionColor[] m_vertice;
        protected VertexBuffer m_vertexBuffer;

#endregion

        public Light(GameObject _gameObject)
            : base(_gameObject) {

        }

        public Light()
            : base() {

        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            m_debugShape.RelateGameObject = m_gameObject;
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.BindToScene(scene);
            }
            scene.m_shadowSystem.AddLight(this);
        }

        protected void UpdateDrawVertex(){
            if(m_vertice == null && m_verticeList != null){
                m_vertice = new VertexPositionColor[m_verticeList.Count];
                for(int i=0; i<m_verticeList.Count; ++i){
                    m_vertice[i] = new VertexPositionColor(Vector3.Zero, Color
                        .White);
                }
            }
            m_vertice[0].Position = new Vector3(m_verticeList[0].X, m_verticeList[0].Y, 0.0f);
            for(int i=1; i<m_verticeList.Count; ++i){
                int index = (i + 1) / 2;
                if( i % 2 == 0){
                    index = m_verticeList.Count - index;
                }
                m_vertice[i].Position = new Vector3(m_verticeList[index].X, 
                                                    m_verticeList[index].Y,
                                                    0.0f);
            }
            m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                typeof(VertexPositionColor), m_verticeList.Count,
                        BufferUsage.None);
            m_vertexBuffer.SetData<VertexPositionColor>(m_vertice);
        }

        public override void UnbindFromScene(Scene _scene) {
            base.UnbindFromScene(_scene);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.Destroy(_scene);
            }
            _scene.m_shadowSystem.RemoveLight(this);
        }

        virtual public void Draw(int timeLastFrame){
            // TODO: use light material
            // draw light
            if(m_vertice == null || !m_isLightOn || !m_renderLight){
                return;
            }
           
            Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
            Effect effect = Mgr<DebugTools>.Singleton.DrawEffect;
            ((BasicEffect)effect).Alpha = DiffuseColor.A / 255.0f;
            ((BasicEffect)effect).DiffuseColor = new Vector3(DiffuseColor.R / 255.0f, DiffuseColor.G / 255.0f, DiffuseColor.B / 255.0f);
            ((BasicEffect)effect).View = Mgr<Camera>.Singleton.View;
            ((BasicEffect)effect).Projection = Mgr<Camera>.Singleton.m_projection;
            ((BasicEffect)effect).VertexColorEnabled = false;
            ((BasicEffect)effect).World = Matrix.CreateTranslation(new Vector3(m_offset.X, m_offset.Y, 0.0f)) * 
                    m_gameObject.AbsTransform;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleStrip,
                    m_vertice,
                    0,
                    m_verticeList.Count - 2);
            }
        }

        /**
         * @brief regardless of obstacles, if the given point is in this light?
         */ 
        public virtual bool IsPointInLightRange(Vector2 _point) {
            if (!m_isLightOn) {
                return false;
            }
            return false;
        }

        public static string GetMenuNames() {
            return "Shadow|LightBase";
        }

        /**
         * @brief get the direction of light at the given point
         */ 
        virtual public Vector2 GetLightDirection(Vector2 _point){
            return -Vector2.UnitY;
        }

        /**
         * @brief regardless of obstacles, if the given convex is in this light
         */ 
        virtual public bool IsBodyInLightRange(Vector2[] _vertices, Matrix _transform) {
            if (!m_isLightOn) {
                return false;
            }
            return false;
        }

        /**
         * @brief if the edge should cast shadow
         */ 
        virtual public bool ShouldEdgeHasShadow(ShadingBody _shadingBody, int _edge) {
            Vector2 startPoint = _shadingBody.GetVertexInWorld(_edge);
            Vector2 edgeVector2 = _shadingBody.GetVertexInWorld((_edge + 1) % _shadingBody.GetVerticesNumber())
                    - startPoint;
            Vector3 edgeVector3 = new Vector3(edgeVector2.X, edgeVector2.Y, 0.0f);
            Vector3 normal = Vector3.Cross(edgeVector3, -Vector3.UnitZ);
            Vector2 lightDirection = GetLightDirection(startPoint);
            Vector2 normal2D = new Vector2(normal.X, normal.Y);
            return Vector2.Dot(normal2D, lightDirection) < 0.0f;
        }

        /**
         * @brief if the given point is enlighted by this light? return false if
         *     light.Enlight = false or light.isLightOn = false
         */     
        virtual public bool IsPointEnlighted(Vector2 _point) {
            if (!m_enlight) {
                return false;
            }
            return IsPointInOnLight(_point);
        }

        /**
         * @brief if the given point is in the light range of this light? return false if
         *      the light is off. This function can be used as a query of sensor.
         */
        virtual public bool IsPointInOnLight(Vector2 _point) {
            if (!m_isLightOn) {
                return false;
            }
            return Mgr<Scene>.Singleton.m_shadowSystem.IsPointInLight(_point, ID);
        }

        public float GetDepth() {
            return 0;
        }

        public int CompareTo(object obj) {
            return 1;
        }
    }
}
