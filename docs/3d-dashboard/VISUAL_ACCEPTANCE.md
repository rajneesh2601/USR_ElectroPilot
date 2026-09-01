# USR ElectroPilot 3D Visual Acceptance

## Reference State Required For Final Comparison

- Plant: Normal
- Mode: Auto
- Hoist H1: Running
- Hoist position: Tank 05
- Tank process: Copper
- Running tanks: 9
- Active alarms: 3
- Plates: Raised above Tank 05
- Camera: Default isometric

## Checklist

| Requirement | Pass/Fail | Evidence | Remaining difference | Correction |
| --- | --- | --- | --- | --- |
| Header matches target | Pass | `phase-17-header.png` | Icons are simplified GDI line drawings rather than final branded artwork | Header now has brand block, status dot, mode badge, hoist indicator, clock/date icon, and operator icon |
| Left navigation matches target | Pass | `phase-14-nav-labels.png` | Uses simplified GDI line icons rather than exact artwork | Approved icon set can replace primitives later |
| Direct sidebar submenu behavior | Pass | `phase-23-sidebar-submenus.png`, `Phase11RuntimeVerifier` | Submenu popups are code-verified, not visible in static screenshot | Sidebar has direct `View`, `Tanks`, `Recipe`, and `Settings` menu entries with only their relevant submenus |
| View command grouping | Pass | `phase-23-sidebar-submenus.png`, `Phase11RuntimeVerifier` | View popup is code-verified, not visible in static screenshot | Camera commands moved from the visible viewport toolbar into the sidebar `View` submenu |
| Right-header account menu | Pass | `phase-20-account-menu.png`, `Phase11RuntimeVerifier` | Account popup is code-verified, not visible in static screenshot | Operator header shows a dropdown cue and exposes Login Another / Logout only |
| Machine viewport composition | Pass | `phase-13-round-geometry.png` | Procedural style, not photoreal asset | Full line is visible with tighter target framing |
| Tank proportions and detail | Pass | `phase-14-nav-labels.png` | Simplified procedural tanks | Tanks include cylindrical outlet/pump cues and readable number labels |
| Portal geometry | Pass | `phase-42-real-h1-imported.png`, `Phase11RuntimeVerifier` | Imported presentation model; customer manufacturing CAD is not available | H1 uses separate imported beveled bridge, columns, end plates, gussets, bogies, wheels, bearings, bolts, cable tray, and drive assets |
| Plate carrier | Pass | `phase-42-real-h1-imported.png`, `Phase11RuntimeVerifier` | Imported mid-detail boards; not a scanned customer rack | Imported inner frame carries linked hangers, clamps, copper bars, and twelve individual PCB workpieces |
| Materials and lighting | Pass | `phase3-material-lighting.png` | Classic WPF Helix, not SharpDX PBR | Reusable material/light foundation is in place |
| Walkway and supports | Pass | `phase-13-round-geometry.png` | Simplified grating/stairs | Guard rails now use round geometry |
| Pumps and pipes | Pass | `phase-13-round-geometry.png` | Still procedural, not exact plant hardware | Cylindrical manifold, drops, valves, and pump bodies are visible |
| KPI cards | Pass | `phase-18-kpi-polish.png`, `Phase11RuntimeVerifier` | Icons are simplified GDI line drawings | Cards bind live values, avoid current-step clipping, and show Critical/Major/Minor alarm mini-values |
| Control row | Pass | `phase-16-command-row.png`, `Phase11RuntimeVerifier` | Icons are simplified GDI line drawings | Command row now uses target-like icon+text SCADA buttons |
| Alarm table | Pass | `phase-15-kpi-alarms.png`, `Phase11RuntimeVerifier` | Empty reference state has no active rows | Header/row/severity styling is in place |
| Right-sidebar bottom action | Pass | `phase-19-menu-shell.png`, `Phase11RuntimeVerifier` | Button icon is simplified GDI artwork | Right sidebar includes fixed `View All Alarms` action like the target |
| Spacing and alignment | Pass | `phase-11-runtime.png`, `phase13-lifecycle.png` | Procedural geometry remains less detailed than target image | Current layout and machine composition are stable |
| Configured tank count | Pass | `phase-25-18-tanks.png`, `Phase11RuntimeVerifier`, SQLite query | Wider plant line is denser than earlier 10-tank evidence | Main-line default and existing database upgrade now enforce 18 active tanks |
| Tank viewport clarity | Pass | `phase-43-real-tanks-pumps.png`, `Phase11RuntimeVerifier` | Full 18-tank overview is necessarily dense | Imported tanks keep open tops and readable upper-front state panels without black tank-top rods |
| Mouse viewport control | Pass | `Phase11RuntimeVerifier` | Controls are verified in code, not shown in a static screenshot | Helix viewport supports left-drag rotate, right/middle-drag pan, mouse-wheel zoom, zoom-around-cursor, rotate-around-click, double-click reset, and safe deferred view commands |
| Reference tank styling | Pass | `phase-43-real-tanks-pumps.png`, `Phase11RuntimeVerifier` | Classic Helix materials remain less realistic than planned SharpDX/PBR | Imported stations include ribbed open shells, beveled rims, sight glasses, labels, live lamps, flanged manifolds, valves, pump volutes, finned motors and coupling guards |
| Tank-parallel carrier binding | Pass | `phase-29-real-hoist-plates.png`, `phase-29-real-hoist-plates-close.png`, `Phase11RuntimeVerifier` | Close-up quality remains procedural | Verifier checks vertical plates are distributed along the tank-parallel carrier and confirms the rack X position moves with H1 |
| Raised/lowered plate clearance | Pass | `phase-37-smooth-hoist-motion.png`, `phase-37-smooth-hoist-motion-close.png`, `Phase11RuntimeVerifier` | Procedural geometry remains simpler than a real imported hoist model | Verifier checks raised H1 plates clear the tank rim, Lowering/Processing plates enter the tank, and lowering/lifting do not jump instantly |
| Smooth hoist motion | Pass | `phase-37-smooth-hoist-motion.png`, `phase-37-smooth-hoist-motion-close.png`, `Phase11RuntimeVerifier` | Motion smoothness is code-verified; static screenshot cannot show intermediate frames | H1 horizontal motion and lift Z interpolation are driven by the WPF viewport timer so view/camera changes do not stop process updates |
| Hoist sequence gating | Pass | `phase-38-sequenced-hoist-motion.png`, `phase-38-sequenced-hoist-motion-close.png`, `Phase11RuntimeVerifier` | Dynamic gating is code-verified; static screenshot cannot show the forbidden overlap | H1 must lift plates fully before horizontal movement starts, and service lower/lift states now hold long enough for visible process motion |
| Command state binding | Pass | `phase-39-command-state-binding.png`, `Phase11RuntimeVerifier` | Static screenshot shows one state only; verifier checks multiple runtime binds | Command buttons now expose active styling and enabled/disabled state from actual bound runtime data for auto, active job, stop, emergency, and reset states |
| Open exposed plate rack | Pass | `phase-31-open-plates-hoist-name.png`, `phase-31-open-plates-hoist-name-close.png`, `Phase11RuntimeVerifier` | Procedural rods and boards remain simpler than a real machine asset | Solid side-cover panels were removed so the green hanging work plates remain visible inside the open carrier |
| Hoist name label | Pass | `phase-36-connected-hoist-motor-name.png`, `phase-36-connected-hoist-motor-name-close.png`, `Phase11RuntimeVerifier` | Uses application identifier and load-capacity cue instead of copying reference company text | H1 hoist now shows beam-aligned `USR H1 500KG` text |
| Real hoist reference correction | Pass | `phase-42-real-h1-imported.png`, `Phase11RuntimeVerifier` | Classic WPF Helix materials remain less realistic than the planned SharpDX/PBR view | Complete imported H1 replaces the procedural fallback and preserves exposed boards, beam branding, attached motors, and correct travel/lift hierarchy |
| No black plate shadow | Pass | `phase-33-no-plate-shadow.png`, `phase-33-no-plate-shadow-close.png`, `Phase11RuntimeVerifier` | Global floor/platform shadows remain outside the tank opening | Removed the black H1 shadow pad that appeared below the hanging plates |
| Single tank opening | Pass | `phase-43-real-tanks-pumps.png`, `Phase11RuntimeVerifier` | Imported presentation model rather than customer manufacturing CAD | Imported tank asset has one open shell and the dynamic layer provides exactly one liquid surface without inner liner walls |
| Hoist drive motor | Pass | `phase-42-real-h1-imported.png`, `Phase11RuntimeVerifier` | Generated presentation asset rather than customer CAD | Imported motor asset includes attached mount, gearbox, coupling, shaft, finned body, fan cover, lift motor, drum, and gearbox bolts |

