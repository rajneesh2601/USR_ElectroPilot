# USR ElectroPilot
**Industrial Electroplating SCADA Platform**
*Powered By US Robotics*

## Tech Stack
- C# .NET Framework 4.8.1
- Windows Forms
- ADO.NET + SQLite
- Visual Studio 2022

## Default Logins
| Username | Password  | Role       |
|----------|-----------|------------|
| admin    | admin123  | Admin      |
| sadmin   | sadmin123 | Supervisor |
| operator | op123     | Operator   |
| viewer   | view123   | Operator   |

## Database
Auto-created at runtime under `USR_ElectroPilot/bin/Debug/Database/usr_electropilot.db`

## How to Run
1. Open `USR_ElectroPilot.slnx` in Visual Studio 2022
2. Right-click solution and restore NuGet packages
3. Build and start with F5
4. Login with admin / admin123

## Branch Strategy
- `main` - stable releases only
- `develop` - active development
- `feature/` - one branch per feature

## Architecture
- Models - pure data classes
- Data - repositories and database access
- Services - business logic
- Controls - reusable UserControls
- Forms - UI only, calls Services
- Helpers - utilities and session
