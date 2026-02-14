---
stepsCompleted: [step-01-validate-prerequisites, step-02-design-epics, step-03-create-stories, step-04-final-validation]
status: 'complete'
completedAt: '2026-02-14'
inputDocuments: [planning-artifacts/prd.md, planning-artifacts/architecture.md]
---

# Cube Collector - Epic Breakdown

## Overview

This document provides the complete epic and story breakdown for Cube Collector, decomposing the requirements from the PRD and Architecture requirements into implementable stories.

## Requirements Inventory

### Functional Requirements

- **FR1:** Player can move a cube across a ground plane using WASD keyboard input
- **FR2:** Player cube moves using physics-based (Rigidbody) motion
- **FR3:** Player cube is constrained to the ground plane boundaries and cannot leave the play area
- **FR4:** The game world contains 5 glowing spheres placed on the ground plane as collectible pickups
- **FR5:** Player can collect a pickup sphere by moving the cube into contact with it
- **FR6:** A collected pickup sphere disappears from the game world upon collection
- **FR7:** Pickup collection is detected via trigger-based collision
- **FR8:** The game tracks the number of pickups the player has collected
- **FR9:** The game detects when all pickups have been collected and triggers a win state
- **FR10:** The game distinguishes between in-progress and won states
- **FR11:** Player can see a score counter displaying current and total pickup counts (e.g., "3/5")
- **FR12:** Player can see a "You Win!" message when all pickups are collected
- **FR13:** The score counter updates immediately when a pickup is collected
- **FR14:** The game presents a fixed top-down camera view of the play area
- **FR15:** The game contains a single scene with ground plane, player cube, pickup spheres, and UI elements

### NonFunctional Requirements

- **NFR1:** The game maintains stable frame rate during gameplay (no frame drops from game logic)
- **NFR2:** Pickup collection feedback (sphere disappearance, score update) occurs within the same frame as collision detection
- **NFR3:** All game logic is structured to be testable via Unity EditMode tests without requiring a running game scene
- **NFR4:** Game logic is separated from MonoBehaviour lifecycle where possible to enable unit testing
- **NFR5:** All C# scripts compile without errors or warnings in Unity 6.3 LTS
- **NFR6:** Each functional requirement has at least one corresponding EditMode test

### Additional Requirements

- **Starter Template:** Unity 6.3 LTS URP project already exists in repository — Epic 1 Story 1 should verify project structure and set up EditMode test assembly, not create the project from scratch
- **Component Architecture:** Direct serialized references between components + C# events (System.Action) for score/win notifications
- **Testability Pattern:** Pure C# logic classes (ScoreTracker, WinCondition) in Assets/Scripts/Core/ with thin MonoBehaviour wrappers in Assets/Scripts/
- **Physics Configuration:** Player Rigidbody (non-kinematic, gravity enabled) + BoxCollider (non-trigger); Pickup SphereCollider (isTrigger = true); Ground plane BoxCollider (static); Boundaries via invisible BoxColliders around play area edges
- **UI Architecture:** Event-driven updates via TextMeshPro — score text shows "{collected}/{total}", win message hidden by default and shown on win event
- **Setup-Oriented Generation Framework:** All game objects, UIs, prefabs, and scene composition are defined entirely in code via setup classes — never manually configured in the Inspector. F5 editor hotkey triggers a full rebuild: delete everything generated, then regenerate from source code. The Inspector is read-only. All configuration lives in setup code constants. Imported assets (models, textures, audio) live in a protected folder and are never deleted.
- **Folder Structure:** Assets/Scripts/, Assets/Scripts/Core/, Assets/Scripts/Setup/, Assets/Editor/, Assets/Tests/EditMode/, Assets/Scenes/, Assets/Generated/Prefabs/, Assets/Generated/Materials/, Assets/Imported/
- **Naming Conventions:** PascalCase for classes/methods/properties/events, camelCase for locals/parameters, underscore-prefixed camelCase for private fields (_scoreTracker), PascalCase constants
- **Test Pattern:** One test class per logic class, method naming MethodName_Condition_ExpectedResult, NUnit [Test] attribute, instantiate pure C# classes with new
- **Event Pattern:** Subscribe in OnEnable, unsubscribe in OnDisable, null-check invoke (OnScoreChanged?.Invoke)
- **Anti-Patterns to Avoid:** No game logic directly in OnTriggerEnter, no FindObjectOfType/GameObject.Find at runtime, no MonoBehaviours untestable without Play mode, no UnityEvent for game logic wiring

