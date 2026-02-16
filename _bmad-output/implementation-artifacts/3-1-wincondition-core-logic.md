# Story 3.1: WinCondition Core Logic

Status: done

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

- [x] Task 1: Create WinCondition class (AC: #1, #2, #5)
  - [x] Create `Assets/Scripts/Core/WinCondition.cs`
  - [x] Constructor accepts `int totalCount` parameter
  - [x] Implement `public bool CheckWin(int collectedCount)` method
  - [x] Returns `true` when `collectedCount >= totalCount`, `false` otherwise
  - [x] Pure C# — no Unity dependencies
- [x] Task 2: Integrate WinCondition into GameManager (AC: #3, #4)
  - [x] Modify `Assets/Scripts/GameManager.cs` (created in Story 2.2)
  - [x] Add `private WinCondition _winCondition;` field
  - [x] Initialize `_winCondition = new WinCondition(totalPickups)` in `Awake()`
  - [x] Add `public event Action OnWinCondition;` event
  - [x] Add `public bool IsGameWon { get; private set; }` property for state distinction (FR10)
  - [x] In `CollectPickup()`, after `_scoreTracker.AddPoint()`, check `_winCondition.CheckWin(_scoreTracker.Collected)`
  - [x] If win condition met: set `IsGameWon = true` and fire `OnWinCondition?.Invoke()`
  - [x] Prevent further collection after win (optional guard in `CollectPickup`)
- [x] Task 3: Create WinConditionTests (AC: #6, #7)
  - [x] Create `Assets/Tests/EditMode/WinConditionTests.cs`
  - [x] Test: `CheckWin_CollectedLessThanTotal_ReturnsFalse`
  - [x] Test: `CheckWin_CollectedEqualsTotal_ReturnsTrue`
  - [x] Test: `CheckWin_CollectedZero_ReturnsFalse`
  - [x] Test: `CheckWin_CollectedGreaterThanTotal_ReturnsTrue` (edge case)
  - [x] Use NUnit `[Test]` attribute
  - [x] Instantiate with `new WinCondition(5)`

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

Claude Opus 4.6

### Debug Log References

No issues encountered during implementation.

### Completion Notes List

- Created `WinCondition` as a pure C# class in `Assets/Scripts/Core/` with zero Unity dependencies
- `CheckWin(int collectedCount)` returns true when `collectedCount >= totalCount` — stateless pure function
- Integrated WinCondition into GameManager: initialized in `Awake()` using `_scoreTracker.Total`
- Added `OnWinCondition` event (System.Action) and `IsGameWon` property to GameManager
- GameManager evaluates win condition after each `CollectPickup()` → `AddPoint()` call
- Added `if (IsGameWon) return;` guard at top of `CollectPickup()` to prevent post-win collection
- All 4 WinCondition unit tests pass; all 23 tests in suite pass with 0 regressions

### Change Log

- 2026-02-15: Implemented Story 3.1 — WinCondition core logic, GameManager integration, and EditMode tests
- 2026-02-15: Code review fixes — Added constructor validation to WinCondition (consistency with ScoreTracker), added 3 new tests (constructor validation + integration test with ScoreTracker). All 26 tests pass, 0 regressions.

### File List

- Assets/Scripts/Core/WinCondition.cs (NEW, REVIEW-MODIFIED)
- Assets/Scripts/GameManager.cs (MODIFIED)
- Assets/Tests/EditMode/WinConditionTests.cs (NEW, REVIEW-MODIFIED)

## Senior Developer Review (AI)

**Reviewer:** Code Review Workflow (Claude Opus 4.6)
**Date:** 2026-02-15
**Outcome:** Approved with fixes applied

### Findings Summary

| # | Severity | Issue | Resolution |
|---|----------|-------|------------|
| M1 | MEDIUM | WinCondition constructor missing validation for `totalCount <= 0` (inconsistent with ScoreTracker) | FIXED — Added `ArgumentOutOfRangeException` guard matching ScoreTracker pattern |
| M2 | MEDIUM | No integration-level test coverage for ACs 3-4 (GameManager wiring) | FIXED — Added `CheckWin_WithScoreTracker_WinConditionEvaluatesCorrectly` test |
| M3 | MEDIUM | No edge case test for invalid constructor input | FIXED — Added `Constructor_ZeroTotalCount_ThrowsArgumentOutOfRangeException` and `Constructor_NegativeTotalCount_ThrowsArgumentOutOfRangeException` |
| L1 | LOW | Magic number `5` hardcoded in GameManager | NOT FIXED — acceptable placeholder for current sprint |
| L2 | LOW | No test with `totalCount=1` boundary | NOT FIXED — covered adequately by existing tests |
| L3 | LOW | All tests reuse magic number 5 | NOT FIXED — tests are clear as-is |

### Test Results After Fixes

- **Total tests:** 26 (was 23)
- **WinCondition tests:** 7 (was 4)
- **All passed:** Yes
- **Regressions:** 0
