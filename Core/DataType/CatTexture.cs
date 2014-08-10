using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class CatTexture : IEffectParameter {
        private Texture2D m_texture;
        public Texture2D value {
            get {
                return m_texture;
            }
            set {
                m_texture = value;
            }
        }

        public CatTexture() { }
        public CatTexture(Texture2D _texture) {
            m_texture = _texture;
        }

        public IEffectParameter ParameterClone() {
            CatTexture newTexture = new CatTexture();
            // do we need deep copy?
            newTexture.value = m_texture;
            return newTexture;
        }

        public void SetParameter(Effect _effect, string _name) {
            _effect.Parameters[_name].SetValue(m_texture);
        }

        public void FromString(string _value) {
            // check the path
            if (_value == "") {
                m_texture = null;
            }
            else {
                m_texture = Mgr<CatProject>.Singleton.contentManger.Load<Texture2D>("image\\" + _value);
                m_texture.Name = _value;
            }
        }

        public string ToValueString() {
            if (m_texture == null) {
                return "";
            }
            else {
                return m_texture.Name;
            }
        }
    }
}
