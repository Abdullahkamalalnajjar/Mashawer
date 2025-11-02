using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Data.Dtos
{
    public class PurchaseItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal PriceTotal { get; set; }
    }

}
