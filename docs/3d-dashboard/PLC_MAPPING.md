# USR ElectroPilot PLC Mapping

Do not guess PLC protocol, register/tag names, scaling, data type, or write safety. Keep unconfirmed PLC writes disabled.

## Known Communication Code

- `Services/IpConnectionClient.cs`: raw TCP client with connect, disconnect, receive loop, send text, status events, and error events.
- `Services/SafePlcAdapter.cs`: safety wrapper around `IpConnectionClient`; reads/status/error events are preserved, but writes are disabled by default and require an explicit confirmation reason before sending.
- `Forms/IpConnectionForm.cs`: manual test screen for IP/host, port, received text/hex log, and raw send input.
- No Modbus, Siemens, OPC UA, Mitsubishi, Omron, serial, register, tag, or address-specific PLC code was found.
- The current implementation is safe for future PLC work because it does not contain confirmed automatic PLC writes.

## Mapping Table

| Value | Existing source | Confirmed address/tag | Scaling | Read/write | Status | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| Plant status | Simulation/database | Unknown | Unknown | Read | Unconfirmed | Needs real PLC mapping |
| Auto/manual mode | Application state | Unknown | Unknown | Read/write | Simulation only | Writes must remain app-local until confirmed |
| Hoist H1 position | Hoist database/service | Unknown | Unknown | Read | Simulation only | Real encoder/register not confirmed |
| Hoist H1 command | Hoist service/dashboard | Unknown | Unknown | Write | Disabled for PLC | Unsafe to map without confirmation |
| Tank level | Tank simulation/database | Unknown | Unknown | Read | Simulation only | Sensor mapping missing |
| Tank temperature | Tank simulation/database | Unknown | Unknown | Read | Simulation only | Sensor mapping missing |
| Rectifier current | Rectifier simulation/database | Unknown | Unknown | Read/write | Simulation only | Real rectifier protocol missing |
| Rectifier voltage | Rectifier simulation/database | Unknown | Unknown | Read/write | Simulation only | Real rectifier protocol missing |
| Emergency stop | Application command/alarm | Unknown | Unknown | Write | Disabled for PLC | Must not write to PLC until confirmed |
| Alarm status | SQLite alarm service | Unknown | Unknown | Read | Application only | PLC alarm mapping missing |

## Phase 0 PLC Inspection Result

The current application has a generic TCP/IP test tool only. It can read and send raw text bytes, but it is not a confirmed PLC adapter. `SafePlcAdapter` exists for future integration and blocks writes by default. All dashboard values must remain simulation/database-backed until real protocol, address/tag, data type, scaling, and write permission information is provided.

## Missing Information Required Before Real PLC Writes

- PLC protocol
- PLC IP address and port
- Tag/register list
- Data types
- Scaling
- Read/write permissions
- Safety interlocks
- Emergency stop wiring behavior