## Screenshots

- Baseline screenshot: `docs/3d-dashboard/screenshots/phase0-baseline-wpf.png`
- Final viewport screenshot: `docs/3d-dashboard/screenshots/phase13-lifecycle.png`
- Final shell screenshot: `docs/3d-dashboard/screenshots/phase1-winforms-shell.png`
- Latest runtime dashboard screenshot: `docs/3d-dashboard/screenshots/phase-11-runtime.png`
- Final comparison screenshot: `docs/3d-dashboard/screenshots/phase-12-final.png`
- Latest visual detail screenshot: `docs/3d-dashboard/screenshots/phase-13-round-geometry.png`
- Latest navigation/label screenshot: `docs/3d-dashboard/screenshots/phase-14-nav-labels.png`
- Latest KPI/alarm screenshot: `docs/3d-dashboard/screenshots/phase-15-kpi-alarms.png`
- Latest command-row screenshot: `docs/3d-dashboard/screenshots/phase-16-command-row.png`
- Latest header screenshot: `docs/3d-dashboard/screenshots/phase-17-header.png`
- Latest KPI polish screenshot: `docs/3d-dashboard/screenshots/phase-18-kpi-polish.png`
- Latest menu-shell screenshot: `docs/3d-dashboard/screenshots/phase-19-menu-shell.png`
- Latest account-menu screenshot: `docs/3d-dashboard/screenshots/phase-20-account-menu.png`
- Latest view-menu screenshot: `docs/3d-dashboard/screenshots/phase-21-view-menu.png`
- Latest sidebar-menu screenshot: `docs/3d-dashboard/screenshots/sidebar-menu-trigger.png`
- Latest direct submenu screenshot: `docs/3d-dashboard/screenshots/phase-23-sidebar-submenus.png`
- Latest startup-performance screenshot: `docs/3d-dashboard/screenshots/phase-24-startup-performance.png`
- Latest 18-tank screenshot: `docs/3d-dashboard/screenshots/phase-25-18-tanks.png`
- Latest clear tank-view screenshot: `docs/3d-dashboard/screenshots/phase-26-clear-tank-view.png`
- Latest mouse/tank-style screenshot: `docs/3d-dashboard/screenshots/phase-27-mouse-tank-style.png`
- Latest horizontal rack screenshot: `docs/3d-dashboard/screenshots/phase-28-horizontal-rack.png`
- Latest horizontal rack close crop: `docs/3d-dashboard/screenshots/phase-28-horizontal-rack-close.png`
- Latest real-hoist screenshot: `docs/3d-dashboard/screenshots/phase-29-real-hoist-plates.png`
- Latest real-hoist close crop: `docs/3d-dashboard/screenshots/phase-29-real-hoist-plates-close.png`
- Latest lift-clearance screenshot: `docs/3d-dashboard/screenshots/phase-30-real-lift-clearance.png`
- Latest lift-clearance close crop: `docs/3d-dashboard/screenshots/phase-30-real-lift-clearance-close.png`
- Latest open-plates/name screenshot: `docs/3d-dashboard/screenshots/phase-31-open-plates-hoist-name.png`
- Latest open-plates/name close crop: `docs/3d-dashboard/screenshots/phase-31-open-plates-hoist-name-close.png`
- Latest real-hoist-match screenshot: `docs/3d-dashboard/screenshots/phase-32-real-hoist-match.png`
- Latest real-hoist-match close crop: `docs/3d-dashboard/screenshots/phase-32-real-hoist-match-close.png`
- Latest no-plate-shadow screenshot: `docs/3d-dashboard/screenshots/phase-33-no-plate-shadow.png`
- Latest no-plate-shadow close crop: `docs/3d-dashboard/screenshots/phase-33-no-plate-shadow-close.png`
- Latest clean-tank-opening screenshot: `docs/3d-dashboard/screenshots/phase-34-clean-tank-opening.png`
- Latest clean-tank-opening crop: `docs/3d-dashboard/screenshots/phase-34-clean-tank-opening-close.png`
- Latest real-hoist-motor screenshot: `docs/3d-dashboard/screenshots/phase-35-real-hoist-motor.png`
- Latest real-hoist-motor crop: `docs/3d-dashboard/screenshots/phase-35-real-hoist-motor-close.png`
- Latest connected-hoist-motor screenshot: `docs/3d-dashboard/screenshots/phase-36-connected-hoist-motor-name.png`
- Latest connected-hoist-motor crop: `docs/3d-dashboard/screenshots/phase-36-connected-hoist-motor-name-close.png`
- Latest smooth-hoist-motion screenshot: `docs/3d-dashboard/screenshots/phase-37-smooth-hoist-motion.png`
- Latest smooth-hoist-motion crop: `docs/3d-dashboard/screenshots/phase-37-smooth-hoist-motion-close.png`
- Latest sequenced-hoist-motion screenshot: `docs/3d-dashboard/screenshots/phase-38-sequenced-hoist-motion.png`
- Latest sequenced-hoist-motion crop: `docs/3d-dashboard/screenshots/phase-38-sequenced-hoist-motion-close.png`
- Latest command-state-binding screenshot: `docs/3d-dashboard/screenshots/phase-39-command-state-binding.png`

