using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransfers.Response.OrderResponse
{
    public class OrderResponse
    {
        public int OrderNumber { get; set; }
        public double TotalMount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
