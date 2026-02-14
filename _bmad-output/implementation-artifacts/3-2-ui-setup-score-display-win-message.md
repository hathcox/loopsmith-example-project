# Story 3.2: UI Setup — Score Display & Win Message

Status: ready-for-dev

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

- [ ] Task 1: Create UIManager MonoBehaviour (AC: #6, #7, #8, #9, #10, #11, #12)
  - [ ] Create `Assets/Scripts/UIManager.cs`
  - [ ] Add serialized/private fields for score text and win message TextMeshProUGUI references
  - [ ] Add reference to GameManager (for event subscription)
  - [ ] In `OnEnable()`: subscribe to `GameManager.OnScoreChanged` and `GameManager.OnWinCondition`
  - [ ] In `OnDisable()`: unsubscribe from both events
  - [ ] Implement `UpdateScore(int collected)` handler: set score text to `$"{collected}/{total}"`
  - [ ] Implement `ShowWinMessage()` handler: enable/show win message GameObject
  - [ ] In `Start()` or `Awake()`: initialize score display to "0/5" and hide win message
  - [ ] Store total count reference from GameManager for display formatting
- [ ] Task 2: Create UISetup class (AC: #1, #2, #3, #4, #5)
  - [ ] Create `Assets/Scripts/Setup/UISetup.cs` implementing `IGameSetup`
  - [ ] Set `ExecutionOrder` after PickupSetup (e.g., order 400)
  - [ ] Create UI Canvas GameObject with `Canvas` component (Screen Space - Overlay)
  - [ ] Add `CanvasScaler` (Scale With Screen Size, reference 1920x1080)
  - [ ] Add `GraphicRaycaster`
  - [ ] Create score text: `GameObject` with `TextMeshProUGUI` component
    - Position: top-left or top-center of screen
    - Default text: "0/5"
    - Font size: ~36
    - Color: white or high-contrast
  - [ ] Create win message: `GameObject` with `TextMeshProUGUI` component
    - Position: center of screen
    - Text: "You Win!"
    - Font size: ~72
    - Color: bright/gold
    - Set `gameObject.SetActive(false)` — hidden by default
  - [ ] Attach UIManager component to Canvas
  - [ ] Wire UIManager references: score text, win message, GameManager
  - [ ] Create EventSystem if one doesn't exist (required for Unity UI)

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

### Debug Log References

### Completion Notes List

### File List
