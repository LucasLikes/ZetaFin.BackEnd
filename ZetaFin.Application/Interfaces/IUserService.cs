using ZetaFin.Application.DTOs;

namespace ZetaFin.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserDto>> GetAllAsync();
}
