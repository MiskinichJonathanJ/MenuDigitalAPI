using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransfers.Request.Dish
{
    public class UpdateDishRequest : DishBaseRequest
    {
        public  bool IsActive { get; set; }
    }
}
