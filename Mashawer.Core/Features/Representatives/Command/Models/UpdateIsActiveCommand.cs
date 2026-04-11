using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Core.Features.Representatives.Command.Models
{
    public class UpdateIsActiveCommand :IRequest<Response<string>>
    {
        public UpdateIsActiveCommand(string userId, bool isActive)
        {
            UserId = userId;
            IsActive = isActive;
        }

        public string UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
