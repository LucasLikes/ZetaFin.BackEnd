using ZetaFin.Application.DTOs;

namespace ZetaFin.Application.Interfaces;

public interface IPreRegistrationService
{
    Task<PreRegistrationResponseDto> CreateAsync(CreatePreRegistrationDto dto);
    Task<IEnumerable<PreRegistrationResponseDto>> GetAllAsync();
}
