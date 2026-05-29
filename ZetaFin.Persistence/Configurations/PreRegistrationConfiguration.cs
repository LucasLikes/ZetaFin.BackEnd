using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Persistence.Configurations;

public class PreRegistrationConfiguration : IEntityTypeConfiguration<PreRegistration>
{
    public void Configure(EntityTypeBuilder<PreRegistration> builder)
    {
        builder.ToTable("pre_registrations");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id");

        builder.Property(p => p.Nome)
            .HasColumnName("nome")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(p => p.WhatsApp)
            .HasColumnName("whatsapp")
            .HasMaxLength(11)
            .IsRequired();

        // Índice único — evita duplicidade
        builder.HasIndex(p => p.WhatsApp)
            .IsUnique()
            .HasDatabaseName("idx_pre_reg_whatsapp");

        builder.Property(p => p.FaixaEtaria)
            .HasColumnName("faixa_etaria")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.AceitouLGPD)
            .HasColumnName("aceitou_lgpd")
            .IsRequired();

        builder.Property(p => p.DataCadastro)
            .HasColumnName("data_cadastro")
            .IsRequired();

        builder.Property(p => p.OrigemLead)
            .HasColumnName("origem_lead")
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(p => p.ConvertidoParaUsuario)
            .HasColumnName("convertido_para_usuario")
            .HasDefaultValue(false);

        builder.Property(p => p.DataConversao)
            .HasColumnName("data_conversao")
            .IsRequired(false);

        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired(false);
    }
}
