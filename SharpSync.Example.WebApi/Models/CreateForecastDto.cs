namespace SharpSync.Example.WebApi.Models;

public class CreateForecastDto
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}
