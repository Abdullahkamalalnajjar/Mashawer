using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Core.Features.UserDailyDiscounts.Command.Models
{
    public class AddUserDailyDiscountCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
        public DateTime DiscountDate { get; set; }
        public decimal DiscountAmount { get; set; }
    }

}
