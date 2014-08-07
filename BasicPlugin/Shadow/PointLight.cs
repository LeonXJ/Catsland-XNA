using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Plugin.BasicPlugin {
    public class PointLight : Light {

#region Properties

        static private int CircleSegment = 16;

        [SerialAttribute]
        protected readonly CatFloat m_outRadius = new CatFloat(1.0f);
        public float OutRaidus {
            set {
                m_outRadius.SetValue(MathHelper.Max(m_inRadius + 0.1f, value));
                UpdateVertex();
            }
            get{
                return m_outRadius.GetValue();
            }
        }

        [SerialAttribute]
        protected readonly CatFloat m_inRadius = new CatFloat(0.5f);
        public float InRadius {
            set {
                m_inRadius.SetValue(MathHelper.Clamp(value, 0.0f, m_outRadius - 0.1f));
            }
            get {
                return m_inRadius.GetValue();
            }
        }

        static private Effect m_pointLightEffect;

#endregion
        
        public PointLight(GameObject _gameObject)
            :base(_gameObject){

        }

        public PointLight()
            : base() {

        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            UpdateVertex();
            UpdateEffect();
        }

        private void UpdateEffect() {
            if (m_pointLightEffect == null) {
                m_pointLightEffect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>
                ("effect\\PointLight");
            }
        }

        virtual protected void UpdateVertex(){
            if (m_verticeList == null) {
                m_verticeList = new List<Vector2>(CircleSegment);
                for (int segment = 0; segment < CircleSegment; ++segment) {
                    m_verticeList.Add(Vector2.Zero);
                }
            }
            for (int segment = 0; segment < CircleSegment; ++segment) {
                m_verticeList[segment] = m_outRadius * new Vector2((float)Math.Cos(2 * segment * MathHelper.Pi / CircleSegment),
                                    (float)Math.Sin(2 * segment * MathHelper.Pi / CircleSegment));
            }
            m_debugShape.SetVertices(m_verticeList);
            UpdateDrawVertex();
        }

        public Vector2 GetCentroidInWorld(){
            return new Vector2(m_gameObject.AbsPosition.X, m_gameObject.AbsPosition.Y)
                + m_offset;
        }

        public override Vector2 GetLightDirection(Vector2 _point) {
            Vector2 center = GetCentroidInWorld();
            Vector2 delta = _point - center;
            if (delta.LengthSquared() < 0.0001) {
                return -Vector2.UnitY;
            }
            else {
                delta.Normalize();
                return delta;
            }
        }

        public override bool IsBodyInLightRange(Vector2[] _vertices, Matrix _transform) {
            // TODO: need to judge whether the centroid is in the body
            // TODO: if necessary, add a broad phase to do bounding circle detection
            if (!m_isLightOn) {
                return false;
            }
            if (_vertices.Length < 2) {
                return false;
            }
            Vector2 centroidInWorld = GetCentroidInWorld();
            Vector2 prePoint = Vector2.Transform(_vertices[0], _transform);
            for (int si = 0; si < _vertices.Length; ++si) {
                int ei = (si + 1) % _vertices.Length;
                Vector2 sp = prePoint;
                Vector2 ep = Vector2.Transform(_vertices[ei], _transform);
                Vector2 v0 = centroidInWorld - sp;
                Vector2 v1 = ep - sp;
                float shadowLength = Vector2.Dot(v0, v1) / v1.Length();
                Vector2 nearPoint = sp + Vector2.Normalize(v1) * shadowLength;
                // same side of different side
                Vector2 v2 = centroidInWorld - ep;
                Vector3 negV0 = new Vector3(-v0.X, -v0.Y, 0.0f);
                Vector3 negV2 = new Vector3(-v2.X, -v2.Y, 0.0f);
                Vector3 negNor = new Vector3(centroidInWorld.X - nearPoint.X, centroidInWorld.Y - nearPoint.Y, 0.0f);
                float crossn0 = Vector3.Cross(negNor, negV0).Z;
                float crossn2 = Vector3.Cross(negNor, negV2).Z;
                if (crossn0 * crossn2 < 0.0f) {
                    // different side, the distant that matter
                    if (negNor.LengthSquared() < m_outRadius * m_outRadius) {
                        return true;
                    }
                }
                else {
                    // on the same side, the min dist that matters
                    if (v0.LengthSquared() < m_outRadius * m_outRadius) {
                        return true;
                    }

                    if (v2.LengthSquared() < m_outRadius * m_outRadius) {
                        return true;
                    }
                }
                prePoint = ep;
            }
            return false;
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
            m_pointLightEffect.CurrentTechnique = m_pointLightEffect.Techniques["Main"];
            Vector2 centroidInWorld = GetCentroidInWorld();
            m_pointLightEffect.Parameters["LightPositionInWorld"].SetValue(new Vector3(centroidInWorld.X, centroidInWorld.Y, 0.0f));
            m_pointLightEffect.Parameters["DiffuseColor"].SetValue(m_diffuseColor.m_value);
            m_pointLightEffect.Parameters["OutRadius"].SetValue(m_outRadius);
            m_pointLightEffect.Parameters["InRadius"].SetValue(m_inRadius);

            m_pointLightEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(new Vector3(m_offset.X, m_offset.Y, 0.0f)) *
                    m_gameObject.AbsTransform);
            m_pointLightEffect.Parameters["View"].SetValue(Mgr<Camera>.Singleton.View);
            m_pointLightEffect.Parameters["Projection"].SetValue(Mgr<Camera>.Singleton.m_projection);

            m_pointLightEffect.CurrentTechnique.Passes["P0"].Apply();

            Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleStrip,
                    m_vertice,
                    0,
                    m_verticeList.Count - 2);
        }

        public override bool IsPointInLightRange(Vector2 _point) {
            if (!m_isLightOn) {
                return false;
            }
            Vector2 centroid = GetCentroidInWorld();
            Vector2 delta = _point - centroid;
            if (delta.LengthSquared() > m_outRadius * m_outRadius) {
                return false;
            }
            else {
                return true;
            }
        }

        public static new string GetMenuNames() {
            return "Shadow|Point Light";
        }
    }
}
