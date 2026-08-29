# USR ElectroPilot Codex Reference

Last updated: 2026-08-22

## Project Location

- Repository root: `D:\Dev\USR_ElectroPilot`
- Application source: `D:\Dev\USR_ElectroPilot\USR_ElectroPilot`
- Solution: `USR_ElectroPilot.slnx`
- Target framework: .NET Framework 4.8.1
- UI framework: Windows Forms
- Database: SQLite through ADO.NET and `System.Data.SQLite.Core`
- Avoided patterns: Entity Framework, DAL/BLL naming

## Current Git State

- Last pushed commit on `main` and `develop`: `beadcc4 feat: add production job hoist sequencing`
- Tag `v1.0.0` points to the earlier release commit before the later SCADA/hoist additions.
- Current local workspace has uncommitted changes after `beadcc4`.
- New local files seen in the workspace:
  - `Forms/DashboardWindowManager.cs`
- Treat those local changes as user/workspace changes. Do not revert them without explicit instruction.
- Latest direction: do not create multiple production lines or SCADA tank rows. Keep one main line, increase the tank count with `Add Tank`, and support only one main hoist, `H1`.

## Application Purpose

USR ElectroPilot is an electroplating plant SCADA/HMI simulator. It models tanks, rectifiers, alarms, users, process recipes, hoists, and production jobs. The current direction is to move from simple tank monitoring toward realistic electroplating automation where hoists carry jobs through a configured tank recipe.

## Main Runtime Flow

1. `Program.Main()` starts the application.
2. `SplashForm` initializes the startup flow.
3. `LoginForm` authenticates users.
4. `MainForm` opens the SCADA dashboard.
5. Dashboard timer updates live plant simulation and UI.
6. Logout returns to login so multiple users can sign in without closing the login system.

## User Roles

Default seeded users:

- `admin / admin123` -> Admin
- `sadmin / sadmin123` -> Supervisor
- `operator / op123` -> Operator
- `viewer / view123` -> Viewer

Role behavior:

- Admin/Supervisor can access management actions such as adding/editing/removing tanks.
- Tank count is increased with `Add Tank`; multiple row/line creation is disabled.
- Viewer is intended as read-only/limited access.

## Important Folders

- `Models`: Plain model classes.
- `Data`: SQLite repositories and database bootstrap.
- `Services`: Business/application services.
- `Controls`: Custom WinForms controls.
- `Forms`: Main UI screens.
- `Forms/Admin`: Admin screens.
- `Helpers`: Session, UI, password, logging, CSV export, constants.
- `Database`: Project database folder placeholder.
- `Logs`: Runtime log placeholder.

## Core Forms

- `MainForm`: Main SCADA dashboard.
- `LoginForm`: User login.
- `SplashForm`: Startup screen.
- `TankEditForm`: Tank create/edit.
- `AlarmHistoryForm`: Alarm history.
- `RecipeForm`: Recipe screen.
- `LoadHistoryForm`: Load history.
- `ShiftReportForm`: Shift report screen.
- `TrendForm`: Trend charting backed by tank history.
- `IpConnectionForm`: Raw TCP/IP test screen for future PLC/device integration.
- `AdminPanelForm`: Admin navigation.
- `UserManagementForm`: User administration.
- `UserActivityForm`: Activity audit.
- `AuditLogForm`: Audit log.
- `SystemSettingsForm`: Settings editor.

## Key Controls

- `ScadaOverviewControl`: Main industrial plant overview. Draws tank lines, tanks, hoists, process panels, recipe grid, active jobs, and tank occupancy.
- `HoistControl`: Hoist status card/control for hoist view.
- `TankControl`: Older individual tank card control.
- `WagonControl`: Wagon/carrier status control.
- `RectifierControl`: Rectifier card.
- `StatusCardControl`: Dashboard summary cards.

## Database Tables

Seeded/implemented tables include:

- `Users`
- `UserActivity`
- `AuditLogs`
- `SystemSettings`
- `Tanks`
- `TankHistory`
- `ProcessSteps`
- `HoistStatus`
- `Hoists`
- `Jobs`
- `Wagons`
- `Rectifiers`
- `Alarms`
- `Recipes`
- `Loads`
- `ShiftReports`
- `Lines` may exist in the current local workspace based on the uncommitted line files.

Important schema notes:

- `ProcessSteps` originally used `TankId` and `StepName`.
- Later compatibility fields were added: `TankNo` and `ProcessName`.
- `HoistStatus` exists for compatibility with earlier single-hoist work.
- `Hoists` and `Jobs` are the newer first-class production sequencing tables.
- `Lines` remains as compatibility schema, but runtime behavior is constrained to `LineId = 1` / `Main Line`.
- `SystemSettings.ScadaTankRows` may exist from earlier work, but visible row creation is disabled.

