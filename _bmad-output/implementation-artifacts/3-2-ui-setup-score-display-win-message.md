# Story 3.2: UI Setup — Score Display & Win Message

Status: done

## Story

As a player,
I want to see my score on screen and a "You Win!" message when I collect all pickups,
So that I have visual feedback on my progress and know when I have completed the game.

## Acceptance Criteria

1. A `UISetup.cs` setup class in `Assets/Scripts/Setup/` programmatically creates a UI Canvas in the GameScene
2. UISetup creates a TextMeshPro score text element positioned on screen displaying "0/5"
3. UISetup creates a TextMeshPro win message panel displaying "You Win!", hidden by default
4. UISetup attaches a UIManager MonoBehaviour to the Canvas and wires references to the text elements via setup code
5. No manual Inspector configuration is required — all UI layout, positioning, and wiring is defined in UISetup.cs
6. When gameplay begins with 0 pickups collected, the score counter displays "0/5"
7. When the ScoreTracker fires `OnScoreChanged`, the score counter updates immediately (e.g., "1/5", "2/5")
8. While the game is in progress, the "You Win!" message is hidden
9. When the GameManager fires `OnWinCondition`, the "You Win!" message is displayed
10. `UIManager.cs` (MonoBehaviour) is in `Assets/Scripts/`
11. UIManager subscribes to ScoreTracker events in `OnEnable` and unsubscribes in `OnDisable`
12. UI elements use TextMeshPro (not legacy UI Text)

## Tasks / Subtasks

