using Mashawer.Core.Features.Representatives.Command.Models;
using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.Representatives.Command.Handler
{
    public class RepresentativeCommandHandler(IRepresentativeService representativeService, IUnitOfWork unitOfWork) : ResponseHandler,
        IRequestHandler<UpdateRepresentativesLocationCommand, Response<string>>,
        IRequestHandler<UpdateRepresentativeInfoCommand, Response<string>>,
        IRequestHandler<MarkIsClientLateCommand, Response<string>>,
        IRequestHandler<MarkOrderTaskIsCompleteCommand, Response<string>>,
        IRequestHandler<TaskDeliveredAtCommand, Response<string>>

    {
        private readonly IRepresentativeService _representativeService = representativeService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Response<string>> Handle(UpdateRepresentativesLocationCommand request, CancellationToken cancellationToken)
        {
            var result = await _representativeService.UpdateLocation(request.UserId, request.RepresentativeLatitude, request.RepresentativeLongitude);
            if (result == "NotFound")
                return NotFound<string>("NotFound");
            return Success<string>(result);

        }
        public async Task<Response<string>> Handle(UpdateRepresentativeInfoCommand request, CancellationToken cancellationToken)
        {
            var result = await _representativeService.UpdateInfo(request.RepresentativeId, request.VehicleUrl!, request.VehicleNumber!, request.VehicleType!, request.VehicleColor!);
            if (result == "NotFound")
                return (NotFound<string>("NotFound"));
            return (Success<string>(result));
        }

        public async Task<Response<string>> Handle(MarkIsClientLateCommand request, CancellationToken cancellationToken)
        {
            var res = await _representativeService.MarkIsClientLate(request.OrderId);
            if (res == "NotFound")
                return NotFound<string>("NotFound");
            return Updated<string>(res);
        }

        public async Task<Response<string>> Handle(MarkOrderTaskIsCompleteCommand request, CancellationToken cancellationToken)
        {

            var order = await _unitOfWork.Orders
                .GetTableAsTracking()
                .Include(x => x.Tasks)
                .Include(x => x.Driver)
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId);

            if (order == null)
                return NotFound<string>("Order not found.");

            var targetTask = order.Tasks.FirstOrDefault(t => t.Id == request.TaskId);

            if (targetTask == null)
                return NotFound<string>("Task not found in this order.");

            targetTask.Status = OrderStatus.Completed;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompeleteAsync();

            return Success<string>("Order task marked as completed.");

        }

        public async Task<Response<string>> Handle(TaskDeliveredAtCommand request, CancellationToken cancellationToken)
        {
            var orderTask = await _unitOfWork.OrderTasks
            .GetTableAsTracking()
            .FirstOrDefaultAsync(x => x.Id == request.TaskId);

            if (orderTask == null)
                return NotFound<string>("Order task not found.");

            orderTask.DeliveredAt = request.DeliveredAt;
            _unitOfWork.OrderTasks.Update(orderTask);
            await _unitOfWork.CompeleteAsync();
            return Success<string>("Order task Arrived");
        }
    }

}
