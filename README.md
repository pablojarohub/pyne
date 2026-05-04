# Food Rush Delivery 🚲🍔

A lightweight 2D arcade delivery game built in Unity. Ride a bicycle, pick up food orders, and deliver them as fast as possible!

## Features

- 🎮 Arcade-style bicycle movement with smooth acceleration
- 📦 Randomized pickup and delivery locations
- ⏱️ Time-based scoring with fast-delivery bonuses
- 🎯 Arrow indicator pointing to your target
- 🎨 Cartoon visual style with thick outlines
- 🔊 Sound effects for pickups and deliveries
- 📈 Increasing difficulty with each delivery

## Controls

| Key | Action |
|-----|--------|
| A / D | Move left / right |
| W | Accelerate |
| S | Brake |

## Project Structure

```
/Assets
  /Scripts
    PlayerController.cs    - Handles input and wobble animations
    BikeController.cs      - 2D physics movement
    DeliveryManager.cs     - Core delivery loop and scoring
    GameManager.cs         - Game state and high-level events
    UIManager.cs           - All UI screens and arrow indicator
    DeliveryPoint.cs       - Pickup/delivery zone triggers
    CameraFollow.cs        - Smooth camera tracking
    WorldBounds.cs         - Keeps player in the map
    Obstacle.cs            - Cars, pedestrians, etc.
  /Prefabs
  /Scenes
    MainScene.unity
  /Sprites
    /character
    /bike
    /environment
  /Audio
  /UI
    /HUD
```

## Setup Instructions

1. Open the project in Unity 2022.3 LTS or newer
2. Open `Assets/Scenes/MainScene.unity`
3. Follow `Assets/Scenes/SETUP_GUIDE.md` to configure the scene
4. Add your own sprites or use placeholder shapes
5. Press Play!

## Scripts Overview

### PlayerController.cs
- Reads A/D/W/S input
- Delegates movement to BikeController
- Manages delivery state (carrying order or not)
- Applies humorous wobble animation to bike and character

### BikeController.cs
- Uses Rigidbody2D for arcade physics
- Smooth acceleration and deceleration
- Brake force for quick stops
- Speed clamping for balanced gameplay

### DeliveryManager.cs
- Maintains lists of pickup and delivery points
- Tracks delivery state machine (Idle → Pickup → Carrying → Delivered)
- Calculates score with time bonuses
- Scales difficulty over time

### GameManager.cs
- Singleton pattern for global access
- Manages game states: Menu, Playing, GameOver
- Persists high score using PlayerPrefs
- Routes audio events

### UIManager.cs
- Main Menu, HUD, and Game Over screens
- Score, timer, and task text updates
- Arrow indicator at screen edge pointing to target
- Button click handlers

## License

MIT License - feel free to use and modify!
