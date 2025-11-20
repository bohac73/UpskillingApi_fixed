namespace UpskillingApi.Models;

public class Cadastro
{
    public string Cpf { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public DateTime DtNasc { get; set; }
    public string Cep { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public int? ProfissaoId { get; set; }
}