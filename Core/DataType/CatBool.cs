using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core{
    public class CatBool : IEffectParameter{
        private bool m_value;
        public CatBool() {
            m_value = false;
        }
        public CatBool(bool _value) {
            m_value = _value;
        }
        public static implicit operator bool(CatBool f) {
            return f.m_value;
        }
        public string ToValueString() {
            return "" + m_value;
        }

        public void SetParameter(Effect _effect, string _string) {
            _effect.Parameters[_string].SetValue(m_value);
        }

        public void FromString(string _value) {
            m_value = bool.Parse(_value);
        }

        public IEffectParameter ParameterClone() {
            return new CatBool(m_value);
        }

        public void SetValue(bool _value) {
            m_value = _value;
        }

        public bool GetValue() {
            return m_value;
        }

    }
}
