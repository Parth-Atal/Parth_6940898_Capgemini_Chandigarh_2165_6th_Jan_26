using Asp.Versioning;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    public ReportsController(IReportService reportService) => _reportService = reportService;

    [HttpGet("sales")]
    public async Task<ActionResult<ApiResponse<SalesReportDto>>> GetSalesReport()
    {
        var report = await _reportService.GetSalesReportAsync();
        return Ok(ApiResponse<SalesReportDto>.Ok(report));
    }
}