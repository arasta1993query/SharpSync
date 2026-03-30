using SharpSync.Core.Attributes;
using System;

namespace SharpSync.Example.WebApi.Models;

[SharpSync]
public class DemoForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}
