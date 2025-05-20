# Code base
_____________________
## Object pooling

Any object that is added to the scene multiple times during gameplay (e.g., projectiles, NPCs, collectables) should inherit from the `IPoolable` interface. This interface minimizes the overhead of adding and removing objects by enabling objects to be added once, which are hidden and reused as needed.

The `ObjectPool` expands dynamically. In future versions, it should also support dynamic shrinking to conserve memory—ideally triggered during events like saving/loading or cutscenes to reduce performance impact.

## Actors


```mermaid
classDiagram
    Actor <|-- Player
    Actor <|-- Troop
    Troop <|-- Infantry
    Troop <|-- Ranged
    Troop <|-- Cavalry
    IPoolable <|.. Troop

    class Actor {
        + Team
        + Stats
        + Direction
        # bloodScene
        # knockback
        + HandleHit() Must be overridden
        + Die() Must be overridden
        + HandleKnockback()
        + GetTeam() Team
        + HasReachedPosition() bool
        + VelocityToward(Vector2) Vector2
        + RotateToward(float)
    }

    class Troop {
        + event RemovedFromSceneEventHandler
        + AddToScene()
        + RemoveFromScene()
        + SetState()
        + HandleHit()
        + Die()
    }

    class IPoolable {
        <<interface>>
        + AddToScene()
        + RemoveFromScene()
    }

    class Infantry {
        # MeleeWeapon
        # Timer patrolTimer
        # Area2D detectionZone
        # Area2D attackZone
        - NavigationAgent2D
        - Actor enemy
        - Rid navigationMap
        + SetState(TroopState newState)
        + Idle()
        + Walking(Vector2 velocity)
        + Attack()
        - refreshDetectionZone()
        - DetectionZoneBodyEntered(Node2D body)
        - DetectionZoneBodyExited(Node2D body)
        - AttackZoneBodyEntered(PhysicsBody2D body)
        - AttackZoneBodyExited(PhysicsBody2D body)
        - PatrolTimerTimeout()
    }

    class Ranged {
        # ProjectileWeapon
        # Timer patrolTimer
        # Area2D detectionZone
        - NavigationAgent2D
        - Actor enemy
        - Rid navigationMap
        + SetState(TroopState newState)
        + Idle()
        + Walking(Vector2 velocity)
        + Attack()
        + Deliver()
        - refreshDetectionZone()
        - DetectionZoneBodyEntered(PhysicsBody2D body)
        - DetectionZoneBodyExited(PhysicsBody2D body)
        - PatrolTimerTimeout()
    }

    class Cavalry {
        // NOT IMPLEMENTED
    }

    class Player {
        + delegate void PlayerHealthChangedEH(float newHealth)
        + delegate void PlayerGoldChangedEH(int newGold, int oldGold)
        + delegate void DiedEH()

        + WeaponManager WeaponsManager
        + AnimationPlayer animationPlayer
        + AudioStreamPlayer coinsSound
        + Joystick movementJoystick
        + Joystick attackJoystick
        + Vector2 attackDirection
        + PackedScene damagePopup
        + GUI gui
        + Globals globals
        + HandleHit(float baseDamage, Vector2 impactPosition)
        + Die()
        + GetGold(int gold)
        + SetGold(int gold)
        + Heal(float hp)
        + CancelAttack()
        + ChangeWeapon()
        + PlayIdle()
        + PlayWalking()
        + PlayAttackAnimation()
        + SetCameraTransform()
        + Save()
        + Load(Dictionary<string, Variant> data)
    }

    class TroopsManager {
        - int startingCount
        - Dictionary<TroopType, ObjectPool<Troop>> troopPool
        + Troop HandleTroopsSpawned(type, stats, Vector2 spawnPosition, origin)
    }
```



## Projectiles

```mermaid
classDiagram
    IPoolable <|.. Projectile
    Projectile <|-- Arrow
    Arrow <|-- FireArrow
    Spell <|-- Projectile
    FireSpell <|-- Spell

    class Projectile {
        + event RemovedFromSceneEH
        + ProjectileType Type
        + float Speed
        + float Damage
        + float Range
        + Team team
        # AudioStreamPlayer2D flyingSound
        # CollisionShape2D collisionShape
        # float traveledDistance
        # float moveAmount
        # Vector2 direction

        + AddToScene()
        + RemoveFromScene()
        + SetDirection(Vector2 direction)
    }

    class Arrow {
        # ArrowBodyEntered(Node body)
    }

    class FireArrow {
        - GpuParticles2D fire
        - GpuParticles2D embers
        + StartEmitting()
        + StopEmitting()
        # ArrowBodyEntered(Node body)
    }

    class Spell {
        # SpellBodyEntered(Node body)
    }

    class FireSpell {
        - GpuParticles2D fire
        - GpuParticles2D embers
        - Timer timer
        + AddToScene()
        + RemoveFromScene()
        + StartEmitting()
        + StopEmitting()
        # SpellBodyEntered(Node body)
    }

    class ProjectileManager {
        - int startingCount
        - Dictionary<ProjectileType, ObjectPool<Projectile>> projectilePool
        + HandleProjectileFired(type, damage, team, position, direction)
    }
```


## Loot

```mermaid
classDiagram
    IPoolable <|.. Coin

    class Coin {
        + event RemovedFromSceneEH
        + int Gold
        - CollisionShape2D takeArea
        - CollisionShape2D slideArea
        - CollisionShape2D collisionShape
        - Vector2 movementDirection
        - Tween tween

        + AddToScene()
        + RemoveFromScene()
        + Move(Vector2 destination)
        - TimerTimeout()
        - Area2DBodyEntered(Node body)
        - Area2DSlideBodyEntered(Node body)
        - Area2DSlideBodyExited(Node body)
    }

    class LootManager {
        - int CoinsCount
        - Dictionary<LootType, ObjectPool<Coin>> lootPool
        + HandleCoinsSpawned(int coins, Vector2 position, bool explosive)
    }
```
## Weapons


```mermaid
classDiagram
    Weapon <|-- Melee
    Weapon <|-- Projectile
    Melee <|-- Sword
    Melee <|-- Spear
    Melee <|-- Fists
    Projectile <|-- Bow
    Projectile <|-- Magic

    class Weapon {
        + float AttackDuration
        + float Damage
        + bool IsAttacking
        + Team team
        # Timer attackCooldown
        # AudioStreamPlayer2D deliverSound
        # AudioStreamPlayer2D attackSound
        + Initialize(Team team)
        + StartCooldown()
    }

    class Melee {
        # CollisionShape2D
        + Idle()
        + Attack()
        + Deliver()
        + Walking()
        + Area2DBodyEntered(Node body)
    }

    class Projectile {
        + delegate void WeaponAmmoChangedEH
        + delegate void PlayerReloadedEH
        + bool CanAim
        + int MaxAmmo
        + int CurrentAmmo
        + Marker2D WeaponDirection
        + Marker2D EndOfWeapon
        # ProjectileType projectileType
        # Globals globals
        + Attack()
        + Deliver()
        + Idle()
        + Walking()
        + bool CanAttack()
        + bool SetCurrentAmmo(int newAmmo)
    }

    class Magic {
        // NOT IMPLEMENTED
    }
```
# Material
______________________
## Sprites

## Animations

## Shaders

