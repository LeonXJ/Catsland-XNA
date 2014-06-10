using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace HorseRiding {
    public class OffSreenSuicide : CatComponent {

        #region Properties

        [SerialAttribute]
        private readonly CatFloat m_suicideDistance = new CatFloat(10.0f);
        public float SuicideDistance{
            set {
                m_suicideDistance.SetValue(MathHelper.Max(value, 0.0f));
            }
            get {
                return m_suicideDistance;
            }
        }

        #endregion

        public OffSreenSuicide() : base() { }
        public OffSreenSuicide(GameObject _gameObject)
            : base(_gameObject) {
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            Camera camera = Mgr<Camera>.Singleton;
            Vector3 delta = m_gameObject.AbsPosition - camera.CameraPosition;
            if (delta.LengthSquared() > m_suicideDistance * m_suicideDistance) {
                Mgr<Scene>.Singleton._gameObjectList.RemoveItem(m_gameObject.GUID);
            }
        }
    }
}
