namespace ZetaFin.Application.DTOs;

public record CreatePreRegistrationDto(
    string Nome,
    string WhatsApp,
    string FaixaEtaria,
    bool AceitouLGPD,
    string OrigemLead = "landing_cta"
);

public record PreRegistrationResponseDto(
    Guid Id,
    string Nome,
    string WhatsApp,
    string FaixaEtaria,
    bool AceitouLGPD,
    string OrigemLead,
    DateTime DataCadastro,
    bool ConvertidoParaUsuario,
    DateTime? DataConversao,
    Guid? UserId
);
