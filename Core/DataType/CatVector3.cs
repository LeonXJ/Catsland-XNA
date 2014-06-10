using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class CatVector3 : IEffectParameter {
        public Vector3 m_value;
        
        public CatVector3() {
            m_value = Vector3.Zero;
        }

        public CatVector3(Vector3 _value) {
            m_value = _value;
        }

        public CatVector3(float _x, float _y, float _z) {
            m_value = new Vector3(_x, _y, _z);
        }

        public static CatVector3 operator -(CatVector3 _value1, CatVector3 _value2) {
            return new CatVector3(_value1.GetValue() - _value2.GetValue());
        }

        public static implicit operator Vector3(CatVector3 f) {
            return f.m_value;
        }

        public void SetValue(Vector3 _value) {
            m_value = _value;
        }

        public Vector3 GetValue() {
            return m_value;
        }

        public void FromString(string _value) {
            string[] values = _value.Split(',');
            m_value.X = float.Parse(values[0]);
            m_value.Y = float.Parse(values[1]);
            m_value.Z = float.Parse(values[2]);
        }

        public string ToValueString() {
            char sep = ',';
            return "" + m_value.X + sep + m_value.Y + sep + m_value.Z;
        }

        public void SetParameter(Effect _effect, string _name) {
            _effect.Parameters[_name].SetValue(m_value);
        }

        public IEffectParameter ParameterClone() {
            return new CatVector3(m_value);
        }

        public float X {
            set { m_value.X = value; }
            get { return m_value.X; }
        }

        public float Y {
            set { m_value.Y = value; }
            get { return m_value.Y; }
        }

        public float Z {
            set { m_value.Z = value; }
            get { return m_value.Z; }
        }
    }
}
