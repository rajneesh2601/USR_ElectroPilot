# Real 3D Model Assets

Place approved real machine assets here for the SharpDX/real-view migration.

Preferred model parts:

- `hoist_h1`
- `tank_standard`
- `motor_gearbox`
- `pipe_manifold`
- `pump`
- `plate_rack`
- `hanging_plate`
- `walkway`

Preferred formats:

- `OBJ` with `MTL` and texture files for textured assets.
- `STL` for simple CAD geometry without textures.
- `FBX` only after conversion to a Helix-supported runtime path.

Runtime loading:

- Classic Helix runtime currently loads `.obj` and `.stl` files directly.
- `.fbx`, `.dae`, `.glb`, and `.gltf` are tracked by the manifest but must be converted or routed through a verified importer before production use.
- Dynamic H1 files must remain separate:
  - `hoist_h1`: main yellow portal/carriage body, authored around the H1 local origin.
  - `motor_gearbox`: compact attached end-drive assembly, authored in the same local coordinate space as `hoist_h1`.
  - `plate_rack`: rack/carrier frame, authored at the raised lift position.
  - `hanging_plate`: work plates/load, authored at the raised lift position.
- The dashboard moves `hoist_h1` and `motor_gearbox` horizontally with H1 travel.
- The dashboard moves `plate_rack` and `hanging_plate` horizontally with H1 travel and vertically with the lift state.
- Do not merge rack/plate geometry into the static tank line. Moving parts must stay independent.

Do not place screenshots here as 3D assets. Reference photos may be stored separately as visual references, but they must not be used as static dashboard backgrounds.

Generated H1 and plant asset source:

- `docs/3d-dashboard/tools/GenerateRealH1Assets.cs` creates the checked-in H1 and plant OBJ/MTL files.
- H1 assets use beveled fabricated beams, separate wheel/bearing assemblies, finned motors, gearboxes, linked hangers, clamps, copper contacts, and twelve PCB workpieces.
- Plant assets use open ribbed tanks, sight glasses, flanged manifolds, valve handwheels, pump volutes, finned electric motors, coupling guards, and open walkway grating.
- Regenerate from the repository root with:
  `docs\3d-dashboard\tools\GenerateRealH1Assets.exe USR_ElectroPilot\Assets\Models`
- Keep the four dynamic H1 files separate so the existing travel and lift transforms remain valid.
- Keep `tank_standard`, `pipe_manifold`, `pump`, and `walkway` separate so one frozen model can be shared by all configured tank instances.
