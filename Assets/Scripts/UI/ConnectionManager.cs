using UnityEngine;
using System;
using System.Collections;
using UnityEngine.XR.Management;
using UnityEngine.XR;
using Unity.XR.Oculus;

namespace MusicRoom.UI
{
    public class ConnectionManager : MonoBehaviour
    {
        public event Action OnConnectionSuccess;
        public event Action<string> OnConnectionError;

        [Header("Simulation Settings")]
        [SerializeField] private bool simulateSuccess = true;  // Always set to true for now
        [SerializeField] private float simulatedConnectionDelay = 2f;

        private bool _isConnecting = false;

        private void Awake()
        {
            Debug.Log("[ConnectionManager] Initializing connection manager");
        }

        public void StartConnection()
        {
            if (_isConnecting)
            {
                Debug.Log("[ConnectionManager] Connection already in progress");
                return;
            }
            
            Debug.Log("[ConnectionManager] Starting connection process");
            _isConnecting = true;
            StartCoroutine(SimulateConnection());
        }

        private IEnumerator SimulateConnection()
        {
            Debug.Log($"[ConnectionManager] Simulating connection (Success: {simulateSuccess})");
            yield return new WaitForSeconds(simulatedConnectionDelay);
            
            _isConnecting = false;
            
            if (simulateSuccess)
            {
                Debug.Log("[ConnectionManager] Connection successful");
                OnConnectionSuccess?.Invoke();
            }
            else
            {
                Debug.LogWarning("[ConnectionManager] Simulated connection failure");
                OnConnectionError?.Invoke("Simulated connection error");
            }
        }
    }
}