using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransfers.Request.Dish
{
    public abstract class DishBaseRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Category { get; set; }
        public string Image { get; set; }
    }
}
