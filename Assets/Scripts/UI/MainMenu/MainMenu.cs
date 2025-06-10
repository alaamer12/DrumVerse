using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MusicRoom.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button startPartyButton;
        [SerializeField] private Button tutorialButton;

        [Header("Scene Names")]
        [SerializeField] private string connectionSceneName = "ConnectionScene";
        [SerializeField] private string errorSceneName = "ErrorScene";

        [Header("Prefabs")]
        [SerializeField] private GameObject connectionManagerPrefab;

        private ConnectionManager _connectionManager;
        private UIManager _uiManager;
        private StartButtonController _startButtonController;

        private void Start()
        {
            Debug.Log("[MainMenu] Start called");
            SetupReferences();
            SetupButtons();
        }

        private void SetupReferences()
        {
            Debug.Log("[MainMenu] Setting up references...");
            
            // First check if ConnectionManager exists
            _connectionManager = FindObjectOfType<ConnectionManager>();
            
            if (_connectionManager == null)
            {
                Debug.Log("[MainMenu] ConnectionManager not found, creating one...");
                
                if (connectionManagerPrefab != null)
                {
                    GameObject connectionObj = Instantiate(connectionManagerPrefab);
                    connectionObj.name = "ConnectionManager";
                    _connectionManager = connectionObj.GetComponent<ConnectionManager>();
                }
                else
                {
                    // Create a basic one if no prefab is assigned
                    GameObject connectionObj = new GameObject("ConnectionManager");
                    _connectionManager = connectionObj.AddComponent<ConnectionManager>();
                    DontDestroyOnLoad(connectionObj);
                }
            }

            Debug.Log($"[MainMenu] ConnectionManager found/created: {_connectionManager != null}");

            _uiManager = UIManager.Instance;
            Debug.Log($"[MainMenu] UIManager found: {_uiManager != null}");

            if (_connectionManager == null)
            {
                Debug.LogError("[MainMenu] Failed to create ConnectionManager!");
                DisableButtons();
                return;
            }
        }

        private void SetupButtons()
        {
            if (startPartyButton != null)
            {
                _startButtonController = startPartyButton.GetComponent<StartButtonController>();
                
                if (_startButtonController == null)
                {
                    // Add StartButtonController to the start button
                    _startButtonController = startPartyButton.gameObject.AddComponent<StartButtonController>();
                }
                
                _startButtonController.Initialize(_connectionManager, connectionSceneName, errorSceneName);
                _startButtonController.Setup();
                Debug.Log("[MainMenu] Start button controller set up");
            }
            else
            {
                Debug.LogError("[MainMenu] Start party button reference missing!");
            }
        }

        private void DisableButtons()
        {
            if (startPartyButton != null)
            {
                startPartyButton.interactable = false;
            }
        }
    }
}
