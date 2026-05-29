namespace ZetaFin.Domain.Entities;

public class PreRegistration
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string WhatsApp { get; private set; } = string.Empty;
    public string FaixaEtaria { get; private set; } = string.Empty;
    public bool AceitouLGPD { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public string OrigemLead { get; private set; } = string.Empty;

    // Conversão futura
    public bool ConvertidoParaUsuario { get; private set; }
    public DateTime? DataConversao { get; private set; }
    public Guid? UserId { get; private set; }

    // EF Core
    protected PreRegistration() { }

    public PreRegistration(
        string nome,
        string whatsApp,
        string faixaEtaria,
        bool aceitouLGPD,
        string origemLead)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        WhatsApp = whatsApp;
        FaixaEtaria = faixaEtaria;
        AceitouLGPD = aceitouLGPD;
        OrigemLead = origemLead;
        DataCadastro = DateTime.UtcNow;
        ConvertidoParaUsuario = false;
    }

    public void ConverterParaUsuario(Guid userId)
    {
        UserId = userId;
        ConvertidoParaUsuario = true;
        DataConversao = DateTime.UtcNow;
    }
}