## Phase 0 Baseline Visual Notes

- The current view shows a dark dashboard shell with WPF-rendered header, sidebar, KPI cards, command buttons, alarm panel, and Helix 3D scene.
- The current shell location does not match the Phase 1 requirement because dashboard cards/text/navigation are inside WPF, not WinForms.
- The current 3D scene is procedural and prototype-like: block tanks, simple rails, simple portal hoist, flat materials, limited piping/pumps/walkway detail.
- The camera crops parts of the tank line at the right side.
- The baseline screenshot runner uses WPF off-screen rendering because direct desktop capture produced a black image in this environment.

## Phase 1 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase1-winforms-shell.png`
- Header, sidebar, KPI cards, command row, alarm table, and dashboard panels are now WinForms controls.
- The central WPF/Helix `ElementHost` is contained inside the center panel and no longer owns dashboard cards/text/navigation.
- The old top WinForms menu strip and extra tab headers are hidden.
- Remaining failed item for next phase: camera/scene composition crops the tank line on the right side.

## Phase 2 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase2-camera.png`
- The complete tank row fits inside the central viewport.
- Front and rear rails are visible.
- The front walkway is visible.
- H1 portal is visible and no longer causes the scene to crop.
- Sidebar includes camera commands: Fit Plant, Front View, Top View, Left View, Right View.
- Remaining failed item for next phase: scene still uses flat prototype materials and simple lighting.

