using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace HorseRiding {
    public class CameraFollower : CatComponent {

        // Warning: Before Start() method is applied to CatComponent,
        // this component has one frame delay.

        #region Properties

        [SerialAttribute]
        private readonly CatFloat m_intensity = new CatFloat(1.0f);
        public float Intensity {
            set {
                m_intensity.SetValue(MathHelper.Max(value, 0.0f));
            }
            get {
                return m_intensity.GetValue();
            }
        }

        #endregion

        public CameraFollower()
            : base() {

        }

        public CameraFollower(GameObject _gameObject)
            : base(_gameObject) {

        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            if (m_enable) {
                Camera camera = Mgr<Camera>.Singleton;
                m_gameObject.Position += camera.Velocity * m_intensity
                                                    * timeLastFrame / 1000.0f;
            }
            
        }
    }
}
