
using Mashawer.Data.Entities.ClasssOfOrder;
using System.Linq.Expressions;

namespace Mashawer.Service.Implementations
{
    public class AgentService(IUnitOfWork unitOfWork) : IAgentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<OrderDto>> GetOrdersByAgentAddressAsync(string userId, DateTime? dateTime)
        {
            // Get agent address
            var agentAddress = await _unitOfWork.Users
                .GetTableNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => x.AgentAddress)
                .FirstOrDefaultAsync();

            if (agentAddress is null)
                throw new Exception("Agent address not found.");

            var targetDate = (dateTime ?? DateTime.Now).Date;

            // Base query
            var query = _unitOfWork.Orders
                .GetTableNoTracking()
                .Include(x => x.Driver)
                .Where(x => x.Driver.Address == agentAddress &&
                            x.CreatedAt.Date == targetDate);

            // Return result
            return await query
                .Select(OrderToDto)
                .ToListAsync();
        }

        #region Expression: Convert Order to OrderDto
        private static readonly Expression<Func<Order, OrderDto>> OrderToDto = o => new OrderDto
        {
            Id = o.Id,
            Type = o.Type.ToString(),

            // 💳 الدفع
            PaymentMethod = o.PaymentMethod.ToString(),
            PaymentStatus = o.PaymentStatus.ToString(),
            PaymobTransactionId = o.PaymobTransactionId,
            IsWalletUsed = o.IsWalletUsed,

            // 🚗 تفاصيل المركبة
            VehicleTypeOfDriver = o.Driver != null ? o.Driver.VehicleType : null,
            VehicleNumber = o.Driver != null ? o.Driver.VehicleNumber : null,

            // 👤 المستخدمين
            ClientId = o.ClientId,
            ClientName = o.Client.FullName,
            ClientPhoneNumber = o.Client.PhoneNumber,
            ClientPhotoUrl = o.Client.ProfilePictureUrl,
            DriverId = o.DriverId,
            DriverName = o.Driver != null ? o.Driver.FullName : null,
            DriverPhoneNumber = o.Driver != null ? o.Driver.PhoneNumber : null,
            DriverPhotoUrl = o.Driver != null ? o.Driver.ProfilePictureUrl : null,

            // ⚙️ الحالة
            Status = o.Status.ToString(),
            CancelReason = o.CancelReason,
            OtherCancelReasonDetails = o.OtherCancelReasonDetails,
            CreatedAt = o.CreatedAt,
            IsClientLate = o.IsClientLate,

            // 💰 المجموع العام
            TotalPrice = o.TotalPrice ?? 0,
            DeliveryPrice = o.TotalDeliveryPrice ?? 0,
            DeducationDelivery = o.DeducationDelivery ?? 0,
            DistanceKm = o.TotalDistanceKm,

            // 🧩 المهام داخل الطلب
            Tasks = o.Tasks.Select(t => new OrderTasksDto
            {
                Id = t.Id,
                OrderId = t.OrderId,
                Type = t.Type.ToString(),
                FromLatitude = t.FromLatitude,
                FromLongitude = t.FromLongitude,
                ToLatitude = t.ToLatitude,
                ToLongitude = t.ToLongitude,
                PickupLocation = t.PickupLocation,
                DeliveryLocation = t.DeliveryLocation,
                DeliveryPrice = t.DeliveryPrice,
                DistanceKm = (double)t.DistanceKm,
                DeliveryDescription = t.DeliveryDescription,
                IsClientPaidForItems = t.IsClientPaidForItems,
                IsDriverReimbursed = t.IsDriverReimbursed,
                ItemPhotoBefore = t.ItemPhotoBefore,
                ItemPhotoAfter = t.ItemPhotoAfter,
                Status = t.Status.ToString(),
                PurchaseItems = t.PurchaseItems.Select(p => new PurchaseItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    PriceTotal = p.PriceTotal
                }).ToList()
            }).ToList()
        };
        #endregion

    }
}
