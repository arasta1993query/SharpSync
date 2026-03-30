using Microsoft.AspNetCore.Mvc;
using SharpSync.Core.Attributes;
using SharpSync.Example.WebApi.Models;
using System.Collections.Generic;

namespace SharpSync.Example.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[SharpSync]
public class DemoForecastController : ControllerBase
{
    [HttpGet]
    public IEnumerable<DemoForecast> Get()
    {
        return new List<DemoForecast> { new DemoForecast { TemperatureC = 20, Summary = "Mild", Date = new System.DateOnly(2026, 1, 1) } };
    }
}
