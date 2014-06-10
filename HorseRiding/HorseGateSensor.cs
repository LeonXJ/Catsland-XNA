using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace HorseRiding {
    public class HorseGateSensor : StaticSensor{

#region Properties

        [SerialAttribute]
        private string m_wonderMusic = "";
        public string WonderMusic {
            set {
                m_wonderMusic = value;
            }
            get {
                return m_wonderMusic;
            }
        }

#endregion

        public HorseGateSensor():base(){}
        public HorseGateSensor(GameObject _gameObject):
            base(_gameObject){}

        protected override bool Enter(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {

            if (_fixtureA.UserData == null && _fixtureB.UserData == null) {
                return true;
            }
            
            PostProcessColorAdjustment colorAdjustment = 
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(
                    typeof(PostProcessColorAdjustment).ToString())
                    as PostProcessColorAdjustment;
            if (colorAdjustment != null) {
                MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
                MovieClip movieClip = motionDelegator.AddMovieClip();
                movieClip.AppendMotion(colorAdjustment.IllumiateRef, new CatFloat(-1.0f), 3000);
                movieClip.Initialize();
            }

            Mgr<CatProject>.Singleton.SoundManager.PlayMusic("music\\" + m_wonderMusic, true, true, 5.0f, 2.0f);
            return true;
        }
    }
}
