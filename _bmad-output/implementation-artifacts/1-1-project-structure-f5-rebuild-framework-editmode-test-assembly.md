# Story 1.1: Project Structure, F5 Rebuild Framework & EditMode Test Assembly

Status: done

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

- [x] Task 1: Create folder structure (AC: #1)
  - [x] Verify `Assets/Scripts/` and `Assets/Scripts/Core/` exist (or create)
  - [x] Create `Assets/Scripts/Setup/` directory
  - [x] Create `Assets/Editor/` directory
  - [x] Create `Assets/Tests/EditMode/` directory
  - [x] Verify `Assets/Scenes/` exists (or create)
  - [x] Create `Assets/Generated/Prefabs/` directory
  - [x] Create `Assets/Generated/Materials/` directory
  - [x] Create `Assets/Imported/` directory
- [x] Task 2: Create EditMode test assembly (AC: #2)
  - [x] Create `Assets/Tests/EditMode/EditMode.asmdef` with reference to main assembly
  - [x] Verify test assembly compiles
- [x] Task 3: Create SetupRunner editor script (AC: #3, #4, #5, #6)
  - [x] Create `Assets/Editor/SetupRunner.cs`
  - [x] Implement F5 key binding using `[MenuItem]` with keyboard shortcut or `EditorApplication` key handling
  - [x] Implement `ClearGenerated()` method that deletes all content under `Assets/Generated/` but preserves the folder structure
  - [x] Implement setup class registration mechanism (e.g., interface `ISetup` with `int Order` property, or attribute-based registration)
  - [x] Implement `RunAllSetups()` that discovers and executes registered setup classes in order
  - [x] Ensure `Assets/Imported/` is never touched by the rebuild process
- [x] Task 4: Verify compilation (AC: #7)
  - [x] Confirm zero compile errors in Unity 6.3 LTS

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

Claude Opus 4.6

### Debug Log References

- Unity batch mode compilation verified with exit code 0
- Removed invalid packages `com.unity.modules.adaptiveperformance` and `com.unity.modules.vectorgraphics` from manifest.json (not available in Unity 6000.0.58f2)

### Completion Notes List

- Created all required project folders with .gitkeep files for git tracking
- Created `IGameSetup` interface in `Assets/Scripts/Setup/` with `ExecutionOrder` and `Execute()` members
- Created `SetupRunner.cs` in `Assets/Editor/` with Cmd+Shift+F5 menu shortcut, reflection-based setup discovery, and `ClearGenerated()` that only operates on `Assets/Generated/` (never touches `Assets/Imported/`)
- Created `EditMode.asmdef` with Editor-only platform, NUnit references, and UNITY_INCLUDE_TESTS define constraint
- Fixed manifest.json by removing two invalid module packages that prevented Unity from starting
- Verified clean compilation in Unity batch mode (exit code 0)

### File List

- Assets/Scripts/Core/.gitkeep (new)
- Assets/Scripts/Setup/.gitkeep (new)
- Assets/Scripts/Setup/IGameSetup.cs (new)
- Assets/Scripts/GameScripts.asmdef (new - added by code review)
- Assets/Editor/.gitkeep (new)
- Assets/Editor/SetupRunner.cs (new)
- Assets/Editor/Editor.asmdef (new - added by code review)
- Assets/Tests/EditMode/.gitkeep (new)
- Assets/Tests/EditMode/EditMode.asmdef (new, modified by code review)
- Assets/Generated/Prefabs/.gitkeep (new)
- Assets/Generated/Materials/.gitkeep (new)
- Assets/Imported/.gitkeep (new)
- Packages/manifest.json (modified - removed invalid packages)
- Packages/packages-lock.json (modified - auto-updated by Unity)
- Assets/Settings/UniversalRenderPipelineGlobalSettings.asset (modified - auto-updated by Unity)
- ProjectSettings/ProjectVersion.txt (modified - auto-updated by Unity)

### Senior Developer Review (AI)

**Reviewer:** Code Review Workflow (Claude Opus 4.6)
**Date:** 2026-02-15
**Issues Found:** 3 High, 4 Medium, 2 Low

**Fixed (7 issues):**
- [H1] Created `GameScripts.asmdef` under `Assets/Scripts/` and updated `EditMode.asmdef` to reference it — AC #2 was not satisfied (empty references array, no main assembly existed)
- [H2] Rewrote `ClearGenerated()` to scan all assets under `Assets/Generated/` (not just subfolders) — root-level generated assets were missed
- [H3] Added subfolder recreation after clearing (`Prefabs/`, `Materials/`) to ensure folder structure is preserved reliably
- [M1] Changed F5 shortcut from `%#F5` (Cmd+Shift+F5) to `_F5` (plain F5) to match AC #3
- [M2/M3] Updated File List to include `packages-lock.json`, `UniversalRenderPipelineGlobalSettings.asset`, and `ProjectVersion.txt`
- [M4] Added assembly name filtering in `RunAllSetups()` to skip system assemblies; wrapped `GetTypes()` in try/catch for `ReflectionTypeLoadException`
- [H1-related] Created `Editor.asmdef` in `Assets/Editor/` referencing `GameScripts` so `SetupRunner.cs` can access `IGameSetup` across assembly boundaries

**Not Fixed (2 Low issues):**
- [L1] No namespace on `IGameSetup` and `SetupRunner` — minor, can address later
- [L2] `.meta` files for `.gitkeep` not documented — auto-generated by Unity, cosmetic

**Compilation:** Verified clean (exit code 0)
**Tests:** EditMode test runner passes (no tests defined yet — expected for infrastructure story)

## Change Log

- 2026-02-15: Implemented Story 1.1 - Created project folder structure, IGameSetup interface, SetupRunner editor script with F5 rebuild framework, and EditMode test assembly. Fixed manifest.json invalid package references.
- 2026-02-15: Code review fixes - Created GameScripts.asmdef and Editor.asmdef for proper assembly references, fixed EditMode.asmdef to reference GameScripts, rewrote ClearGenerated() to clear all assets (not just subfolders) and recreate folder structure, changed F5 shortcut to plain F5, hardened RunAllSetups() against ReflectionTypeLoadException.
