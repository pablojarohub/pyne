# Scene Setup Guide

## MainScene.unity Setup Instructions

### 1. Create the Player
1. Create an empty GameObject named **Player**
2. Add components:
   - `Rigidbody2D` (Body Type: Dynamic, Gravity Scale: 0, Freeze Rotation: true)
   - `BoxCollider2D` (size ~1x1)
   - `PlayerController`
   - `BikeController`
   - `WorldBounds`
3. Tag it as **Player**
4. Create child objects:
   - **BikeVisuals** (SpriteRenderer with bike sprite)
   - **CharacterVisuals** (SpriteRenderer with character sprite)
5. Assign BikeVisuals and CharacterVisuals in PlayerController inspector

### 2. Create the Camera
1. Main Camera settings:
   - Projection: **Orthographic**
   - Size: **8**
   - Background: pleasant sky color (e.g., #87CEEB)
2. Add `CameraFollow` script
   - Target: Player
   - Offset: (0, 0, -10)

### 3. Create Pickup Points (Restaurants)
Create 3 empty GameObjects and name them:
- **BurgerPlace**
- **KebabKing**
- **MarketStore**

For each:
1. Add `LocationName` script with display name
2. Add `DeliveryPoint` (isPickupPoint = true)
3. Add `BoxCollider2D` (Is Trigger = true)
4. Add child SpriteRenderer with building sprite
5. Add child **ActiveIndicator** (glowing sprite or particle)

### 4. Create Delivery Points (Houses)
Create 5-8 empty GameObjects named House1, House2, etc.
For each:
1. Add `LocationName` with display name
2. Add `DeliveryPoint` (isPickupPoint = false)
3. Add `BoxCollider2D` (Is Trigger = true)
4. Add child SpriteRenderer with house sprite

### 5. Create Environment
- Roads: Large sprites with road texture
- Sidewalks: Border sprites
- Buildings: Decorative background buildings

### 6. Create UI Canvas
Create a Canvas (Screen Space - Overlay) with:

#### MainMenuPanel
- Title Text: "FOOD RUSH DELIVERY"
- Start Button
- Quit Button
- High Score Text

#### HUDPanel
- Score Text (top-left)
- Task Text (top-center)
- Timer Text + Fill Image (top-right)
- Arrow Indicator (center, rotated toward target)

#### GameOverPanel
- "GAME OVER" title
- Final Score
- Deliveries count
- Reason text
- Restart Button
- Menu Button

### 7. Create GameManager
Create empty GameObject **GameManager**:
- Add `GameManager` script
- Assign Player, DeliveryManager, UIManager
- Add AudioSources for music and SFX
- Assign audio clips

### 8. Create DeliveryManager
Create empty GameObject **DeliveryManager**:
- Add `DeliveryManager` script
- Assign all PickupPoints and DeliveryPoints
- Assign UIManager

### 9. Assign References
- GameManager → Player, DeliveryManager, UIManager
- DeliveryManager → UIManager, all points
- UIManager → all UI panels, Player transform, Camera
- CameraFollow → Player

### 10. Physics Settings
Edit → Project Settings → Physics 2D:
- Gravity: (0, 0)
- Default Contact Offset: 0.01

## Layer Setup (Optional)
- Player
- Obstacles
- Pickup
- Delivery
- Road

## Input Setup
The game uses keyboard input (A/D, W/S) handled in PlayerController.
No need to configure Unity Input Manager for basic controls.
