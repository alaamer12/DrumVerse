# Scripts Directory Documentation

## Overview
This directory contains all C# scripts for the Music Room VR project. Follow these guidelines for consistent and maintainable code.

## Directory Structure

```
Scripts/
├── DrumKit/
│   ├── Core/
│   │   ├── DrumController.cs        # Main drum behavior
│   │   ├── DrumPiece.cs            # Individual drum piece logic
│   │   └── DrumKitManager.cs       # Overall drum kit management
│   ├── Interaction/
│   │   ├── DrumStick.cs            # Drumstick behavior
│   │   └── DrumHitDetector.cs      # Hit detection and physics
│   └── Audio/
│       ├── DrumAudioManager.cs     # Drum-specific audio
│       └── VelocityProcessor.cs    # Hit velocity processing
├── Input/
│   ├── VRInputManager.cs           # VR input handling
│   └── HandPresencePhysics.cs      # Hand physics interactions
├── VR/
│   ├── VRRig.cs                    # VR player setup
│   └── VRUIInteractor.cs           # UI interactions in VR
└── Management/
    ├── GameManager.cs              # Game state management
    └── AudioManager.cs             # Global audio system
```

## Script Templates

### Basic MonoBehaviour Template
```csharp
using UnityEngine;
using System;

namespace MusicRoom.DrumKit
{
    /// <summary>
    /// Brief description of the class purpose
    /// </summary>
    public class NewScript : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private Component _reference;
        
        [Header("Settings")]
        [SerializeField] private float _setting;
        
        private float _privateVar;
        #endregion

        #region Properties
        public float PublicProperty { get; private set; }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            // Initialize components
        }

        private void Start()
        {
            // Setup
        }

        private void Update()
        {
            // Frame update logic
        }
        #endregion

        #region Public Methods
        public void PublicMethod()
        {
            // Implementation
        }
        #endregion

        #region Private Methods
        private void PrivateMethod()
        {
            // Implementation
        }
        #endregion
    }
}
```

### Interface Template
```csharp
namespace MusicRoom.DrumKit.Interfaces
{
    /// <summary>
    /// Interface description
    /// </summary>
    public interface IInteractable
    {
        void OnInteractionStart();
        void OnInteractionEnd();
    }
}
```

## Coding Guidelines

### 1. Naming Conventions
- **Classes**: PascalCase
  ```csharp
  public class DrumController
  ```
- **Methods**: PascalCase
  ```csharp
  public void HandleDrumHit()
  ```
- **Variables**: camelCase
  ```csharp
  private float hitVelocity;
  ```
- **Properties**: PascalCase
  ```csharp
  public float Velocity { get; private set; }
  ```
- **Interfaces**: IPascalCase
  ```csharp
  public interface IHittable
  ```

### 2. Code Organization
- Use #region directives to organize code
- Keep methods short and focused
- Use dependency injection where possible
- Implement interfaces for better modularity

### 3. Performance Considerations
```csharp
public class OptimizedBehavior : MonoBehaviour
{
    // Cache component references
    private Transform _transform;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        // Cache references once
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Use non-allocating physics
    private static readonly Collider[] _hitColliders = new Collider[10];
    
    private void CheckCollisions()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(
            _transform.position, 
            1.0f, 
            _hitColliders
        );
    }
}
```

### 4. Documentation
- Use XML comments for public methods and classes
- Document complex algorithms
- Include examples for non-obvious usage

### 5. Error Handling
```csharp
public class RobustBehavior : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        if (_audioSource == null)
        {
            Debug.LogError($"AudioSource not assigned on {gameObject.name}");
            enabled = false;
            return;
        }
    }
}
```

## Common Patterns

### 1. Singleton Pattern (when necessary)
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

### 2. Event System
```csharp
public class DrumEvents : MonoBehaviour
{
    public static event Action<float> OnDrumHit;

    protected virtual void RaiseDrumHit(float velocity)
    {
        OnDrumHit?.Invoke(velocity);
    }
}
```

### 3. Object Pooling
```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _poolSize = 20;

    private Queue<GameObject> _pool;

    private void Start()
    {
        _pool = new Queue<GameObject>();
        for (int i = 0; i < _poolSize; i++)
        {
            var obj = Instantiate(_prefab);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}
```
