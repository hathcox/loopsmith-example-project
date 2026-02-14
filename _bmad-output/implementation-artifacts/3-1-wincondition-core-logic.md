# Story 3.1: WinCondition Core Logic

Status: ready-for-dev

## Story

As a developer,
I want a pure C# WinCondition class that evaluates whether all pickups have been collected,
So that win state detection is testable via EditMode tests and decoupled from MonoBehaviour lifecycle.

## Acceptance Criteria

1. A WinCondition initialized with a total pickup count of 5 returns false when `CheckWin` is called with a collected count less than 5
2. A WinCondition initialized with a total pickup count of 5 returns true when `CheckWin` is called with a collected count equal to 5
3. When the ScoreTracker fires `OnScoreChanged`, the GameManager evaluates WinCondition and fires `OnWinCondition` if all pickups are collected
4. The game state transitions from in-progress to won
5. `WinCondition.cs` is located in `Assets/Scripts/Core/`
6. `WinConditionTests.cs` exists in `Assets/Tests/EditMode/` with tests covering all above criteria
7. Tests use the `MethodName_Condition_ExpectedResult` naming convention

## Tasks / Subtasks

- [ ] Task 1: Create WinCondition class (AC: #1, #2, #5)
  - [ ] Create `Assets/Scripts/Core/WinCondition.cs`
  - [ ] Constructor accepts `int totalCount` parameter
  - [ ] Implement `public bool CheckWin(int collectedCount)` method
  - [ ] Returns `true` when `collectedCount >= totalCount`, `false` otherwise
  - [ ] Pure C# — no Unity dependencies
- [ ] Task 2: Integrate WinCondition into GameManager (AC: #3, #4)
  - [ ] Modify `Assets/Scripts/GameManager.cs` (created in Story 2.2)
  - [ ] Add `private WinCondition _winCondition;` field
  - [ ] Initialize `_winCondition = new WinCondition(totalPickups)` in `Awake()`
  - [ ] Add `public event Action OnWinCondition;` event
  - [ ] Add `public bool IsGameWon { get; private set; }` property for state distinction (FR10)
  - [ ] In `CollectPickup()`, after `_scoreTracker.AddPoint()`, check `_winCondition.CheckWin(_scoreTracker.Collected)`
  - [ ] If win condition met: set `IsGameWon = true` and fire `OnWinCondition?.Invoke()`
  - [ ] Prevent further collection after win (optional guard in `CollectPickup`)
- [ ] Task 3: Create WinConditionTests (AC: #6, #7)
  - [ ] Create `Assets/Tests/EditMode/WinConditionTests.cs`
  - [ ] Test: `CheckWin_CollectedLessThanTotal_ReturnsFalse`
  - [ ] Test: `CheckWin_CollectedEqualsTotal_ReturnsTrue`
  - [ ] Test: `CheckWin_CollectedZero_ReturnsFalse`
  - [ ] Test: `CheckWin_CollectedGreaterThanTotal_ReturnsTrue` (edge case)
  - [ ] Use NUnit `[Test]` attribute
  - [ ] Instantiate with `new WinCondition(5)`

## Dev Notes

### Architecture Compliance

- **Pure C# Class:** WinCondition has ZERO Unity dependencies. No `using UnityEngine;`. Plain C# class.
- **Location:** `Assets/Scripts/Core/WinCondition.cs` — Core folder for pure logic only.
- **GameManager Integration:** GameManager owns both ScoreTracker and WinCondition. It evaluates win after each score change.
- **Event Pattern:** `OnWinCondition` uses `System.Action` (no parameters needed — win is binary).
- **State Pattern:** `IsGameWon` property distinguishes in-progress vs won states (FR10).

### Technical Requirements

- `using System;` only
- `CheckWin` is a pure function — no side effects, no state mutation
- WinCondition does NOT store collected count — it receives it as a parameter. This keeps it stateless and simple.
- GameManager wires the flow: ScoreTracker.OnScoreChanged → check WinCondition → fire OnWinCondition if true
- Consider: after win, GameManager could ignore further `CollectPickup` calls via `if (IsGameWon) return;`

### File Structure Requirements

```
Assets/
├── Scripts/
│   ├── Core/
│   │   └── WinCondition.cs        # NEW - pure C# win evaluation
│   └── GameManager.cs             # MODIFIED - add WinCondition integration
├── Tests/
│   └── EditMode/
│       └── WinConditionTests.cs   # NEW - EditMode unit tests
```

### Testing Requirements

- **Framework:** NUnit (Unity Test Framework)
- **Naming:** `MethodName_Condition_ExpectedResult`
- **Assertions:** `Assert.IsTrue`, `Assert.IsFalse`
- **Coverage:** Test boundary conditions (0, below total, at total, above total)
- **No MonoBehaviour:** Tests use `new WinCondition(5)` directly

### Dependencies

- **Requires Story 1.1:** EditMode.asmdef for tests
- **Requires Story 2.1:** ScoreTracker (GameManager integration)
- **Requires Story 2.2:** GameManager.cs exists (will be modified)

### References

- [Source: _bmad-output/planning-artifacts/architecture.md#Testability Pattern]
- [Source: _bmad-output/planning-artifacts/architecture.md#Component Architecture]
- [Source: _bmad-output/planning-artifacts/architecture.md#Test Patterns]
- [Source: _bmad-output/planning-artifacts/epics.md#Story 3.1]

## Dev Agent Record

### Agent Model Used

### Debug Log References

### Completion Notes List

### File List
