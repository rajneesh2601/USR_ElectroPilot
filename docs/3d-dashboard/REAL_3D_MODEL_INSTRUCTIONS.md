# Real 3D Model Instructions

Read this file before any prompt that touches the dashboard 3D view, model quality, PLC-driven visualization, hoist/tank visuals, or real plant rendering.

## Target Direction

- The real-view target is `HelixToolkit.Wpf.SharpDX` plus imported real 3D assets.
- The current procedural `HelixToolkit.Wpf` builders are acceptable only as fallback/reference geometry.
- Do not continue improving the scene only by adding more boxes/cylinders if the user asks for real/non-cartoon visuals.
- Real visual quality must come from approved `OBJ`, `STL`, `FBX` converted to supported runtime format, or another verified model format.
- Keep the application as WinForms .NET Framework 4.8.1. Do not convert the full app to WPF or Unity unless the user explicitly changes architecture.

## Required First Checks

Before changing 3D code:

1. Read `GOAL.md`, `IMPLEMENTATION_PLAN.md`, `PROGRESS.md`, `VISUAL_ACCEPTANCE.md`, `PLC_MAPPING.md`, and this file.
2. Check current packages and confirm whether `HelixToolkit.Wpf.SharpDX` is installed.
3. Search for imported asset files under the repo before assuming assets exist.
4. Confirm the current incomplete phase from `PROGRESS.md`.
5. Keep real PLC writes disabled unless real protocol, tags/registers, scaling, data types, and write permissions are confirmed.

## Asset Rules

- Preferred asset folder: `USR_ElectroPilot/Assets/Models/`.
- Preferred model parts:
  - `hoist_h1`
  - `tank_standard`
  - `motor_gearbox`
  - `pipe_manifold`
  - `pump`
  - `plate_rack`
  - `hanging_plate`
  - `walkway`
- Each imported model must have a clear local origin, scale, orientation, and named parent transform.
- Moving parts must stay separate from static parts:
  - H1 portal/carriage: horizontal travel transform.
  - Lift/rack/plates: vertical lift transform nested under H1.
  - Tank liquids/status lights: data-driven material or transform updates.
- Do not use screenshot/reference images as static dashboard backgrounds.
- Do not copy supplier/company text from reference images. Use project-safe labels such as `USR H1 500KG`.

## Rendering Rules

- Prefer SharpDX/PBR only after a small compatibility spike proves the package builds in .NET Framework 4.8.1.
- If SharpDX migration is not stable, keep classic Helix and import real assets with `ModelImporter`, `ObjReader`, or `StLReader`.
- Do not break existing mouse controls: rotate, pan, zoom, reset, and view commands must keep working.
- Do not rebuild the full static scene on every timer tick. Dynamic updates should move transforms/materials only.
- Preserve smooth motion:
  - Horizontal H1 motion must interpolate.
  - Lowering/lifting must interpolate.
  - H1 must lift before horizontal travel when plates are down.

## PLC/Data Binding Rules

- PLC or simulation values should update the 3D scene through existing dashboard/service binding, not direct UI-thread blocking calls.
- Required bindings:
  - Hoist tank/position to H1 X transform.
  - Hoist status to lift/rack/plate Z transform.
  - Tank level to liquid surface.
  - Tank alarm/fault/warning/running to status lamps/materials.
  - Emergency stop to dashboard and visual state.
- Use `System.Windows.Forms.Timer` or WPF `DispatcherTimer` for UI animation. Do not use `Thread.Sleep` in UI/process flow.

## Verification Rules

Do not mark a real-view phase complete until:

- Debug build passes.
- Release build passes.
- Existing runtime verifier passes or is updated for the new asset path.
- Visual screenshot/crop proves the imported model is visible and not replaced by a static image.
- H1 movement, lift movement, and tank status updates still work after camera/view changes.
- Safe PLC write blocking behavior remains intact.

## Decision Defaults

- Default real-view route: keep WinForms, migrate/evaluate SharpDX, import real models.
- Default model format: `OBJ` for textured assets or `STL` for simple CAD assets, depending on available files.
- Default fallback if no real model files are provided: create an asset-loader layer first, keep existing procedural builders active, and wait for approved model files.
