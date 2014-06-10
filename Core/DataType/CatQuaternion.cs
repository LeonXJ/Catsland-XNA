using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class CatQuaternion : IEffectParameter {
        public Quaternion m_value;

        public CatQuaternion() {
            m_value = Quaternion.Identity;
        }

        public CatQuaternion(Quaternion _value) {
            m_value = _value;
        }

        public CatQuaternion(float _yaw, float _pitch, float _roll) {
            m_value = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
        }

        public CatQuaternion(Vector3 _axis, float _angle) {
            m_value = Quaternion.CreateFromAxisAngle(_axis, _angle);
        }

        public static implicit operator Quaternion(CatQuaternion _catQuaternion) {
            return _catQuaternion.m_value;
        }

        public void SetValue(Quaternion _value) {
            m_value = _value;
        }

        public Quaternion GetValue() {
            return m_value;
        }

        public void FromString(string _value) {
            string[] values = _value.Split(',');
            m_value.X = float.Parse(values[0]);
            m_value.Y = float.Parse(values[1]);
            m_value.Z = float.Parse(values[2]);
            m_value.W = float.Parse(values[3]);
        }

        public string ToValueString() {
            char sep = ',';
            return "" + m_value.X + sep + m_value.Y + sep + m_value.Z + sep + m_value.W;
        }

        public void SetParameter(Effect _effect, string _name) {
            _effect.Parameters[_name].SetValue(m_value);
        }

        public IEffectParameter ParameterClone() {
            return new CatQuaternion(m_value);
        }

        public static Vector3 QuaternionToEulerDegreeVector3(Quaternion _rotation) {
            Vector3 euler = new Vector3();
            // pitch
            euler.X = MathHelper.ToDegrees(
                        (float)Math.Asin(MathHelper.Clamp(2 * (_rotation.W * _rotation.X - _rotation.Y * _rotation.Z),
                                                          -1.0f, 1.0f)));
            // roll
            euler.Y = MathHelper.ToDegrees(
                        (float)Math.Atan2(2 * (_rotation.W * _rotation.Y + _rotation.Z * _rotation.X),
                                          1 - 2 * (_rotation.X * _rotation.X + _rotation.Y * _rotation.Y)));
            // yaw
            euler.Z = MathHelper.ToDegrees(
                        (float)Math.Atan2(2 * (_rotation.W * _rotation.Z + _rotation.X * _rotation.Y),
                                          1 - 2 * (_rotation.Z * _rotation.Z + _rotation.X * _rotation.X)));
            return euler;
        }
    }
}
