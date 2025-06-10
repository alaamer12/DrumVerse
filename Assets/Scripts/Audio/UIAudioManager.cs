using UnityEngine;

namespace MusicRoom.Audio
{
    public class UIAudioManager : MonoBehaviour
    {
        #region Singleton
        public static UIAudioManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSource();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        private AudioSource _audioSource;

        private void InitializeAudioSource()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        public void PlayUISound(AudioClip sound, float volume = 1f)
        {
            if (sound == null) return;
            
            _audioSource.clip = sound;
            _audioSource.volume = volume;
            _audioSource.Play();
        }
    }
}
