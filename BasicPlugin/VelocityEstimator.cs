using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin {
    public class VelocityEstimator : CatComponent{
        /*
         * Estimate the velocity of GameObject
         */

        #region Properties

        [SerialAttribute]
        private readonly CatFloat m_velocitySmooth = new CatFloat(0.2f);
        public float Smooth {
            set {
                m_velocitySmooth.SetValue(value);
            }
            get {
                return m_velocitySmooth.GetValue();
            }
        }
        private Vector3 m_previousVelocity;
        private bool m_isPreviousPositionValid = false;
        private Vector3 m_previousPosition;
        private Vector3 m_velocity;
        public Vector3 Velocity {
            get {
                return m_velocity;
            }
        }

        #endregion

        public VelocityEstimator()
            : base() {

        }

        public VelocityEstimator(GameObject _gameObject)
            : base(_gameObject) {

        }
        
        public override void Initialize(Scene scene) {
            base.Initialize(scene);

            m_isPreviousPositionValid = false;
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            if (!m_isPreviousPositionValid) {
                m_isPreviousPositionValid = true;
            }
            else {
                Vector3 deltaPosition = m_gameObject.AbsPosition - m_previousPosition;
                m_velocity = m_previousVelocity * m_velocitySmooth +
                    (1.0f - m_velocitySmooth) * deltaPosition * 1000.0f / timeLastFrame;
                m_previousVelocity = m_velocity;
            }
            m_previousPosition = m_gameObject.AbsPosition;
        }
    }
}
