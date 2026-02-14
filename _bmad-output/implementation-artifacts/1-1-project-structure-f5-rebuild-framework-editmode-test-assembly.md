# Story 1.1: Project Structure, F5 Rebuild Framework & EditMode Test Assembly

Status: ready-for-dev

## Story

As a developer,
I want the Unity project structure verified, the Setup-Oriented Generation Framework established, and the EditMode test assembly configured,
So that all future stories use code-driven setup classes triggered by F5 and can include EditMode unit tests.

## Acceptance Criteria

1. The following folders exist: `Assets/Scripts/`, `Assets/Scripts/Core/`, `Assets/Scripts/Setup/`, `Assets/Editor/`, `Assets/Tests/EditMode/`, `Assets/Scenes/`, `Assets/Generated/Prefabs/`, `Assets/Generated/Materials/`, `Assets/Imported/`
2. An `EditMode.asmdef` assembly definition is created in `Assets/Tests/EditMode/` referencing the main scripts assembly
3. A `SetupRunner.cs` editor script exists in `Assets/Editor/` that binds to the F5 key
4. Pressing F5 in the Unity Editor executes the full rebuild sequence: clear all content under `Assets/Generated/` then run all setup classes in registration order
5. SetupRunner provides a registration mechanism for setup classes to define their execution order
6. The `Assets/Imported/` folder is never touched by the F5 rebuild process
7. The project compiles without errors

## Tasks / Subtasks

- [ ] Task 1: Create folder structure (AC: #1)
  - [ ] Verify `Assets/Scripts/` and `Assets/Scripts/Core/` exist (or create)
  - [ ] Create `Assets/Scripts/Setup/` directory
  - [ ] Create `Assets/Editor/` directory
  - [ ] Create `Assets/Tests/EditMode/` directory
  - [ ] Verify `Assets/Scenes/` exists (or create)
  - [ ] Create `Assets/Generated/Prefabs/` directory
  - [ ] Create `Assets/Generated/Materials/` directory
  - [ ] Create `Assets/Imported/` directory
- [ ] Task 2: Create EditMode test assembly (AC: #2)
  - [ ] Create `Assets/Tests/EditMode/EditMode.asmdef` with reference to main assembly
  - [ ] Verify test assembly compiles
- [ ] Task 3: Create SetupRunner editor script (AC: #3, #4, #5, #6)
  - [ ] Create `Assets/Editor/SetupRunner.cs`
  - [ ] Implement F5 key binding using `[MenuItem]` with keyboard shortcut or `EditorApplication` key handling
  - [ ] Implement `ClearGenerated()` method that deletes all content under `Assets/Generated/` but preserves the folder structure
  - [ ] Implement setup class registration mechanism (e.g., interface `ISetup` with `int Order` property, or attribute-based registration)
  - [ ] Implement `RunAllSetups()` that discovers and executes registered setup classes in order
  - [ ] Ensure `Assets/Imported/` is never touched by the rebuild process
- [ ] Task 4: Verify compilation (AC: #7)
  - [ ] Confirm zero compile errors in Unity 6.3 LTS

## Dev Notes

### Architecture Compliance

- **Engine:** Unity 6.3 LTS with Universal Render Pipeline (URP)
- **Setup-Oriented Generation Framework:** All game objects, UIs, prefabs, and scene composition defined entirely in code via setup classes. F5 triggers full rebuild. Inspector is read-only. All configuration lives in setup code constants.
- **Pattern:** Setup classes go in `Assets/Scripts/Setup/`, editor scripts in `Assets/Editor/`
- **Anti-pattern:** Never manually configure values in the Inspector. Never create assets by hand that could be generated.

### Technical Requirements

- SetupRunner must be an `Editor` script (only runs in Editor, not in builds)
- Use `UnityEditor` namespace for editor functionality
- F5 binding approach options: `[MenuItem("Tools/Rebuild %#F5")]` or custom `EditorWindow` with key event handling. Recommend `MenuItem` with shortcut for simplicity.
- Setup class interface should be simple — suggest `IGameSetup` interface with `int ExecutionOrder { get; }` and `void Execute()` method
- `ClearGenerated()` should use `AssetDatabase.DeleteAsset()` for proper Unity asset cleanup, followed by `AssetDatabase.Refresh()`
- Registration: Use reflection to find all classes implementing `IGameSetup`, sort by `ExecutionOrder`, and execute

### File Structure Requirements

```
Assets/
├── Editor/
│   └── SetupRunner.cs
├── Scripts/
│   ├── Core/           (empty, ready for logic classes)
│   └── Setup/          (empty, ready for setup classes)
├── Tests/
│   └── EditMode/
│       └── EditMode.asmdef
├── Scenes/             (existing)
├── Generated/
│   ├── Prefabs/        (empty, populated by F5)
│   └── Materials/      (empty, populated by F5)
└── Imported/           (empty, protected from F5)
```

### Testing Requirements

- No tests required for this story (infrastructure setup)
- EditMode.asmdef must be configured to run in EditMode only (`"includePlatforms": ["Editor"]`)
- Assembly reference must include the main scripts assembly so tests can access game classes

### Naming Conventions

- C# classes: PascalCase (`SetupRunner`, `IGameSetup`)
- Private fields: `_camelCase`
- Public methods: PascalCase
- One class per file, filename matches class name

### Project Structure Notes

- This is a greenfield Unity 6.3 LTS URP project — the Unity project already exists in the repository
- This story verifies and extends the existing structure, it does NOT create the project from scratch
- The `Assets/Generated/` folder replaces the standard `Assets/Prefabs/` and `Assets/Materials/` folders from the architecture document — all generated content goes here

### References

- [Source: _bmad-output/planning-artifacts/architecture.md#Starter Template Evaluation]
- [Source: _bmad-output/planning-artifacts/architecture.md#Implementation Patterns & Consistency Rules]
- [Source: _bmad-output/planning-artifacts/epics.md#Story 1.1]
- [Source: _bmad-output/planning-artifacts/architecture.md#Project Structure & Boundaries]

## Dev Agent Record

### Agent Model Used

### Debug Log References

### Completion Notes List

### File List
