using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransfers.Request.Order
{
    public class Items
    {
        public required Guid Id { get; set; }
        public required int Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
