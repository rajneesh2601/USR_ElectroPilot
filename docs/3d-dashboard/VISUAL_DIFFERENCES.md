# USR ElectroPilot Visual Differences

This file tracks the new continuation goal focused on making the 3D dashboard visually closer to the target dashboard.

## Current Major Differences

- The central machine still uses procedural low-poly geometry.
- Tank, pump, pipe, valve, hoist, plate, and material detail are improved but still not photoreal plant-asset quality.
- Dashboard navigation now uses simplified line icons; exact branded icon artwork is still not installed.
- KPI cards now use simplified line icons; exact branded icon artwork is still not installed.
- Header icons now use simplified line drawings; exact branded icon artwork is still not installed.
- Full `MainForm` screenshot capture remains unreliable in this environment because of the `ElementHost` path; host-control screenshot capture works.
- Real PLC protocol, tag/register mapping, scaling, and write permissions are still unconfirmed, so PLC writes remain disabled.

## Resolved Differences

- Camera framing now fits the main line and keeps H1 visible.
- H1 is presented over Tank 05 in the reference state.
- Left navigation now contains only the requested navigation entries; plant commands and camera commands are separated.
- Runtime verification confirms hoist movement updates dynamic 3D visuals without rebuilding the static tank line.
- Phase 13 adds cylindrical process pipes, rounded guard rails, pump bodies, valve handles, hoist wheels, motor/drum cues, round lift hooks, and plate rivet details.
- Phase 13 camera framing keeps the full tank line visible in the latest host-control screenshot.
- Phase 14 adds icon+label sidebar navigation and readable Helix billboard tank number labels.
- Phase 15 adds icon-backed KPI cards and stronger alarm table header/row/severity styling.
- Phase 16 adds icon+text bottom command buttons and fixes current-step KPI multiline clipping.
- Phase 17 adds a target-like brand header, status dots, mode badge, hoist indicator, clock/date icon, and operator icon.
- Phase 18 fixes Current Step KPI icon/text overlap and adds Critical/Major/Minor mini-values to the Active Alarms KPI card.
- Phase 19 adds a grouped hamburger menu with submenus, preserves a compact bottom command row, adds the right-sidebar View All Alarms action, and updates bottom labels to `Stop` / `IP Connection`.
- Phase 20 adds the right-header Operator dropdown cue and a focused account submenu for Login Another / Logout.
- Phase 21 moves camera/view commands into a grouped hamburger `View` submenu and removes the visible viewport camera toolbar.
- Phase 22 moves the grouped dashboard menu trigger from the top header into the left sidebar `Menu` entry with hover/click submenu access.
- Phase 23 removes the old Plant Operations, Production, and Engineering menu groups; `View`, `Tanks`, `Recipe`, and `Settings` now own their relevant submenus directly from the sidebar.

## Required Direction

- Preserve the existing WinForms dashboard shell.
- Preserve WPF/Helix inside WinForms.
- Preserve current simulation, database, and safe PLC/IP structure.
- Improve one visual system at a time with build and screenshot evidence.
