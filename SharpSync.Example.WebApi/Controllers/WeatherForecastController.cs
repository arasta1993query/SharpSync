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

    [HttpGet("search")]
    public IEnumerable<DemoForecast> Search([FromQuery] string? q, int? minTemp)
    {
        return new List<DemoForecast>();
    }

    [HttpGet("advanced-search")]
    public IEnumerable<DemoForecast> AdvancedSearch([FromQuery] SearchFilter filter, int page)
    {
        return new List<DemoForecast>();
    }

    [HttpPost]
    public DemoForecast Create([FromBody] CreateForecastDto dto)
    {
        return new DemoForecast { Summary = dto.Summary, TemperatureC = dto.TemperatureC };
    }

    [HttpPut("{id}")]
    public DemoForecast Update(int id, [FromBody] CreateForecastDto dto)
    {
        return new DemoForecast { Summary = dto.Summary, TemperatureC = dto.TemperatureC };
    }
}
