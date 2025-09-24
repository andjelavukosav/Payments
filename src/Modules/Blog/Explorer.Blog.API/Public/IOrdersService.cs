using Explorer.Blog.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Public
{
    public interface IOrdersService
    {
        void CreateOrder(Guid userId, List<ShoppingCartItemDTO> items, double totalPrice);
        Task<List<ShoppingCartItemDTO>> GetPurchasedToursAsync(Guid userId);

        // ➕ Dodaj metode za korpu
        Task AddToCartAsync(Guid userId, ShoppingCartItemDTO item);
        Task<IEnumerable<ShoppingCartItemDTO>> GetCartAsync(Guid userId);
        Task RemoveFromCartAsync(Guid userId, string tourId);
        Task ClearCartAsync(Guid userId);

    }
}
