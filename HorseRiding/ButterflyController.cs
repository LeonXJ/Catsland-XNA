using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace HorseRiding {
    public class ButterflyController : CatComponent{

#region Properties

        [SerialAttribute]
        private readonly CatFloat m_verticalCycleSmall = new CatFloat(1.0f);
        public float VerticalCycleSmall {
            set {
                m_verticalCycleSmall.SetValue(
                          MathHelper.Clamp(value, 0.0f, m_verticalCycleLarge));
            }
            get {
                return m_verticalCycleSmall.GetValue();
            }
        }
        [SerialAttribute]
        private readonly CatFloat m_verticalAmplitudeSmall = new CatFloat(1.0f);
        public float VerticalAmplitudeSmall {
            set {
                m_verticalAmplitudeSmall.SetValue(MathHelper.Max(value, 0));
            }
            get {
                return m_verticalAmplitudeSmall;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_verticalCycleLarge = new CatFloat(3.0f);
        public float VerticalCycleLarge {
            set {
                m_verticalCycleLarge.SetValue(
                                MathHelper.Max(value, m_verticalCycleSmall));
            }
            get {
                return m_verticalCycleLarge.GetValue();
            }
        }
        [SerialAttribute]
        private readonly CatFloat m_verticalAmplitudeLarge = new CatFloat(1.0f);
        public float VerticalAmplitudeLarge {
            set {
                m_verticalAmplitudeLarge.SetValue(MathHelper.Max(value, 0));
            }
            get {
                return m_verticalAmplitudeLarge;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_horizontalVelocity = new CatFloat(0.0f);
        public float HorizontalVelocity {
            set {
                m_horizontalVelocity.SetValue(value);
            }
            get {
                return m_horizontalVelocity.GetValue();
            }
        }

        private float m_verticalPhaseSmall = 0.0f;
        private float m_verticalPhaseLarge = 0.0f;

        

#endregion

        public ButterflyController() : base() { }
        public ButterflyController(GameObject _gameObject)
            : base(_gameObject) {
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            float timeInSecond = timeLastFrame / 1000.0f;
            float verticalVelocity = CalculateVerticalVelocity(timeInSecond);

            m_gameObject.Position += new Vector3(m_horizontalVelocity,
                                                 verticalVelocity,
                                                 0) * timeInSecond;
        }

        protected float CalculateVerticalVelocity(float _timeInSecond) {

            float velocity = 0.0f;

            m_verticalPhaseSmall += _timeInSecond;
            if (m_verticalPhaseSmall > m_verticalCycleSmall) {
                m_verticalPhaseSmall -= m_verticalCycleSmall;
            }
            velocity += m_verticalAmplitudeSmall *
                            (float)Math.Sin(Math.PI * 2 * m_verticalPhaseSmall
                                                       / m_verticalCycleSmall);

            m_verticalPhaseLarge += _timeInSecond;
            if (m_verticalPhaseLarge > m_verticalCycleLarge) {
                m_verticalPhaseLarge -= m_verticalCycleLarge;
            }
            velocity += m_verticalAmplitudeLarge *
                            (float)Math.Sin(Math.PI * 2 * m_verticalPhaseLarge
                                                       / m_verticalCycleLarge);

            return velocity;
        }
    }
}
