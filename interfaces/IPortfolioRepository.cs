using api.Models;

namespace api.interfaces
{
    public interface IPortfolioRepository
    {
        Task<List<Stock>> GetUserPortfolioAsync(AppUser user);
        Task<Portfolio> CreateAsync(Portfolio portfolio);
    }
}