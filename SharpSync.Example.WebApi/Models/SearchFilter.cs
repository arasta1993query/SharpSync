namespace SharpSync.Example.WebApi.Models;

public class SearchFilter
{
    public string? Term { get; set; }
    public int? MinTemperature { get; set; }
    public bool IncludeHistorical { get; set; }
}
