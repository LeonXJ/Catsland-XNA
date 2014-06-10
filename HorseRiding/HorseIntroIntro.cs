using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace HorseRiding {
    public class HorseIntroIntro : CatComponent {

        #region

        private bool m_hasIssued = false;

        #endregion

        public HorseIntroIntro() : base() { }
        public HorseIntroIntro(GameObject _gameObject) :
            base(_gameObject) { }

        private void DoAct() {

            PostProcessColorAdjustment colorAdjustment =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(
                typeof(PostProcessColorAdjustment).ToString())
                as PostProcessColorAdjustment;
            if (colorAdjustment != null) {
                MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
                MovieClip movieClip = motionDelegator.AddMovieClip();
                movieClip.AppendMotion(colorAdjustment.IllumiateRef,
                    new CatFloat(0.0f), 2000);
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
