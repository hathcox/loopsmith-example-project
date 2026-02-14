---
stepsCompleted: [step-01-init, step-02-discovery, step-03-success, step-04-journeys, step-05-domain-skipped, step-06-innovation-skipped, step-07-project-type, step-08-scoping-skipped, step-09-functional, step-10-nonfunctional, step-11-polish, step-12-complete]
inputDocuments: [inline-product-brief-cube-collector]
workflowType: 'prd'
classification:
  projectType: game-validation-target
  domain: gaming-developer-tooling
  complexity: low
  projectContext: greenfield
---

# Product Requirements Document - Cube Collector

**Author:** Iggy
**Date:** 2026-02-14

## Executive Summary

Cube Collector is a minimal Unity URP game prototype where the player controls a cube on a flat plane, collects 5 glowing spheres using WASD movement, and wins when all spheres are collected. A score counter tracks progress.

This project exists solely to validate the LoopSmith automated dev-review-test-commit loop. It is intentionally the simplest possible game that still exercises all LoopSmith phases — producing real C# scripts, testable logic, and reviewable code across a small number of stories. The project is disposable and will be deleted when validation is complete.

**Target User:** Iggy — LoopSmith developer who needs a controlled test target to validate the automated dev loop before using it on real game prototypes.

**Key Differentiator:** Intentional minimalism — every feature produces testable C# code (EditMode unit tests), and the entire backlog is small enough to complete in a single LoopSmith run.

## Success Criteria

### User Success

- LoopSmith developer (Iggy) can point LoopSmith at this project and watch it autonomously develop all stories without manual intervention
- The resulting Unity project builds and runs without errors
- Pressing play shows a movable cube on a ground plane, collectible glowing spheres, an updating score counter, and a "You Win!" message upon collecting all pickups

### Business Success

- LoopSmith's automated dev-review-test-commit loop is validated end-to-end against a real Unity project
- Successful completion provides confidence to use LoopSmith on real game prototypes
- The full backlog is completed in a single LoopSmith run

### Technical Success

- All Unity EditMode tests pass
- All git commits are clean and properly formatted
- All stories reach `done` status in sprint-status.yaml
- Every feature produces testable C# code with corresponding EditMode unit tests

### Measurable Outcomes

- 100% of stories reach `done` status
- 0 test failures in EditMode test suite
- 0 build errors in Unity project
- 0 manual interventions required during LoopSmith run

## Product Scope

### MVP - Minimum Viable Product

1. **Player Movement** — A cube on a ground plane that moves with WASD input. Camera is fixed top-down.
2. **Pickup System** — Glowing spheres scattered on the plane. Player cube collides with them to collect. Spheres disappear on collection.
3. **Score and Win Condition** — A score counter increments on each pickup. When all pickups are collected, the game triggers a win state.
4. **Basic UI** — On-screen score text. A simple "You Win!" message when all pickups are collected.

### Growth Features (Post-MVP)

None. This project is intentionally minimal and disposable.

### Vision (Future)

None. This project's sole purpose is to validate LoopSmith. Delete it when testing is complete.

## User Journeys

### Journey 1: The Player — Success Path

A player launches Cube Collector. They see a cube on a flat ground plane and glowing spheres scattered around it. They press WASD and the cube moves responsively across the plane. They steer toward a glowing sphere, collide with it — the sphere disappears and the on-screen score counter updates from "0/5" to "1/5". They navigate to each remaining sphere, collecting them one by one. The score reads "5/5" and a "You Win!" message appears on screen. The core gameplay loop is complete.

### Journey 2: The Player — Edge Case

A player launches the game and moves the cube toward the edge of the ground plane. The cube does not fall off or become stuck — boundaries are handled gracefully. They approach a sphere at an oblique angle — collision detection still triggers correctly and the pickup is collected. They collect all pickups and win normally despite non-optimal movement paths and edge-of-plane exploration.

### Journey 3: Iggy — LoopSmith Validation Path

