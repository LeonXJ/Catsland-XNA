using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class CatVector2 : IEffectParameter {
        private Vector2 m_value;
        public CatVector2() {
            m_value = Vector2.Zero;
        }

        public CatVector2(Vector2 _value) {
            m_value = _value;
        }

        public CatVector2(float _x, float _y) {
            m_value = new Vector2(_x, _y);
        }


        public static CatVector2 operator -(CatVector2 _value1, CatVector2 _value2) {
            return new CatVector2(_value1.GetValue() - _value2.GetValue());
        }

        public static implicit operator Vector2(CatVector2 f) {
            return f.m_value;
        }

        public void SetValue(Vector2 _value) {
            m_value = _value;
        }

        public Vector2 GetValue() {
            return m_value;
        }

        public void FromString(string _value) {
            string[] values = _value.Split(',');
            m_value.X = float.Parse(values[0]);
            m_value.Y = float.Parse(values[1]);
        }

        public string ToValueString() {
            char sep = ',';
            return "" + m_value.X + sep + m_value.Y;
        }

        public void SetParameter(Effect _effect, string _name) {
            _effect.Parameters[_name].SetValue(m_value);
        }

        public IEffectParameter ParameterClone() {
            return new CatVector2(m_value);
        }

        public float X {
            set { m_value.X = value; }
            get { return m_value.X; }
        }

        public float Y {
            set { m_value.Y = value; }
            get { return m_value.Y; }
        }
    }
}
