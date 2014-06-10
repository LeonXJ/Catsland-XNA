using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Catsland.Plugin.BasicPlugin;

namespace HorseRiding {
    public class WonderIntro : CatComponent{

#region Properties

        [SerialAttribute]
        private string m_introGameObjectName = "";
        public string IntroGameObjectName {
            set {
                m_introGameObjectName = value;
            }
            get {
                return m_introGameObjectName;
            }
        }

        private bool m_hasIssued = false;

#endregion

        public WonderIntro()
            : base() {

        }

        public WonderIntro(GameObject _gameObject):
            base(_gameObject) {

        }

        private void DoAct() {
            ModelComponent model = ModelComponent.GetModelOfGameObjectInCurrentScene(m_introGameObjectName);
            if (model != null) {
                MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
                MovieClip movieClip = motionDelegator.AddMovieClip();

                movieClip.AppendMotion(
                    model.GetCatModelInstance().GetMaterial().GetParameter("Alpha"),
                    new CatFloat(1.0f),
                    2000);
                movieClip.AppendEmptyTime(2000);
                movieClip.AppendMotion(
                    model.GetCatModelInstance().GetMaterial().GetParameter("Alpha"),
                    new CatFloat(0.0f),
                    2000);

                movieClip.Initialize();
            }

        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            if (!m_hasIssued) {
                m_hasIssued = true;
                DoAct();
            }
        }
    }
}
