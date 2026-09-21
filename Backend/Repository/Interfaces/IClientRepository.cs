// Backend.Repository.Interfaces/IClientRepository.cs
using Backend.Models;

namespace Backend.Repository.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<ApplicationUser>> GetClientsAsync();
        Task<Cart?> GetClientCartAsync(string userId);

        // Ajout de ces méthodes
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserAsync(ApplicationUser user);
    }
}