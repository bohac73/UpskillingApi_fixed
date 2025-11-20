using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UpskillingApi.Data;
using UpskillingApi.Models;
using UpskillingApi.Services;

namespace UpskillingApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/objetivos")]
[Authorize]
public class ObjetivosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDataCacheService _cache;

    public ObjetivosController(AppDbContext context, IDataCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<Objetivo>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _context.Objetivos.AsNoTracking();
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderBy(o => o.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        _cache.SetObjetivos(items);

        var links = new List<LinkDto>
        {
            new LinkDto("self", Url.ActionLink(nameof(GetAll), values: new { pageNumber, pageSize, version = HttpContext.GetRequestedApiVersion()?.ToString() })!, "GET"),
            new LinkDto("create", Url.ActionLink(nameof(Create), values: new { version = HttpContext.GetRequestedApiVersion()?.ToString() })!, "POST")
        };

        if (pageNumber < totalPages)
        {
            links.Add(new LinkDto("next", Url.ActionLink(nameof(GetAll), values: new { pageNumber = pageNumber + 1, pageSize, version = HttpContext.GetRequestedApiVersion()?.ToString() })!, "GET"));
        }

        if (pageNumber > 1)
        {
            links.Add(new LinkDto("prev", Url.ActionLink(nameof(GetAll), values: new { pageNumber = pageNumber - 1, pageSize, version = HttpContext.GetRequestedApiVersion()?.ToString() })!, "GET"));
        }

        var result = new PagedResult<Objetivo>(items, pageNumber, pageSize, totalCount, totalPages, links);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Objetivo>> GetById(int id)
    {
        var item = await _context.Objetivos.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        var links = new List<LinkDto>
        {
            new LinkDto("self", Url.ActionLink(nameof(GetById), values: new { id, version = HttpContext.GetRequestedApiVersion()?.ToString() })!, "GET"),
            new LinkDto("update", Url.ActionLink(nameof(Update), values: new { id, version = HttpContext.GetRequestedApiVersion()?.ToString() })!, "PUT"),
            new LinkDto("delete", Url.ActionLink(nameof(Delete), values: new { id, version = HttpContext.GetRequestedApiVersion()?.ToString() })!, "DELETE")
        };

        return Ok(new { item, links });
    }

    [HttpPost]
    public async Task<ActionResult<Objetivo>> Create([FromBody] Objetivo item)
    {
        _context.Objetivos.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = item.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Objetivo item)
    {
        if (id != item.Id)
        {
            return BadRequest(new { message = "ID da URL deve ser igual ao do corpo." });
        }

        var existing = await _context.Objetivos.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.CpfUsuario = item.CpfUsuario;
        existing.ObjetivoTexto = item.ObjetivoTexto;
        existing.DtDefinicao = item.DtDefinicao;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _context.Objetivos.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        _context.Objetivos.Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}