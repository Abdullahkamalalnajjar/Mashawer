using Mashawer.Data.Entities.ClasssOfOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.EF.Repositories
{
    public class OrderTaskRepository:GenericRepository<OrderTask>, IOrderTaskRepository
    {
        public OrderTaskRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
  
}
