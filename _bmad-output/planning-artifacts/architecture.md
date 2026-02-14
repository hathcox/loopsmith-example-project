---
stepsCompleted: [1, 2, 3, 4, 5, 6, 7, 8]
lastStep: 8
status: 'complete'
completedAt: '2026-02-14'
inputDocuments: [planning-artifacts/prd.md]
workflowType: 'architecture'
project_name: 'Cube Collector'
user_name: 'Iggy'
date: '2026-02-14'
---

# Architecture Decision Document

_This document builds collaboratively through step-by-step discovery. Sections are appended as we work through each architectural decision together._

## Project Context Analysis

### Requirements Overview

**Functional Requirements:**
15 FRs covering 5 capability areas (Player Movement, Pickup Collection, Score & Win Condition, User Interface, Scene & Camera). All requirements describe a simple collect-and-win game loop. No complex state machines, no networking, no persistence. Architecturally straightforward — each capability area maps cleanly to a single component.

**Non-Functional Requirements:**
6 NFRs across Performance, Testability, and Code Quality. The critical architectural driver is testability (NFR3, NFR4) — all game logic must be unit-testable via EditMode tests without a running scene. This mandates separating pure logic from MonoBehaviour lifecycle.

**Scale & Complexity:**

- Primary domain: Unity single-player game
- Complexity level: Low
- Estimated architectural components: 5

### Technical Constraints & Dependencies

- Unity 6.3 LTS with Universal Render Pipeline (URP)
- Legacy Input Manager (not new Input System)
- Rigidbody-based movement (physics-dependent)
- EditMode tests only (no PlayMode tests)
- Single scene architecture

### Cross-Cutting Concerns Identified

- EditMode testability — affects every component's structure (logic separation from MonoBehaviour)

## Starter Template Evaluation

### Primary Technology Domain

Unity 6.3 LTS game project — single-player, single-scene, URP render pipeline.

### Starter: Unity URP Project Template

The Unity project already exists in the repository, created via Unity Hub with the URP template. No alternative starters were evaluated — Unity URP template is the only sensible choice for this project.

**Architectural Decisions Provided by Starter:**

- **Engine & Runtime:** Unity 6.3 LTS, C# / .NET
- **Render Pipeline:** URP configured and ready
- **Build Tooling:** Unity build system
- **Testing Framework:** Unity Test Framework (NUnit-based) — EditMode test assembly
- **Code Organization:** Standard Unity `Assets/` folder structure

**Project Structure:**

- `Assets/Scripts/` — Game scripts
- `Assets/Tests/EditMode/` — EditMode unit tests
- `Assets/Scenes/` — Game scenes
- `Assets/Prefabs/` — Prefab assets

**Note:** The Unity project is already created. Epic 1 Story 1 should focus on verifying project structure and setting up the EditMode test assembly, not project creation.

## Core Architectural Decisions

### Decision Priority Analysis

**Critical Decisions (Block Implementation):**

1. Component Architecture — direct references + C# events
2. Testability Pattern — pure C# logic classes with thin MonoBehaviour wrappers
3. Physics Configuration — Rigidbody + triggers for collection
4. UI Architecture — event-driven updates

**Deferred Decisions (Not Applicable):**

- Data Architecture: No database or persistence — N/A
- Authentication & Security: No users or network — N/A
- API & Communication: No external APIs — N/A
- Infrastructure & Deployment: Unity build system only — N/A

### Component Architecture

**Decision:** Direct serialized references between components, with C# events for score/win notifications.

**Rationale:** Direct references are the simplest approach for a 5-component project. C# events cleanly decouple score change and win condition notifications to the UI layer, keeping the logic testable.

