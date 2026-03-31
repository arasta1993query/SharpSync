using Microsoft.AspNetCore.Mvc;
using SharpSync.Core.Attributes;
using SharpSync.Example.WebApi.Models;
using System.Collections.Generic;

namespace SharpSync.Example.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[SharpSync]
public class DemoForecastController : ControllerBase
{
    [HttpGet("all")]
    public IEnumerable<DemoForecast> Get()
    {
        return new List<DemoForecast> { new DemoForecast { TemperatureC = 20, Summary = "Mild", Date = new System.DateOnly(2026, 1, 1) } };
    }

    [HttpGet("{id}")]
    public DemoForecast GetById(int id)
    {
        return new DemoForecast { TemperatureC = 25, Summary = "Hot", Date = new System.DateOnly(2026, 1, 2) };
    }

    [HttpGet("/global-stats")]
    public string GetStats()
    {
        return "Stats: 100% Sync";
    }
}
