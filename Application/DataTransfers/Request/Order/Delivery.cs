using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransfers.Request.Order
{
    public class Delivery
    {
        public required int Id { get; set; }
        public string? To { get; set; }
    }
}