### FR Coverage Map

| FR | Epic | Description |
|---|---|---|
| FR1 | Epic 1 | WASD keyboard movement |
| FR2 | Epic 1 | Physics-based Rigidbody motion |
| FR3 | Epic 1 | Ground plane boundary constraints |
| FR4 | Epic 2 | 5 glowing pickup spheres on ground plane |
| FR5 | Epic 2 | Collect pickup by moving into contact |
| FR6 | Epic 2 | Collected pickup disappears |
| FR7 | Epic 2 | Trigger-based collision detection |
| FR8 | Epic 2 | Track number of pickups collected |
| FR9 | Epic 3 | Detect all pickups collected, trigger win |
| FR10 | Epic 3 | Distinguish in-progress vs won states |
| FR11 | Epic 3 | Score counter display ("3/5") |
| FR12 | Epic 3 | "You Win!" message on completion |
| FR13 | Epic 3 | Immediate score update on collection |
| FR14 | Epic 1 | Fixed top-down camera view |
| FR15 | Epic 1 | Single scene with all game elements |

## Epic List

### Epic 1: Project Foundation & Playable Movement
Player launches the game, sees a cube on a ground plane with a top-down camera, and moves it with WASD within boundaries.
**FRs covered:** FR1, FR2, FR3, FR14, FR15

### Epic 2: Collectible Pickups
Player sees glowing spheres on the plane and collects them by moving into them — spheres disappear on contact and a running score is tracked internally.
**FRs covered:** FR4, FR5, FR6, FR7, FR8

### Epic 3: Win Condition & Game UI
Player sees their score on screen ("3/5"), and when all pickups are collected, sees "You Win!" — completing the full game loop.
**FRs covered:** FR9, FR10, FR11, FR12, FR13

## Epic 1: Project Foundation & Playable Movement

Player launches the game, sees a cube on a ground plane with a top-down camera, and moves it with WASD within boundaries.

### Story 1.1: Project Structure, F5 Rebuild Framework & EditMode Test Assembly

As a developer,
I want the Unity project structure verified, the Setup-Oriented Generation Framework established, and the EditMode test assembly configured,
So that all future stories use code-driven setup classes triggered by F5 and can include EditMode unit tests.

**Acceptance Criteria:**

**Given** the Unity 6.3 LTS URP project exists in the repository
**When** the project structure is verified and framework is created
**Then** the following folders exist: Assets/Scripts/, Assets/Scripts/Core/, Assets/Scripts/Setup/, Assets/Editor/, Assets/Tests/EditMode/, Assets/Scenes/, Assets/Generated/Prefabs/, Assets/Generated/Materials/, Assets/Imported/
**And** an EditMode.asmdef assembly definition is created in Assets/Tests/EditMode/ referencing the main scripts assembly
**And** a SetupRunner.cs editor script exists in Assets/Editor/ that binds to the F5 key
**And** pressing F5 in the Unity Editor executes the full rebuild sequence: clear all content under Assets/Generated/ → run all setup classes in registration order
**And** SetupRunner provides a registration mechanism for setup classes to define their execution order
**And** the Assets/Imported/ folder is never touched by the F5 rebuild process
**And** the project compiles without errors

### Story 1.2: Scene Setup — Ground Plane & Fixed Top-Down Camera

As a player,
I want to see a ground plane from a fixed top-down camera view when I launch the game,
So that I have a clear visual play area.

**Acceptance Criteria:**

