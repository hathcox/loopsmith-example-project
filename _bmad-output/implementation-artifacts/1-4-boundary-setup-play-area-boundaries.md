# Story 1.4: Boundary Setup — Play Area Boundaries

Status: ready-for-dev

## Story

As a player,
I want to be prevented from moving the cube off the ground plane,
So that I stay within the playable area.

## Acceptance Criteria

1. SceneSetup.cs (or a dedicated section within it) programmatically creates 4 invisible boundary GameObjects around the play area edges
2. Each boundary has a static BoxCollider (non-trigger) sized and positioned via setup code constants
3. Boundary GameObjects have no visible renderer
4. The boundary colliders prevent the cube from leaving the play area during gameplay
5. The cube responds naturally to the boundary (no jittering or getting stuck)

## Tasks / Subtasks

- [ ] Task 1: Add boundary creation to SceneSetup (AC: #1, #2, #3)
  - [ ] Add boundary creation method to `Assets/Scripts/Setup/SceneSetup.cs`
  - [ ] Create 4 boundary wall GameObjects (North, South, East, West)
  - [ ] Position each boundary at the edge of the ground plane
  - [ ] Size each boundary BoxCollider to be tall enough to block the player cube (e.g., height 2 units) and long enough to cover the full edge
  - [ ] Set colliders as non-trigger, static
  - [ ] Remove or disable MeshRenderer on boundaries (invisible)
  - [ ] Define all boundary positions and sizes as setup code constants
- [ ] Task 2: Verify physics interaction (AC: #4, #5)
  - [ ] Test that player cube cannot pass through boundaries from any direction
  - [ ] Verify no jittering occurs when cube presses against boundary
  - [ ] Adjust boundary thickness if needed (recommend 1 unit thick walls)

## Dev Notes

### Architecture Compliance

- **Setup-Oriented:** Boundaries created in SceneSetup.cs alongside ground plane and camera. All positions/sizes as constants.
- **Physics:** Boundaries are static BoxColliders — Unity physics handles collision automatically with the player's Rigidbody.
- **No scripts needed:** Boundaries are passive physics objects. No MonoBehaviour required.

### Technical Requirements

- **Boundary sizing:** If ground plane is 20x20 centered at origin, boundaries at x=±10 and z=±10
- **Boundary dimensions:** Walls should be thin (0.5-1 unit) but tall enough to block (2 units height)
- **North wall:** Position `(0, 1, 10.5)`, Scale `(20, 2, 1)` — covers full north edge
- **South wall:** Position `(0, 1, -10.5)`, Scale `(20, 2, 1)`
- **East wall:** Position `(10.5, 1, 0)`, Scale `(1, 2, 20)`
- **West wall:** Position `(-10.5, 1, 0)`, Scale `(1, 2, 20)`
- **Visibility:** Use `DestroyImmediate(wall.GetComponent<MeshRenderer>())` or don't add one. If using `CreatePrimitive`, it adds a MeshRenderer by default — remove it.
- **Jitter prevention:** Static colliders + non-kinematic Rigidbody player = standard Unity collision. Should not jitter. If issues arise, slightly increase wall thickness.
- **Parent object:** Consider creating an empty "Boundaries" parent GameObject for scene hierarchy organization

### File Structure Requirements

```
Assets/
├── Scripts/
│   └── Setup/
│       └── SceneSetup.cs          # MODIFIED - add boundary creation
```

No new files — boundaries are added to the existing SceneSetup.cs from Story 1.2.

### Testing Requirements

- No EditMode tests required (physics collision is runtime behavior)
- Verification: Player cube cannot leave the play area from any edge in Play mode

### Dependencies

- **Requires Story 1.1:** IGameSetup interface, SetupRunner
- **Requires Story 1.2:** SceneSetup with ground plane (boundary positions relative to ground plane size)

### References

- [Source: _bmad-output/planning-artifacts/architecture.md#Physics Configuration]
- [Source: _bmad-output/planning-artifacts/epics.md#Story 1.4]

## Dev Agent Record

### Agent Model Used

### Debug Log References

### Completion Notes List

### File List
