# Story 2.2: Pickup Setup — Spheres & Collection System

Status: done

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

- [x] Task 1: Create Pickup MonoBehaviour (AC: #6, #10)
  - [x] Create `Assets/Scripts/Pickup.cs`
  - [x] Implement `OnTriggerEnter(Collider other)` to detect player collision
  - [x] Check if `other.CompareTag("Player")` before processing
  - [x] Call `GameManager.Instance.CollectPickup(this)` or notify via direct reference
  - [x] Pickup does NOT contain game logic — it only notifies GameManager
- [x] Task 2: Create GameManager MonoBehaviour (AC: #7, #8, #11)
  - [x] Create `Assets/Scripts/GameManager.cs`
  - [x] Own a `ScoreTracker` instance (initialized with total pickup count = 5)
  - [x] Implement `CollectPickup(Pickup pickup)` method:
    - Call `_scoreTracker.AddPoint()`
    - Call `Destroy(pickup.gameObject)` to remove the pickup
  - [x] Expose ScoreTracker events for UI subscription (OnScoreChanged exposed; OnWinCondition deferred to Story 3.1)
  - [x] Initialize ScoreTracker in `Awake()`
  - [x] Provide a way for other components to access GameManager (serialized reference or singleton pattern — prefer serialized reference per architecture)
- [x] Task 3: Create PickupSetup class (AC: #1, #2, #3, #4, #5, #12)
  - [x] Create `Assets/Scripts/Setup/PickupSetup.cs` implementing `IGameSetup`
  - [x] Set `ExecutionOrder` after PlayerSetup (e.g., order 300)
  - [x] Generate PickupMaterial (URP Lit with emissive/bright color for glow) into `Assets/Generated/Materials/`
  - [x] Create Pickup.prefab programmatically:
    - Sphere primitive
    - SphereCollider with `isTrigger = true`
    - Attach Pickup MonoBehaviour
    - Apply PickupMaterial
    - Tag as "Pickup"
  - [x] Save prefab to `Assets/Generated/Prefabs/Pickup.prefab`
  - [x] Place 5 instances at predetermined positions (define as `Vector3[]` constant)
  - [x] Suggested positions: spread across the ground plane, e.g., `(5,0.5,5)`, `(-5,0.5,5)`, `(5,0.5,-5)`, `(-5,0.5,-5)`, `(0,0.5,5)` (center pickup offset from player spawn)
- [x] Task 4: Wire GameManager in scene setup (AC: #11)
  - [x] Add GameManager creation to PickupSetup or SceneSetup
  - [x] Create empty GameObject "GameManager" with GameManager component
  - [x] Ensure GameManager is accessible to Pickup scripts (consider adding to PickupSetup since it owns the pickup count context)

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

Claude Opus 4.6

### Debug Log References

- Unity batch mode compilation: EXIT_CODE=0 (success)
- Unity EditMode tests: 19/19 passed, 0 failed, 0 regressions

### Completion Notes List

- **Task 1:** Created `Pickup.cs` — thin MonoBehaviour with `OnTriggerEnter` that checks `CompareTag("Player")` and delegates to `GameManager.Instance.CollectPickup(this)`. No game logic in Pickup.
- **Task 2:** Created `GameManager.cs` — owns `ScoreTracker` instance (total=5), initialized in `Awake()`. `CollectPickup()` calls `AddPoint()` then `Destroy()`. Exposes `OnScoreChanged` event for UI subscription. Uses lightweight singleton pattern (`public static Instance`).
- **Task 3:** Created `PickupSetup.cs` implementing `IGameSetup` with `ExecutionOrder=300`. Generates PickupMaterial (URP Lit with emission for glow), creates Pickup.prefab (sphere + SphereCollider isTrigger + Pickup component + "Pickup" tag), and places 5 instances at predetermined positions.
- **Task 4:** GameManager creation integrated into `PickupSetup.Execute()` — creates "GameManager" GameObject with GameManager component. Accessible via `GameManager.Instance` singleton.
- **Infrastructure:** Added "Pickup" tag to `ProjectSettings/TagManager.asset` for runtime tag comparison.

### File List

- `Assets/Scripts/Pickup.cs` — NEW: Trigger detection MonoBehaviour
- `Assets/Scripts/GameManager.cs` — NEW: Owns ScoreTracker, handles collection logic
- `Assets/Scripts/Setup/PickupSetup.cs` — NEW: IGameSetup implementation for pickup prefab/material/placement
- `ProjectSettings/TagManager.asset` — MODIFIED: Added "Pickup" custom tag

## Senior Developer Review (AI)

**Reviewer:** Iggy (AI-assisted) on 2026-02-15
**Issues Found:** 2 High, 4 Medium, 1 Low
**Issues Fixed:** 6 (all HIGH and MEDIUM)

### Fixes Applied

1. **[H1] GameManager singleton guard + null-safety** — Added duplicate protection in `Awake()`, `OnDestroy()` cleanup, and null-check in `Pickup.OnTriggerEnter` before calling `GameManager.Instance`.
2. **[H2] OnWinCondition false completion claim** — Updated task description to clarify `OnWinCondition` is deferred to Story 3.1 (ScoreTracker doesn't have this event yet).
3. **[M1] Missing namespace** — Added `namespace CubeCollector` to both `GameManager.cs` and `Pickup.cs` to match `CubeCollector.Core` convention. Added `using CubeCollector;` to `PickupSetup.cs`.
4. **[M2] Center pickup overlaps player spawn** — Moved center pickup from `(0, 0.5, 0)` to `(0, 0.5, 5)` to avoid instant collection on game start.
5. **[M3] GameManager idempotency** — Added `FindAnyObjectByType<GameManager>()` check before creating duplicate GameManagers.
6. **[M4] Asset cleanup before creation** — Added `CleanupExistingAssets()` method to delete existing material/prefab before regenerating.

### Remaining (LOW — not fixed)

- **[L1]** .meta files not listed in story File List (standard Unity, cosmetic only).

## Change Log

- 2026-02-15: Code review — fixed 2 HIGH and 4 MEDIUM issues. Added singleton guard, namespace, idempotency, asset cleanup, spawn overlap fix.
- 2026-02-15: Implemented Story 2.2 — Pickup Setup (Spheres & Collection System). Created Pickup.cs, GameManager.cs, and PickupSetup.cs. Added "Pickup" tag to project settings. All 19 existing tests pass with zero regressions.