- [x] Task 1: Create UIManager MonoBehaviour (AC: #6, #7, #8, #9, #10, #11, #12)
  - [x] Create `Assets/Scripts/UIManager.cs`
  - [x] Add serialized/private fields for score text and win message TextMeshProUGUI references
  - [x] Add reference to GameManager (for event subscription)
  - [x] In `OnEnable()`: subscribe to `GameManager.OnScoreChanged` and `GameManager.OnWinCondition`
  - [x] In `OnDisable()`: unsubscribe from both events
  - [x] Implement `UpdateScore(int collected)` handler: set score text to `$"{collected}/{total}"`
  - [x] Implement `ShowWinMessage()` handler: enable/show win message GameObject
  - [x] In `Start()` or `Awake()`: initialize score display to "0/5" and hide win message
  - [x] Store total count reference from GameManager for display formatting
- [x] Task 2: Create UISetup class (AC: #1, #2, #3, #4, #5)
  - [x] Create `Assets/Scripts/Setup/UISetup.cs` implementing `IGameSetup`
  - [x] Set `ExecutionOrder` after PickupSetup (e.g., order 400)
  - [x] Create UI Canvas GameObject with `Canvas` component (Screen Space - Overlay)
  - [x] Add `CanvasScaler` (Scale With Screen Size, reference 1920x1080)
  - [x] Add `GraphicRaycaster`
  - [x] Create score text: `GameObject` with `TextMeshProUGUI` component
    - Position: top-left or top-center of screen
    - Default text: "0/5"
    - Font size: ~36
    - Color: white or high-contrast
  - [x] Create win message: `GameObject` with `TextMeshProUGUI` component
    - Position: center of screen
    - Text: "You Win!"
    - Font size: ~72
    - Color: bright/gold
    - Set `gameObject.SetActive(false)` — hidden by default
  - [x] Attach UIManager component to Canvas
  - [x] Wire UIManager references: score text, win message, GameManager
  - [x] Create EventSystem if one doesn't exist (required for Unity UI)

## Dev Notes

### Architecture Compliance

- **Event-Driven UI:** UIManager subscribes to events from GameManager/ScoreTracker. UI has no knowledge of game logic — it only reacts to events.
- **Setup-Oriented:** All Canvas, TextMeshPro elements, layout, and wiring created in UISetup.cs. Zero Inspector configuration.
- **Thin MonoBehaviour:** UIManager only subscribes to events and updates text. No game logic.
- **Event Lifecycle:** Subscribe in `OnEnable`, unsubscribe in `OnDisable` — prevents memory leaks and null reference errors.

### Technical Requirements

- **TextMeshPro:** Use `TMPro.TextMeshProUGUI` for UI text (not `TextMeshPro` which is for 3D text)
- **Required import:** `using TMPro;` in UIManager.cs
- **Canvas setup:** `Canvas.renderMode = RenderMode.ScreenSpaceOverlay`
- **CanvasScaler:** `uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize`, `referenceResolution = new Vector2(1920, 1080)`
- **Score format:** `$"{collected}/{total}"` — e.g., "3/5"
- **Win message visibility:** Use `gameObject.SetActive(true/false)` to show/hide
- **Event wiring in UISetup:** After creating UIManager and attaching to Canvas, set the private/serialized fields programmatically. May need reflection or public setter methods if fields are private. Recommend public `Initialize(TextMeshProUGUI scoreText, TextMeshProUGUI winText, GameManager gm)` method.
- **TextMeshPro package:** Ensure TMPro is available in the project (included by default in Unity 6.3 LTS). The TMP Essential Resources may need to be imported on first use.
- **EventSystem:** `new GameObject("EventSystem").AddComponent<EventSystem>().gameObject.AddComponent<StandaloneInputModule>()`

### File Structure Requirements

```
Assets/
├── Scripts/
│   ├── UIManager.cs               # NEW - event-driven UI updates
│   └── Setup/
│       └── UISetup.cs             # NEW - creates Canvas + UI elements
```

### Testing Requirements

- UIManager is a thin MonoBehaviour — no pure logic to EditMode test
- Score formatting (`$"{collected}/{total}"`) is trivial and doesn't warrant a separate test class
- Verification: Score displays "0/5" at start, updates on collection, "You Win!" appears when all collected

### Anti-Patterns to Avoid

- Do NOT use legacy `UnityEngine.UI.Text` — use TextMeshPro
- Do NOT use `UnityEvent` for game logic wiring — use `System.Action`
- Do NOT poll for score changes in `Update()` — use event subscription
- Do NOT put game logic in UIManager — it only updates display

### Dependencies

- **Requires Story 1.1:** IGameSetup interface, SetupRunner
- **Requires Story 1.2:** GameScene exists
- **Requires Story 2.1:** ScoreTracker (events to subscribe to)
- **Requires Story 2.2:** GameManager (event source, owns ScoreTracker)
- **Requires Story 3.1:** WinCondition integrated into GameManager (OnWinCondition event)

### References

- [Source: _bmad-output/planning-artifacts/architecture.md#UI Architecture]
- [Source: _bmad-output/planning-artifacts/architecture.md#Communication Patterns]
- [Source: _bmad-output/planning-artifacts/architecture.md#Component Architecture]
- [Source: _bmad-output/planning-artifacts/epics.md#Story 3.2]

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

- Unity batch mode compilation: Tundra build success (1.47s), 24 items updated
- EditMode test run: 26 passed, 0 failed, 0 skipped

### Completion Notes List

- **Task 1:** Created `UIManager.cs` as a thin MonoBehaviour that subscribes to `GameManager.OnScoreChanged` and `GameManager.OnWinCondition` events. Uses `Initialize()` method pattern for programmatic wiring from UISetup. Subscribes in both `Initialize()` (for initial setup) and `OnEnable()` (for re-enable scenarios), unsubscribes in `OnDisable()`. Score displays as `"{collected}/{total}"` format. Win message shown via `SetActive(true)`.
- **Task 2:** Created `UISetup.cs` implementing `IGameSetup` with ExecutionOrder 400 (after PickupSetup at 300). Programmatically creates Canvas (ScreenSpaceOverlay), CanvasScaler (ScaleWithScreenSize, 1920x1080), GraphicRaycaster, score text (top-center, white, 36pt), win message (center, gold, 72pt, hidden by default), EventSystem, and wires UIManager to Canvas with all references.
- **Assembly reference fix:** Added `Unity.TextMeshPro` to `GameScripts.asmdef` references to resolve TMPro compilation errors.
- **Testing:** Per story Dev Notes, UIManager is a thin MonoBehaviour with no pure logic suitable for EditMode testing. Score formatting is trivial. Verification confirmed via Unity compilation (clean build) and full regression suite (26/26 tests pass).

### Change Log

- 2026-02-15: Implemented Story 3.2 — UI Setup (Score Display & Win Message)
- 2026-02-15: Code review — fixed 6 issues (3 HIGH, 3 MEDIUM). See Senior Developer Review below.

### File List

- Assets/Scripts/UIManager.cs (NEW)
- Assets/Scripts/Setup/UISetup.cs (NEW)
- Assets/Scripts/GameScripts.asmdef (MODIFIED — added Unity.TextMeshPro reference)

## Senior Developer Review (AI)

**Reviewer:** Iggy (via Claude Opus 4.6) on 2026-02-15
**Outcome:** Approved with fixes applied

### Issues Found: 3 HIGH, 3 MEDIUM, 3 LOW

#### HIGH Issues (Fixed)

1. **H1: Double Event Subscription Bug** (`UIManager.cs:22-23,29-30`) — `Initialize()` and `OnEnable()` both subscribed to events independently, causing duplicate handler invocations on disable/re-enable cycles. **Fix:** Centralized subscription into `Subscribe()`/`Unsubscribe()` helper methods. `Initialize()` calls `Subscribe()` once after setting references. `OnEnable()` only subscribes if `_isInitialized` flag is set.

2. **H2: AC #11 Violated — Events subscribed in Initialize, not OnEnable** (`UIManager.cs:22-23`) — AC #11 mandates subscription in `OnEnable`, but original code subscribed in `Initialize()`. **Fix:** Restructured so `OnEnable()` handles subscription for re-enable scenarios, with `Initialize()` calling `Subscribe()` for initial wiring (since `OnEnable` fires before `Initialize` is called by UISetup).

3. **H3: Hardcoded "0/5" in UISetup score text** (`UISetup.cs:67`) — Score text default was `"0/5"` instead of using dynamic total from GameManager. **Fix:** Changed default text to empty string `""` — `UIManager.Initialize()` immediately sets the correct dynamic value.

#### MEDIUM Issues (Fixed)

4. **M1: No null guards in event handlers** (`UIManager.cs:42,47`) — `UpdateScore()` and `ShowWinMessage()` accessed fields without null checks. **Fix:** Added null guards for `_scoreText` and `_winMessageObject`.

5. **M2: UISetup doesn't check for duplicate Canvas** (`UISetup.cs`) — Running F5 rebuild twice would create duplicate canvases. **Fix:** Added `CleanupExisting()` method that destroys any existing Canvas before creating a new one.

6. **M3: No idempotency check for UIManager** (`UISetup.cs:28`) — Combined with M2, duplicate UIManagers could be created. **Fix:** Resolved by M2 fix — destroying the Canvas also removes its UIManager component.

#### LOW Issues (Not Fixed — Acceptable)

7. **L1: GameScripts.asmdef empty rootNamespace** — Minor consistency issue, not blocking.
8. **L2: UISetup doesn't log element positions** — Minor debugging aid gap.
9. **L3: Magic numbers in RectTransform positioning** — Inline values without named constants, inconsistent with other setup classes.

### Verification

- Unity batch mode compilation: Clean build (exit code 0)
- EditMode test run: 26 passed, 0 failed, 0 skipped