**Given** the F5 rebuild framework is in place
**When** F5 is pressed in the Unity Editor
**Then** a SceneSetup.cs setup class in Assets/Scripts/Setup/ programmatically creates the GameScene
**And** the GameScene contains a ground plane GameObject with a BoxCollider (static) and a generated GroundMaterial (URP Lit)
**And** the Main Camera is positioned directly above looking straight down providing a top-down view of the play area
**And** the camera position is fixed and does not move during gameplay
**And** GroundMaterial is generated into Assets/Generated/Materials/
**And** no manual Inspector configuration is required — all values are defined in setup code constants

### Story 1.3: Player Setup — Cube with WASD Physics Movement

As a player,
I want to move a cube across the ground plane using WASD keys,
So that I can navigate the play area.

**Acceptance Criteria:**

**Given** the GameScene exists with a ground plane
**When** F5 is pressed in the Unity Editor
**Then** a PlayerSetup.cs setup class in Assets/Scripts/Setup/ programmatically creates the player cube in the GameScene
**And** the player cube has a Rigidbody (non-kinematic, gravity enabled) and BoxCollider (non-trigger) configured via setup code
**And** the player cube has a PlayerController MonoBehaviour attached via setup code
**And** a PlayerMaterial (URP Lit) is generated into Assets/Generated/Materials/ and applied to the cube
**And** the player cube is tagged "Player" via setup code

**Given** the GameScene is running
**When** the player presses W, A, S, or D keys
**Then** the player cube moves in the corresponding direction using Rigidbody physics (not Transform translation)
**And** movement is applied via AddForce or velocity in FixedUpdate
**And** the PlayerController reads input via Legacy Input Manager (Input.GetAxis)

### Story 1.4: Boundary Setup — Play Area Boundaries

As a player,
I want to be prevented from moving the cube off the ground plane,
So that I stay within the playable area.

**Acceptance Criteria:**

**Given** the GameScene exists with a ground plane and player cube
**When** F5 is pressed in the Unity Editor
**Then** SceneSetup.cs (or a dedicated section within it) programmatically creates 4 invisible boundary GameObjects around the play area edges
**And** each boundary has a static BoxCollider (non-trigger) sized and positioned via setup code constants
**And** boundary GameObjects have no visible renderer

**Given** the player cube is near the edge of the ground plane during gameplay
**When** the player moves toward the edge
**Then** the boundary colliders prevent the cube from leaving the play area
**And** the cube responds naturally to the boundary (no jittering or getting stuck)

## Epic 2: Collectible Pickups

Player sees glowing spheres on the plane and collects them by moving into them — spheres disappear on contact and a running score is tracked internally.

### Story 2.1: ScoreTracker Core Logic

As a developer,
I want a pure C# ScoreTracker class that tracks pickup collection count and fires events on changes,
So that score state is testable via EditMode tests and decoupled from MonoBehaviour lifecycle.

**Acceptance Criteria:**

**Given** a ScoreTracker is initialized with a total pickup count
**When** AddPoint() is called
**Then** the collected count increments by 1
**And** an OnScoreChanged event fires with the new collected count

**Given** a ScoreTracker with 0 collected out of 5 total
**When** AddPoint() is called 3 times
**Then** the collected count equals 3
**And** OnScoreChanged fired exactly 3 times

**Given** a ScoreTracker is initialized
**When** the current state is queried
**Then** the collected count and total count are both accessible

**And** ScoreTracker.cs is located in Assets/Scripts/Core/
**And** ScoreTrackerTests.cs exists in Assets/Tests/EditMode/ with tests covering all above criteria
**And** tests use the MethodName_Condition_ExpectedResult naming convention

### Story 2.2: Pickup Setup — Spheres & Collection System

As a player,
I want to see glowing spheres on the ground plane and collect them by moving my cube into them,
So that I can interact with the game world and gather pickups.

**Acceptance Criteria:**

