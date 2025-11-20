using Microsoft.Extensions.Caching.Memory;
using UpskillingApi.Models;

namespace UpskillingApi.Services;

public interface IDataCacheService
{
    void SetCadastros(IEnumerable<Cadastro> cadastros);
    IEnumerable<Cadastro>? GetCadastros();

    void SetMeusProgresso(IEnumerable<MeuProgresso> itens);
    IEnumerable<MeuProgresso>? GetMeusProgresso();

    void SetObjetivos(IEnumerable<Objetivo> itens);
    IEnumerable<Objetivo>? GetObjetivos();
}

public class DataCacheService : IDataCacheService
{
    private readonly IMemoryCache _cache;

    public DataCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void SetCadastros(IEnumerable<Cadastro> cadastros) =>
        _cache.Set("cadastros_last", cadastros.ToList(), TimeSpan.FromMinutes(10));

    public IEnumerable<Cadastro>? GetCadastros() =>
        _cache.TryGetValue("cadastros_last", out IEnumerable<Cadastro>? value) ? value : null;

    public void SetMeusProgresso(IEnumerable<MeuProgresso> itens) =>
        _cache.Set("meusprogresso_last", itens.ToList(), TimeSpan.FromMinutes(10));

    public IEnumerable<MeuProgresso>? GetMeusProgresso() =>
        _cache.TryGetValue("meusprogresso_last", out IEnumerable<MeuProgresso>? value) ? value : null;

    public void SetObjetivos(IEnumerable<Objetivo> itens) =>
        _cache.Set("objetivos_last", itens.ToList(), TimeSpan.FromMinutes(10));

    public IEnumerable<Objetivo>? GetObjetivos() =>
       _cache.TryGetValue("objetivos_last", out IEnumerable<Objetivo>? value) ? value : null;
}