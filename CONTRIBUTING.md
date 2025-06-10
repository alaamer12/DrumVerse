# Collaboration Guidelines

This guide explains how to work together effectively on our VR Music Room project without blocking each other.

## Project Structure 📁

Our project is organized to allow parallel work:

```
Assets/
├── Scenes/
│   ├── MainScene.unity              # Main VR room scene
│   └── TestScenes/                  # Each person can have their own test scene
│       ├── test_drums.unity
│       ├── test_audio.unity
│       └── test_interaction.unity
├── Scripts/
│   ├── DrumKit/                     # Drum-specific scripts
│   ├── VR/                          # VR interaction scripts
│   ├── Audio/                       # Audio management scripts
│   └── Core/                        # Shared core functionality
└── Prefabs/                         # Reusable components
```

## Understanding Prefabs 🎯

Prefabs are Unity's way of creating reusable game objects. Think of them as templates:

### Why Use Prefabs?
1. **Consistency**: Change the original prefab, all instances update automatically
2. **Reusability**: Create a drum once, use it multiple times
3. **Collaboration**: Each person can work on different prefabs without conflicts
4. **Testing**: Test your feature in isolation before adding to main scene

### Example: Drum Kit Prefabs
```
Prefabs/
├── DrumKit/
│   ├── CompleteDrumKit.prefab       # Full drum set
│   ├── Drums/
│   │   ├── KickDrum.prefab          # Base drum
│   │   ├── SnareDrum.prefab         # Snare drum
│   │   └── HiHat.prefab             # Hi-hat cymbal
│   └── DrumSticks/
│       └── DrumStick.prefab         # VR controller drumstick
```

### Working with Prefabs
1. Create your object in your test scene
2. Test it thoroughly
3. Drag it to the Prefabs folder to create a prefab
4. Use instances of your prefab in other scenes


### Script Organization
- Keep scripts modular and focused
- Use interfaces for communication between systems
- Example structure to avoid conflicts:
```csharp
// Person 1: Working on drum physics
public interface IDrumPhysics
{
    void OnHit(Vector3 position, float force);
}

// Person 2: Working on audio
public interface IDrumAudio
{
    void PlaySound(float velocity);
}

```

## Plastic SCM

1. **Before Starting Work**
   ```
   Update workspace to latest version
   ```