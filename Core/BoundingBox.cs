using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;

/**
 * @file CatsBoundingBox is used to perform collision detection
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {

    /**
     * @brief CatsBoundingBox, do collision detection
     * 
     * The bounding box can be any convex on XY plane,
     * and a interval on Z
     * */
    public class CatsBoundingBox {
        // children BoundingBox
        public List<CatsBoundingBox> m_subBounding;
        // parent BoundingBox
        public CatsBoundingBox m_parentBounding;

        // vertex of the BoundingBox
        public Vector2[] m_convex;
        // interval on Z
        public Vector2 m_heightRange;

        // set to true if it is a simple box BoundingBox
        public bool m_isRectangle = false;
        public Vector2 m_XBound;
        public Vector2 m_YBound;

        public CatsBoundingBox() {
            m_XBound = new Vector2(-0.1f, 0.1f);
            m_YBound = new Vector2(-0.1f, 0.1f);
            m_heightRange = new Vector2(0.0f, 0.1f);
            m_isRectangle = false;
        }

        /**
         * @brief set vertexes from rectangle
         * 
         * the profile of the rectangle should be set first
         * */
        public void UpdateBoundFromRectangle() {
            m_convex = new Vector2[4];

            m_convex[0] = new Vector2(m_XBound.X, m_YBound.Y);
            m_convex[1] = new Vector2(m_XBound.Y, m_YBound.Y);
            m_convex[2] = new Vector2(m_XBound.Y, m_YBound.X);
            m_convex[3] = new Vector2(m_XBound.X, m_YBound.X);

            m_isRectangle = true;
        }

        public void AddChildBoundingBox(CatsBoundingBox boundingBox) {
            m_subBounding.Add(boundingBox);
        }

        /**
         * @brief judge whether this boundingBox collide with other
         * 
         * @param boundingBox the other boundingBox we want to test
         * @param XYOffset the offset of the other boundingBox on XY
         * @param ZOffset the offset of the other boundingBox on Z
         * @param iXYOffset the offset of this boundingBox on XY
         * @param iZOffset the offset of this boundingBox on Z
         * 
         * @result whether they collide
         * */
        public bool IsCollide(CatsBoundingBox boundingBox,
            Vector2 XYOffset, float ZOffset,
            Vector2 iXYOffset, float iZOffset) {
            // test Collide on Z
            Vector2 iZRange = new Vector2(iZOffset, iZOffset) + m_heightRange;
            Vector2 ZRange = new Vector2(ZOffset, ZOffset) + boundingBox.m_heightRange;

            if (iZRange.X > ZRange.Y || iZRange.Y < ZRange.X) {
                // no need to do further test if no contact on Z
                return false;
            }

            // test Collide on XYPlane
            // do the separation axes algorithm
            // potential separation axes. they get converted into push 
            List<Vector2> axis = new List<Vector2>();

            // max of 16 vertexes per polygon 
            int i, j;
            for (j = m_convex.Length - 1, i = 0;
                i < m_convex.Length; j = i, i++) {
                Vector2 edge = m_convex[i] - m_convex[j];
                Vector2 normal = new Vector2(-edge.Y, edge.X);
                axis.Add(normal);

                if (AxisSeparatePolygons(normal, m_convex, boundingBox.m_convex,
                    iXYOffset, iZOffset, XYOffset, ZOffset)) {
                    return false;
                }
            }

            for (j = boundingBox.m_convex.Length - 1, i = 0;
                i < boundingBox.m_convex.Length; j = i, i++) {
                Vector2 edge = boundingBox.m_convex[i] - boundingBox.m_convex[j];
                Vector2 normal = new Vector2(-edge.Y, edge.X);
                axis.Add(normal);

                if (AxisSeparatePolygons(normal, m_convex, boundingBox.m_convex,
                    iXYOffset, iZOffset, XYOffset, ZOffset)) {
                    return false;
                }
            }

            /*
            // find the MTD among all the separation vectors 
            MTD = FindMTD(Axis, iNumAxis); 

            // makes sure the push vector is pushing A away from B 
            Vector D = A.Position – B.Position; 
            if (D dot MTD < 0.0f) 
                MTD = -MTD; 
            */
            // this level collide, check sub boundingbox
            if (m_subBounding != null && m_subBounding.Count > 0) {
                foreach (CatsBoundingBox bounding in m_subBounding) {
                    bool result = bounding.IsCollide(boundingBox,
                        XYOffset, ZOffset,
                        iXYOffset, iZOffset);
                    if (result == true) {
                        return true;
                    }
                }
                return false;
            }
            else {
                return true;
            }
        }


        /**
         * @brief Separate Axis Algorithm
         * 
         * @param Axis the axis the points will be cast to
         * @param A the vertexes of polygon A
         * @param B the vertexes of polygon B
         * 
         * @result collide?
         * */
        bool AxisSeparatePolygons(Vector2 Axis, Vector2[] A, Vector2[] B,
            Vector2 AXYOffset, float AZOffset,
            Vector2 BXYOffset, float BZOffset) {
            float mina, maxa;
            float minb, maxb;

            Vector2 MinMax = CalculateInterval(Axis, A, AXYOffset, AZOffset);
            mina = MinMax.X;
            maxa = MinMax.Y;
            MinMax = CalculateInterval(Axis, B, BXYOffset, BZOffset);
            minb = MinMax.X;
            maxb = MinMax.Y;

            if (mina > maxb || minb > maxa) {
                return true;
            }

            // find the interval overlap 
            /*
            float d0 = maxa - minb; 
            float d1 = maxb - mina; 
            float depth = (d0 < d1)? d0 : d1; 

            
            // convert the separation axis into a push vector (re-normalise 
            // the axis and multiply by interval overlap) 
            float axis_length_squared = Axis dot Axis; 

            Axis *= depth / axis_length_squared; 
            */
            return false;
        }


        /**
         * @brief calculate the interval which hold points in P
         * 
         * @param Axis the axis the points cast to
         * @param P the points
         * @param XYOffset the offset of points on XY
         * @param ZOffset the offset of points on Z
         * 
         * @result the interval
         * */
        Vector2 CalculateInterval(Vector2 Axis, Vector2[] P,
            Vector2 XYOffset, float ZOffset) {
            float min, max;
            float d = Vector2.Dot(Axis, P[0] + XYOffset);
            min = max = d;
            int i;
            for (i = 0; i < P.Length; ++i) {
                float e = Vector2.Dot(P[i] + XYOffset, Axis);
                if (e < min) {
                    min = e;
                }
                else if (e > max) {
                    max = e;
                }
            }
            return new Vector2(min, max);
        }

        /**
         * @brief save the bounding box and its children to XML node
         * 
         * @param node the parent XML node
         * @param doc the XML doc
         * 
         * @result success?
         * */
        public bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement boundingBox = doc.CreateElement("BoundingBox");
            node.AppendChild(boundingBox);

            boundingBox.SetAttribute("isRectangle", "" + m_isRectangle);

            XmlElement heightRange = doc.CreateElement("HeightRange");
            boundingBox.AppendChild(heightRange);
            heightRange.SetAttribute("min", "" + m_heightRange.X);
            heightRange.SetAttribute("max", "" + m_heightRange.Y);

            if (m_isRectangle) {
                XmlElement xBound = doc.CreateElement("XBound");
                boundingBox.AppendChild(xBound);
                xBound.SetAttribute("min", "" + m_XBound.X);
                xBound.SetAttribute("max", "" + m_XBound.Y);

                XmlElement yBound = doc.CreateElement("YBound");
                boundingBox.AppendChild(yBound);
                yBound.SetAttribute("min", "" + m_YBound.X);
                yBound.SetAttribute("max", "" + m_YBound.Y);
            }

            // TODO: add sub boundingBox 
            return true;
        }


        /**
         * @brief create a BoundingBox and its children from XML node
         * 
         * @param node the XML node
         * @param scene the scene it belongs to
         * @param gameObject the gameObject it belong to
         * 
         * @result CatsBoundingBox
         * */
        public static CatsBoundingBox LoadFromNode(XmlNode node, Scene scene, GameObject gameObject) {
            XmlElement boundingBox = (XmlElement)node;

            bool isRectangle = bool.Parse(boundingBox.GetAttribute("isRectangle"));

            // heightRange
            XmlElement heightRange = (XmlElement)boundingBox.SelectSingleNode("HeightRange");
            Vector2 newHeightRange = new Vector2(float.Parse(heightRange.GetAttribute("min")),
                                        float.Parse(heightRange.GetAttribute("max")));
            CatsBoundingBox newBoundingBox = new CatsBoundingBox();
            newBoundingBox.m_heightRange = newHeightRange;
            newBoundingBox.m_isRectangle = isRectangle;

            if (isRectangle) {
                // XBound
                XmlElement xBound = (XmlElement)boundingBox.SelectSingleNode("XBound");
                newBoundingBox.m_XBound = new Vector2(float.Parse(xBound.GetAttribute("min")),
                                            float.Parse(xBound.GetAttribute("max")));
                // YBound
                XmlElement yBound = (XmlElement)boundingBox.SelectSingleNode("YBound");
                newBoundingBox.m_YBound = new Vector2(float.Parse(yBound.GetAttribute("min")),
                                                float.Parse(yBound.GetAttribute("max")));

                newBoundingBox.UpdateBoundFromRectangle();
            }

            // TODO: sub boundingBox
            return newBoundingBox;
        }

        public bool VerticalDownRayHit(Vector2 _rayXY, float _rayHeight, Vector2 _offsetXY, float _offsetHeight, ref Vector3 _hitPoint) {
            if (m_convex == null) {
                return false;
            }
            // vertical judge, inside may not lead to hit
            if (_rayHeight <  m_heightRange.Y + _offsetHeight) {
                return false;
            }

            int i;
            int crossCount = 0;
            for (i = 0; i < m_convex.Length; ++i) {
                Vector2 point1 = m_convex[i] + _offsetXY;
                Vector2 point2 = m_convex[(i + 1) % m_convex.Length] + _offsetXY;

                // parallel
                if (point1.Y == point2.Y) {
                    continue;
                }
                // not possible hit
                if (_rayXY.Y >= Math.Max(point1.Y, point2.Y)) {
                    continue;
                }
                if (_rayXY.Y < Math.Min(point1.Y, point2.Y)) {
                    continue;
                }
                // get hit point
                double hitX = (double)(_rayXY.Y - point1.Y) * (point2.X - point1.X) / (point2.Y - point1.Y) + point1.X;
                if (hitX < _rayXY.X) {
                    ++crossCount;
                }
            }
            _hitPoint.X = _rayXY.X;
            _hitPoint.Y = _rayXY.Y;
            _hitPoint.Z = m_heightRange.Y + _offsetHeight;
            return (crossCount % 2) != 0;
        }
    }
}
