# Real 3D Migration Tasks

Use this task list for the real/non-cartoon 3D migration. Complete one phase at a time and record build/runtime/visual evidence in `PROGRESS.md`.

## Phase 40 - SharpDX Compatibility and Asset Pipeline

- Install/evaluate `HelixToolkit.Wpf.SharpDX` for .NET Framework 4.8.1.
- Create the model asset folder and manifest contract.
- Keep the current classic Helix viewport active until SharpDX builds and can render a small proof scene.
- Do not remove the procedural builders in this phase.

## Phase 41 - Imported Model Loader

- Add a model loader service for approved local assets.
- Support transform metadata: origin, scale, rotation, static/dynamic parent.
- Fallback to current procedural geometry when a model part is missing.

## Phase 42 - Real Hoist H1 Asset

- Import/attach `hoist_h1`, `motor_gearbox`, `plate_rack`, and `hanging_plate` model parts.
- Bind H1 travel and lift transforms to the existing hoist simulation.
- Verify smooth movement and lift-before-travel behavior.

## Phase 43 - Real Tank and Plant Assets

- Import/attach `tank_standard`, `pipe_manifold`, `pump`, and `walkway` assets.
- Bind tank liquid/status updates without rebuilding the full scene each timer tick.
- Verify 18-tank layout remains aligned and readable.

## Phase 44 - SharpDX Runtime View Switch

- Switch the production 3D viewport to SharpDX only after the proof scene and imported parts build cleanly.
- Preserve WinForms dashboard shell, sidebar menus, command row, alarm grid, and KPI bindings.
- Keep classic Helix fallback available until final acceptance passes.

## Phase 45 - Final Real-View Acceptance

- Capture full-dashboard and close-up screenshots.
- Verify Debug and Release builds.
- Run runtime verifier for command state, hoist motion, lift motion, tank status, and PLC write blocking.
- Update `VISUAL_ACCEPTANCE.md` with real imported model evidence.
