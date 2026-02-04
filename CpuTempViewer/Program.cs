using System.Globalization;
using System.Management;

static double? TryToCelsius(object? wmiValue)
{
    if (wmiValue is null)
    {
        return null;
    }

    if (!double.TryParse(wmiValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var raw))
    {
        return null;
    }

    // WMI MSAcpi_ThermalZoneTemperature reports in tenths of Kelvin.
    return (raw / 10.0) - 273.15;
}

Console.WriteLine("CPU temperature (WMI MSAcpi_ThermalZoneTemperature)");
Console.WriteLine(new string('=', 58));

try
{
    using var searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
    using var results = searcher.Get();

    if (results.Count == 0)
    {
        Console.WriteLine("No temperature sensors were reported. Try running as administrator or use a vendor tool.");
        return;
    }

    var anyOutput = false;
    foreach (ManagementObject sensor in results)
    {
        var name = sensor["InstanceName"]?.ToString() ?? "Unknown";
        var temperature = TryToCelsius(sensor["CurrentTemperature"]);

        if (temperature is null)
        {
            Console.WriteLine($"{name}: unavailable");
            continue;
        }

        Console.WriteLine($"{name}: {temperature.Value:F1} °C");
        anyOutput = true;
    }

    if (!anyOutput)
    {
        Console.WriteLine("Sensors were detected, but no readable temperatures were found.");
    }
}
catch (ManagementException ex)
{
    Console.WriteLine("Failed to query WMI for temperatures.");
    Console.WriteLine(ex.Message);
}
