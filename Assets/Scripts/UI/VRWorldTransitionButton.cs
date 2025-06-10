using UnityEngine;
using UnityEngine.UI;
using MusicRoom.Core;
using MusicRoom.Audio;

namespace MusicRoom.UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(ButtonSoundPlayer))]
    public class VRWorldTransitionButton : MonoBehaviour
    {
        private void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(OnTransitionButtonClicked);
        }

        private void OnTransitionButtonClicked()
        {
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadVRWorld();
            }
            else
            {
                Debug.LogError("SceneTransitionManager not found in the scene!");
            }
        }
    }
}
