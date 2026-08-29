# USR ElectroPilot Work Summary

## Current project path

All application source work is inside:

`D:\Dev\USR_ElectroPilot\USR_ElectroPilot`

The outer folder `D:\Dev\USR_ElectroPilot` is the repository and solution root. It contains Git metadata, the solution file, packages, ignored build output, and IDE files.

## Completed work

- Created database helper, SQLite connection factory, repositories, models, helpers, and services.
- Added startup flow with `SplashForm` and login flow with `LoginForm`.
- Added dashboard UI in `MainForm` with toolbar, status cards, SCADA-style tank mimic, wagons, rectifiers, and alarm grid.
- Added custom controls for tanks, wagons, rectifiers, status cards, and a full-line SCADA overview.
- Added tank add/edit/remove workflow.
- Added admin panel with user management, user activity, audit log, and system settings screens.
- Added operational forms:
  - `AlarmHistoryForm`
  - `RecipeForm`
  - `LoadHistoryForm`
  - `ShiftReportForm`
  - `TrendForm`
- Added `System.Windows.Forms.DataVisualization` chart support for trends.
- Fixed the `MainForm` timer crash by initializing the WinForms component container before constructing `simulatorTimer`.
- Added SCADA operations from `ch.txt`: Start All, Stop All, Reset Alarms, per-tank Start/Stop/Fault/Reset/Remove buttons, live clock, and plant status label.
- Added default seed creation for tanks `T1` through `T10` when the tank table is empty.
- Updated simulator behavior so running tanks persist live values and generate warning/fault alarms.
- Updated login flow so logout returns to the login screen and another user can sign in immediately.
- Added visible default-user guidance on `LoginForm` and corrected the default `viewer` role.
- Added a tank historian repository/service and changed tank trends to use recorded time-series data.
- Reworked the tank tab into a reference-style electroplating SCADA mimic with two process rails, compact tank cells, live values, wagon/hoist indicators, alarm legend, and right-click tank actions.
- Added `ProcessSteps` and `HoistStatus` SQLite tables with ADO.NET repositories/services and default nine-stage electroplating sequence.
- Added Auto/Manual, Start Cycle, Stop Cycle, Emergency Stop, and Reset controls to the main toolbar.
- Added a timer-driven hoist/carrier simulation with persisted hoist state, process countdown, current stage display, and right-side dashboard cards.
- Changed the SCADA mimic palette to a distinct steel/blue industrial style instead of matching the reference video colors.
- Removed the visible multiple-row/multiple-line creation path; tank count now increases through `Add Tank` on the main production line.
- Improved SCADA overview spacing and status-card text fitting so longer process names do not clip.
- Fixed hoist movement arrival logic so transitions such as T1 to T2 clamp to the target and continue into processing.
- Upgraded the SCADA overview with an internal plant header, framed process area, darker operator panel, row labels, and clearer tank status/value styling.
- Added first-class `Hoists` and `Jobs` tables plus `HoistService`, `ProcessRecipeService`, and `JobService` for production sequencing.
- Added `HoistControl` and upgraded the overview to show hoist state, process recipe, active jobs, and tank occupancy.
- Process sequencing now moves jobs through Moving, Lowering, Processing, Lifting, Complete states against the configured recipe.
- Latest direction: keep one main line, normalize new/edit tanks to `LineId = 1`, and support only one main hoist, `H1`.
- Added a future-use TCP/IP communication module with `IpConnectionClient`, `IpDataMessageModel`, and `IpConnectionForm` for connecting by IP/port, viewing received data, and sending raw text data.
- Fixed startup migration failures caused by duplicate `Tanks.TankNumber` and `Lines.LineName` values from earlier multi-line data by normalizing tanks and lines safely before assigning final main-line values.
- Upgraded `ScadaOverviewControl` toward the attached 3D SCADA reference using live WinForms/GDI+ rendering: dark dashboard header, left navigation, right KPI cards, bottom alarm table, isometric process tanks, blue rails/walkway, and a heavy yellow portal wagon that moves horizontally above the tanks from the existing hoist state.
- Hid the older top status-card strip on `MainForm` because the upgraded overview now contains the dashboard KPI cards in the right-side panel.
- Added a WPF `ElementHost` dashboard path inside WinForms using HelixToolkit.Wpf: `Plant3DHostControl` hosts `Plant3DView`, and `MainForm` now renders the main SCADA overview through an actual Helix 3D scene with tanks, rails, walkway, and portal hoists.
- Hid the legacy top menu/tab navigation and moved operator actions to live dashboard sidebar/bottom buttons.
- Changed database migration, hoist service, status summary, and dashboard rendering to keep/use only single hoist `H1`.

## Latest verified commits

- `7d3270c feat: add AlarmHistoryForm`
- `d9ee987 feat: add RecipeForm`
- `df943d4 feat: add LoadHistoryForm`
- `ddb14a4 feat: add ShiftReportForm`
- `8aaf09e feat: add TrendForm with DataVisualization chart`
- `9f9de57 feat: add CSV export to all forms`
- `31553b4 feat: apply role-based UI restrictions`
- `18c925f style: apply dark industrial theme to all forms`
- `292a9b1 fix: resolve all compile errors and warnings`
- `d9cc17f feat: add SCADA operation controls from ch prompt`
- `fe6f04a fix: return to login after user logout`
- This follow-up change adds historical tank trend recording from `newprpmpt.txt` guidance.
- This follow-up change adds a SCADA mimic tank overview based on the provided electroplating HMI reference.
- This follow-up change completes the requested process-step and hoist-status SCADA workflow using the existing project folder style.

## Release verification

- Debug build passes with 0 warnings and 0 errors.
- Release build passes with 0 warnings and 0 errors.
- Final smoke test covers database initialization, `MainForm` construction, and CSV export.
- `ch.txt` follow-up smoke test covers default tank seeding, tank start/fault/reset flow, active alarm reset, and `TankControl` operation buttons.
- Multi-login smoke test covers admin login, logout, viewer login, viewer role verification, and login form reset.
- Historian smoke test covers tank snapshot recording, history retrieval, and `TrendForm` load using time-series data.
- SCADA mimic verification covers debug/release build compilation and existing tank action wiring through the overview context menu.
- Process SCADA smoke test covers `ProcessSteps`, `HoistStatus`, and off-screen render of the overview dashboard.
- Earlier SCADA row smoke test is now superseded by the single-main-line direction; row creation is intentionally hidden.
- Hoist transition smoke test covers T1 to T2 movement reaching processing instead of staying in moving state.
- Sequencing smoke test covers starting a production job, advancing it through recipe steps, DB-backed single-hoist processing, and off-screen SCADA render with jobs/hoist state.
- IP connection smoke test covers loopback TCP connect, receive, send, and echo response.
- 3D-style dashboard render check saved an off-screen preview to `bin\dashboard-3d-render-check.png`.
- WPF/Helix integration builds in Debug and Release with 0 errors. Runtime host construction passes; full binding inside a PowerShell reflection host needs manual binding redirection, while the built EXE output includes the required dependency DLL and generated binding redirect.
- `v1.0.0` was published before the `ch.txt` follow-up additions.
