using Mashawer.Api.Base;
using Mashawer.Core.Features.Orders.Commands.Models;
using Mashawer.Core.Features.Orders.Queries.Models;
using Mashawer.Data.AppMetaData;
using Mashawer.Service.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class OrderController(IOrderService _orderService) : AppBaseController
    {
        private readonly IOrderService orderService = _orderService;

        [HttpPost(Router.OrderRouting.Create)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        [HttpPut(Router.OrderRouting.UpdateStatus)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(new { message = result });
        }

        [HttpGet(Router.OrderRouting.GetById)]
        public async Task<IActionResult> GetOrderById([FromRoute] int id)
        {
            var result = await Mediator.Send(new GetOrderByIdQuery(id));
            return NewResult(result);
        }

        [HttpGet(Router.OrderRouting.List)]
        public async Task<IActionResult> GetOrders()
        {
            var result = await Mediator.Send(new GetOrdersQuery());
            return NewResult(result);
        }

        [HttpGet(Router.OrderRouting.GetOrdersByClientId)]
        public async Task<IActionResult> GetOrdersByClientId([FromQuery] string clientId)
        {
            var result = await Mediator.Send(new GetOrdersByClientIdQuery { ClientId = clientId });
            return NewResult(result);
        }


        [HttpGet(Router.OrderRouting.GetOrdersByDriverId)]
        public async Task<IActionResult> GetOrdersByDriverId([FromQuery] string DriverId)
        {
            var result = await Mediator.Send(new GetOrdersByDriverIdQuery { DriverId = DriverId });
            return NewResult(result);
        }
        [HttpPost(Router.OrderRouting.UploadOrderPhotos)]
        public async Task<IActionResult> UploadOrderPhotos([FromForm] AddOrderPhotosCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet(Router.OrderRouting.GetNearbyPendingOrders)]
        public async Task<IActionResult> GetPendingNearbyOrders(
    [FromQuery] double lat,
    [FromQuery] double lng,
    [FromQuery] double radiusKm = 20,
    [FromQuery] int take = 50)
        {
            var data = await _orderService.GetNearbyPendingOrdersAsync(lat, lng, Math.Min(radiusKm, 20), take);
            return Ok(new { succeeded = true, data });
        }
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            await orderService.TestingAsync();
            return Ok(new { message = "Test method executed." });
        }
    }
}
