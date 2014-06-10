using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class CatFloat : IEffectParameter {
        private float m_value;
        
        public CatFloat() {
            m_value = 0.0f;
        }

        public CatFloat(float _value) {
            m_value = _value;
        }

        public static implicit operator float(CatFloat f){
            return f.m_value;
        }

        public IEffectParameter ParameterClone() {
            return new CatFloat(m_value);
        }

        public void SetParameter(Effect _effect, string _name) {
            _effect.Parameters[_name].SetValue(m_value);
        }

        public void FromString(string _value) {
            m_value = float.Parse(_value);
        }

        public string ToValueString() {
            return m_value.ToString();
        }

        public void SetValue(float _value) {
            m_value = _value;
        }

        public float GetValue() {
            return m_value;
        }
    }
}
