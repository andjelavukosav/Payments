using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IOrdersRepository
    {
        public ShoppingCart CreateShoppingCart(ShoppingCart shoppingCart);
        public TourPurchaseToken CreateTourPurchaseToken(TourPurchaseToken tourPurchaseToken);
        public ShoppingCartItem CreateShoppingCartItem(ShoppingCartItem shoppingCartItem);
        Task<ShoppingCart?> GetByUserIdAsync(Guid userId);
        Task AddToCartAsync(Guid userId, ShoppingCartItem item);
        Task<IEnumerable<ShoppingCartItem>> GetCartByUserIdAsync(Guid userId);
        Task RemoveFromCartAsync(Guid userId, string tourId);
        Task ClearCartAsync(Guid userId);
    }
}
