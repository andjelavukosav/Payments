using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Grpc.Core;

namespace Explorer.API.Controllers
{
    public class OrdersServiceController : OrdersService.OrdersServiceBase
    {
        private readonly ILogger<OrdersServiceController> _logger;

        private readonly IOrdersService _ordersService;

        public OrdersServiceController(ILogger<OrdersServiceController> logger, IOrdersService ordersService)
        {
            _logger = logger;
            _ordersService = ordersService;
        }

        public override Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Primljena nova narudžbina sa ID: {UserId}", request.UserId);

            // 1. Parsiraj UserId iz stringa u Guid
            Guid userId = Guid.Parse(request.UserId);

            // 2. Mapiraj RepeatedField<OrderTour> u List<ShoppingCartItemDTO>
            var items = request.Tours
                .Select(t => new ShoppingCartItemDTO(t.Id, t.Name, t.Price))
                .ToList();


            // 3. Pozovi servis
            _ordersService.CreateOrder(userId, items, request.TotalPrice);

            var response = new CreateOrderResponse
            {
                Success = true
            };

            return Task.FromResult(response);
        }

        // gRPC metoda za preuzimanje kupljenih tura korisnika
        public override async Task<GetPurchasedToursResponse> GetPurchasedTours(GetPurchasedToursRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId is required"));
            }

            // Parsiraj UserId u Guid
            Guid userId;
            if (!Guid.TryParse(request.UserId, out userId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid UserId format"));
            }

            // Pozovi servis koji koristi repozitorijum
            var purchasedTours = await _ordersService.GetPurchasedToursAsync(userId);

            // Vraćanje samo ID-jeva tura kako proto definicija zahteva
            var response = new GetPurchasedToursResponse();
            response.TourIds.AddRange(purchasedTours.Select(t => t.TourId));

            return response;
        }

        public override async Task<CartResponse> AddToCart(AddToCartRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.UserId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId is required"));

            var userId = Guid.Parse(request.UserId);

            var item = new ShoppingCartItemDTO(request.Tour.Id, request.Tour.Name, request.Tour.Price);

            await _ordersService.AddToCartAsync(userId, item);

            var items = await _ordersService.GetCartAsync(userId);
            return MapToCartResponse(items);
        }

        public override async Task<CartResponse> GetCart(GetCartRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.UserId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId is required"));

            var userId = Guid.Parse(request.UserId);

            var items = await _ordersService.GetCartAsync(userId);
            return MapToCartResponse(items);
        }

        public override async Task<CartResponse> RemoveFromCart(RemoveFromCartRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.TourId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId and TourId are required"));

            var userId = Guid.Parse(request.UserId);

            await _ordersService.RemoveFromCartAsync(userId, request.TourId);

            var items = await _ordersService.GetCartAsync(userId);
            return MapToCartResponse(items);
        }

        public override async Task<CartResponse> ClearCart(ClearCartRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.UserId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId is required"));

            var userId = Guid.Parse(request.UserId);

            await _ordersService.ClearCartAsync(userId);

            return new CartResponse(); // prazna korpa
        }

        // Helper za mapiranje DTO -> gRPC response
        private CartResponse MapToCartResponse(IEnumerable<ShoppingCartItemDTO> items)
        {
            var response = new CartResponse();
            response.Items.AddRange(items.Select(i => new CartTour
            {
                Id = i.TourId,
                Name = i.Name,
                Price = i.Price
            }));

            response.TotalPrice = items.Sum(i => i.Price);

            return response;
        }

    }
}
