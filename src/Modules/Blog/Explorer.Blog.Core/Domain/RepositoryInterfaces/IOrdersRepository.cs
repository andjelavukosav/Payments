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

    }
}
