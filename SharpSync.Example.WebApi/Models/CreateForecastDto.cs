using System.ComponentModel.DataAnnotations;

namespace SharpSync.Example.WebApi.Models;

public class CreateForecastDto
{
    [Required]
    public DateTime Date { get; set; }

    [Range(-100, 100)]
    public int TemperatureC { get; set; }

    [StringLength(50, MinimumLength = 3)]
    public string? Summary { get; set; }
}