**Given** the F5 rebuild framework and GameScene exist
**When** F5 is pressed in the Unity Editor
**Then** a PickupSetup.cs setup class in Assets/Scripts/Setup/ programmatically creates a Pickup.prefab in Assets/Generated/Prefabs/
**And** the Pickup.prefab is a sphere with a SphereCollider (isTrigger = true), a Pickup MonoBehaviour, and a generated PickupMaterial (URP Lit, glowing appearance)
**And** each pickup is tagged "Pickup" via setup code
**And** PickupSetup places 5 instances of the Pickup.prefab at predetermined positions on the ground plane defined as setup code constants
**And** PickupMaterial is generated into Assets/Generated/Materials/

**Given** the player cube moves into contact with a pickup sphere during gameplay
**When** OnTriggerEnter detects the collision
**Then** the pickup sphere is destroyed (disappears from the game world)
**And** the GameManager calls ScoreTracker.AddPoint()
**And** the collection and score update occur within the same frame

**Given** a pickup has already been collected
**When** the player returns to that position
**Then** nothing happens — the sphere is gone and no duplicate collection occurs

**And** Pickup.cs (MonoBehaviour) is in Assets/Scripts/
**And** GameManager.cs (MonoBehaviour) is in Assets/Scripts/ and owns the ScoreTracker instance
**And** no manual Inspector configuration is required — all prefab composition and placement is defined in PickupSetup.cs

## Epic 3: Win Condition & Game UI

Player sees their score on screen ("3/5"), and when all pickups are collected, sees "You Win!" — completing the full game loop.

### Story 3.1: WinCondition Core Logic

As a developer,
I want a pure C# WinCondition class that evaluates whether all pickups have been collected,
So that win state detection is testable via EditMode tests and decoupled from MonoBehaviour lifecycle.

**Acceptance Criteria:**

**Given** a WinCondition is initialized with a total pickup count of 5
**When** CheckWin is called with a collected count less than 5
**Then** it returns false

**Given** a WinCondition is initialized with a total pickup count of 5
**When** CheckWin is called with a collected count equal to 5
**Then** it returns true

**Given** the ScoreTracker fires OnScoreChanged
**When** the GameManager receives the event
**Then** the GameManager evaluates WinCondition and fires OnWinCondition if all pickups are collected
**And** the game state transitions from in-progress to won

**And** WinCondition.cs is located in Assets/Scripts/Core/
**And** WinConditionTests.cs exists in Assets/Tests/EditMode/ with tests covering all above criteria
**And** tests use the MethodName_Condition_ExpectedResult naming convention

### Story 3.2: UI Setup — Score Display & Win Message

As a player,
I want to see my score on screen and a "You Win!" message when I collect all pickups,
So that I have visual feedback on my progress and know when I have completed the game.

**Acceptance Criteria:**

**Given** the F5 rebuild framework and GameScene exist
**When** F5 is pressed in the Unity Editor
**Then** a UISetup.cs setup class in Assets/Scripts/Setup/ programmatically creates a UI Canvas in the GameScene
**And** UISetup creates a TextMeshPro score text element positioned on screen displaying "0/5"
**And** UISetup creates a TextMeshPro win message panel displaying "You Win!", hidden by default
**And** UISetup attaches a UIManager MonoBehaviour to the Canvas and wires references to the text elements via setup code
**And** no manual Inspector configuration is required — all UI layout, positioning, and wiring is defined in UISetup.cs

**Given** the GameScene is running and the player has collected 0 pickups
**When** gameplay begins
**Then** the score counter displays "0/5" on screen

**Given** the player collects a pickup
**When** the ScoreTracker fires OnScoreChanged
**Then** the score counter updates immediately to reflect the new count (e.g., "1/5", "2/5")

**Given** the player has not yet collected all pickups
**When** the game is in progress
**Then** the "You Win!" message is hidden

**Given** the player collects the final pickup
**When** the GameManager fires OnWinCondition
**Then** a "You Win!" message is displayed on screen

**And** UIManager.cs (MonoBehaviour) is in Assets/Scripts/
**And** UIManager subscribes to ScoreTracker events in OnEnable and unsubscribes in OnDisable
**And** UI elements use TextMeshPro (not legacy UI Text)
