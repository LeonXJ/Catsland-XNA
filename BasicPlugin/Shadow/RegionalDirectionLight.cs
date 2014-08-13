using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Plugin.BasicPlugin {
    public class RegionalDirectionLight : Light{

#region Properties

        [SerialAttribute]
        protected readonly CatVector2 m_direction = new CatVector2(-Vector2.UnitY);
        public Vector2 Direction {
            set {
                if (value.LengthSquared() < 0.001f) {
                    return;
                }
                else {
                    m_direction.SetValue(value / value.Length());
                }
            }
            get {
                return m_direction;
            }
        }
        public float DegreeToDown {
            set {
                float radian = MathHelper.ToRadians(value);
                m_direction.SetValue(new Vector2((float)(-Math.Sin(radian)), (float)(-Math.Cos(radian))));
            }
            get {
                float rawDegree = MathHelper.ToDegrees((float)Math.Asin(m_direction.X));
                if (m_direction.Y < 0.0f) { // 3,4 phase
                    return -rawDegree;
                }
                else {
                    if (m_direction.X < 0.0f) { // 2 phase
                        return 180 + rawDegree;
                    }
                    else {
                        return rawDegree - 180; // 1 phase
                    }
                }
            }
        }

        [SerialAttribute]
        protected readonly CatVector2 m_size = new CatVector2(Vector2.One);
        public Vector2 Size {
            set {
                m_size.SetValue(new Vector2(MathHelper.Max(value.X, 0.0f),
                                            MathHelper.Max(value.Y, 0.0f)));
                UpdateVertex();
            }
            get {
                return m_size;
            }
        }

        static private Effect m_directionLightEffect;

#endregion

        public RegionalDirectionLight(GameObject _gameObject)
            : base(_gameObject){
        
        }

        public RegionalDirectionLight()
            :base(){}

        public override void Initialize(Catsland.Core.Scene scene) {
            base.Initialize(scene);
            UpdateVertex();
            UpdateEffect();
            
        }

        private void UpdateEffect() {
            if (m_directionLightEffect == null) {
                m_directionLightEffect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>
                    ("effect\\RegionalDirectionLight");
            }
        }

        virtual protected void UpdateVertex() {
            if (m_verticeList == null) {
                m_verticeList = new List<Vector2>(4);
                for (int segment = 0; segment < 4; ++segment) {
                    m_verticeList.Add(Vector2.Zero);
                }
            }
            float half_width = m_size.X / 2.0f;
            float half_height = m_size.Y / 2.0f;
            m_verticeList[0] = new Vector2( half_width,  half_height);
            m_verticeList[1] = new Vector2( half_width, -half_height);
            m_verticeList[2] = new Vector2(-half_width, -half_height);
            m_verticeList[3] = new Vector2(-half_width,  half_height);
            m_debugShape.SetAsRectangle(m_size, m_offset);
            UpdateDrawVertex();
        }

        public override Vector2 GetLightDirection(Vector2 _point) {
            return Vector2.Transform(m_direction, m_gameObject.AbsTransform)
                - new Vector2(m_gameObject.AbsPosition.X, m_gameObject.AbsPosition.Y);
        }

        public override bool IsBodyInLightRange(Vector2[] _vertices, Matrix _transform) {
            return CatMath.IsConvexIntersect(m_verticeList.ToArray(), m_gameObject.AbsTransform,
                _vertices, _transform);
        }

        public override void Draw(int timeLastFrame) {
            base.Draw(timeLastFrame);

            if (!m_isLightOn || !m_renderLight) {
                return;
            }
            if (m_vertice == null) {
                return;
            }
            Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
            m_directionLightEffect.CurrentTechnique = m_directionLightEffect.Techniques["Main"];

            m_directionLightEffect.Parameters["DiffuseColor"].SetValue(m_diffuseColor.m_value);
            m_directionLightEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(new Vector3(m_offset.X, m_offset.Y, 0.0f)) *
                    m_gameObject.AbsTransform);
            m_directionLightEffect.Parameters["View"].SetValue(Mgr<Camera>.Singleton.View);
            m_directionLightEffect.Parameters["Projection"].SetValue(Mgr<Camera>.Singleton.m_projection);

            m_directionLightEffect.CurrentTechnique.Passes["P0"].Apply();

            Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleStrip,
                    m_vertice,
                    0,
                    2);
        }

        public override bool IsPointInLightRange(Vector2 _point) {
            if (!m_isLightOn) {
                return false;
            }
            Vector2 pointInLocal = Vector2.Transform(_point, Matrix.Invert(m_gameObject.AbsTransform));
            float halfWidth = m_size.X / 2.0f;
            float halfHeight = m_size.Y / 2.0f;
            if (pointInLocal.X + m_offset.X < -halfWidth || pointInLocal.X + m_offset.X > halfWidth) {
                return false;
            }
            if (pointInLocal.Y + m_offset.Y < -halfHeight || pointInLocal.Y + m_offset.Y > halfHeight) {
                return false;
            }
            return true;
        }

        public static new string GetMenuNames() {
            return "Shadow|Regional Direction Light";
        }

    }
}
