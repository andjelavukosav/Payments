using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.Core.UseCases
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrdersRepository _ordersRepository;

        public OrdersService(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public void CreateOrder(Guid userId, List<ShoppingCartItemDTO> items, double totalPrice)
        {
            // --- Validacije ---
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            if (items == null || items.Count == 0)
                throw new ArgumentException("Items list cannot be null or empty", nameof(items));

            // --- Kreiranje shopping cart-a ---
            ShoppingCart shoppingCart = new ShoppingCart
            {
                TouristId = userId.ToString(),
                ShoppingCartItems = items
                    .Select(dto => new ShoppingCartItem(dto.TourId, dto.Name, dto.Price))
                    .ToList(),
                TotalPrice = totalPrice
            };

            // --- Kreiranje tokena (ako je potrebno) ---
            List<TourPurchaseToken> purchaseTokens = items
                .Select(dto => new TourPurchaseToken(userId.ToString(), dto.TourId))
                .ToList();

            // --- Sačuvaj sve u bazi u jednom pozivu ---
            _ordersRepository.CreateShoppingCart(shoppingCart);

            // --- Poveži DTO sa kreiranim ShoppingCart.Id ---
            foreach (var dto in items)
            {
                dto.ShoppingCartId = shoppingCart.Id;
            }

            // --- Opcionalno: batch insert tokena ---
            // Ako repozitorijum podržava batch insert, možeš ovo uraditi:
            foreach (var token in purchaseTokens)
            {
                _ordersRepository.CreateTourPurchaseToken(token);
            }
        }

        public async Task<List<ShoppingCartItemDTO>> GetPurchasedToursAsync(Guid userId)
        {
            var cart = await _ordersRepository.GetByUserIdAsync(userId);

            if (cart == null || cart.ShoppingCartItems == null)
                return new List<ShoppingCartItemDTO>();

            return cart.ShoppingCartItems
                .Select(i => new ShoppingCartItemDTO(i.TourId, i.Name, i.Price))
                .ToList();
        }

    }
}
