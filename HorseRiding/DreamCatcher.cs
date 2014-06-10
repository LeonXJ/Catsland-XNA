using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Catsland.Plugin.BasicPlugin;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Catsland.Plugin;

namespace HorseRiding {
    public class DreamCatcher : StaticSensor, IQTEAction, IUIDrawable{
        
#region Properties

        GameObject m_player = null;

        [SerialAttribute]
        private string m_playerGameObjectName = "";
        public string PlayerGameObjectName {
            set {
                m_playerGameObjectName = value;
            }
            get {
                return m_playerGameObjectName;
            }
        }

        [SerialAttribute]
        private string m_scoreboardGameObjectName = "";
        public string ScoreboardGameObjectName {
            set {
                m_scoreboardGameObjectName = value;
            }
            get {
                return m_scoreboardGameObjectName;
            }
        }

        SpriteFont m_font;
        string m_text = "";
        bool m_isInQTE = false;
        bool m_sucess = false;
        int m_qteTimeInMS = 0;
        private Vector2 m_tipPosition;
        private Random m_random = new Random();

#endregion

        public DreamCatcher()
            : base() {

        }
        public DreamCatcher(GameObject _gameObject):
            base(_gameObject) {

        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            m_font = Mgr<CatProject>.Singleton.contentManger.Load<SpriteFont>("font\\keycodeFont");
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            scene._uiRenderer.AddUI(this);
        }

        public override void Destroy() {
            base.Destroy();
            Mgr<Scene>.Singleton._uiRenderer.RemoveUI(this);
        }