## Phase 3 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase3-material-lighting.png`
- The central 3D viewport renders with a named industrial material palette.
- Rails, walkway, tank shells, tank liquid, hoist steel, dark metal, and status indicators now use reusable materials.
- Lighting now uses ambient plus key, fill, and warm rim directional lights.
- Remaining failed item for next phase: tanks are still block-like and need a reusable detailed tank component.

## Phase 4 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase4-detailed-tank.png`
- Tanks now render through a reusable component.
- Each tank has top rim detail, inset liquid, front label area, status lamps, equipment blocks, pipe cue, and support legs.
- Remaining failed item for next phase: full line rails/platform/tank placement should be generated by a configurable line builder.

## Phase 5 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase5-configurable-line.png`
- Tank placement, rails, platform, guardrail posts, and foundation slab are generated by the tank line builder.
- The full line remains centered and H1 stays aligned to tank positions.
- Remaining failed item for next phase: walkway and support structure need more industrial detail.

## Phase 6 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase6-structure.png`
- Walkway, handrails, posts, access stairs, support beams, and crossmembers are visible.
- The added support geometry remains aligned to the single tank row.
- Remaining failed item for next phase: upper hoist rails should be isolated as exactly two aligned longitudinal rails.

## Phase 7 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase7-hoist-rails.png`
- The travel rail component generates exactly two upper longitudinal rails: front and rear.
- Rail posts and cross ties are generated from the same length and remain aligned with the tank line.
- Remaining failed item for next phase: H1 portal hoist should be isolated into its own mechanically clearer component.

## Phase 8 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase8-portal-hoist.png`
- H1 portal hoist is generated by a dedicated component.
- Bridge beam, four legs, rail shoes, trolley/motor, lift crossbar, and status stack are aligned from one X position.
- Remaining failed item for next phase: lift assembly and hanging plates should be isolated as a Z-only component.

## Phase 9 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase9-lift-plates.png`
- Lift bar, hook points, carrier bar, and individual hanging plates are generated by `LiftAssembly3DBuilder`.
- Lift height is controlled by a single `liftZ` value from the hoist status.
- Remaining failed item for next phase: visual hoist interpolation needs to be wired into the WinForms timer refresh path.

## Phase 10 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase10-animation.png`
- Hoist visual interpolation is now timer-driven from `MainForm` and no blocking wait path was introduced.
- The viewport remains nonblank and correctly composed.
- Remaining failed item for next phase: dashboard state labels should be more clearly bound to live/simulated status.

## Phase 11 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase11-live-binding.png`
- Dashboard binding code now colors status labels, binds active alarm color, binds hoist position from visual position, and displays remaining step seconds.
- Full WinForms screenshot capture blocked in this environment, so visual proof uses the stable offscreen WPF viewport capture while Debug/Release builds verify WinForms code.
- Remaining failed item for next phase: PLC safety needs an explicit write-disabled adapter layer.

## Phase 12 Visual Notes

- Phase 12 is not visual.
- Existing IP communication remains available and `SafePlcAdapter` blocks future PLC writes by default.
- Remaining failed item for next phase: viewport lifecycle should avoid unnecessary camera resets and disposed updates.

## Phase 13 Visual Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase13-lifecycle.png`
- Viewport remains correctly composed after camera lifecycle changes.
- Camera is preserved across normal data refreshes unless plant bounds change.
- Remaining failed item for next phase: final acceptance checklist needs to be closed with available evidence.

## Phase 14 Visual Notes

- Final visual evidence uses `phase1-winforms-shell.png` for the WinForms dashboard shell and `phase13-lifecycle.png` for the latest 3D viewport.
- Full WinForms dashboard capture currently blocks in this environment, but Debug/Release builds verify the WinForms implementation.
- Current result is a functional procedural Helix 3D simulator, not a photoreal imported plant asset.

## Visual Continuation Phase 1 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-01-camera.png`
- Default camera now uses tighter elevated front-left three-quarter framing.
- H1 is visible over Tank 05 in the reference screenshot state.
- Tank 01, Tank 10, front walkway, and both travel rails are visible.
- Helix view cube is hidden in the operator viewport.
- Remaining failed item for next phase: portal hoist needs clearer industrial proportions and mechanical detail.

## Visual Continuation Phase 2 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-02-portal.png`
- H1 is centered above Tank 05 and spans the front/rear travel rails.
- Portal legs now sit on bogie/wheel assemblies near the rails.
- Travel motor, gearbox, side connection plates, guide rails, and bolt cues are visible.
- Remaining failed item for next phase: hanging plates are still too thin and not readable enough.

## Visual Continuation Phase 3 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-03-plates.png`
- Twelve separate plates are visible under the H1 lift assembly.
- The plate assembly is raised above Tank 05 in the reference state.
- Carrier bar, hook/hanger cues, and dark plate material are visible.
- Remaining failed item for next phase: Tank 05 needs production-quality construction detail.

## Visual Continuation Phase 4 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-04-tank05.png`
- Tank 05 now has extra liner, recessed liquid, ribs, outlet, valve, flange, pump connection, indicator, and front detail.
- Detail is intentionally limited to Tank 05 for this approval phase.
- Remaining failed item for next phase: reuse the accepted detail across all configured tanks.

