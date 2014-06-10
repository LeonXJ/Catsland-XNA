using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Catsland.Core {
    public class SoundEffectPack {
        public string m_name;
        public SoundEffectInstance m_soundEffectInstance;
        public AudioEmitter m_audioEmiiter;
        public AudioListener m_audioListener;

        public SoundEffectPack(string _soundName,
            SoundEffectInstance _soundEffectInstance,
            AudioEmitter _audioEmitter, AudioListener _audioListener) {
            m_name = _soundName;
            m_soundEffectInstance = _soundEffectInstance;
            m_audioEmiiter = _audioEmitter;
            m_audioListener = _audioListener;
        }

        public SoundEffectPack(string _soundName, float _volume = 1.0f,
            float _dopplerScale = 1.0f) {
            m_name = _soundName;
            SoundEffect soundEffect = Mgr<CatProject>.Singleton.contentManger.
                Load<SoundEffect>(_soundName);
            m_soundEffectInstance = soundEffect.CreateInstance();
            m_soundEffectInstance.Volume = _volume;
            m_audioListener = new AudioListener();
            m_audioEmiiter = new AudioEmitter();
            m_audioEmiiter.DopplerScale = _dopplerScale;
        }

        public void UpdateListener(Vector3 _position, Vector3 _forward,
            Vector3 _up, Vector3 _velocity) {
            m_audioListener.Position = _position;
            m_audioListener.Forward = _forward;
            m_audioListener.Up = _up;
            m_audioListener.Velocity = _velocity;
        }

        public void ApplyUpdate() {
            m_soundEffectInstance.Apply3D(m_audioListener, m_audioEmiiter);
        }
    }
}
