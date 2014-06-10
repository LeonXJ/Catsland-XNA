using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class CatVector4 : IEffectParameter {
        private Vector4 m_value;
        public CatVector4() {
            m_value = Vector4.Zero;
        }

        public CatVector4(Vector4 _value) {
            m_value = _value;
        }

        public CatVector4(float X, float Y, float Z, float W) {
            m_value = new Vector4(X, Y, Z, W);
        }

        public static implicit operator Vector4(CatVector4 f) {
            return f.m_value;
        }

        public string ToValueString() {
            char sep = ',';
            return "" + m_value.X + sep + m_value.Y + sep + m_value.Z + sep + m_value.W;
        }

        public void SetParameter(Effect _effect, string _name) {
            _effect.Parameters[_name].SetValue(m_value);
        }

        public IEffectParameter ParameterClone() {
            return new CatVector4(m_value);
        }

        public void FromString(string _value) {
            string[] values = _value.Split(',');
            m_value.X = float.Parse(values[0]);
            m_value.Y = float.Parse(values[1]);
            m_value.Z = float.Parse(values[2]);
            m_value.W = float.Parse(values[3]);
        }

        public void SetValue(Vector4 _value) {
            m_value = _value;
        }

        public Vector4 GetValue() {
            return m_value;
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

        public float W {
            set { m_value.W = value; }
            get { return m_value.W; }
        }

    }
}
