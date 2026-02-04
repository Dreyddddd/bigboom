# CPU Temperature Viewer (Windows)

This is a small .NET console app that prints CPU temperature readings via WMI.

## Requirements
- Windows 10/11
- .NET 6 SDK or later

## Build & Run
```bash
cd CpuTempViewer
dotnet restore
dotnet run
```

## Notes
- The app reads `MSAcpi_ThermalZoneTemperature` from WMI (`root\\WMI`). Some systems do not expose CPU temperature through this provider.
- If no values appear, try running the console as Administrator or use OEM monitoring tools.
