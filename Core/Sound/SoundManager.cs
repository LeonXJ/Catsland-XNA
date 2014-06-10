using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Catsland.Core {
    public class SoundManager {

        /*
         * Usage:
         *  Mgr<CatProject>.Singleton.m_soundManager.PlayMusic("music\\highsea");
        */

        #region Properties
        private const float DefaultFadeInSecound = 1.0f;
        private const float DefaultFadeOutSecond = 2.0f;
        // SoundEffect
        private string m_primaryMusicName;
        private string m_secondaryMusicName;
        private SoundEffect m_primaryMusic;
        private SoundEffect m_secondaryMusic;
        private SoundEffectInstance m_primaryMusicInstance;
        private SoundEffectInstance m_secondaryMusicInstance;
        private float m_fadeInPerSecond;
        private float m_fateOutPerSecond;
        private List<SoundEffectPack> m_soundBank;
        private Dictionary<string, int> m_soundLimit;
        private Dictionary<string, int> m_playingSoundCount;
        private Random m_random = new Random();

        #endregion

        public SoundManager() {
            m_primaryMusicName = "";
            m_secondaryMusicName = "";
            m_soundBank = new List<SoundEffectPack>();
            m_soundLimit = new Dictionary<string, int>();
            m_playingSoundCount = new Dictionary<string, int>();
        }

        ~SoundManager() {
            Destory();
        }

        public void Destory() {
            if (m_primaryMusicInstance != null) {
                m_primaryMusicInstance.Dispose();
                m_primaryMusicInstance = null;
            }
            if (m_secondaryMusicInstance != null) {
                m_secondaryMusicInstance.Dispose();
                m_secondaryMusicInstance = null;
            }
        }

        // This function need some explanation
        public bool PlayMusic(string _musicName, bool _isCountinuePause = true,
            bool _is_loop = true,
            float _fadeInSecond = DefaultFadeInSecound,
            float _fadeOutSecond = DefaultFadeOutSecond) {

            if (m_primaryMusicInstance == null) {
                CreateNewPrimaryMusic(_musicName, _is_loop);
                UpdateFading(_fadeInSecond, _fadeOutSecond);
            }
            else { // m_primaryMusicInstance != null
                if (m_primaryMusicName == _musicName) {
                    UpdateFading(_fadeInSecond, _fadeOutSecond);
                }
                else { // m_primaryMusicName != _musicName
                    if (m_secondaryMusicInstance != null) {
                        if (m_secondaryMusicName == _musicName) {
                            SwapPrimarySecondary();
                            UpdateFading(_fadeInSecond, _fadeOutSecond);
                            if (!_isCountinuePause) {
                                CreateNewPrimaryMusic(_musicName, _is_loop);
                            }
                        }
                        else { // m_secondaryMusicName != _musicName
                            m_secondaryMusicInstance.Stop();
                            SwapPrimarySecondary();
                            CreateNewPrimaryMusic(_musicName, _is_loop);
                        }
                    }
                    else { // m_secondaryMusicName == null
                        SwapPrimarySecondary();
                        CreateNewPrimaryMusic(_musicName, _is_loop);
                    }
                }
            }
            //m_primaryMusicInstance.IsLooped = _is_loop;
            m_primaryMusicInstance.Play();
            return true;
        }

        public void PlaySound(string _soundName, Vector3 _position) {
            PlaySound(_soundName, _position, Vector3.UnitZ, Vector3.UnitY, Vector3.Zero);
        }

        public void RandomPlaySound(string _soundName, Vector3 _position,
            float _randomVolumeRange, float _randomPitchRange) {
            PlaySound(_soundName, _position, Vector3.UnitZ, Vector3.UnitY, Vector3.Zero,
                _randomVolumeRange, _randomPitchRange);
        }

        public void SetSoundLimit(string _soundName, int _limitNumber) {
            if (!m_soundLimit.ContainsKey(_soundName)) {
                m_soundLimit.Add(_soundName, _limitNumber);
            }
            else {
                m_soundLimit[_soundName] = _limitNumber;
            }
        }

        private bool GetSoundFromLimit(string _soundName) {
            if (m_soundLimit.ContainsKey(_soundName)) {
                if (!m_playingSoundCount.ContainsKey(_soundName)) {
                    m_playingSoundCount[_soundName] = 0;
                }
                if (m_playingSoundCount[_soundName] >= m_soundLimit[_soundName]) {
                    return false;
                }
                else {
                    m_playingSoundCount[_soundName] += 1;
                    return true;
                }
            }
            return true;
        }

        private void FreeSoundFromLimit(string _soundName) {
            if (m_playingSoundCount.ContainsKey(_soundName)) {
                m_playingSoundCount[_soundName] -= 1;
            }
        }

        public void PlaySound(string _soundName, Vector3 _position,
            Vector3 _forward, Vector3 _up, Vector3 _velocity,
            float _randomVolumeRange = 0.0f, float _randomPitchRange = 0.0f) {
            SoundEffect soundEffect = Mgr<CatProject>.Singleton.contentManger.Load<SoundEffect>
                       (_soundName);
            // get from limit
            if (!GetSoundFromLimit(_soundName)) {
                return;
            }

            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
            soundEffectInstance.Pitch =
                _randomPitchRange * 2.0f * ((float)m_random.NextDouble() - 0.5f);

            soundEffectInstance.Volume = 1.0f -
                _randomVolumeRange * (float)m_random.NextDouble();
            AudioEmitter audioEmitter = new AudioEmitter();
            AudioListener audioListener = new AudioListener();
            audioListener.Position = Mgr<Camera>.Singleton.CameraPosition;
            audioListener.Forward = Mgr<Camera>.Singleton.Forward;
            audioListener.Up = Mgr<Camera>.Singleton.Up;
            audioListener.Velocity = Mgr<Camera>.Singleton.Velocity;
            audioEmitter.DopplerScale = 10.0f;
            audioEmitter.Position = _position;
            audioEmitter.Forward = _forward;
            audioEmitter.Up = _up;
            audioEmitter.Velocity = _velocity;
            soundEffectInstance.Apply3D(audioListener, audioEmitter);
            SoundEffectPack soundEffectPack =
                new SoundEffectPack(_soundName, soundEffectInstance,
                    audioEmitter, audioListener);
            m_soundBank.Add(soundEffectPack);
            soundEffectInstance.Play();
        }

        private void SwapPrimarySecondary() {
            string tempName = m_secondaryMusicName;
            m_secondaryMusicName = m_primaryMusicName;
            m_primaryMusicName = tempName;
            SoundEffect tempSoundEffect = m_secondaryMusic;
            m_secondaryMusic = m_primaryMusic;
            m_primaryMusic = tempSoundEffect;
            SoundEffectInstance tempSoundEffectInstance = m_secondaryMusicInstance;
            m_secondaryMusicInstance = m_primaryMusicInstance;
            m_primaryMusicInstance = tempSoundEffectInstance;
        }

        private void UpdateFading(float _fadeInSecond, float _fadeOutSecond) {
            m_fadeInPerSecond = 1.0f / _fadeInSecond;
            m_fateOutPerSecond = -1.0f / _fadeOutSecond;
        }

        private void CreateNewPrimaryMusic(string _musicName, bool _is_loop) {
            if (m_primaryMusicInstance != null) {
                m_primaryMusicInstance.Dispose();
            }
            m_primaryMusic = Mgr<CatProject>.Singleton.contentManger.Load<SoundEffect>
                       (_musicName);
            m_primaryMusicInstance = m_primaryMusic.CreateInstance();
            m_primaryMusicInstance.IsLooped = _is_loop;
            m_primaryMusicInstance.Volume = 0.0f;
            m_primaryMusicName = _musicName;
        }

        public void Update(int timeLastFrame) {
            #region Update Music

            // update primary music
            if (m_primaryMusicInstance != null &&
                m_primaryMusicInstance.Volume < 1.0f) {
                float targetVolume = m_primaryMusicInstance.Volume +
                        m_fadeInPerSecond * timeLastFrame / 1000.0f;
                if (targetVolume > 1.0f) {
                    m_primaryMusicInstance.Volume = 1.0f;
                }
                else {
                    m_primaryMusicInstance.Volume = targetVolume;
                }
            }
            // update secondary music
            if (m_secondaryMusicInstance != null &&
                m_secondaryMusicInstance.State != SoundState.Paused) {
                float targetVolume = m_secondaryMusicInstance.Volume +
                    m_fateOutPerSecond * timeLastFrame / 1000.0f;
                if (targetVolume < 0.0f) {
                    m_secondaryMusicInstance.Volume = 0.0f;
                    m_secondaryMusicInstance.Pause();
                }
                else {
                    m_secondaryMusicInstance.Volume = targetVolume;
                }
            }
            #endregion
            #region Update Sound Bank
            foreach (SoundEffectPack soundEffectPack in m_soundBank) {
                if (soundEffectPack.m_soundEffectInstance.State == SoundState.Stopped) {
                    soundEffectPack.m_soundEffectInstance.Dispose();
                    FreeSoundFromLimit(soundEffectPack.m_name);
                }
                else {
                    // update camera position and
                    Camera camera = Mgr<Camera>.Singleton;
                    soundEffectPack.UpdateListener(camera.CameraPosition,
                        camera.Forward, camera.Up, camera.Velocity);
                    soundEffectPack.ApplyUpdate();
                }
            }
            m_soundBank.RemoveAll(item => item.m_soundEffectInstance.IsDisposed);
            #endregion
        }
    }
}