## Visual Continuation Phase 5 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-05-tanks.png`
- All ten tanks now reuse the detailed production tank geometry.
- Tank order, spacing, levels, and status indicators remain visible.
- Remaining failed item for next phase: pumps, valves, and pipes need a connected process manifold.

## Visual Continuation Phase 6 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-06-piping.png`
- A front process manifold now runs across the line.
- Tank drop pipes, valve bodies, colored handles, pump/motor blocks, and mounting plates are visible.
- Remaining failed item for next phase: walkway should read more like supported metal grating with consistent handrails.

## Visual Continuation Phase 7 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-07-structure.png`
- Walkway grating slats, handrails, support posts, and stair connection are visible.
- Structure remains aligned to the tank line and does not float.
- Remaining failed item for next phase: material library should expose the required industrial material set.

## Visual Continuation Phase 8 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-08-materials.png`
- Material library now exposes the required named materials.
- WPF Phong-style material groups are used because SharpDX/PBR migration has not been verified for this solution.
- Remaining failed item for next phase: lighting and contact shadows need stronger depth.

## Visual Continuation Phase 9 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-09-lighting.png`
- Key/fill/rim lighting is stronger and lightweight contact shadows are visible.
- Classic WPF Helix does not provide the requested SharpDX shadow-map pipeline without a package migration.
- Remaining failed item for next phase: left navigation must be cleaned up and camera commands moved out of the nav.

## Visual Continuation Phase 10 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-10-dashboard.png`
- Left navigation now contains only Overview, Process, Tanks, Hoist, Alarms, Trends, Reports, and Settings.
- Camera commands are in a compact toolbar above the viewport.
- Plant command buttons remain in the bottom control row.
- Host-control screenshot mode captures the dashboard shell without using the blocking `MainForm` capture path.
- Remaining failed item for next phase: final runtime animation/live-value verification is pending.

## Visual Continuation Phase 11 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-11-runtime.png`
- Runtime verifier: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Hoist X movement now refreshes the dynamic hoist visual layer without rebuilding the static tank/rail/walkway scene.
- Lowering, Processing, and Lifting states update through the timer-safe binding path without blocking or cross-thread errors in the verifier.
- KPI/header/alarm bindings verify total tanks, running tanks, hoist position, current step, remaining seconds, emergency state, active alarm count, and manual/auto mode.
- PLC separation remains safe: `SafePlcAdapter` keeps writes disabled by default and requires an explicit confirmation reason before write enable.
- Remaining difference for final handoff: the central machine is still procedural Helix geometry, not a photoreal imported plant asset.

## Visual Continuation Phase 12 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-12-final.png`
- Final build evidence: Debug `bin\VisualP11Debug\` and Release `bin\VisualP11Release\` passed after the last 3D update change.
- Final runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Dashboard shell, side navigation, central WPF/Helix viewport, right KPI cards, bottom command row, alarm table, single H1 hoist, tank line, pipes, pumps, walkway, plates, materials, and lighting are present in the final evidence.
- Current app logic shows `Plant: Alarm` when active alarms are present. The target reference text asks for `Plant: Normal` and `Active Alarms: 3` at the same time; this is treated as a reference-state conflict, not a code defect.
- Remaining difference: the machine is a procedural Helix model. A photoreal match would require a dedicated 3D asset pass or verified SharpDX/PBR migration.

## Visual Continuation Phase 13 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-13-round-geometry.png`
- Build evidence: Debug `bin\VisualP13Debug\` and Release `bin\VisualP13Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Added `IndustrialShape3DBuilder` for reusable low-poly cylinder meshes inside the existing WPF Helix viewport.
- Replaced visible box-only cues with round process piping, rounded guard rails, cylindrical pump bodies, valve handles, hoist wheel/motor/drum cues, lift hangers, and plate rivets.
- Default camera was widened after inspection to keep the full tank line visible while preserving the lower target framing.
- Remaining difference: visual quality is improved but remains procedural; imported high-fidelity assets or a verified SharpDX/PBR path would be needed for a near-photoreal match.

## Visual Continuation Phase 14 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-14-nav-labels.png`
- Build evidence: Debug `bin\VisualP14Debug\` and Release `bin\VisualP14Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Added `ScadaNavButton` owner-drawn WinForms navigation with simple line icons and selected-state styling.
- Replaced text-only left navigation with icon+label entries for Overview, Process, Tanks, Hoist, Alarms, Trends, Reports, and Settings.
- Added Helix `BillboardTextVisual3D` tank number labels so tank IDs are readable at dashboard camera distance.
- Remaining difference: nav icons are simplified line drawings, not a final branded icon asset set.

## Visual Continuation Phase 15 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-15-kpi-alarms.png`
- Build evidence: Debug `bin\VisualP15Debug\` and Release `bin\VisualP15Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Added `ScadaKpiIcon` owner-drawn WinForms icon control for KPI cards.
- Right-side KPI cards now include target-like icon badges, divider lines, fitted values, and captions.
- Alarm grid now has stronger headers, row height, alternating row color, horizontal grid lines, and severity/status color formatting.
- Remaining difference: KPI icons are procedural line drawings, not final branded artwork.

