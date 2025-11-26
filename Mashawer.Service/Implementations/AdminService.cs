

using Mashawer.Data.Entities.ClasssOfOrder;
using System.Linq.Expressions;

namespace Mashawer.Service.Implementations
{
    public class AdminService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<string> AccpetOrRejectRequestAgentAsync(int requestId, UserType userType, UpgradeRequestStatus upgradeRequestStatus)
        {
            var request = await _unitOfWork.UserUpgradeRequests.GetTableAsTracking()
                 .FirstOrDefaultAsync(x => x.Id == requestId);
            if (request is null)
            {
                return "NotFound";
            }
            var user = await _unitOfWork.Users.GetTableAsTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId);
            user.UserType = userType; // Assuming the request is for an agent upgrade
            if (userType == UserType.Agent)
                user.AgentAddress = request.Address;
            if (userType == UserType.Representative)
                user.RepresentativeAddress = request.Address;
            await _userManager.UpdateAsync(user);
            request.Status = upgradeRequestStatus;
            _unitOfWork.UserUpgradeRequests.Update(request);
            return ("Updated");
        }

        public async Task<List<OrderDto>> GetAllOrdersDpendOnStatusAsync(OrderStatus orderStatus, string? address, DateTime? dateTime)
        {
            var ordersQuery = _unitOfWork.Orders.GetTableNoTracking().Include(x => x.Driver)
                .Where(o => o.Status == orderStatus);
            var targetDate = (dateTime ?? DateTime.Now).Date;

            // If an address is provided, filter orders by the driver's address
            if (!string.IsNullOrEmpty(address))
            {
                ordersQuery = ordersQuery.Where(o => o.Driver != null && o.Driver.Address == address);
            }
            var result = await ordersQuery.Select(OrderToDto).Where(c => c.CreatedAt.Date == targetDate).ToListAsync();
            return result;
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
