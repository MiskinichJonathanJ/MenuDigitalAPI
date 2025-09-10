using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransfers.Request.Order
{
    public class ItemRequest
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
