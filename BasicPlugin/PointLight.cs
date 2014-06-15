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
                    m_verticeList.Add(new Vector2((float)Math.Cos(2 * segment * MathHelper.Pi / CircleSegment),
                                        (float)Math.Sin(2 * segment * MathHelper.Pi / CircleSegment)));
                }
            }
            m_debugShape.SetVertices(m_verticeList);
            UpdateDrawVertex();
        }

        public override Vector2 GetLightDirection(Vector2 _point) {
            Vector2 center = new Vector2(m_gameObject.AbsPosition.X, m_gameObject.AbsPosition.Y)
                + m_offset;
            Vector2 delta = _point - center;
            if (delta.LengthSquared() < 0.0001) {
                return -Vector2.UnitY;
            }
            else {
                delta.Normalize();
                return delta;
            }
        }

        public static new string GetMenuNames() {
            return "Shadow|Point Light";
        }
    }
}