## Visual Continuation Phase 16 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-16-command-row.png`
- Build evidence: Debug `bin\VisualP16Debug\` and Release `bin\VisualP16Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Added `ScadaCommandButton` owner-drawn WinForms command button control.
- Bottom command row now uses icon+text buttons for Auto, Manual, Start Job, Stop Plant, Emergency Stop, Reset, and IP Connect.
- Emergency and stop actions use red/danger styling; start uses green; connection/auto use blue.
- Current-step KPI layout was adjusted to prevent multiline clipping.
- Remaining difference: command icons are procedural line drawings, not final branded artwork.

## Visual Continuation Phase 17 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-17-header.png`
- Build evidence: Debug `bin\VisualP17Debug\` and Release `bin\VisualP17Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Added `ScadaBrandHeader` owner-drawn WinForms brand/header control.
- Added `ScadaHeaderItem` owner-drawn Label-compatible header item control for bound status values.
- Header now shows a hamburger cue, blue `USR` branding, plant status icon/dot, Auto/Manual badge, H1 hoist icon/dot, clock/date icon, and operator icon.
- Existing dashboard binding fields remain Label-compatible for runtime verification.
- Remaining difference: header icons are procedural line drawings, not final branded artwork.

## Visual Continuation Phase 18 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-18-kpi-polish.png`
- Build evidence: Debug `bin\VisualP18Debug\` and Release `bin\VisualP18Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Current Step KPI card spacing was adjusted so the icon and multiline step/time text do not overlap.
- Active Alarms KPI card now has target-style Critical, Major, and Minor mini-values below the main alarm count.
- Alarm severity values are derived from the existing simulation/database status values; no PLC mappings or writes were changed.
- Remaining difference: KPI icons are procedural line drawings, not final branded artwork.

## Visual Continuation Phase 19 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-19-menu-shell.png`
- Build evidence: Debug `bin\VisualP19Debug\` and Release `bin\VisualP19Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Hamburger header now owns a grouped dashboard menu instead of exposing many commands in the main header.
- Menu groups are Plant Operations, Production, Engineering, Maintenance, and Account.
- Bottom command row keeps only common operating controls and now uses target labels `Stop` and `IP Connection`.
- Right sidebar now includes a fixed `View All Alarms` action at the bottom.
- Verifier now checks the grouped menu structure plus `View All Alarms` and `IP Connection` presence.
- Existing command routing, permission checks, simulation, and PLC/IP safety behavior remain unchanged.

## Visual Continuation Phase 20 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-20-account-menu.png`
- Build evidence: Debug `bin\VisualP20Debug\` and Release `bin\VisualP20Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Operator header item now has a dropdown arrow like the reference.
- Operator header owns a focused account context menu with Login Another and Logout.
- Header label rendering now uses no-wrap text trimming so the right-side user label stays clean at dashboard size.
- Verifier now checks the account menu item count and command tags.
- Existing command routing, permission checks, simulation, and PLC/IP safety behavior remain unchanged.

## Visual Continuation Phase 21 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-21-view-menu.png`
- Build evidence: Debug `bin\VisualP21Debug\` and Release `bin\VisualP21Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Removed the visible viewport camera toolbar so the central plant area matches the cleaner reference layout.
- Added a grouped hamburger `View` submenu containing Fit Plant, Front View, Top View, Left View, and Right View.
- Camera commands still execute locally against the WPF/Helix viewport when selected from the menu.
- Verifier now checks the `View` submenu and confirms `Fit Plant` is not present as a visible dashboard control.
- Existing command routing, permission checks, simulation, and PLC/IP safety behavior remain unchanged.

## Visual Continuation Phase 22 Notes

- Screenshot: `docs/3d-dashboard/screenshots/sidebar-menu-trigger.png`
- Build evidence: Debug `bin\SidebarMenuDebug\` and Release `bin\SidebarMenuRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Removed the grouped menu trigger from the top header brand area.
- Added a left-sidebar `Menu` entry that opens the same grouped submenu on hover or click.
- Header brand is now display-only and no longer has a dashboard context menu attached.
- Verifier now confirms the sidebar menu entry exists and the grouped menu is not attached to the header brand.
- Existing command routing, permission checks, simulation, and PLC/IP safety behavior remain unchanged.

## Visual Continuation Phase 23 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-23-sidebar-submenus.png`
- Build evidence: Debug `bin\SidebarMenuP23Debug\` and Release `bin\SidebarMenuP23Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Removed the old grouped dashboard menu and removed Plant Operations, Production, and Engineering menu groups.
- Added direct left-sidebar `View` submenu for Fit Plant, Front View, Top View, Left View, and Right View.
- Moved Add Tank, Edit Tank, and Remove Tank into the `Tanks` sidebar submenu.
- Added direct left-sidebar `Recipe` submenu with Create Recipe and Edit Recipe, both routed to the existing Recipe editor command.
- Moved User Management into the `Settings` sidebar submenu.
- Existing command routing, permission checks, simulation, and PLC/IP safety behavior remain unchanged.

