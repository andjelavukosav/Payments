using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly BlogContext _dbContext;

        public OrdersRepository(BlogContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ShoppingCartItem CreateShoppingCartItem(ShoppingCartItem shoppingCartItem)
        {
            _dbContext.ShoppingCartItems.Add(shoppingCartItem);
            _dbContext.SaveChanges();
            return shoppingCartItem;
        }

        public ShoppingCart CreateShoppingCart(ShoppingCart shoppingCart)
        {
            _dbContext.ShoppingCarts.Add(shoppingCart);
            _dbContext.SaveChanges();
            return shoppingCart;
        }

        public TourPurchaseToken CreateTourPurchaseToken(TourPurchaseToken tourPurchaseToken)
        {
            _dbContext.TourPurchaseTokens.Add(tourPurchaseToken);
            _dbContext.SaveChanges();
            return tourPurchaseToken;
        }

        public async Task<ShoppingCart?> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                .FirstOrDefaultAsync(c => c.TouristId == userId.ToString());
        }

        // ➕ implementacije novih metoda
        public async Task AddToCartAsync(Guid userId, ShoppingCartItem item)
        {
            var cart = await _dbContext.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                .FirstOrDefaultAsync(c => c.TouristId == userId.ToString());

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    TouristId = userId.ToString(),
                    ShoppingCartItems = new List<ShoppingCartItem>(),
                    TotalPrice = 0
                };
                _dbContext.ShoppingCarts.Add(cart);
            }

            cart.ShoppingCartItems.Add(item);
            cart.TotalPrice = cart.ShoppingCartItems.Sum(i => i.Price);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetCartByUserIdAsync(Guid userId)
        {
            var cart = await _dbContext.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                .FirstOrDefaultAsync(c => c.TouristId == userId.ToString());

            if (cart == null || cart.ShoppingCartItems == null)
                return new List<ShoppingCartItem>();

            // Dohvati sve token-e za korisnika
            var tokens = await _dbContext.TourPurchaseTokens
                .Where(t => t.TouristId == userId.ToString() && t.Status != TourPurchaseToken.TokenStatus.Expired)
                .ToListAsync();

            // Filtriraj stavke korpe koje već imaju token (tj. kupljene)
            var filteredItems = cart.ShoppingCartItems
                .Where(item => !tokens.Any(t => t.TourId == item.TourId))
                .ToList();

            return filteredItems;
        }


        public async Task RemoveFromCartAsync(Guid userId, string tourId)
        {
            var cart = await _dbContext.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                .FirstOrDefaultAsync(c => c.TouristId == userId.ToString());

            if (cart == null) return;

            var item = cart.ShoppingCartItems.FirstOrDefault(i => i.TourId == tourId);
            if (item != null)
            {
                cart.ShoppingCartItems.Remove(item);
                _dbContext.ShoppingCartItems.Remove(item);
                cart.TotalPrice = cart.ShoppingCartItems.Sum(i => i.Price);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _dbContext.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                .FirstOrDefaultAsync(c => c.TouristId == userId.ToString());

            if (cart == null) return;

            _dbContext.ShoppingCartItems.RemoveRange(cart.ShoppingCartItems);
            cart.ShoppingCartItems.Clear();
            cart.TotalPrice = 0;

            await _dbContext.SaveChangesAsync();
        }
    }
}
