# Story 2.1: ScoreTracker Core Logic

Status: done

## Story

As a developer,
I want a pure C# ScoreTracker class that tracks pickup collection count and fires events on changes,
So that score state is testable via EditMode tests and decoupled from MonoBehaviour lifecycle.

## Acceptance Criteria

1. A ScoreTracker initialized with a total pickup count increments collected count by 1 when `AddPoint()` is called
2. An `OnScoreChanged` event fires with the new collected count each time `AddPoint()` is called
3. When `AddPoint()` is called 3 times on a ScoreTracker with 0/5, the collected count equals 3 and `OnScoreChanged` fired exactly 3 times
4. The collected count and total count are both publicly accessible
5. `ScoreTracker.cs` is located in `Assets/Scripts/Core/`
6. `ScoreTrackerTests.cs` exists in `Assets/Tests/EditMode/` with tests covering all above criteria
7. Tests use the `MethodName_Condition_ExpectedResult` naming convention

## Tasks / Subtasks

- [x] Task 1: Create ScoreTracker class (AC: #1, #2, #4, #5)
  - [x] Create `Assets/Scripts/Core/ScoreTracker.cs`
  - [x] Constructor accepts `int totalCount` parameter
  - [x] Implement `public int Collected { get; private set; }` property
  - [x] Implement `public int Total { get; }` property (set in constructor)
  - [x] Implement `public event Action<int> OnScoreChanged;`
  - [x] Implement `public void AddPoint()` method that increments `Collected` and fires `OnScoreChanged?.Invoke(Collected)`
- [x] Task 2: Create ScoreTrackerTests (AC: #3, #6, #7)
  - [x] Create `Assets/Tests/EditMode/ScoreTrackerTests.cs`
  - [x] Test: `AddPoint_WhenCalled_IncrementsCollectedCount`
  - [x] Test: `AddPoint_WhenCalled_FiresOnScoreChangedEvent`
  - [x] Test: `AddPoint_CalledThreeTimes_CollectedEqualsThree`
  - [x] Test: `AddPoint_CalledThreeTimes_EventFiredExactlyThreeTimes`
  - [x] Test: `Constructor_WhenInitialized_CollectedIsZero`
  - [x] Test: `Constructor_WithTotalFive_TotalEqualsFive`
  - [x] Use NUnit `[Test]` attribute, `Assert.AreEqual`, `Assert.IsTrue`
  - [x] Instantiate ScoreTracker with `new ScoreTracker(5)` — no MonoBehaviour needed

## Dev Notes

### Architecture Compliance

- **Pure C# Class:** ScoreTracker has ZERO Unity dependencies. No `using UnityEngine;`. It is a plain C# class instantiated with `new`.
- **Event Pattern:** Use `System.Action<int>` for `OnScoreChanged`. Null-check invoke: `OnScoreChanged?.Invoke(Collected);`
- **Testability:** This class is the poster child for NFR3/NFR4 — fully testable in EditMode without a running scene.
- **Location:** `Assets/Scripts/Core/ScoreTracker.cs` — Core folder is for pure logic classes only.

### Technical Requirements

- `using System;` for `Action<T>` delegate
- No `using UnityEngine;` — this is intentional to enforce purity
- Class should be `public class ScoreTracker` (not sealed, not static)
- Constructor: `public ScoreTracker(int totalCount)`
- Thread safety not required (single-threaded Unity context)

### File Structure Requirements

```
Assets/
├── Scripts/
│   └── Core/
│       └── ScoreTracker.cs         # NEW - pure C# score tracking
├── Tests/
│   └── EditMode/
│       └── ScoreTrackerTests.cs    # NEW - EditMode unit tests
```

### Testing Requirements

- **Framework:** NUnit (Unity Test Framework)
- **Test attribute:** `[Test]` on each test method
- **Test class:** `[TestFixture]` on class (optional in Unity but good practice)
- **Naming:** `MethodName_Condition_ExpectedResult` pattern
- **Assertions:** `Assert.AreEqual(expected, actual)`, `Assert.IsTrue(condition)`
- **Event testing:** Subscribe a counter lambda to `OnScoreChanged` before calling `AddPoint()`, then assert counter value
- **No MonoBehaviour:** Tests instantiate with `new ScoreTracker(5)` directly

### Naming Conventions

- Class: `ScoreTracker` (PascalCase)
- Properties: `Collected`, `Total` (PascalCase)
- Events: `OnScoreChanged` (PascalCase with On prefix)
- Method: `AddPoint` (PascalCase)
- Private fields: `_collected` if used (underscore prefix camelCase)
- Test class: `ScoreTrackerTests`
- Test methods: `AddPoint_WhenCalled_IncrementsCollectedCount`

### Dependencies

- **No story dependencies** — this is a standalone pure C# class
- **Requires Story 1.1:** EditMode.asmdef must exist for tests to compile

### References

- [Source: _bmad-output/planning-artifacts/architecture.md#Testability Pattern]
- [Source: _bmad-output/planning-artifacts/architecture.md#Communication Patterns]
- [Source: _bmad-output/planning-artifacts/architecture.md#Test Patterns]
- [Source: _bmad-output/planning-artifacts/epics.md#Story 2.1]

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

- Unity batch mode test run: 15/15 tests passed (0 failures, 0 skipped)
- All 6 new ScoreTrackerTests passed on first run
- All 9 existing SceneSetupBoundaryTests passed (no regressions)

### Completion Notes List

- Created `ScoreTracker` as a pure C# class with zero Unity dependencies (`using System;` only)
- Constructor accepts `int totalCount`, stores as read-only `Total` property
- `Collected` property uses `{ get; private set; }` pattern, starts at 0
- `AddPoint()` increments `Collected` and invokes `OnScoreChanged?.Invoke(Collected)`
- All 6 tests follow `MethodName_Condition_ExpectedResult` naming convention
- Tests use `[TestFixture]` and `[Test]` attributes with NUnit assertions
- Tests instantiate via `new ScoreTracker(5)` — no MonoBehaviour needed

### File List

- `Assets/Scripts/Core/ScoreTracker.cs` (NEW)
- `Assets/Tests/EditMode/ScoreTrackerTests.cs` (NEW)

## Change Log

- 2026-02-15: Implemented ScoreTracker pure C# class and 6 EditMode unit tests covering all acceptance criteria
- 2026-02-15: Code review (AI) — 6 MEDIUM issues found and auto-fixed:
  1. Added `CubeCollector.Core` namespace to ScoreTracker.cs
  2. Added `CubeCollector.Tests` namespace to ScoreTrackerTests.cs
  3. Added bounds guard in `AddPoint()` to prevent over-collection beyond `Total`
  4. Added constructor validation throwing `ArgumentOutOfRangeException` for zero/negative `totalCount`
  5. Added 4 new edge-case tests: zero total, negative total, over-collection no-increment, over-collection no-event
  6. Migrated all assertions from `Assert.AreEqual` to fluent `Assert.That(actual, Is.EqualTo(expected))` syntax
  - 3 LOW issues noted but not fixed (event argument value verification, missing `using System`, setup/teardown pattern)
  - Unity batch mode: 19/19 tests passed (10 ScoreTracker + 9 SceneSetupBoundary), 0 failures
