namespace UpskillingApi.Models;

public class Objetivo
{
    public int Id { get; set; }
    public string CpfUsuario { get; set; } = null!;
    public string ObjetivoTexto { get; set; } = null!;
    public DateTime DtDefinicao { get; set; }
}