using ZetaFin.Domain.Entities;

namespace ZetaFin.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken?> GetByIdAsync(Guid id);
    Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(Guid userId);
    Task UpdateAsync(RefreshToken refreshToken);
    Task DeleteAsync(Guid id);
    Task<int> RevokeAllByUserIdAsync(Guid userId, string reason = "Logout");
    Task<int> RevokeExpiredAsync();
}