## Process Recipe

Default nine-stage electroplating sequence:

1. Loading Station
2. Cleaning Tank
3. Rinse Tank 1
4. Acid Tank
5. Rinse Tank 2
6. Electroplating Tank
7. Rinse Tank 3
8. Drying Tank
9. Unloading Station

The recipe is read through `ProcessRecipeService`.

## Hoist And Job Sequencing

Current intended sequence:

```text
Moving -> Lowering -> Processing -> Lifting -> next tank
```

Main objects:

- `HoistModel`
- `JobModel`
- `ProcessStepModel`

Main services:

- `HoistService`
- `JobService`
- `ProcessRecipeService`
- `IpConnectionClient` provides reusable TCP connect/read/send behavior and raises UI-safe consumable events.

Hoist states:

- `Idle`
- `Moving`
- `Lowering`
- `Processing`
- `Lifting`
- `Fault`
- `EmergencyStop`

The last verified sequencing smoke test created a job, assigned a hoist, advanced through recipe steps, and reached `Processing`.

## SCADA Dashboard Features

Implemented SCADA/HMI behavior:

- Dark header and industrial steel/blue plant palette.
- Horizontal main-line tank layout. Increase the configured process by adding tanks, not by creating more lines.
- Moving hoist animation.
- Multi-hoist support on the main line.
- Tank status colors.
- Tank occupancy display.
- Process recipe panel.
- Hoist queue panel.
- Active job panel.
- Alarm panel/grid.
- Right-side dashboard summary cards.
- Auto Mode and Manual Mode.
- Start Cycle and Stop Cycle.
- Emergency Stop and Reset.

## Alarm System

Alarm generation includes:

- Tank fault.
- High temperature.
- Low chemical level.
- Emergency stop.
- Hoist movement error.
- Process timeout logic exists but should be reviewed for realistic timeout thresholds.

Duplicate active alarms are deduped by source/message in `AlarmService` and `AlarmRepository`.

## Trends And Historian

- `TankHistoryRepository` records tank snapshots.
- `TankHistoryService` writes and retrieves historian data.
- `TrendForm` uses recorded time-series tank history for tank trends.

## Build And Verification

Known build command:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' USR_ElectroPilot.slnx /p:Configuration=Release /p:Platform="Any CPU" /v:minimal
```

When the app is running under Visual Studio, normal `bin\Debug\USR_ElectroPilot.exe` can be locked. Use a separate debug output for verification:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' USR_ElectroPilot.slnx /p:Configuration=Debug /p:Platform="Any CPU" /p:OutDir="bin\VerifyDebug\" /v:minimal
```

Last known checks before this reference:

- Release build passed.
- Debug verification build passed with `bin\VerifyDebug`.
- Sequencing smoke test passed for job/hoist process advancement.
- SCADA render smoke tests passed.

No build was run while creating this reference file.

## Current Risks / Watch Items

- The working tree is currently dirty with many local changes. Review before committing.
- Some local changes appear to introduce line-management features. Confirm whether they are intentional before modifying them.
- Existing databases may contain older seed data such as 10 tanks even though the default new seed sequence creates 9 process tanks.
- `HoistStatus` and `Hoists` overlap conceptually. Prefer `Hoists`/`Jobs` for new sequencing work, keep `HoistStatus` only for compatibility unless refactoring is requested.
- The dashboard has evolved quickly; manual UI verification in the running WinForms app is important after renderer changes.

## Recommended Next Work

1. Review and stabilize the uncommitted local dashboard and single-line changes.
2. Keep hoist logic focused on one main hoist, `H1`; do not add multi-hoist routing unless the product direction changes again.
3. Add a proper recipe editor for `ProcessSteps`.
4. Add job creation controls and job queue management instead of only `Start Cycle`.
5. Add persisted tank occupancy if occupancy should survive app restart.
6. Add more focused smoke tests for:
   - single-hoist sequencing,
   - emergency stop recovery,
   - manual mode movement,
   - recipe changes,
   - main-line tank count changes.

## Coding Rules To Preserve

- Keep work inside `D:\Dev\USR_ElectroPilot\USR_ElectroPilot` for application source.
- Do not place source files outside the project path unless explicitly requested.
- Use ADO.NET SQLite repositories.
- Do not introduce Entity Framework.
- Use existing folder names: `Models`, `Data`, `Services`, `Controls`, `Forms`, `Helpers`, `Database`.
- Do not revert user/local changes without explicit approval.
- Use `apply_patch` for manual code/document edits.
