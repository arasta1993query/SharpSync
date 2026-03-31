using Microsoft.AspNetCore.Http;

namespace SharpSync.Example.WebApi.Models;

public class UploadReportDto
{
    public string? ReportName { get; set; }
    public IFormFile? ReportFile { get; set; }
}
