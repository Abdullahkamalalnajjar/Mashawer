using Azure;
using Mashawer.Data.Entities.ClasssOfOrder;
using System.Linq.Expressions;

namespace Mashawer.Service.Implementations
{
    public class OrderService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser>_userManager = userManager;

        public async Task<string> CreateOrderAsync(Order order, CancellationToken cancellationToken)
        {
            await _unitOfWork.Orders.AddAsync(order, cancellationToken);
            return "Created";
        }

        #region  Expression to convert Order to OrderDto
        private static readonly Expression<Func<Order, OrderDto>> OrderToDto = o => new OrderDto
        {
            Id = o.Id,
            FromLatitude = o.FromLatitude,
            FromLongitude = o.FromLongitude,
            ToLatitude = o.ToLatitude,
            ToLongitude = o.ToLongitude,
            ClientId = o.ClientId,
            ClientName = o.Client.FullName,
            CreatedAt = o.CreatedAt,
            EstimatedArrivalTime = o.EstimatedArrivalTime,
            PickupLocation = o.PickupLocation,
            Price = o.Price,
            DriverId = o.DriverId,
            DriverName = o.Driver.FullName,
            VehicleType = o.VehicleType,
            CancelReason = o.CancelReason.ToString(),
            Status = o.Status.ToString(),
            OtherCancelReasonDetails = o.OtherCancelReasonDetails,
            DeliveryLocation = o.DeliveryLocation,
            ItemPhotoAfter = o.ItemPhotoAfter,
            ItemPhotoBefore = o.ItemPhotoBefore,
            ItemDescription=o.ItemDescription
            
        };

        #endregion 
        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            return await _unitOfWork.Orders.GetTableNoTracking()
                 .Include(o => o.Client)
                 .Include(o => o.Driver)
                .Select(OrderToDto)
                .ToListAsync();

        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByClientIdAsync(string clientId)
        {
            var client = await _userManager.FindByIdAsync(clientId);
            if (client == null)
                return Enumerable.Empty<OrderDto>();
            return await _unitOfWork.Orders.GetTableNoTracking()
               .Where(x => x.ClientId == clientId)
               .Select(OrderToDto)
               .ToListAsync();

        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByDriverIdAsync(string driverId)
        {
            var driver = await _userManager.FindByIdAsync(driverId);
            if (driver == null)
                return Enumerable.Empty<OrderDto>();
            return await _unitOfWork.Orders.GetTableNoTracking()
              .Where(x => x.DriverId == driverId)
              .Select(OrderToDto)
              .ToListAsync();
        }

        public async Task<string> AddOrderPhotosAsync(int orderId, string? photoBefore, string? photoAfter, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                return "Order not found";

            if (!string.IsNullOrEmpty(photoBefore))
                order.ItemPhotoBefore = photoBefore;

            if (!string.IsNullOrEmpty(photoAfter))
                order.ItemPhotoAfter = photoAfter;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompeleteAsync();

            return "Photos updated successfully";
        }
    }
    }

