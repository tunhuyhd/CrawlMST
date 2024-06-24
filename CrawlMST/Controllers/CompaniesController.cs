using CrawlMST.Models;
using CrawlMST.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrawlMST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly CrawlService _crawlService;

    public CompaniesController(ApplicationDbContext context, CrawlService crawlService)
    {
        _context = context;
        _crawlService = crawlService;
    }

    [HttpPost("crawl")]
    public async Task<IActionResult> CrawlData([FromBody] CrawlRequest request)
    {
        await _crawlService.CrawlAndSaveDataAsync(request.Tinh, request.PageNumber);
        return Ok();
    }
}