        protected override bool Enter(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {

            if (m_player != null) {
                return true;
            }

            if (_fixtureA.UserData == null && _fixtureB.UserData == null) {
                return true;
            }

            m_player = Mgr<Scene>.Singleton._gameObjectList.
                GetOneGameObjectByName(m_playerGameObjectName);
            QTE qte = m_player.GetComponent(typeof(QTE).ToString()) as QTE;
            if (qte.IsInQTE()) {
                return true;
            }
            // slow down and blur
            MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
            MovieClip movieClip = motionDelegator.AddMovieClip();
            movieClip.AppendMotion(Mgr<GameEngine>.Singleton.TimeScaleRef, new CatFloat(0.1f), 200);
            PostProcessMotionBlur motionBlur =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessMotionBlur).ToString())
                as PostProcessMotionBlur;
            int time = movieClip.GetStartTick();
            if (motionBlur != null) {
                movieClip.AddMotion(motionBlur.BlurIntensityRef, new CatFloat(0.98f), time,
                    200);
            }
            PostProcessVignette vignette =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessVignette).ToString())
                as PostProcessVignette;
            if (vignette != null) {
                movieClip.AddMotion(vignette.RadiusRef, new CatVector2(0.0f, 0.9f), time, 200);
            }
            movieClip.Initialize();
            // qte
            int qteNum = 1 + m_random.Next(4);
            for (int i = 0; i < qteNum; ++i) {
                int randKey = 65 + m_random.Next(90 - 65 + 1);
                QTEPack qtePack = new QTEPack((Keys)randKey, 800);
                qte.AppendEvent(qtePack);
            }
            qte.StartQTE(this);
            m_isInQTE = true;
            return true;
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            if (m_sucess) {
                ButterflyController bfc = m_gameObject.GetComponent(
                    typeof(ButterflyController).ToString()) as ButterflyController;
                if (bfc != null) {
                    Vector3 delta = m_player.AbsPosition + new Vector3(-0.25f, 0.15f, 0)
                        - m_gameObject.AbsPosition;
                    m_gameObject.Position += new Vector3(0.01f * delta.X, delta.Y * 0.001f, 0);
                }
            }
        }

        public void OnSuccess() {
            m_isInQTE = false;
            m_sucess = true;
            // go normal
            MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
            MovieClip movieClip = motionDelegator.AddMovieClip();
            movieClip.AppendMotion(Mgr<GameEngine>.Singleton.TimeScaleRef, new CatFloat(1.0f), 500);
            PostProcessMotionBlur motionBlur =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessMotionBlur).ToString())
                as PostProcessMotionBlur;
            int time = movieClip.GetStartTick();
            if (motionBlur != null) {
                movieClip.AddMotion(motionBlur.BlurIntensityRef, new CatFloat(0.2f), time,
                    500);
            }
            PostProcessVignette vignette =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessVignette).ToString())
                as PostProcessVignette;
            if (vignette != null) {
                movieClip.AddMotion(vignette.RadiusRef, new CatVector2(0.0f, 1.4f), time, 500);
            }
            movieClip.Initialize();
            // change color
            Random random = new Random();
            CatColor color = new CatColor();
            color.SetFromHSV(new Vector4((float)random.NextDouble(), 0.9f, 0.8f, 0.0f));
            ModelComponent parentModel = m_gameObject.GetComponent(typeof(ModelComponent).ToString())
                as ModelComponent;
            if (parentModel != null) {
                parentModel.GetCatModelInstance().GetMaterial().SetParameter("BiasColor", color);
            }
            // long tail
            if (m_gameObject.Children != null && m_gameObject.Children.Count > 0) {
                GameObject emit = m_gameObject.Children[0];
                ParticleEmitter emitter = emit.GetComponent(
                    typeof(ParticleEmitter).ToString()) as ParticleEmitter;
                emitter.ParticleLifetimeInMS = 400;

                ModelComponent childModel = emit.GetComponent(typeof(ModelComponent).ToString())
                    as ModelComponent;
                if (childModel != null) {
                    childModel.GetCatModelInstance().GetMaterial().SetParameter("BiasColor", color);
                }
            }
            // add score
            GameObject scoreboardGameObject = Mgr<Scene>.Singleton._gameObjectList.
                GetOneGameObjectByName(m_scoreboardGameObjectName);
            if (scoreboardGameObject != null) {
                HorseScoreboard scoreboard =
                    scoreboardGameObject.GetComponent(typeof(HorseScoreboard).ToString())
                    as HorseScoreboard;
                if (scoreboard != null) {
                    scoreboard.AddScore();
                }
            }
        }

        public void OnFail() {
            m_isInQTE = false;
            MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
            MovieClip movieClip = motionDelegator.AddMovieClip();
            movieClip.AppendMotion(Mgr<GameEngine>.Singleton.TimeScaleRef, new CatFloat(1.0f), 500);
            PostProcessMotionBlur motionBlur =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessMotionBlur).ToString())
                as PostProcessMotionBlur;
            int time = movieClip.GetStartTick();
            if (motionBlur != null) {
                movieClip.AddMotion(motionBlur.BlurIntensityRef, new CatFloat(0.2f), time,
                    500);
            }
            PostProcessVignette vignette =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(typeof(PostProcessVignette).ToString())
                as PostProcessVignette;
            if (vignette != null) {
                movieClip.AddMotion(vignette.RadiusRef, new CatVector2(0.0f, 1.4f), time, 500);
            }
            movieClip.Initialize();
        }

        public void Announce(Keys _key, int _time) {
            if (m_text != _key.ToString()) {
                GenerateRandomScreenSpacePosition();
                m_text = _key.ToString();
            }
            m_qteTimeInMS = _time;
        }

        private void GenerateRandomScreenSpacePosition() {
            int screenWidth =
                Mgr<GraphicsDevice>.Singleton.PresentationParameters.BackBufferWidth;
            int screenHeight =
                Mgr<GraphicsDevice>.Singleton.PresentationParameters.BackBufferHeight;
            m_tipPosition.X =
                (int)((0.5f + 0.3f * 2.0f * (m_random.NextDouble() - 0.5f)) * screenWidth);
            m_tipPosition.Y = 
                (int)((0.5f + 0.2f * 2.0f * (m_random.NextDouble() - 0.5f)) * screenHeight);
        }

        public void Draw(SpriteBatch _spriteBatch, int _timeInMS) {
            if (m_isInQTE) {
                
                
                float scale = 1.0f;
                if (m_qteTimeInMS < 500) {
                    scale =  m_qteTimeInMS / 500.0f;
                }
                
                _spriteBatch.DrawString(m_font, m_text, m_tipPosition * 0.5f,
                                new Color(1.0f, 0.0f, 0.0f, 1.5f-scale), 0.0f, 
                                new Vector2(0, 0), scale, 
                                SpriteEffects.None, 1);
            }
        }
    }
}
