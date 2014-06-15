using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Plugin.BasicPlugin {
    public class LightShadow : Drawable {

#region Properties

        private static float ShadowDistance = 10.0f;

        private Vector2[] m_vertexList = new Vector2[4];

        private VertexPositionColor[] m_vertices;
        private VertexBuffer m_vertexBuffer;

#endregion

        static public LightShadow UpdateShadow(
            LightShadow _toBeUpdate, Light _light, 
            ShadingBody _body, int _edgeIndex) {

            if (_edgeIndex > _body.GetVerticesNumber()) {
                return null;
            }
            Vector2 startPoint = _body.GetVertexInWorld(_edgeIndex);
            Vector2 endPoint;
            if (_edgeIndex == _body.GetVerticesNumber()) {
                endPoint = _body.GetVertexInWorld(0);
            }
            else {
                endPoint = _body.GetVertexInWorld(_edgeIndex + 1);
            }
            Vector2 startTill = startPoint +
                _light.GetLightDirection(startPoint) * ShadowDistance;
            Vector2 endTill = endPoint +
                _light.GetLightDirection(endPoint) * ShadowDistance;
            // update _toBeUpdate
            LightShadow shadow = _toBeUpdate;
            if (shadow == null) {
                shadow = new LightShadow();
            }
            shadow.m_vertexList[0] = startPoint;
            shadow.m_vertexList[1] = startTill;
            shadow.m_vertexList[2] = endTill;
            shadow.m_vertexList[3] = endPoint;

            return shadow;
        }

        // If the given point is on the same side of all the edge, it's inside the polygen
        // Else, it is outside
        public bool IsPointInShadow(Vector2 _point) {
            int count_side1 = 0;
            int count_side2 = 0;
            for (int si = 0; si < 4; ++si) {
                int ei = (si + 1) % 4;
                float value = (_point.X - m_vertexList[si].X) * (m_vertexList[ei].Y - m_vertexList[si].Y) -
                    (m_vertexList[ei].X - m_vertexList[si].X) * (_point.Y - m_vertexList[si].Y);
                if (value > 0.0f) {
                    count_side1 += 1;
                }
                else {
                    count_side2 += 1;
                }
            }
            if (count_side1 == 0 || count_side2 == 0) {
                return true;
            }
            return false;
        }

        public void BindToScene(Scene _scene) {
            // TODO: Bind shadow to shadow list
            _scene._debugDrawableList.AddItem(this);
        }

        private void UpdateDrawVertex() {
            if (m_vertices == null) {
                m_vertices = new VertexPositionColor[4];
                for (int i = 0; i < 4; ++i) {
                    m_vertices[i] = new VertexPositionColor(Vector3.Zero, Color.White);
                }
            }
            // update position
            for (int i = 0; i < 4; ++i) {
                m_vertices[i].Position = new Vector3(m_vertexList[i].X, m_vertexList[i].Y, 0.0f);
            }
            if (m_vertexBuffer == null) {
                m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                typeof(VertexPositionColor), 4,
                        BufferUsage.None);
            }
            m_vertexBuffer.SetData<VertexPositionColor>(m_vertices);
        }


    }
}
