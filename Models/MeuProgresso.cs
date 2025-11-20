namespace UpskillingApi.Models;

public class MeuProgresso
{
    public int Id { get; set; }
    public string CpfUsuario { get; set; } = null!;
    public int CursoId { get; set; }
    public string Status { get; set; } = "nao iniciado";
    public DateTime? DtConclusao { get; set; }
}