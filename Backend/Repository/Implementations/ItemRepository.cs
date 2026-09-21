using Backend.Data;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Implementations
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            return await _context.Items
                .Include(i => i.Category)
                .ToListAsync();
        }

        public async Task<Item> GetByIdAsync(int id)
        {
            return await _context.Items
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.ItemId == id);
        }

        public async Task<Item> AddAsync(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            // Rechargement immédiat AVEC la catégorie incluse
            var createdItem = await _context.Items
                .Include(i => i.Category) // <-- La jointure est faite ici
                .FirstOrDefaultAsync(i => i.ItemId == item.ItemId);

            return createdItem;
            
        }

        public async Task<Item> UpdateAsync(Item item)
        {
            // 1. Marquer l'entité comme modifiée et sauvegarder
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // 2. Recharger l'entité depuis la base de données, en incluant la Category
            var updatedItem = await _context.Items
                .Include(i => i.Category) // <-- Ajoutez cette ligne
                .FirstOrDefaultAsync(i => i.ItemId == item.ItemId);

            return updatedItem; // Retourne l'objet complet
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