## Visual Continuation Phase 24 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-24-startup-performance.png`
- Build evidence: Debug `bin\StartupPerf5Debug\` and Release `bin\StartupPerf5Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Startup performance evidence: `StartupPerfProbe` reported repeated database initialization calls are now skipped after the first successful initialization and live dashboard 3D host creation is deferred (`32609 ms` eager vs `153 ms` deferred in this environment).
- Splash screen now starts immediately and performs database initialization in the background instead of blocking the first paint.
- Splash progress delay was reduced so login is shown soon after initialization completes.
- `MainForm` creates the dashboard shell first and defers WPF/Helix viewport initialization until after the form can paint.
- Dashboard render remains visually intact after the startup changes.

## Visual Continuation Phase 25 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-25-18-tanks.png`
- Build evidence: Debug `bin\EighteenTanksDebug\` and Release `bin\EighteenTanksRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Database evidence: `DatabaseHelper.InitializeDatabase()` upgraded the existing SQLite database to active main-line tanks `1-18`.
- The host dashboard screenshot shows the single main line with 18 tanks and `TOTAL TANKS` KPI value `18`.

## Visual Continuation Phase 26 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-26-clear-tank-view.png`
- Build evidence: Debug `bin\ClearViewDebug\` and Release `bin\ClearViewRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Removed the dark hoist-rail cross ties and tank-top dark rods because they blocked tank visibility at the dashboard camera angle.
- The 18-tank line remains visible with one H1 hoist and the clearer top view of the tanks.

## Visual Continuation Phase 27 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-27-mouse-tank-style.png`
- Build evidence: Debug `bin\MouseTank2Debug\` and Release `bin\MouseTank2Release\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- The central dashboard viewport uses `HelixToolkit.Wpf` mouse gestures for rotate, pan, zoom, zoom-around-cursor, rotate-around-click, and double-click reset.
- Tank details were adjusted toward the supplied real-plant reference: lighter grey bodies, readable white labels, round status lamps, grey pipes, blue pump bodies, and orange valve handles.

## Visual Continuation Phase 28 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-28-horizontal-rack.png`
- Close crop: `docs/3d-dashboard/screenshots/phase-28-horizontal-rack-close.png`
- Build evidence: Debug `bin\HorizontalRackDebug\` and Release `bin\HorizontalRackRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- The old vertical hanging plate bundle was replaced by a shallow horizontal carrier/rack under H1.
- The rack remains in the dynamic hoist visual layer, so hoist X movement and lift-state changes carry the rack with H1.

## Visual Continuation Phase 29 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-29-real-hoist-plates.png`
- Close crop: `docs/3d-dashboard/screenshots/phase-29-real-hoist-plates-close.png`
- Build evidence: Debug `bin\RealHoistDebug\` and Release `bin\RealHoistRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Hoist yellow was brightened and the H1 portal was upgraded with beam side plates, bolt cues, side drive/motor detail, black lift guide details, and chain/rod cues.
- The tank-parallel carrier now holds individual vertical green hanging work plates, matching the real hoist reference more closely while preserving H1 movement and lift binding.

## Visual Continuation Phase 30 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-30-real-lift-clearance.png`
- Close crop: `docs/3d-dashboard/screenshots/phase-30-real-lift-clearance-close.png`
- Build evidence: Debug `bin\RealLiftClearanceDebug\` and Release `bin\RealLiftClearanceRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Raised H1 lift height was increased so hanging plates clear the tank rim instead of staying inside the tank.
- Lowering and Processing states keep the plates inside the tank, preserving the simulated electroplating sequence behavior.

## Visual Continuation Phase 31 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-31-open-plates-hoist-name.png`
- Close crop: `docs/3d-dashboard/screenshots/phase-31-open-plates-hoist-name-close.png`
- Build evidence: Debug `bin\OpenPlatesNameDebug\` and Release `bin\OpenPlatesNameRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Removed the solid side-cover panels from the H1 plate rack and replaced them with open round side rails.
- Added a readable `USR H1` yellow hoist label instead of copying the reference image company text.

## Visual Continuation Phase 32 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-32-real-hoist-match.png`
- Close crop: `docs/3d-dashboard/screenshots/phase-32-real-hoist-match-close.png`
- Build evidence: Debug `bin\RealHoistMatchDebug\` and Release `bin\RealHoistMatchRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Removed the silver rack cage so the hanging plates are not boxed in and remain visible like the supplied real hoist reference.
- Repositioned the `USR H1` label onto the yellow front beam and added a verifier check for the label placement.

## Visual Continuation Phase 33 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-33-no-plate-shadow.png`
- Close crop: `docs/3d-dashboard/screenshots/phase-33-no-plate-shadow-close.png`
- Build evidence: Debug `bin\NoPlateShadowDebug\` and Release `bin\NoPlateShadowRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Removed the black H1 `SoftContactShadow` pad from the tank opening below the hanging plates.
- Added a verifier check so that black plate-area pad does not return.

## Visual Continuation Phase 34 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-34-clean-tank-opening.png`
- Crop: `docs/3d-dashboard/screenshots/phase-34-clean-tank-opening-close.png`
- Build evidence: Debug `bin\CleanTankOpeningDebug\` and Release `bin\CleanTankOpeningRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Removed the duplicate top liquid panel from `AddProductionTankDetails`.
- Removed tall internal `TankBase` liner wall panels that made each tank look like another tank inside it.

