using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class CatPoint : IEffectParameter {
        private Point m_value;
        public CatPoint(){
            m_value = Point.Zero;
        }

        public CatPoint(Point _value){
            m_value = _value;
        }

        public CatPoint(int _x, int _y){
            m_value = new Point(_x, _y);
        }

        public static implicit operator Point(CatPoint _catPoint) {
            return _catPoint.m_value;
        }

        public void SetValue(Point _value) {
            m_value = _value;
        }

        public void SetValue(CatPoint _value) {
            m_value = _value.m_value;
        }

        public Point GetValue() {
            return m_value;
        }

        public void FromString(string _value) {
            string[] values = _value.Split(',');
            m_value.X = int.Parse(values[0]);
            m_value.Y = int.Parse(values[1]);
        }

        public string ToValueString() {
            char sep = ',';
            return "" + m_value.X + sep + m_value.Y;
        }

        public void SetParameter(Effect _effect, string _name) {
            _effect.Parameters[_name].SetValue(new int[2]{m_value.X, m_value.Y});
        }

        public IEffectParameter ParameterClone() {
            return new CatPoint(m_value);
        }

        public int X {
            set { m_value.X = value;}
            get { return m_value.X; }
        }

        public int Y {
            set { m_value.Y = value; }
            get { return m_value.Y; }
        }
    }
}
