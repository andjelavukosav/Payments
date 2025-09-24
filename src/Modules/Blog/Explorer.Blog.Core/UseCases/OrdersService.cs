using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Domain;

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
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            if (items == null || items.Count == 0)
                throw new ArgumentException("Items list cannot be null or empty", nameof(items));

            var shoppingCart = new ShoppingCart
            {
                TouristId = userId.ToString(),
                ShoppingCartItems = items
                    .Select(dto => new ShoppingCartItem(dto.TourId, dto.Name, dto.Price))
                    .ToList(),
                TotalPrice = totalPrice
            };

            var purchaseTokens = items
                .Select(dto => new TourPurchaseToken(userId.ToString(), dto.TourId))
                .ToList();

            _ordersRepository.CreateShoppingCart(shoppingCart);

            foreach (var dto in items)
            {
                dto.ShoppingCartId = shoppingCart.Id;
            }

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

        // ➕ Implementacije za korpu
        public async Task AddToCartAsync(Guid userId, ShoppingCartItemDTO item)
        {
            await _ordersRepository.AddToCartAsync(userId, new ShoppingCartItem(item.TourId, item.Name, item.Price));
        }

        public async Task<IEnumerable<ShoppingCartItemDTO>> GetCartAsync(Guid userId)
        {
            var items = await _ordersRepository.GetCartByUserIdAsync(userId);
            return items.Select(i => new ShoppingCartItemDTO(i.TourId, i.Name, i.Price));
        }

        public async Task RemoveFromCartAsync(Guid userId, string tourId)
        {
            await _ordersRepository.RemoveFromCartAsync(userId, tourId);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            await _ordersRepository.ClearCartAsync(userId);
        }
    }
}