Iggy completes the BMAD pipeline, producing a Product Brief, PRD, Architecture document, and Epics with Stories. He points LoopSmith at the Cube Collector project. LoopSmith picks up the first story from the sprint backlog, implements the required C# scripts, writes EditMode unit tests, passes code review, and creates a clean git commit. This cycle repeats for each story in sequence. Iggy checks sprint-status.yaml — all stories show `done`. He opens the Unity project, presses Play, and verifies the game works as specified. LoopSmith validation is complete.

### Journey Requirements Summary

- **Journey 1** reveals: WASD movement input handling, collision detection between player cube and pickup spheres, pickup collection and destruction, score tracking and increment logic, win condition trigger, UI score display, UI win message display
- **Journey 2** reveals: ground plane boundary handling, robust collision detection from all approach angles, edge-case movement resilience
- **Journey 3** reveals: testable C# code per feature, EditMode unit test coverage, clean git commit formatting, sprint status tracking integration

## Game-Specific Technical Requirements

### Project-Type Overview

Cube Collector is a minimal Unity URP game prototype serving as a LoopSmith validation target. All technical decisions favor simplicity and testability.

### Technical Architecture Considerations

- **Engine:** Unity 6.3 LTS with Universal Render Pipeline (URP)
- **Input:** Legacy Input Manager (`Input.GetAxis("Horizontal")` / `Input.GetAxis("Vertical")`)
- **Physics:** Rigidbody-based movement on the player cube (not Transform translation)
- **Camera:** Fixed top-down camera (no camera controller needed)
- **Scene Structure:** Single scene containing ground plane, player cube, 5 pickup spheres, UI canvas
- **Testing:** EditMode unit tests only — no PlayMode tests

### Implementation Considerations

- Pickup spheres use trigger colliders (`OnTriggerEnter`) for collection
- Player cube uses a Rigidbody with movement applied via `AddForce` or velocity setting in `FixedUpdate`
- Ground plane boundary handling to prevent the cube from leaving the play area
- All game logic must be structured for EditMode testability (separate logic from MonoBehaviour where possible)
- 5 pickup spheres placed at predetermined positions on the ground plane

## Functional Requirements

### Player Movement

- **FR1:** Player can move a cube across a ground plane using WASD keyboard input
- **FR2:** Player cube moves using physics-based (Rigidbody) motion
- **FR3:** Player cube is constrained to the ground plane boundaries and cannot leave the play area

### Pickup Collection

- **FR4:** The game world contains 5 glowing spheres placed on the ground plane as collectible pickups
- **FR5:** Player can collect a pickup sphere by moving the cube into contact with it
- **FR6:** A collected pickup sphere disappears from the game world upon collection
- **FR7:** Pickup collection is detected via trigger-based collision

### Score & Win Condition

- **FR8:** The game tracks the number of pickups the player has collected
- **FR9:** The game detects when all pickups have been collected and triggers a win state
- **FR10:** The game distinguishes between in-progress and won states

### User Interface

- **FR11:** Player can see a score counter displaying current and total pickup counts (e.g., "3/5")
- **FR12:** Player can see a "You Win!" message when all pickups are collected
- **FR13:** The score counter updates immediately when a pickup is collected

### Scene & Camera

- **FR14:** The game presents a fixed top-down camera view of the play area
- **FR15:** The game contains a single scene with ground plane, player cube, pickup spheres, and UI elements

## Non-Functional Requirements

### Performance

- **NFR1:** The game maintains stable frame rate during gameplay (no frame drops from game logic)
- **NFR2:** Pickup collection feedback (sphere disappearance, score update) occurs within the same frame as collision detection

### Testability

- **NFR3:** All game logic is structured to be testable via Unity EditMode tests without requiring a running game scene
- **NFR4:** Game logic is separated from MonoBehaviour lifecycle where possible to enable unit testing

### Code Quality

- **NFR5:** All C# scripts compile without errors or warnings in Unity 6.3 LTS
- **NFR6:** Each functional requirement has at least one corresponding EditMode test
