using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class CatMatrix : IEffectParameter {
        private Matrix m_matrix;
        public Matrix value {
            get {
                return m_matrix;
            }
            set {
                m_matrix = value;
            }
        }

        public CatMatrix() { }
        public CatMatrix(Matrix _matrix){
            m_matrix = _matrix;
        }

        public static implicit operator Matrix(CatMatrix _catMatrix){
            return _catMatrix.value;
        }

        public void SetValue(Matrix _matrix) {
            m_matrix = _matrix;
        }

        public IEffectParameter ParameterClone() {
            CatMatrix matrix = new CatMatrix();
            matrix.value = m_matrix;
            return matrix;
        }
        public void SetParameter(Effect _effect, string _name) {
            _effect.Parameters[_name].SetValue(m_matrix);
        }

        public void FromString(string _value) {
            string[] values = _value.Split(',');
            if (values.Length != 16) {
                return;
            }

            m_matrix.M11 = int.Parse(values[0]);
            m_matrix.M12 = int.Parse(values[1]);
            m_matrix.M13 = int.Parse(values[2]);
            m_matrix.M14 = int.Parse(values[3]);

            m_matrix.M21 = int.Parse(values[4]);
            m_matrix.M22 = int.Parse(values[5]);
            m_matrix.M23 = int.Parse(values[6]);
            m_matrix.M24 = int.Parse(values[7]);

            m_matrix.M31 = int.Parse(values[8]);
            m_matrix.M32 = int.Parse(values[9]);
            m_matrix.M33 = int.Parse(values[10]);
            m_matrix.M34 = int.Parse(values[11]);

            m_matrix.M41 = int.Parse(values[12]);
            m_matrix.M42 = int.Parse(values[13]);
            m_matrix.M43 = int.Parse(values[14]);
            m_matrix.M44 = int.Parse(values[15]);
        }

        public string ToValueString() {
            char sep = ',';
            return "" + m_matrix.M11 + sep + m_matrix.M12 + sep + m_matrix.M13 + sep + m_matrix.M13 + sep
                      + m_matrix.M21 + sep + m_matrix.M22 + sep + m_matrix.M23 + sep + m_matrix.M23 + sep
                      + m_matrix.M31 + sep + m_matrix.M32 + sep + m_matrix.M33 + sep + m_matrix.M33 + sep
                      + m_matrix.M41 + sep + m_matrix.M42 + sep + m_matrix.M43 + sep + m_matrix.M43;
                
        }
    }
}
