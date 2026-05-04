# Prefabs

Create the following prefabs by dragging configured GameObjects from the Hierarchy into this folder:

## Player.prefab
- Player GameObject with all scripts and colliders
- Child: BikeVisuals
- Child: CharacterVisuals

## Bike.prefab
- (Optional) Separate bike visual prefab for reuse

## Building.prefab
- Base building with LocationName and Collider2D
- Configure as pickup or delivery point

## PickupPoint.prefab
- Restaurant/shop with DeliveryPoint (isPickupPoint = true)
- Active indicator child
- Particle system child

## DeliveryPoint.prefab
- House/building with DeliveryPoint (isPickupPoint = false)
- Active indicator child
- Particle system child

## ObstacleCar.prefab
- Car sprite with Obstacle script
- Rigidbody2D (Kinematic) or simple collider
- Moves back and forth (optional script)

## ObstaclePedestrian.prefab
- Pedestrian sprite with Obstacle script
- Simple wander AI (optional)

## How to Create Prefabs
1. Configure a GameObject in the scene exactly as needed
2. Drag it from Hierarchy into this /Prefabs folder
3. Delete the original from the scene (keep the prefab)
4. Instantiate prefabs via scripts or drag into scenes as needed
