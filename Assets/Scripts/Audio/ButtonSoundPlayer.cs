using UnityEngine;
using UnityEngine.UI;

namespace MusicRoom.Audio
{
    [RequireComponent(typeof(Button))]
    public class ButtonSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip clickSound;
        [SerializeField] [Range(0f, 1f)] private float volume = 1f;

        private void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(PlayClickSound);
        }

        private void PlayClickSound()
        {
            if (UIAudioManager.Instance != null)
            {
                UIAudioManager.Instance.PlayUISound(clickSound, volume);
            }
        }
    }
}
