using UnityEngine;
using UnityEngine.Audio;

namespace Match3
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Singleton;
        [SerializeField]
        private AudioMixerGroup _mixer;
        [SerializeField]
        private AudioSource _matchSource;
        [SerializeField]
        private AudioSource _blasterSource;
        [SerializeField]
        private AudioSource _m18Source;
        [SerializeField]
        private AudioSource _sunSource;
        [SerializeField]
        private AudioClip _match;
        [SerializeField]
        private AudioClip _toolHit;
        [SerializeField]
        private AudioClip _blaster;
        [SerializeField]
        private AudioClip _m18;
        [SerializeField]
        private AudioClip _sun;
        private AudioClip _currentClip;

        private void Start()
        {
            if (!Singleton)
            {
                Singleton = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        public void ToolHit() => _matchSource.PlayOneShot(_toolHit);

        public void Match()
        {
            if (_matchSource.isPlaying) return;
            _matchSource.PlayOneShot(_match);
        }

        public void BlasterActive()
        {
            if (_blasterSource.isPlaying) return;
            _blasterSource.PlayOneShot(_blaster);
        }

        public void M18Active()
        {
            if (_m18Source.isPlaying) return;
            _m18Source.PlayOneShot(_m18);
        }

        public void SunActive()
        {
            if (_sunSource.isPlaying) return;
            _sunSource.PlayOneShot(_sun);
        }

        public void ChangeVolume(float s) => _mixer.audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 20, s));
    }
}