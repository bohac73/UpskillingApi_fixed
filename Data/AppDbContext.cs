using Microsoft.EntityFrameworkCore;
using UpskillingApi.Models;

namespace UpskillingApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cadastro> Cadastros => Set<Cadastro>();
    public DbSet<MeuProgresso> MeusProgresso => Set<MeuProgresso>();
    public DbSet<Objetivo> Objetivos => Set<Objetivo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cadastro>(entity =>
        {
            entity.ToTable("TB_CADASTRO");
            entity.HasKey(e => e.Cpf);
            entity.Property(e => e.Cpf).HasColumnName("CPF").HasMaxLength(11).IsRequired();
            entity.Property(e => e.Nome).HasColumnName("NOME").HasMaxLength(80).IsRequired();
            entity.Property(e => e.DtNasc).HasColumnName("DT_NASC").IsRequired();
            entity.Property(e => e.Cep).HasColumnName("CEP").HasMaxLength(8).IsRequired();
            entity.Property(e => e.Email).HasColumnName("EMAIL").HasMaxLength(120).IsRequired();
            entity.Property(e => e.Senha).HasColumnName("SENHA").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Cidade).HasColumnName("CIDADE").HasMaxLength(60);
            entity.Property(e => e.Estado).HasColumnName("ESTADO").HasMaxLength(2);
            entity.Property(e => e.ProfissaoId).HasColumnName("PROFISSAO_ID");
        });

        modelBuilder.Entity<MeuProgresso>(entity =>
        {
            entity.ToTable("TB_MEUPROGRESSO");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CpfUsuario).HasColumnName("CPF_USUARIO").HasMaxLength(11).IsRequired();
            entity.Property(e => e.CursoId).HasColumnName("CURSO_ID").IsRequired();
            entity.Property(e => e.Status).HasColumnName("STATUS").HasMaxLength(20).HasDefaultValue("nao iniciado");
            entity.Property(e => e.DtConclusao).HasColumnName("DT_CONCLUSAO");
        });

        modelBuilder.Entity<Objetivo>(entity =>
        {
            entity.ToTable("TB_OBJETIVOS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CpfUsuario).HasColumnName("CPF_USUARIO").HasMaxLength(11).IsRequired();
            entity.Property(e => e.ObjetivoTexto).HasColumnName("OBJETIVO").HasMaxLength(200).IsRequired();
            entity.Property(e => e.DtDefinicao).HasColumnName("DT_DEFINICAO");
        });
    }
}