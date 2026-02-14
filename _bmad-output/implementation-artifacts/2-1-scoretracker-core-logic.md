# Story 2.1: ScoreTracker Core Logic

Status: ready-for-dev

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

- [ ] Task 1: Create ScoreTracker class (AC: #1, #2, #4, #5)
  - [ ] Create `Assets/Scripts/Core/ScoreTracker.cs`
  - [ ] Constructor accepts `int totalCount` parameter
  - [ ] Implement `public int Collected { get; private set; }` property
  - [ ] Implement `public int Total { get; }` property (set in constructor)
  - [ ] Implement `public event Action<int> OnScoreChanged;`
  - [ ] Implement `public void AddPoint()` method that increments `Collected` and fires `OnScoreChanged?.Invoke(Collected)`
- [ ] Task 2: Create ScoreTrackerTests (AC: #3, #6, #7)
  - [ ] Create `Assets/Tests/EditMode/ScoreTrackerTests.cs`
  - [ ] Test: `AddPoint_WhenCalled_IncrementsCollectedCount`
  - [ ] Test: `AddPoint_WhenCalled_FiresOnScoreChangedEvent`
  - [ ] Test: `AddPoint_CalledThreeTimes_CollectedEqualsThree`
  - [ ] Test: `AddPoint_CalledThreeTimes_EventFiredExactlyThreeTimes`
  - [ ] Test: `Constructor_WhenInitialized_CollectedIsZero`
  - [ ] Test: `Constructor_WithTotalFive_TotalEqualsFive`
  - [ ] Use NUnit `[Test]` attribute, `Assert.AreEqual`, `Assert.IsTrue`
  - [ ] Instantiate ScoreTracker with `new ScoreTracker(5)` — no MonoBehaviour needed

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

### Debug Log References

### Completion Notes List

### File List
