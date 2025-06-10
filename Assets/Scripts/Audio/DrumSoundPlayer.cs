using UnityEngine;

namespace MusicRoom.DrumKit.Audio
{
    public class DrumSoundPlayer : MonoBehaviour
    {
        #region Variables
        [Header("Audio Settings")]
        [SerializeField] private AudioClip _drumSound;
        [SerializeField] private float _volume = 1.0f;
        [SerializeField] private KeyCode _keyToPress = KeyCode.None;
        
        private AudioSource _audioSource;
        #endregion

        #region Unity Methods
        private void Start()
        {
            // Get or add AudioSource component
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Configure audio source
            _audioSource.playOnAwake = false;
            _audioSource.clip = _drumSound;
            _audioSource.volume = _volume;
        }

        private void Update()
        {
            // Play sound when assigned key is pressed
            if (Input.GetKeyDown(_keyToPress))
            {
                PlaySound();
            }
        }
        
        private void OnMouseDown()
        {
            // Play sound when clicked with mouse
            PlaySound();
        }
        #endregion

        #region Public Methods
        public void PlaySound()
        {
            if (_audioSource != null && _drumSound != null)
            {
                _audioSource.Play();
            }
        }
        #endregion
    }
} 