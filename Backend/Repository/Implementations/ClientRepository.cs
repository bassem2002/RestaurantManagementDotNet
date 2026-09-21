// Backend.Repository.Implementations/ClientRepository.cs

using Backend.Data;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class ClientRepository : IClientRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public ClientRepository(
        UserManager<ApplicationUser> userManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IEnumerable<ApplicationUser>> GetClientsAsync()
    {
        return await _userManager.GetUsersInRoleAsync("Client");
    }

    public async Task<Cart?> GetClientCartAsync(string userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Item)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    // Nouvelle méthode
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    // Nouvelle méthode
    public async Task<bool> UpdateUserAsync(ApplicationUser user)
    {
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}