# Music Room VR Project

A Virtual Reality music room where users can play various instruments, starting with a fully interactive drum kit.

## Project Structure

```
Assets/
├── Models/          # 3D models and meshes
├── Materials/       # Materials and textures
├── Audio/          # Sound files and music
├── Scripts/        # C# scripts
├── Prefabs/        # Unity prefabs
├── Scenes/         # Unity scenes
├── XR/             # XR-specific settings
└── XRI/            # XR Interaction Toolkit settings
```

## Development Guidelines

### Code Organization

All code should be placed in the `Assets/Scripts` directory, organized by feature:

```
Scripts/
├── DrumKit/           # Drum-specific functionality
│   ├── Core/          # Core drum mechanics
│   └── Audio/         # Drum sound management
├── Input/             # Input handling
├── VR/                # VR-specific systems
└── Management/        # Game managers and systems
```

### Coding Standards

1. **Naming Conventions**
   - Classes: PascalCase (e.g., `DrumController`)
   - Methods: PascalCase (e.g., `HandleDrumHit`)
   - Variables: camelCase (e.g., `drumVelocity`)
   - Private fields: _camelCase (e.g., `_audioSource`)
   - Constants: UPPER_CASE (e.g., `MAX_VELOCITY`)

2. **Code Organization**
   ```csharp
   using UnityEngine;
   using System;
   // Third-party imports
   
   namespace MusicRoom.DrumKit
   {
       public class DrumController : MonoBehaviour
       {
           #region Variables
           [SerializeField] private AudioSource _audioSource;
           #endregion
   
           #region Unity Methods
           void Start() { }
           void Update() { }
           #endregion
   
           #region Public Methods
           public void HandleHit() { }
           #endregion
   
           #region Private Methods
           private void ProcessVelocity() { }
           #endregion
       }
   }
   ```


### Version Control

1. **Commit Messages**
   - Start with a verb (Add, Fix, Update, Refactor)
   - Be descriptive but concise
   - Example: "Add velocity-based sound variation to drums"

2. **Branches**
   - main: Production-ready code

## Getting Started

1. Clone the repository
2. Open in Unity 2022.3 or later
3. Install required packages:
   - XR Interaction Toolkit
   - XR Plugin Management
4. Open the main scene in `Assets/Scenes`
5. Configure your VR device in Project Settings

## Dependencies

- Unity 2022.3+
- XR Interaction Toolkit 2.3.0+
- XR Plugin Management
- Meta 