## Visual Continuation Phase 35 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-35-real-hoist-motor.png`
- Crop: `docs/3d-dashboard/screenshots/phase-35-real-hoist-motor-close.png`
- Build evidence: Debug `bin\RealHoistMotorDebug\` and Release `bin\RealHoistMotorRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Replaced the simple hoist motor cue with a connected bridge-drive assembly.
- Added gearbox, shaft/coupler, finned horizontal motor body, dark end caps, bolts, and support bracket on the yellow H1 bridge.

## Visual Continuation Phase 36 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-36-connected-hoist-motor-name.png`
- Crop: `docs/3d-dashboard/screenshots/phase-36-connected-hoist-motor-name-close.png`
- Build evidence: Debug `bin\ConnectedHoistMotorDebug\` and Release `bin\ConnectedHoistMotorRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Shortened and reattached the H1 side motor so it sits on a bolted gearbox and bracket assembly instead of protruding far outside the hoist body.
- Replaced the small hoist tag with beam-aligned `USR H1 500KG` text to look closer to industrial header marking without copying the reference branding.

## Visual Continuation Phase 37 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-37-smooth-hoist-motion.png`
- Crop: `docs/3d-dashboard/screenshots/phase-37-smooth-hoist-motion-close.png`
- Build evidence: Debug `bin\SmoothHoistMotionDebug\` and Release `bin\SmoothHoistMotionRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Moved H1 visual smoothing into `Plant3DView` using a WPF `DispatcherTimer`, so 3D animation continues independently of WinForms simulator refresh timing and camera/view changes.
- Added continuous lift progress to the hoist builder, so lowering and lifting interpolate through intermediate Z positions instead of jumping instantly.
- Added a deferred-view command guard so Fit/Front/Top/Left/Right view commands are handled safely even when the 3D viewport was deferred during startup.

## Visual Continuation Phase 38 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-38-sequenced-hoist-motion.png`
- Crop: `docs/3d-dashboard/screenshots/phase-38-sequenced-hoist-motion-close.png`
- Build evidence: Debug `bin\SequencedHoistMotionDebug\` and Release `bin\SequencedHoistMotionRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Increased service-side lower/lift transition hold time so jobs do not immediately jump from Lowering to Processing or from Lifting to Moving.
- Added viewport sequence gating so an early Moving target first raises the plates, keeps X fixed, and only then allows horizontal travel.

## Visual Continuation Phase 39 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-39-command-state-binding.png`
- Build evidence: Debug `bin\CommandStateBindingDebug\` and Release `bin\CommandStateBindingRelease\` passed.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS`.
- Added active-state rendering to SCADA command buttons.
- Bound command enabled/active state from live dashboard data so Auto, Manual, Start Job, Stop, Emergency Stop, and Reset reflect the actual process state after commands run.
- Verifier now checks normal auto, active job, and emergency stop command-row states.

## Visual Continuation Phase 42 Notes

- Screenshot: `docs/3d-dashboard/screenshots/phase-42-real-h1-imported.png`
- Full dashboard: `docs/3d-dashboard/screenshots/phase-42-real-h1-dashboard.png`
- Build evidence: Debug `bin\Real3DPhase42AssetDebug\` and Release `bin\Real3DPhase42AssetRelease\` passed with zero warnings and errors.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS` with imported-asset readiness, geometry bounds, X travel, smooth lift, tank clearance, and lift-before-travel checks.
- The complete H1 now loads from four independent OBJ assets instead of rendering the procedural fallback.
- The imported model follows the real-machine reference with a wide yellow fabricated bridge, two rail columns, bogies, attached geared motors, moving inner frame, linked hangers, and twelve exposed PCB boards.
- This phase proves the asset and animation hierarchy. Final photoreal material quality still depends on the Phase 44 SharpDX/PBR runtime switch and, ideally, customer-approved CAD/texture assets.

## Visual Continuation Phase 43 Notes

- Close screenshot: `docs/3d-dashboard/screenshots/phase-43-real-tanks-pumps.png`
- Full dashboard: `docs/3d-dashboard/screenshots/phase-43-real-plant-dashboard.png`
- Build evidence: Debug `bin\Real3DPhase43Debug\` and Release `bin\Real3DPhase43Release\` passed with zero warnings and errors.
- Runtime evidence: `Phase11RuntimeVerifier` returned `PHASE11 PASS` with 18-instance asset counts, shared frozen geometry, one liquid surface per tank, dynamic level movement, dynamic fault-lamp binding, H1 motion, and safe PLC write blocking.
- All stations now load separate imported tank, manifold, pump/motor, and walkway modules while preserving the configured 1.45-unit tank pitch.
- Static imported geometry is shared rather than cloned 18 times. Liquid, label, selection and status visuals update independently without rebuilding the static plant scene.
- The inspection render confirms clean open tanks, ribs, sight glasses, flanges, valves, detailed pump/motor assemblies and open service grating.
- Final PBR material and lighting quality remains Phase 44 work under `HelixToolkit.Wpf.SharpDX`.
