using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class PointLight : Light {

#region Properties

        static private int CircleSegment = 16;

        [SerialAttribute]
        private readonly CatFloat m_radius = new CatFloat(0.4f);
        public float Raidus {
            set {
                m_radius.SetValue(MathHelper.Max(0.0f, value));
                UpdateVertex();
            }
            get{
                return m_radius.GetValue();
            }
        }

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
        }

        private void UpdateVertex(){
            if (m_verticeList == null) {
                m_verticeList = new List<Vector2>(CircleSegment);
                for (int segment = 0; segment < CircleSegment; ++segment) {
                    m_verticeList.Add(m_radius * new Vector2((float)Math.Cos(2 * segment * MathHelper.Pi / CircleSegment),
                                        (float)Math.Sin(2 * segment * MathHelper.Pi / CircleSegment)));
                }
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

        public override bool IsBodyInLight(ShadingBody _shadingBody) {
            // TODO: need to judge whether the centroid is in the body
            // TODO: if necessary, add a broad phase to do bounding circle detection
            int num = _shadingBody.GetVerticesNumber();
            if (num < 2) {
                return false;
            }
            Vector2 centroidInWorld = GetCentroidInWorld();
            Vector2 prePoint = _shadingBody.GetVertexInWorld(0);
            for (int si = 0; si < num; ++si) {
                int ei = (si + 1) % num;
                Vector2 sp = prePoint;
                Vector2 ep = _shadingBody.GetVertexInWorld(ei);
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
                    if (negNor.LengthSquared() < m_radius * m_radius) {
                        return true;
                    }
                }
                else {
                    // on the same side, the min dist that matters
                    if (v0.LengthSquared() < m_radius * m_radius) {
                        return true;
                    }
                    
                    if (v2.LengthSquared() < m_radius * m_radius) {
                        return true;
                    }
                }
                prePoint = ep;
            }
            return false;
        }

        public static new string GetMenuNames() {
            return "Shadow|Point Light";
        }
    }
}
