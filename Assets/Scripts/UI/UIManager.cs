using UnityEngine;
using UnityEngine.SceneManagement;

namespace MusicRoom.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            Debug.Log("[UIManager] Initializing...");
            
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[UIManager] Instance created and set to DontDestroyOnLoad");
            }
            else
            {
                Debug.Log("[UIManager] Duplicate instance found, destroying this one");
                Destroy(gameObject);
            }
        }

        public void LoadScene(string sceneName)
        {
            Debug.Log($"[UIManager] Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }

        private void OnEnable()
        {
            Debug.Log("[UIManager] Enabled");
        }

        private void OnDisable()
        {
            Debug.Log("[UIManager] Disabled");
        }

        public void LoadSceneAdditive(string sceneName)
        {
            Debug.Log($"[UIManager] Loading scene additively: {sceneName}");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public void StartParty()
        {
            LoadScene("ConnectionScene");
        }

        public void ShowTutorial()
        {
            // TODO: Implement tutorial
            Debug.Log("Tutorial not implemented yet");
        }

        public void RetryConnection()
        {
            LoadScene("ConnectionScene");
        }

        public void ViewConnectionDetails()
        {
            // TODO: Implement connection details view
            Debug.Log("Connection details not implemented yet");
        }

        public void QuitApplication()
        {
            Application.Quit();
        }
    }
}
