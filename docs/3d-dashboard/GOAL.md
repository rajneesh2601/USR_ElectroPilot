# USR ElectroPilot 3D Dashboard Goal

Complete the USR ElectroPilot 3D dashboard until it is a functional, client-ready WinForms .NET Framework 4.8.1 application using WPF/Helix inside WinForms.

The dashboard must preserve existing application and PLC/IP communication functionality, keep the main app as WinForms, and use WPF/Helix only for the central 3D viewport.

## Current Product Direction

- One main electroplating line.
- Tank count increases through Add Tank.
- One main hoist only: H1.
- No multiple production-line creation.
- No multiple SCADA tank rows.
- Existing PLC/IP functionality must remain safe.

## Completion Criteria

The goal is complete only when:

1. The complete WinForms dashboard matches the target layout closely.
2. The central machine is detailed, interactive Helix 3D.
3. The portal hoist geometry is mechanically correct.
4. Live and simulated machine values work.
5. Every implementation phase is built and verified.
6. The final visual acceptance checklist passes.
7. Existing PLC/IP functionality remains safe and operational.

## Technology Constraints

- C#
- Windows Forms
- .NET Framework 4.8.1
- ADO.NET SQLite
- Existing solution architecture
- WPF `ElementHost` inside WinForms
- Helix Toolkit for 3D rendering
- Existing logging and communication services

Do not convert the full application to WPF.

Do not use the dashboard reference image as a static background.

## Phase 0 Baseline Findings

- Current project target is .NET Framework 4.8.1.
- Current 3D package is `HelixToolkit.Wpf 3.1.2`; `HelixToolkit.Wpf.SharpDX` is not installed yet.
- Current 3D integration uses WinForms `ElementHost` in `Plant3DHostControl`.
- Current `Plant3DView` owns the full dashboard shell in WPF, including header, navigation, KPI cards, controls, alarm panel, and 3D viewport.
- Current product direction is single main line and single main hoist `H1`.
- Existing communication code is a raw TCP/IP client/form for future PLC/device communication, not a confirmed PLC protocol adapter.
- Existing simulation code updates tanks, rectifiers, jobs, alarms, and hoist state from application services and SQLite.
- No external 3D assets were found in the repository.
