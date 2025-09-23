using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}
