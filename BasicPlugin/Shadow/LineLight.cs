using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


// NOT CORRECT, THE DIRECTION OF LINE LIGHT IS COMPLICATE

namespace Catsland.Plugin.BasicPlugin {
    public class LineLight : Light{

#region Properties

        [SerialAttribute]
        private readonly CatVector2 m_direction = new CatVector2(0.0f, -1.0f);
        public Vector2 Direction {
            set {
                if (value.LengthSquared() < 0.001f) {
                    return;
                }
                m_direction.SetValue(Vector2.Normalize(value));
                UpdateVertex();
            }
            get {
                return m_direction;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_nearWidth = new CatFloat(0.2f);
        public float NearWidth {
            set {
                m_nearWidth.SetValue(MathHelper.Max(0.0f, value));
                UpdateVertex();
            }
            get {
                return m_nearWidth;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_farWidth = new CatFloat(0.5f);
        public float FarWidth {
            set {
                m_farWidth.SetValue(MathHelper.Max(0.0f, value));
                UpdateVertex();
            }
            get {
                return m_farWidth;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_farDistance = new CatFloat(0.5f);
        public float FarDistance {
            set {
                m_farDistance.SetValue(MathHelper.Max(0.0f, value));
                UpdateVertex();
            }
            get {
                return m_farDistance;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_fadeDistance = new CatFloat(0.5f);
        public float FadeDistance {
            set {
                m_fadeDistance.SetValue(MathHelper.Clamp(value, 0.0f, m_farDistance));
            }
            get {
                return m_fadeDistance;
            }
        }

        static private Effect m_lineLightEffect;

#endregion

        public LineLight(GameObject _gameObject)
            : base(_gameObject) { }

        public LineLight()
            : base() { }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            UpdateVertex();
            UpdateEffect();
        }

        private void UpdateEffect() {
            if (m_lineLightEffect == null) {
                m_lineLightEffect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>
                    ("effect\\LineLight");
            }
        }

        private void UpdateVertex() {
            if (m_verticeList == null) {
                m_verticeList = new List<Vector2>(4);
                for (int i = 0; i < 4; ++i) {
                    m_verticeList.Add(Vector2.Zero);
                }
            }
            /*********
             *     0 - near width - 1
             *    /        |         \
             *   /       height       \
             *  /          |           \
             * 3 ----- far width ------ 2
             ******** */
            
            Vector2 parDirection = new Vector2(m_direction.Y, -m_direction.X);
            m_verticeList[0] = -parDirection * m_nearWidth * 0.5f;
            m_verticeList[1] = parDirection * m_nearWidth * 0.5f;
            m_verticeList[2] = m_direction.GetValue() * m_farDistance.GetValue()
                + parDirection * m_farWidth * 0.5f;
            m_verticeList[3] = m_direction.GetValue() * m_farDistance.GetValue()
                - parDirection * m_farWidth * 0.5f;
            UpdateDrawVertex();
        }

        public Vector2 GetCentroidInWorld() {
            return new Vector2(m_gameObject.AbsPosition.X, m_gameObject.AbsPosition.Y)
                + m_offset;
        }

        public override Vector2 GetLightDirection(Vector2 _point) {
            Vector2 centroidInWorld = GetCentroidInWorld();
            Matrix translateMatrx = Matrix.CreateTranslation(new Vector3(m_offset.X, m_offset.Y, 0.0f))
                * m_gameObject.AbsTransform;
            Vector2 farPointInWorld = Vector2.Transform(m_direction.GetValue() * m_farDistance.GetValue(),
                                            translateMatrx);        
            Vector2 v0 = farPointInWorld - centroidInWorld;
            Vector2 v1 = _point - centroidInWorld;
            Vector2 paraDirection = new Vector2(v0.Y, -v0.X);
            paraDirection.Normalize();
            float castLength = Vector2.Dot(v0, v1) / v0.Length();
            float standardWidth = NearWidth + (castLength / FarDistance) * (FarWidth - NearWidth);
            float paraLength = Vector2.Dot(paraDirection, v1);
            if (standardWidth < 0.001f) {
                return -Vector2.UnitY;
            }

            float percent = paraLength * 2.0f / standardWidth;
            Vector2 point1 = centroidInWorld + paraDirection * NearWidth * percent;
            Vector2 point2 = farPointInWorld + paraDirection * FarDistance * percent;
            return Vector2.Normalize(point2 - point1);
        }

        public override void Draw(int timeLastFrame) {
            base.Draw(timeLastFrame);
        

        
        }

        public override bool IsBodyInLight(ShadingBody _shadingBody) {
            return CatMath.IsConvexIntersect(
                m_verticeList.ToArray(), 
                Matrix.CreateTranslation(new Vector3(m_offset.X, m_offset.Y, 0.0f))
                    * m_gameObject.AbsTransform,
                _shadingBody.GetVertices(),
                Matrix.CreateTranslation(new Vector3(_shadingBody.Offset.X, _shadingBody.Offset.Y, 0.0f))
                    * _shadingBody.m_gameObject.AbsTransform);
        }

        public static new string GetMenuNames() {
            return "Shadow|Line Light";
        }
    }

}
