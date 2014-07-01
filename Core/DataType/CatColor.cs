using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class CatColor : IEffectParameter {
        public Vector4 m_value;

        public CatColor() {
            m_value = Vector4.Zero;
        }
        
        public CatColor(float _r, float _g, float _b, float _a){
            m_value = new Vector4(_r, _g, _b, _a);
        }

        public CatColor(Vector4 _color){
            m_value = _color;
        }

        public float R {
            get {
                return m_value.X;
            }
        }

        public float G {
            get {
                return m_value.Y;
            }
        }

        public float B {
            get {
                return m_value.Z;
            }
        }

        public float A {
            get {
                return m_value.W;
            }
        }

        public Vector3 RGB {
            get {
                return new Vector3(R, G, B);
            }
        }

        public void SetValue(Color _color) {
            m_value.X = _color.R / 255.0f;
            m_value.Y = _color.G / 255.0f;
            m_value.Z = _color.B / 255.0f;
            m_value.W = _color.A / 255.0f;
        }
        
        public static implicit operator Vector4(CatColor _color){
            return _color.m_value; 
        }

        public static implicit operator Color(CatColor _color) {
            Color color = new Color((int)(_color.m_value.X * 255),
                             (int)(_color.m_value.Y * 255),
                             (int)(_color.m_value.Z * 255),
                             (int)(_color.m_value.W * 255));
            return color;
        }

        public string ToValueString() {
            char sep = ',';
            return "" + m_value.X + sep + m_value.Y + sep + m_value.Z + sep + m_value.W;
        }

        public void SetParameter(Effect _effect, string _name) {
            _effect.Parameters[_name].SetValue(m_value);
        }

        public IEffectParameter ParameterClone() {
            return new CatColor(m_value);
        }

        public void FromString(string _value) {
            string[] values = _value.Split(',');
            m_value.X = float.Parse(values[0]);
            m_value.Y = float.Parse(values[1]);
            m_value.Z = float.Parse(values[2]);
            m_value.W = float.Parse(values[3]);
        }

        public void SetFromHSV(Vector4 _hsva) {
            Vector3 hueColor = GetColorByHue(_hsva.X);
            Vector3 result = Vector3.Lerp(Vector3.Zero,
                        Vector3.Lerp(Vector3.One, hueColor, _hsva.Y),
                        _hsva.Z);
            m_value.X = result.X;
            m_value.Y = result.Y;
            m_value.Z = result.Z;
            m_value.W = _hsva.W;
        }

        private Vector3 GetColorByHue(float _hue) {
            Vector3 color;
            float hue = MathHelper.Clamp(_hue, 0, 1);
            float hue6 = hue * 6.0f;
            int z = (int)MathHelper.Min(hue6, 5);
            float f = hue6 - z;
            switch ((int)(hue * 6.0f)) {
                case 0:
                    color.X = 1.0f;
                    color.Y = f;
                    color.Z = 0.0f;
                    break;
                case 1:
                    color.X = 1.0f - f;
                    color.Y = 1.0f;
                    color.Z = 0.0f;
                    break;
                case 2:
                    color.X = 0.0f;
                    color.Y = 1.0f;
                    color.Z = f;
                    break;
                case 3:
                    color.X = 0.0f;
                    color.Y = 1.0f - f;
                    color.Z = 1.0f;
                    break;
                case 4:
                    color.X = f;
                    color.Y = 0.0f;
                    color.Z = 1.0f;
                    break;
                default:
                    color.X = 1.0f;
                    color.Y = 0.0f;
                    color.Z = 1.0f - f;
                    break;
            }
            return color;
        }
    }
}
