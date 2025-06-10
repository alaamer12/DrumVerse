using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.XR;
using System.Collections.Generic;

namespace MusicRoom.Core
{
    public class SceneTransitionManager : MonoBehaviour
    {
        #region Singleton
        public static SceneTransitionManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        public const string VR_WORLD_SCENE = "VRWorldScene";
        
        [Header("VR Settings")]
        [SerializeField] private bool forceVRMode = true;
        [SerializeField] private bool allowNonVRFallback = true;

        private string[] _supportedDevices = new string[] {
            "OpenXR Display", 
            "Oculus", 
            "OpenVR Display",
            "MockHMD Display"
        };

        private bool _vrInitialized = false;

        public void LoadVRWorld()
        {
            StartCoroutine(LoadVRWorldRoutine());
        }

        private IEnumerator LoadVRWorldRoutine()
        {
            // Try to initialize VR if forced
            if (forceVRMode)
            {
                Debug.Log("[SceneTransitionManager] Attempting to initialize VR");
                yield return StartCoroutine(InitializeVR());
            }
            
            // Log status
            if (_vrInitialized)
            {
                Debug.Log("[SceneTransitionManager] VR initialized successfully");
            }
            else if (allowNonVRFallback)
            {
                Debug.LogWarning("[SceneTransitionManager] VR initialization failed, continuing in non-VR mode");
            }
            else
            {
                Debug.LogError("[SceneTransitionManager] VR initialization failed and non-VR mode is disabled");
                yield break; // Stop loading if VR failed and fallback is not allowed
            }
            
            // Load the VR scene regardless of VR status if fallback is allowed
            Debug.Log($"[SceneTransitionManager] Loading scene: {VR_WORLD_SCENE}");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(VR_WORLD_SCENE);
            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        private IEnumerator InitializeVR()
        {
            // Reset VR initialization status
            _vrInitialized = false;
            
            // Try using the modern XR subsystem approach first
            List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(displaySubsystems);

            if (displaySubsystems.Count > 0)
            {
                Debug.Log($"[SceneTransitionManager] Found {displaySubsystems.Count} XR display subsystems");
                
                // Get the first available display subsystem
                XRDisplaySubsystem displaySubsystem = displaySubsystems[0];
                
                if (!displaySubsystem.running)
                {
                    Debug.Log("[SceneTransitionManager] Starting XR Display Subsystem...");
                    displaySubsystem.Start();
                    yield return null; // Wait one frame for the change to take effect
                }
                
                _vrInitialized = displaySubsystem.running;
            }
            else
            {
                Debug.Log("[SceneTransitionManager] No XR display subsystems found, trying legacy XR settings");
                
                // Try legacy approach using a separate method
                yield return StartCoroutine(InitializeLegacyVR());
            }
            
            if (_vrInitialized)
            {
                Debug.Log("[SceneTransitionManager] VR initialized successfully");
            }
            else
            {
                Debug.LogWarning("[SceneTransitionManager] Failed to initialize VR");
            }
        }
        
        private IEnumerator InitializeLegacyVR()
        {
            // Check if legacy XR system has a device
            if (!XRSettings.isDeviceActive)
            {
                // Try loading multiple supported devices
                foreach (string deviceName in _supportedDevices)
                {
                    try
                    {
                        Debug.Log($"[SceneTransitionManager] Trying to load device: {deviceName}");
                        XRSettings.LoadDeviceByName(deviceName);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"[SceneTransitionManager] Error loading device {deviceName}: {e.Message}");
                        continue;
                    }
                    
                    // This yield is outside of try-catch
                    yield return new WaitForSeconds(0.5f);
                    
                    if (XRSettings.isDeviceActive)
                    {
                        Debug.Log($"[SceneTransitionManager] Successfully loaded device: {deviceName}");
                        _vrInitialized = true;
                        yield break;
                    }
                }
                
                // Last resort - try the empty string which sometimes defaults to the available device
                try
                {
                    XRSettings.LoadDeviceByName("");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[SceneTransitionManager] Error loading default device: {e.Message}");
                    _vrInitialized = false;
                    yield break;
                }
                
                yield return new WaitForSeconds(0.5f);
                _vrInitialized = XRSettings.isDeviceActive;
            }
            else
            {
                // Device is already active
                Debug.Log($"[SceneTransitionManager] XR Device already active: {XRSettings.loadedDeviceName}");
                _vrInitialized = true;
            }
        }
    }
}
