using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.EF.Repositories
{
    public class UserDailyDiscountRepository:GenericRepository<UserDailyDiscount>, IUserDailyDiscountRepository
    {
        public UserDailyDiscountRepository(ApplicationDbContext context):base(context)
        {
            
        }
    }
}
