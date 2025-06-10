using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MusicRoom.UI
{
    [RequireComponent(typeof(Button))]
    public class StartButtonController : MonoBehaviour
    {
        private Button _startButton;
        private string _connectionSceneName = "ConnectionScene"; // Default scene name
        private string _errorSceneName = "ErrorScene"; // Default scene name
        private ConnectionManager _connectionManager;
        private bool _isConnecting = false;
        private bool _isConnected = false;

        public void Initialize(ConnectionManager connectionManager, string connectionSceneName, string errorSceneName)
        {
            Debug.Log("[StartButtonController] Initializing...");
            _connectionManager = connectionManager;
            
            if (!string.IsNullOrEmpty(connectionSceneName))
                _connectionSceneName = connectionSceneName;
                
            if (!string.IsNullOrEmpty(errorSceneName))
                _errorSceneName = errorSceneName;

            if (_connectionManager != null)
            {
                _connectionManager.OnConnectionSuccess += HandleConnectionSuccess;
                _connectionManager.OnConnectionError += HandleConnectionError;
                Debug.Log("[StartButtonController] Event handlers registered");
            }
            else
            {
                Debug.LogError("[StartButtonController] ConnectionManager is null!");
            }
        }

        public void Setup()
        {
            _startButton = GetComponent<Button>();
            if (_startButton != null)
            {
                _startButton.onClick.AddListener(OnStartPartyClicked);
                Debug.Log("[StartButtonController] Button setup complete");
            }
            else
            {
                Debug.LogError("[StartButtonController] Button component not found!");
            }
        }

        private void OnStartPartyClicked()
        {
            Debug.Log("[StartButtonController] Start button clicked");
            if (!_isConnecting && !_isConnected)
            {
                if (_connectionManager != null)
                {
                    Debug.Log("[StartButtonController] Starting connection...");
                    _isConnecting = true;
                    _connectionManager.StartConnection();
                }
                else
                {
                    Debug.LogError("[StartButtonController] Connection Manager is null, can't start connection");
                    TryLoadScene(_connectionSceneName);
                }
            }
        }

        private void HandleConnectionSuccess()
        {
            Debug.Log("[StartButtonController] Connection successful");
            _isConnecting = false;
            _isConnected = true;
            TryLoadScene(_connectionSceneName);
        }

        private void HandleConnectionError(string error)
        {
            Debug.LogWarning($"[StartButtonController] Connection error: {error}");
            _isConnecting = false;
            _isConnected = false;
            TryLoadScene(_errorSceneName);
        }

        private void TryLoadScene(string sceneName)
        {
            Debug.Log($"[StartButtonController] Attempting to load scene: {sceneName}");
            
            if (SceneUtility.GetBuildIndexByScenePath(sceneName) >= 0)
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError($"[StartButtonController] Scene '{sceneName}' does not exist in build settings!");
                // Fallback - stay on current scene
            }
        }

        private void OnDestroy()
        {
            Debug.Log("[StartButtonController] OnDestroy called");
            if (_connectionManager != null)
            {
                _connectionManager.OnConnectionSuccess -= HandleConnectionSuccess;
                _connectionManager.OnConnectionError -= HandleConnectionError;
            }

            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(OnStartPartyClicked);
            }
        }
    }
}
