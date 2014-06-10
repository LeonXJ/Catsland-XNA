using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Catsland.Plugin {
    public class SoundEmitter : CatComponent {

#region Properties

        [SerialAttribute]
        private readonly CatVector3 m_offset = new CatVector3();
        public Vector3 Offset {
            set {
                m_offset.SetValue(value);
            }
            get {
                return m_offset.GetValue();
            }
        }

        Dictionary<string, List<SoundEffectPack>> m_playingSoundEffectInstance;
        Dictionary<string, Queue<SoundEffectPack>> m_freeSoundEffectInstances;

#endregion

        public SoundEmitter()
            : base() {
            CreateData();
        }

        public SoundEmitter(GameObject _gameObject)
            : base(_gameObject) {
            CreateData();
        }

        private void CreateData(){
            m_playingSoundEffectInstance = new Dictionary<string, List<SoundEffectPack>>();
            m_freeSoundEffectInstances = new Dictionary<string, Queue<SoundEffectPack>>();
        }

        public void PlaySound(string _soundName, bool _continueOld = false, 
            float _volume = 1.0f, float _dopplerScale = 1.0f) {
            // check if the sound is playing
            if (_continueOld) {
                if (m_playingSoundEffectInstance.ContainsKey(_soundName) &&
                    m_playingSoundEffectInstance[_soundName].Count() > 0) {
                    return;
                }
            }
            // check if the sound is in free queue
            SoundEffectPack soundEffectPack =
                GetFromFreeQueue(_soundName);
            // not in free queue, create new
            if (soundEffectPack == null) {
                soundEffectPack = new SoundEffectPack(_soundName, 
                    _volume, _dopplerScale);
            }
            UpdateSoundEffectPack(soundEffectPack);
            AddToPlayingQueue(_soundName, soundEffectPack);
            soundEffectPack.m_soundEffectInstance.Play();            
        }

        private SoundEffectPack GetFromFreeQueue(string _soundName) {
            if (m_freeSoundEffectInstances.ContainsKey(_soundName) &&
                m_freeSoundEffectInstances[_soundName].Count > 0) {
                // fetch the free instance
                SoundEffectPack soundEffectPack =
                    m_freeSoundEffectInstances[_soundName].Dequeue();
                if (m_freeSoundEffectInstances[_soundName].Count == 0) {
                    m_freeSoundEffectInstances.Remove(_soundName);
                }
                return soundEffectPack;
            }
            else {
                return null;
            }
        }

        private void UpdateSoundEffectPack(SoundEffectPack _pack){
            AudioEmitter emitter = _pack.m_audioEmiiter;
            emitter.Position = m_gameObject.AbsPosition + Offset;
            VelocityEstimator velocityEstimator = 
                m_gameObject.GetComponent(typeof(VelocityEstimator).ToString())
                as VelocityEstimator;
            if (velocityEstimator != null) {
                emitter.Velocity = velocityEstimator.Velocity;
            }
            
            Camera camera = Mgr<Camera>.Singleton;
            _pack.UpdateListener( camera.CameraPosition, camera.Forward, 
                camera.Up, camera.Velocity);
            _pack.ApplyUpdate();
        }

        private void AddToPlayingQueue(string _soundName, SoundEffectPack _pack) {
            if (!m_playingSoundEffectInstance.ContainsKey(_soundName)) {
                m_playingSoundEffectInstance.Add(_soundName, new List<SoundEffectPack>());
            }
            m_playingSoundEffectInstance[_soundName].Add(_pack);
        }

        private void AddToFreeQueue(string _soundName, SoundEffectPack _pack) {
            if (!m_freeSoundEffectInstances.ContainsKey(_soundName)) {
                m_freeSoundEffectInstances.Add(_soundName, new Queue<SoundEffectPack>());
            }
            m_freeSoundEffectInstances[_soundName].Enqueue(_pack);
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            List<string> keyToDelete = new List<string>();
            foreach (KeyValuePair<string, List<SoundEffectPack>> keyValue in
                m_playingSoundEffectInstance) {
                foreach (SoundEffectPack soundEffectPack in keyValue.Value) {
                    if (soundEffectPack.m_soundEffectInstance.State == SoundState.Stopped) {
                        AddToFreeQueue(keyValue.Key, soundEffectPack);
                    }
                    else {
                        UpdateSoundEffectPack(soundEffectPack);
                    }
                }
                keyValue.Value.RemoveAll(item => 
                    item.m_soundEffectInstance.State == SoundState.Stopped);
                if (keyValue.Value.Count == 0) {
                    keyToDelete.Add(keyValue.Key);
                }
            }
            // delete key
            foreach (string key in keyToDelete) {
                m_playingSoundEffectInstance.Remove(key);
            }
        }
    }
}
