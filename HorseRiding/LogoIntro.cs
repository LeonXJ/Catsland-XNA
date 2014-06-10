using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Catsland.Plugin.BasicPlugin;

namespace HorseRiding {
    public class LogoIntro : CatComponent {

        #region Properties

        [SerialAttribute]
        private string m_logoObjectName = "";
        public string LogoObjectName {
            set {
                m_logoObjectName = value;
            }
            get {
                return m_logoObjectName;
            }
        }

        [SerialAttribute]
        private string m_introObjectName = "";
        public string IntroObjectName {
            set {
                m_introObjectName = value;
            }
            get {
                return m_introObjectName;
            }
        }

        [SerialAttribute]
        private string m_emitterObjectName = "";
        public string EmitterObjectName {
            set {
                m_emitterObjectName = value;
            }
            get {
                return m_emitterObjectName;
            }
        }

        [SerialAttribute]
        private string m_nextSceneName = "";
        public string NextSceneName {
            set {
                m_nextSceneName = value;
            }
            get {
                return m_nextSceneName;
            }
        }

        [SerialAttribute]
        private string m_musicName = "";
        public string MusicName {
            set {
                m_musicName = value;
            }
            get {
                return m_musicName;
            }
        }

        private bool m_hasIssued = false;

        #endregion

        public LogoIntro() : base() { }
        public LogoIntro(GameObject _gameObject) :
            base(_gameObject) {

        }

        private void DoAct() {
            MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
            MovieClip movieClip = motionDelegator.AddMovieClip();
            // logo
            ModelComponent logoModel =
                ModelComponent.GetModelOfGameObjectInCurrentScene(m_logoObjectName);
            if (logoModel != null) {
                movieClip.AppendMotion(
                    logoModel.GetCatModelInstance().GetMaterial().GetParameter("Alpha"),
                    new CatFloat(1.0f),
                    2000);
                movieClip.AppendEmptyTime(2000);
                movieClip.AppendMotion(
                    logoModel.GetCatModelInstance().GetMaterial().GetParameter("Alpha"),
                    new CatFloat(0.0f),
                    2000);
            }
            // intro
            ModelComponent introModel =
                ModelComponent.GetModelOfGameObjectInCurrentScene(m_introObjectName);
            if (introModel != null) {
                movieClip.AppendMovieClip(new PlayMusic(m_musicName));
                movieClip.AppendMotion(
                    introModel.GetCatModelInstance().GetMaterial().GetParameter("Alpha"),
                    new CatFloat(1.0f),
                    2000);
                movieClip.AppendEmptyTime(3000);
            }

            // storm
            int timestamp = movieClip.GetEditCurClip();
            ParticleEmitter emitter =
                ParticleEmitter.GetParticleEmitterOfGameObjectInCurrentScene(m_emitterObjectName);
            if (emitter != null) {
                movieClip.AppendMovieClip(new StormOn(emitter));
                movieClip.AppendMotion(emitter.GenerateRatePerSecondRef,
                    new CatInteger(512), 5000);
            }

            // Screen bright
            PostProcessColorAdjustment colorAdjustment =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessColorAdjustment).ToString())
                as PostProcessColorAdjustment;
            if (colorAdjustment != null) {
                movieClip.AddMotion(colorAdjustment.IllumiateRef, new CatFloat(1.0f), timestamp, 5000);
            }
            movieClip.AppendMovieClip(new SwitchScene(m_nextSceneName));
            movieClip.Initialize();
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            if (!m_hasIssued) {
                DoAct();
                m_hasIssued = true;
            }
        }


    }

    // turn on storm
    public class StormOn : ActionClip {
        ParticleEmitter m_emitter = null;
        public StormOn(ParticleEmitter _emitter)
            : base() {
            m_emitter = _emitter;
        }

        public override void Play() {
            m_emitter.IsEmitting = true;
        }
    }

    // switch scene
    public class SwitchScene : ActionClip {

        private string m_nextSceneName = "";
        public SwitchScene(string _nextSceneName)
            : base() {
            m_nextSceneName = _nextSceneName;
        }

        public override void Play() {
            Mgr<GameEngine>.Singleton.DoSwitchScene(
                Mgr<CatProject>.Singleton.GetSceneFileAddress(m_nextSceneName));
        }
    }

    // play music
    public class PlayMusic : ActionClip {
        private string m_musicName = "";
        public PlayMusic(string _musicName)
        : base(){
            m_musicName = _musicName;
        }

        public override void Play() {
            base.Play();
            Mgr<CatProject>.Singleton.SoundManager.PlayMusic("music\\" + m_musicName);
        }
    }
}
