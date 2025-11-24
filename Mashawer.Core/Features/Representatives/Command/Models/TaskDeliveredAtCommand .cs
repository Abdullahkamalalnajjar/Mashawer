using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Core.Features.Representatives.Command.Models
{
    public class TaskDeliveredAtCommand:IRequest<Response<string>>
    {
        public int TaskId { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
