using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class CatInteger : IEffectParameter {
        private int m_value;
        public CatInteger() {
            m_value = 0;
        }
        public CatInteger(int _value) {
            m_value = _value;
        }

        public static implicit operator int(CatInteger f) {
            return f.m_value;
        }

        public string ToValueString() {
            return "" + m_value;
        }

        public void SetParameter(Effect _effect, string _string) {
            _effect.Parameters[_string].SetValue(m_value); 
        }

        public void FromString(string _value) {
            m_value = int.Parse(_value);
        }

        public IEffectParameter ParameterClone() {
            return new CatInteger(m_value);
        }

        public void SetValue(int _value) {
            m_value = _value;
        }

        public int GetValue() {
            return m_value;
        }

    }
}