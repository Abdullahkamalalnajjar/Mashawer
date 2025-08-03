using Mashawer.Core.Features.Orders.Commands.Models;
using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Core.Features.Orders.Commands.Handler
{
    public class OrderCommandHandler(IOrderService orderService, IMapper mapper, IUnitOfWork unitOfWork) : ResponseHandler,
        IRequestHandler<CreateOrderCommand, Response<string>>
    {
        private readonly IOrderService _orderService = orderService;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Response<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Order>(request);
            var result = await _orderService.CreateOrderAsync(order, cancellationToken);
            if (result == "Created")
            {
                await _unitOfWork.CompeleteAsync();
                return Created("Order has been created");
            }
            return UnprocessableEntity<string>("Exist error when make order");
        }
    }
}
