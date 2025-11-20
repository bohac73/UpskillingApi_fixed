using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UpskillingApi.Data;
using UpskillingApi.Models;
using UpskillingApi.Services;

namespace UpskillingApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cadastros")]
[Authorize]
public class CadastrosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDataCacheService _cache;
    private readonly ILogger<CadastrosController> _logger;

    public CadastrosController(AppDbContext context, IDataCacheService cache, ILogger<CadastrosController> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<Cadastro>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _context.Cadastros.AsNoTracking();
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderBy(c => c.Nome)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        _cache.SetCadastros(items);

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

        var result = new PagedResult<Cadastro>(items, pageNumber, pageSize, totalCount, totalPages, links);
        return Ok(result);
    }

    [HttpGet("{cpf}")]
    public async Task<ActionResult<Cadastro>> GetByCpf(string cpf)
    {
        var cadastro = await _context.Cadastros.FindAsync(cpf);
        if (cadastro == null)
        {
            return NotFound();
        }

        var links = new List<LinkDto>
        {
            new LinkDto("self", Url.ActionLink(nameof(GetByCpf), values: new { cpf, version = HttpContext.GetRequestedApiVersion()?.ToString() })!, "GET"),
            new LinkDto("update", Url.ActionLink(nameof(Update), values: new { cpf, version = HttpContext.GetRequestedApiVersion()?.ToString() })!, "PUT"),
            new LinkDto("delete", Url.ActionLink(nameof(Delete), values: new { cpf, version = HttpContext.GetRequestedApiVersion()?.ToString() })!, "DELETE")
        };

        return Ok(new { cadastro, links });
    }

    [HttpPost]
    public async Task<ActionResult<Cadastro>> Create([FromBody] Cadastro cadastro)
    {
        _context.Cadastros.Add(cadastro);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro ao criar cadastro");
            return BadRequest(new { message = "Erro ao criar cadastro. Verifique se CPF/e-mail são únicos e dados válidos." });
        }

        return CreatedAtAction(nameof(GetByCpf), new { cpf = cadastro.Cpf, version = HttpContext.GetRequestedApiVersion()?.ToString() }, cadastro);
    }

    [HttpPut("{cpf}")]
    public async Task<IActionResult> Update(string cpf, [FromBody] Cadastro cadastro)
    {
        if (!string.Equals(cpf, cadastro.Cpf, StringComparison.Ordinal))
        {
            return BadRequest(new { message = "CPF da URL deve ser igual ao do corpo." });
        }

        var existing = await _context.Cadastros.FindAsync(cpf);
        if (existing == null)
        {
            return NotFound();
        }

        existing.Nome = cadastro.Nome;
        existing.DtNasc = cadastro.DtNasc;
        existing.Cep = cadastro.Cep;
        existing.Email = cadastro.Email;
        existing.Senha = cadastro.Senha;
        existing.Cidade = cadastro.Cidade;
        existing.Estado = cadastro.Estado;
        existing.ProfissaoId = cadastro.ProfissaoId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{cpf}")]
    public async Task<IActionResult> Delete(string cpf)
    {
        var existing = await _context.Cadastros.FindAsync(cpf);
        if (existing == null)
        {
            return NotFound();
        }

        _context.Cadastros.Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}