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
        public void CreateOrder(Guid userId, List<ShoppingCartItemDTO> items, double totalPrice);
        //List<ShoppingCartItemDTO> GetPurchasedTours(Guid userId); // <-- metoda koja ti treba
        Task<List<ShoppingCartItemDTO>> GetPurchasedToursAsync(Guid userId);

    }
}
