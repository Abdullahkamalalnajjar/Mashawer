using Mashawer.Api.Base;
using Mashawer.Core.Features.Notifications.Commands.Models;
using Mashawer.Core.Features.Notifications.Queries.Models;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{
    [Authorize]
    public class NotificationController : AppBaseController
    {
        [HttpGet(Router.NotificationRouting.GetNotificationsForUser)]
        public async Task<IActionResult> GetNotificationsForUser()
        {
            var response = await Mediator.Send(new GetNotificationOfUserQuery());
            return NewResult(response);
        }
        [HttpPost(Router.NotificationRouting.MarkAsRead)]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var response = await Mediator.Send(new MarkAsReadCommand(notificationId));
            return NewResult(response);
        }

        [HttpDelete(Router.NotificationRouting.DeleteNotification)]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var response = await Mediator.Send(new DeleteNotificationCommand(notificationId));
            return NewResult(response);
        }
        [HttpDelete(Router.NotificationRouting.DeleteAllNotifications)]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            var response = await Mediator.Send(new DeleteAllNotificationCommand());
            return NewResult(response);
        }
    }
}
