using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Catsland.Plugin.BasicPlugin;

namespace HorseRiding {
    public class FireworkController : CatComponent {
        #region Properties

        [SerialAttribute]
        private readonly CatVector3 m_randomSize = new CatVector3(2.0f, 1.0f, 1.0f);
        public Vector3 RandomRangeSize {
            set {
                float width = MathHelper.Max(value.X, 0.0f);
                float height = MathHelper.Max(value.Y, 0.0f);
                float depth = MathHelper.Max(value.Z, 0.0f);
                m_randomSize.SetValue(new Vector3(width, height, depth));
            }
            get {
                return m_randomSize;
            }
        }

        [SerialAttribute]
        private readonly CatInteger m_intervalInMS = new CatInteger(500);
        public int IntervalInMS {
            set {
                m_intervalInMS.SetValue((int)MathHelper.Max(0, value));
            }
            get {
                return m_intervalInMS;
            }
        }

        [SerialAttribute]
        private readonly CatInteger m_oneShotNumber = new CatInteger(64);
        public int OneShotNumber {
            set {
                m_oneShotNumber.SetValue((int)MathHelper.Max(value, 0));
            }
            get {
                return m_oneShotNumber;
            }
        }

        [SerialAttribute]
        private string m_emitterGameObjectName = "";
        public string EmitterGameObjectName {
            set {
                m_emitterGameObjectName = value;
            }
            get {
                return m_emitterGameObjectName;
            }
        }

        [SerialAttribute]
        private string m_fireworkSoundName = "";
        public string FireworkSoundName {
            set {
                m_fireworkSoundName = value;
            }
            get {
                return m_fireworkSoundName;
            }
        }
        [SerialAttribute]
        private readonly CatInteger m_fireworkSoundLimit = new CatInteger(3);
        public int FireworkSoundLimit {
            set {
                m_fireworkSoundLimit.SetValue((int)MathHelper.Max(0, value));
            }
            get {
                return m_fireworkSoundLimit;
            }
        }


        private Random m_random = new Random();
        private int m_accumulateMS = 0;
        #endregion

        public FireworkController() : base() { }
        public FireworkController(GameObject _gameObject):
            base(_gameObject){}

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            GameObject emitterGameObject =
                Mgr<Scene>.Singleton._gameObjectList.
                    GetOneGameObjectByName(m_emitterGameObjectName);
            if (emitterGameObject == null) {
                return;
            }
            ParticleEmitter emitter = emitterGameObject.GetComponent(
                typeof(ParticleEmitter).ToString()) as ParticleEmitter;
            if (emitter == null) {
                return;
            }

            m_accumulateMS += timeLastFrame;
            if (m_accumulateMS < m_intervalInMS) {
                return;
            }
            m_accumulateMS -= m_intervalInMS;
            // position
            Vector3 position = m_gameObject.AbsPosition - m_randomSize.GetValue()/2.0f +
                new Vector3((float)(m_random.NextDouble() * m_randomSize.X), 
                            (float)(m_random.NextDouble() * m_randomSize.Y),
                            (float)(m_random.NextDouble() * m_randomSize.Z));
            emitter.OneShot(m_oneShotNumber, position);
            // sound
            Camera camera = Mgr<Camera>.Singleton;
            Vector3 distanceToCamera = camera.CameraPosition - position;
            distanceToCamera.Z = 0.0f;
            float distance2 = distanceToCamera.LengthSquared();
            if (distance2 < 2.0f * 2.0f) {
                if (m_fireworkSoundName != "") {
                    string fireworkSoundFile = "sound\\" + m_fireworkSoundName;
                    Mgr<CatProject>.Singleton.SoundManager.SetSoundLimit(fireworkSoundFile,
                        m_fireworkSoundLimit);
                    Mgr<CatProject>.Singleton.SoundManager.RandomPlaySound(fireworkSoundFile,
                    position + new Vector3(0.0f, 0.0f, 20.0f), 0.2f, 0.2f);

                }
            }

            
            // background
//             if (m_random.NextDouble() > 0.9f) {
//                 MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
//                 MovieClip movieClip = motionDelegator.AddMovieClip();
//                 PostProcessColorAdjustment colorAdjustment =
//                     Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(
//                         typeof(PostProcessColorAdjustment).ToString())
//                         as PostProcessColorAdjustment;
//                 movieClip.AppendMotion(colorAdjustment.IllumiateRef, new CatFloat(0.6f), 100);
//                 movieClip.AppendMotion(colorAdjustment.IllumiateRef, new CatFloat(0.0f), 100);
//                 movieClip.Initialize();
//             }
            

            
        }

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);
            Update(timeLastFrame);
        }

    }
}
