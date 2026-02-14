# Story 2.2: Pickup Setup — Spheres & Collection System

Status: ready-for-dev

## Story

As a player,
I want to see glowing spheres on the ground plane and collect them by moving my cube into them,
So that I can interact with the game world and gather pickups.

## Acceptance Criteria

1. A `PickupSetup.cs` setup class in `Assets/Scripts/Setup/` programmatically creates a `Pickup.prefab` in `Assets/Generated/Prefabs/`
2. The Pickup.prefab is a sphere with a SphereCollider (isTrigger = true), a Pickup MonoBehaviour, and a generated PickupMaterial (URP Lit, glowing appearance)
3. Each pickup is tagged "Pickup" via setup code
4. PickupSetup places 5 instances of the Pickup.prefab at predetermined positions on the ground plane defined as setup code constants
5. PickupMaterial is generated into `Assets/Generated/Materials/`
6. When the player cube moves into contact with a pickup sphere during gameplay, the pickup sphere is destroyed (disappears)
7. On collection, the GameManager calls `ScoreTracker.AddPoint()`
8. The collection and score update occur within the same frame
9. A previously collected pickup position has no effect when revisited
10. `Pickup.cs` (MonoBehaviour) is in `Assets/Scripts/`
11. `GameManager.cs` (MonoBehaviour) is in `Assets/Scripts/` and owns the ScoreTracker instance
12. No manual Inspector configuration is required — all prefab composition and placement is defined in PickupSetup.cs

## Tasks / Subtasks

- [ ] Task 1: Create Pickup MonoBehaviour (AC: #6, #10)
  - [ ] Create `Assets/Scripts/Pickup.cs`
  - [ ] Implement `OnTriggerEnter(Collider other)` to detect player collision
  - [ ] Check if `other.CompareTag("Player")` before processing
  - [ ] Call `GameManager.Instance.CollectPickup(this)` or notify via direct reference
  - [ ] Pickup does NOT contain game logic — it only notifies GameManager
- [ ] Task 2: Create GameManager MonoBehaviour (AC: #7, #8, #11)
  - [ ] Create `Assets/Scripts/GameManager.cs`
  - [ ] Own a `ScoreTracker` instance (initialized with total pickup count = 5)
  - [ ] Implement `CollectPickup(Pickup pickup)` method:
    - Call `_scoreTracker.AddPoint()`
    - Call `Destroy(pickup.gameObject)` to remove the pickup
  - [ ] Expose ScoreTracker events for UI subscription (OnScoreChanged, OnWinCondition)
  - [ ] Initialize ScoreTracker in `Awake()`
  - [ ] Provide a way for other components to access GameManager (serialized reference or singleton pattern — prefer serialized reference per architecture)
- [ ] Task 3: Create PickupSetup class (AC: #1, #2, #3, #4, #5, #12)
  - [ ] Create `Assets/Scripts/Setup/PickupSetup.cs` implementing `IGameSetup`
  - [ ] Set `ExecutionOrder` after PlayerSetup (e.g., order 300)
  - [ ] Generate PickupMaterial (URP Lit with emissive/bright color for glow) into `Assets/Generated/Materials/`
  - [ ] Create Pickup.prefab programmatically:
    - Sphere primitive
    - SphereCollider with `isTrigger = true`
    - Attach Pickup MonoBehaviour
    - Apply PickupMaterial
    - Tag as "Pickup"
  - [ ] Save prefab to `Assets/Generated/Prefabs/Pickup.prefab`
  - [ ] Place 5 instances at predetermined positions (define as `Vector3[]` constant)
  - [ ] Suggested positions: spread across the ground plane, e.g., `(5,0.5,5)`, `(-5,0.5,5)`, `(5,0.5,-5)`, `(-5,0.5,-5)`, `(0,0.5,0)`
- [ ] Task 4: Wire GameManager in scene setup (AC: #11)
  - [ ] Add GameManager creation to PickupSetup or SceneSetup
  - [ ] Create empty GameObject "GameManager" with GameManager component
  - [ ] Ensure GameManager is accessible to Pickup scripts (consider adding to PickupSetup since it owns the pickup count context)

## Dev Notes

### Architecture Compliance

- **Thin MonoBehaviour Pattern:** Pickup.cs only handles `OnTriggerEnter` and delegates to GameManager. No score logic in Pickup.
- **Component Architecture:** GameManager owns ScoreTracker (pure C#). Pickup notifies GameManager. No direct Pickup→ScoreTracker coupling.
- **Setup-Oriented:** Prefab creation, material generation, and scene placement all in PickupSetup.cs code constants.
- **Event Pattern:** GameManager exposes `ScoreTracker.OnScoreChanged` for future UI subscription. Use `System.Action` events.
- **Anti-pattern:** Do NOT put score logic directly in `OnTriggerEnter`. Do NOT use `FindObjectOfType` at runtime.

### Technical Requirements

- **Trigger Physics:** Pickup has SphereCollider (isTrigger=true). Player has Rigidbody + BoxCollider (non-trigger). Unity calls `OnTriggerEnter` on the Pickup when they overlap.
- **Tag Check:** `other.CompareTag("Player")` is more performant than `other.tag == "Player"`
- **Destroy Timing:** `Destroy(pickup.gameObject)` happens at end of frame but collision is detected in physics step — this satisfies NFR2 (same-frame feedback)
- **Pickup Y position:** 0.5f (half unit above ground so sphere sits on surface, matching player cube)
- **PickupMaterial glow:** Set `_EmissionColor` on URP Lit material, enable `_EMISSION` keyword for emissive glow effect
- **GameManager access pattern:** Architecture says "direct serialized references" — but since we use Setup-Oriented Generation, consider a simple singleton or static reference set during setup. A lightweight `public static GameManager Instance { get; private set; }` set in `Awake()` is acceptable for this minimal project.

### File Structure Requirements

```
Assets/
├── Scripts/
│   ├── Pickup.cs                  # NEW - trigger detection MonoBehaviour
│   ├── GameManager.cs             # NEW - owns ScoreTracker, handles collection
│   └── Setup/
│       └── PickupSetup.cs         # NEW - creates prefab + places pickups
├── Generated/
│   ├── Prefabs/
│   │   └── Pickup.prefab          # NEW - generated pickup sphere prefab
│   └── Materials/
│       └── PickupMaterial.mat     # NEW - glowing material
```

### Testing Requirements

- ScoreTracker is already tested in Story 2.1
- Pickup.cs and GameManager.cs are thin MonoBehaviours — no pure logic to EditMode test
- Verification: Moving player cube into a sphere collects it (disappears) and ScoreTracker increments

### Dependencies

- **Requires Story 1.1:** IGameSetup interface, SetupRunner
- **Requires Story 1.2:** SceneSetup with ground plane
- **Requires Story 1.3:** Player cube with "Player" tag and Rigidbody for trigger detection
- **Requires Story 2.1:** ScoreTracker pure C# class

### References

- [Source: _bmad-output/planning-artifacts/architecture.md#Physics Configuration]
- [Source: _bmad-output/planning-artifacts/architecture.md#Component Architecture]
- [Source: _bmad-output/planning-artifacts/architecture.md#Data Flow]
- [Source: _bmad-output/planning-artifacts/epics.md#Story 2.2]

## Dev Agent Record

### Agent Model Used

### Debug Log References

### Completion Notes List

### File List
