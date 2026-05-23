using ZetaFin.Domain.Entities;

namespace ZetaFin.Domain.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByEmailAsync(string email);
    Task<bool> CheckUserPasswordAsync(string email, string password);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}
