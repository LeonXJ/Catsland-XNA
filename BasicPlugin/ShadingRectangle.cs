using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class ShadingRectangle : ShadingBody {
#region Properties

        [SerialAttribute]
        private readonly CatVector2 m_size = new CatVector2(0.2f, 0.2f);
        public Vector2 Size {
            set {
                float width = MathHelper.Max(0.0f, value.X);
                float height = MathHelper.Max(0.0f, value.Y);
                m_size.SetValue(new Vector2(width, height));
                UpdateVertex();
            }
            get {
                return m_size.GetValue();
            }
        }

#endregion

        public ShadingRectangle(GameObject _gameObject)
            : base(_gameObject) {
        }

        public ShadingRectangle():base(){
        
        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            UpdateVertex();
        }

        private void UpdateVertex() {
            // shading vertex
            float halfWidth = m_size.X / 2.0f;
            float halfHeight = m_size.Y / 2.0f;
            if (m_vertices == null) {
                m_vertices = new List<Vector2>(4);
                for (int i = 0; i < 4; ++i) {
                    m_vertices.Add(Vector2.Zero);
                }
                
            }
            m_vertices[0] = new Vector2(-halfWidth,  halfHeight);
            m_vertices[1] = new Vector2( halfWidth,  halfHeight);
            m_vertices[2] = new Vector2( halfWidth, -halfHeight);
            m_vertices[3] = new Vector2(-halfWidth, -halfHeight);
            // debug box
            m_debugShape.SetVertices(m_vertices);
        }

        public static new string GetMenuNames() {
            return "Shadow|ShadingRectangle";
        }
    }
}
