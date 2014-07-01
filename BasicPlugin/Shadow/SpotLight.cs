﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class SpotLight : PointLight{

#region Properties

        static private int DegreePerPoint = 10;

        [SerialAttribute]
        private CatFloat m_fanInDegree = new CatFloat(15.0f);
        public float FanInDegree {
            set {
                m_fanInDegree.SetValue(MathHelper.Clamp(value, 0.0f, 90.0f));
                m_verticeList = null;
                m_vertice = null;
                UpdateVertex();
            }
            get {
                return m_fanInDegree.GetValue();
            }
        }

        [SerialAttribute]
        private CatFloat m_directionToDown = new CatFloat(0.0f);
        public float DirectionToDown {
            set {
                m_directionToDown.SetValue(MathHelper.Clamp(value, -180.0f, 180.0f));
                UpdateVertex();
            }
            get {
                return m_directionToDown.GetValue();
            }
        }


#endregion

        public SpotLight(GameObject _gameObject)
            : base(_gameObject){
        }

        public SpotLight()
            : base() {
        }

        protected override void UpdateVertex() {
            if (m_verticeList == null) {
                int pointNumber = 2 + (int)Math.Ceiling(m_fanInDegree * 2 / DegreePerPoint);
                m_verticeList = new List<Vector2>(pointNumber);
                for (int segment = 0; segment < pointNumber; ++segment) {
                    m_verticeList.Add(Vector2.Zero);
                }
            }
            float beginDegree = m_directionToDown + m_fanInDegree;
            float endDegree = m_directionToDown - m_fanInDegree;
            int curIndex = 1;
            while (beginDegree > endDegree) {
                m_verticeList[curIndex] = m_outRadius
                    * new Vector2((float)Math.Sin(beginDegree * 2 * MathHelper.Pi / 180),
                                  -(float)Math.Cos(beginDegree * 2 * MathHelper.Pi / 180));
                beginDegree -= DegreePerPoint;
                curIndex += 1;
            }
            if (curIndex < m_verticeList.Count) {
                m_verticeList[curIndex] = m_outRadius
                    * new Vector2((float)Math.Sin(endDegree * 2 * MathHelper.Pi / 180),
                                  -(float)Math.Cos(endDegree * 2 * MathHelper.Pi / 180));
            }
            m_debugShape.SetVertices(m_verticeList);
            UpdateDrawVertex();
        }

        public override bool IsBodyInLight(Vector2[] _vertices, Matrix _transform) {
            return CatMath.IsConvexIntersect(m_verticeList.ToArray(), 
                Matrix.CreateTranslation(new Vector3(m_offset.X, m_offset.Y, 0.0f))
                    * m_gameObject.AbsTransform,
                _vertices, _transform); 
        }

        public override bool IsPointInLight(Vector2 _point) {
            Vector2 centroid = GetCentroidInWorld();
            Vector2 delta = _point - centroid;
            if (delta.LengthSquared() > m_outRadius * m_outRadius) {
                return false;
            }
            return true;
//             Vector2 frontPoint = Vector2.Transform(new Vector2((float)Math.Sin(m_directionToDown * 2 * MathHelper.Pi / 180),
//                                   -(float)Math.Cos(m_directionToDown * 2 * MathHelper.Pi / 180)), 
           // TODO: judge by angle                       
        }

        public static new string GetMenuNames() {
            return "Shadow|Spot Light";
        }

    }
}
