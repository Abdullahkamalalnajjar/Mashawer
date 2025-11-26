using Mashawer.Core.Features.Admin.Query.Model;

namespace Mashawer.Core.Features.Admin.Query.Handler
{
    public class AdminQueryHandler(IAdminService adminService) : ResponseHandler,
        IRequestHandler<GetOrdersByStatusAndAddressQuery, Response<List<OrderDto>>>
    {
        private readonly IAdminService _adminService = adminService;

        public async Task<Response<List<OrderDto>>> Handle(GetOrdersByStatusAndAddressQuery request, CancellationToken cancellationToken)
        {
            var result = await _adminService.GetAllOrdersDpendOnStatusAsync(request.OrderStatus, request.Address, request.DateTime);
            return Success(result);
        }
    }
}
