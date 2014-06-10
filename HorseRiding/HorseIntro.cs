using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework.Input;

namespace HorseRiding {
    public class HorseIntro : StaticSensor, IQTEAction {

#region Properties

        [SerialAttribute]
        private string m_barrierGeneraterName = "";
        public string BarrierGeneraterName {
            set {
                m_barrierGeneraterName = value;
            }
            get {
                return m_barrierGeneraterName;
            }
        }
        

#endregion


        public HorseIntro() : base() { }
        public HorseIntro(GameObject _gameObject) :
            base(_gameObject) {

        }

        protected override bool Enter(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {

            if (_fixtureA.UserData == null && _fixtureB.UserData == null) {
                return true;
            }

            UIMessageBox msgBox = 
                m_gameObject.GetComponent(typeof(UIMessageBox).ToString())
                as UIMessageBox;
            if (msgBox != null) {
                msgBox.DoShow(this);
                // do pause
                MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
                MovieClip movieClip = motionDelegator.AddMovieClip();
                movieClip.AppendMotion(Mgr<GameEngine>.Singleton.TimeScaleRef, new CatFloat(0.1f), 1000);
                PostProcessMotionBlur motionBlur =
                    Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessMotionBlur).ToString())
                    as PostProcessMotionBlur;
                int time = movieClip.GetStartTick();
                if (motionBlur != null) {
                    movieClip.AddMotion(motionBlur.BlurIntensityRef, new CatFloat(0.94f), time,
                        1000);
                }
                PostProcessColorAdjustment colorAdjustment =
                    Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessColorAdjustment).ToString())
                    as PostProcessColorAdjustment;
                if (colorAdjustment != null) {
                    movieClip.AddMotion(colorAdjustment.IllumiateRef, new CatFloat(-0.6f), time, 1000);
                }
                PostProcessVignette vignette =
                    Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessVignette).ToString())
                    as PostProcessVignette;
                if (vignette != null) {
                    movieClip.AddMotion(vignette.RadiusRef, new CatVector2(0.0f, 0.6f), time, 1000);
                }
                movieClip.Initialize();
            }

            // barrier on
            GameObject barrierGenerater = 
                Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(m_barrierGeneraterName);
            if (barrierGenerater != null) {
                EndlessBlock eb = 
                    barrierGenerater.GetComponent(typeof(EndlessBlock).ToString()) 
                    as EndlessBlock;
                if (eb != null) {
                    eb.BarrierOn = true;
                }
            }

            return true;
        }

        public void OnSuccess() {
            MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
            MovieClip movieClip = motionDelegator.AddMovieClip();
            movieClip.AppendMotion(Mgr<GameEngine>.Singleton.TimeScaleRef, new CatFloat(1.0f), 1000);
            PostProcessMotionBlur motionBlur =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessMotionBlur).ToString())
                as PostProcessMotionBlur;
            int time = movieClip.GetStartTick();
            if (motionBlur != null) {
                movieClip.AddMotion(motionBlur.BlurIntensityRef, new CatFloat(0.2f), time,
                    1000);
            }
            PostProcessColorAdjustment colorAdjustment =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessColorAdjustment).ToString())
                as PostProcessColorAdjustment;
            if (colorAdjustment != null) {
                movieClip.AddMotion(colorAdjustment.IllumiateRef, new CatFloat(0.0f), time, 1000);
            }
            PostProcessVignette vignette =
                    Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessVignette).ToString())
                    as PostProcessVignette;
            if (vignette != null) {
                movieClip.AddMotion(vignette.RadiusRef, new CatVector2(0.0f, 1.6f), time, 1000);
            }
            movieClip.Initialize();
        }

        public void OnFail() {

        }

        public void Announce(Keys _key, int _time) {

        }
    }
}
