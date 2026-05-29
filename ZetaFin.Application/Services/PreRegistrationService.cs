using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Application.Services;

public class PreRegistrationService : IPreRegistrationService
{
    private readonly IPreRegistrationRepository _repository;

    public PreRegistrationService(IPreRegistrationRepository repository)
    {
        _repository = repository;
    }

    public async Task<PreRegistrationResponseDto> CreateAsync(CreatePreRegistrationDto dto)
    {
        // Validar LGPD obrigatório
        if (!dto.AceitouLGPD)
            throw new InvalidOperationException("O consentimento LGPD é obrigatório.");

        // Sanitizar WhatsApp — apenas dígitos
        var whatsApp = new string(dto.WhatsApp.Where(char.IsDigit).ToArray());

        if (whatsApp.Length < 10 || whatsApp.Length > 11)
            throw new ArgumentException("WhatsApp inválido. Informe DDD + número (10 ou 11 dígitos).");

        // Verificar duplicidade
        var existing = await _repository.GetByWhatsAppAsync(whatsApp);
        if (existing is not null)
            throw new InvalidOperationException("Este WhatsApp já está na lista de espera.");

        // Validar faixa etária
        var faixasValidas = new[] { "18-24", "25-34", "35-44", "45-54", "55+" };
        if (!faixasValidas.Contains(dto.FaixaEtaria))
            throw new ArgumentException("Faixa etária inválida.");

        var entity = new PreRegistration(
            nome: dto.Nome.Trim(),
            whatsApp: whatsApp,
            faixaEtaria: dto.FaixaEtaria,
            aceitouLGPD: true,
            origemLead: dto.OrigemLead
        );

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<IEnumerable<PreRegistrationResponseDto>> GetAllAsync()
    {
        var all = await _repository.GetAllAsync();
        return all.Select(ToDto);
    }

    private static PreRegistrationResponseDto ToDto(PreRegistration e) => new(
        e.Id,
        e.Nome,
        e.WhatsApp,
        e.FaixaEtaria,
        e.AceitouLGPD,
        e.OrigemLead,
        e.DataCadastro,
        e.ConvertidoParaUsuario,
        e.DataConversao,
        e.UserId
    );
}