**Pattern:**
- `PlayerController` (MonoBehaviour) holds reference to `PickupCollector`
- `PickupCollector` fires `OnPickupCollected` event
- `GameManager` subscribes to pickup events, delegates to `ScoreTracker` (pure C#)
- `ScoreTracker` fires `OnScoreChanged` and `OnWinCondition` events
- `UIManager` subscribes to score/win events and updates UI text

### Testability Pattern

**Decision:** Pure C# classes for all game logic. MonoBehaviours are thin wrappers.

**Rationale:** NFR3 and NFR4 require EditMode testability without a running scene. Pure C# classes can be instantiated directly in EditMode tests with `new`. MonoBehaviours only handle Unity lifecycle (input, collision callbacks, component wiring) and delegate to the logic classes.

**Testable Logic Classes:**
- `ScoreTracker` — tracks collected count, total count, exposes score state and events
- `WinCondition` — evaluates whether all pickups are collected
- `MovementCalculator` — computes movement vectors from input (optional, may be too thin to extract)

**Thin MonoBehaviour Wrappers:**
- `PlayerController` — reads input in `FixedUpdate`, applies force to Rigidbody
- `Pickup` — handles `OnTriggerEnter`, notifies collector
- `GameManager` — wires components together, owns `ScoreTracker`
- `UIManager` — subscribes to events, updates TextMeshPro elements

### Physics Configuration

**Decision:** Standard Unity physics with trigger-based collection.

- **Player cube:** Rigidbody (non-kinematic, gravity enabled) + BoxCollider (non-trigger)
- **Pickup spheres:** SphereCollider (isTrigger = true), no Rigidbody needed
- **Ground plane:** BoxCollider (non-trigger, static)
- **Boundaries:** Invisible BoxColliders around the play area edges (static, non-trigger)
- **Collision detection:** Player's `OnTriggerEnter` detects pickup spheres by tag or layer

### UI Architecture

**Decision:** Event-driven UI updates via C# events.

**Rationale:** Decouples UI from game logic. The `ScoreTracker` fires events when score changes; `UIManager` subscribes and updates TextMeshPro text. The win message is shown when `OnWinCondition` fires. This keeps `ScoreTracker` fully testable — it has no knowledge of UI components.

**UI Components:**
- Score text: TextMeshPro showing "{collected}/{total}"
- Win message: TextMeshPro panel, hidden by default, shown on win event

### Decision Impact Analysis

**Implementation Sequence:**
1. Project setup and test assembly configuration
2. Pure logic classes (`ScoreTracker`, `WinCondition`) with EditMode tests
3. Player movement (Rigidbody + input)
4. Pickup system (trigger colliders + collection logic)
5. UI layer (event subscriptions + display)
6. Scene assembly and integration

**Cross-Component Dependencies:**
- `ScoreTracker` is the central hub — `GameManager` writes to it, `UIManager` reads from it via events
- `PlayerController` and `Pickup` are independent — connected only through Unity's physics collision system
- All MonoBehaviours depend on their corresponding logic classes, but logic classes have zero Unity dependencies

## Implementation Patterns & Consistency Rules

### Naming Patterns

**C# Code Naming:**
- Classes, methods, properties, events: PascalCase (`PlayerController`, `CollectPickup()`, `OnScoreChanged`)
- Local variables, parameters: camelCase (`moveDirection`, `pickupCount`)
- Private fields: camelCase with underscore prefix (`_scoreTracker`, `_rigidbody`)
- Constants: PascalCase (`MaxPickups`, `MoveSpeed`)

**File Naming:**
- Script files match class name exactly: `PlayerController.cs`, `ScoreTracker.cs`
- Test files: `{ClassName}Tests.cs` (e.g., `ScoreTrackerTests.cs`)
- One class per file

**Unity Naming:**
- Tags: PascalCase — `"Pickup"` for pickup spheres, `"Player"` for player cube
- Scene: `GameScene.unity`
- Materials: PascalCase descriptive — `GroundMaterial`, `PlayerMaterial`, `PickupMaterial`

### Structure Patterns

**Folder Organization:**
- `Assets/Scripts/` — MonoBehaviour scripts (`PlayerController.cs`, `Pickup.cs`, `GameManager.cs`, `UIManager.cs`)
- `Assets/Scripts/Core/` — Pure C# logic classes (`ScoreTracker.cs`, `WinCondition.cs`)
- `Assets/Tests/EditMode/` — All EditMode test files
- `Assets/Scenes/` — `GameScene.unity`
- `Assets/Prefabs/` — `Pickup.prefab`
- `Assets/Materials/` — URP materials for ground, player, pickup

### Communication Patterns

**Event System:**
- Use `System.Action` or `System.Action<T>` delegates (not UnityEvent)
- Event naming: `public event Action<int> OnScoreChanged;`
- Subscribe in `OnEnable`, unsubscribe in `OnDisable`
- Null-check before invoking: `OnScoreChanged?.Invoke(newScore);`

### Process Patterns

**Error Handling:** No error handling framework needed. No network, no I/O, no user input validation.

**Diagnostics:** `Debug.Log` for development only. No logging framework.

### Test Patterns

**Test Organization:**
- One test class per logic class
- Test method naming: `MethodName_Condition_ExpectedResult`
  - Example: `CollectPickup_WhenPickupExists_IncrementsScore`
  - Example: `CheckWinCondition_WhenAllCollected_ReturnsTrue`
- Use NUnit `[Test]` attribute
- Use `Assert.AreEqual`, `Assert.IsTrue`, `Assert.IsFalse`
- Tests instantiate pure C# classes directly with `new`

### Enforcement Guidelines

**All AI Agents MUST:**

1. Never put game logic directly in MonoBehaviour methods — always delegate to pure C# classes in `Assets/Scripts/Core/`
2. Follow the exact folder structure defined above
3. Use `System.Action` events, not `UnityEvent`
4. Name test methods with the `MethodName_Condition_ExpectedResult` pattern
5. Tag pickup spheres with `"Pickup"` and set their colliders as triggers
6. Use PascalCase for all public members, camelCase with underscore prefix for private fields

**Anti-Patterns to Avoid:**
- Putting score logic inside `OnTriggerEnter` directly
- Using `FindObjectOfType` or `GameObject.Find` at runtime
- Creating MonoBehaviours that cannot be tested without Play mode
- Using UnityEvent in the Inspector for game logic wiring
- Mixing test files with production scripts

## Project Structure & Boundaries

### Complete Project Directory Structure

```
CubeCollector/
├── Assets/
│   ├── Scenes/
│   │   └── GameScene.unity
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── ScoreTracker.cs          # Pure C# — score state + events
│   │   │   └── WinCondition.cs          # Pure C# — win evaluation
│   │   ├── PlayerController.cs          # MonoBehaviour — input + Rigidbody movement
│   │   ├── Pickup.cs                    # MonoBehaviour — trigger detection
│   │   ├── GameManager.cs              # MonoBehaviour — wires components, owns ScoreTracker
│   │   └── UIManager.cs               # MonoBehaviour — subscribes to events, updates UI
│   ├── Tests/
│   │   └── EditMode/
│   │       ├── EditMode.asmdef          # Assembly definition for EditMode tests
│   │       ├── ScoreTrackerTests.cs
│   │       └── WinConditionTests.cs
│   ├── Prefabs/
│   │   └── Pickup.prefab               # Pickup sphere prefab
│   └── Materials/
│       ├── GroundMaterial.mat
│       ├── PlayerMaterial.mat
│       └── PickupMaterial.mat
├── Packages/
│   └── manifest.json
└── ProjectSettings/
    └── (Unity-managed)
```

### Requirements to Structure Mapping

| FR Category | Files |
|---|---|
| Player Movement (FR1-3) | `PlayerController.cs` |
| Pickup Collection (FR4-7) | `Pickup.cs`, `GameManager.cs` |
| Score & Win (FR8-10) | `Core/ScoreTracker.cs`, `Core/WinCondition.cs`, `GameManager.cs` |
| User Interface (FR11-13) | `UIManager.cs` |
| Scene & Camera (FR14-15) | `GameScene.unity` |

### Component Boundaries

```
[Input] → PlayerController → [Rigidbody Physics]
                                    ↓ (OnTriggerEnter)
                              Pickup → GameManager
                                           ↓
                                     ScoreTracker (pure C#)
                                      ↓ events ↓
                              UIManager    WinCondition (pure C#)
```

### Data Flow

1. `PlayerController` reads input → applies force to Rigidbody
2. Physics system detects trigger collision between player and pickup
3. `Pickup.OnTriggerEnter` → calls `GameManager.CollectPickup()`
4. `GameManager` → calls `ScoreTracker.AddPoint()` → destroys pickup GameObject
5. `ScoreTracker` fires `OnScoreChanged` event → `UIManager` updates score text
6. `ScoreTracker` checks `WinCondition` → if true, fires `OnWinCondition` → `UIManager` shows win message

## Architecture Validation Results

### Coherence Validation

**Decision Compatibility:** All decisions work together. Pure C# logic classes + thin MonoBehaviour wrappers + System.Action events + trigger colliders form a coherent, simple architecture with no conflicts.

**Pattern Consistency:** Naming conventions (PascalCase classes, camelCase locals, underscore-prefixed privates) are standard C# and consistent throughout. Event naming follows `On{Action}` pattern uniformly.

**Structure Alignment:** Folder structure (`Scripts/Core/` for logic, `Scripts/` for MonoBehaviours, `Tests/EditMode/` for tests) directly supports the testability pattern.

### Requirements Coverage Validation

**Functional Requirements Coverage:**

| FR | Architectural Support |
|---|---|
| FR1-3 (Movement) | `PlayerController` + Rigidbody + boundary colliders |
| FR4-7 (Pickups) | `Pickup` + trigger colliders + `GameManager` |
| FR8-10 (Score/Win) | `ScoreTracker` + `WinCondition` (pure C#) |
| FR11-13 (UI) | `UIManager` + event subscriptions + TextMeshPro |
| FR14-15 (Scene/Camera) | `GameScene.unity` + fixed camera |

**Non-Functional Requirements Coverage:**

| NFR | Architectural Support |
|---|---|
| NFR1-2 (Performance) | Single scene, minimal components, same-frame events |
| NFR3-4 (Testability) | Pure C# logic classes, EditMode test assembly |
| NFR5-6 (Code Quality) | Clear structure, test-per-class pattern |

All 15 FRs and 6 NFRs are fully covered. No gaps.

### Implementation Readiness Validation

**Decision Completeness:** All critical decisions documented. Technology stack, component architecture, testability pattern, physics configuration, UI architecture, and event system are fully specified.

**Structure Completeness:** Complete directory tree with every file named. All components mapped to specific locations.

**Pattern Completeness:** Naming, structure, communication, test, and enforcement patterns are comprehensive. Anti-patterns documented.

### Gap Analysis Results

No critical or important gaps found. The architecture is intentionally minimal and complete for the defined scope.

### Architecture Completeness Checklist

- [x] Project context analyzed and scale assessed
- [x] Technical constraints identified (Unity 6.3 LTS, URP, Legacy Input, Rigidbody)
- [x] All critical decisions documented
- [x] Technology stack fully specified
- [x] Naming conventions established
- [x] Structure patterns defined
- [x] Communication patterns specified (System.Action events)
- [x] Complete directory structure defined
- [x] Component boundaries established
- [x] Requirements-to-structure mapping complete
- [x] Data flow documented

### Architecture Readiness Assessment

**Overall Status:** READY FOR IMPLEMENTATION

**Confidence Level:** High

**Key Strengths:**
- Every FR maps to a specific file with clear responsibility
- Testability pattern ensures all logic is EditMode-testable
- Component boundaries are simple and well-defined
- Event-driven communication keeps components decoupled
- Zero unnecessary complexity

**First Implementation Priority:**
1. Set up EditMode test assembly (`EditMode.asmdef`)
2. Implement pure C# logic classes (`ScoreTracker`, `WinCondition`) with tests
3. Build MonoBehaviour wrappers and wire scene
