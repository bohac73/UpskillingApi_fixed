using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UpskillingApi.Data;

namespace UpskillingApi.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/cadastros")]
public class CadastrosControllerV2 : ControllerBase
{
    private readonly AppDbContext _context;

    public CadastrosControllerV2(AppDbContext context)
    {
        _context = context;
    }

    // Novo endpoint exclusivo da versão 2
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var total = await _context.Cadastros.CountAsync();
        var cidades = await _context.Cadastros
            .Select(c => c.Cidade)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .CountAsync();

        var hoje = DateTime.Today;
        var idades = await _context.Cadastros
            .Select(c => hoje.Year - c.DtNasc.Year)
            .ToListAsync();

        var idadeMedia = idades.Count > 0 ? idades.Average() : 0;

        return Ok(new
        {
            TotalUsuarios = total,
            CidadesDistintas = cidades,
            IdadeMedia = idadeMedia
        });
    }
}