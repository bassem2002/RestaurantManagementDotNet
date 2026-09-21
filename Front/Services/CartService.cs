using Backend.Models;
using Front.Models;

namespace Front.Services
{
    public class CartService
    {
        private readonly List<Item> items = new();

        public IReadOnlyList<Item> Items => items;

        public void Add(Item item)
        {
            items.Add(item);
        }

        public void Remove(Item item)
        {
            items.Remove(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public decimal Total =>
            items.Sum(i => i.Price);
    }
}
