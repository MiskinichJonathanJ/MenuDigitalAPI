using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransfers.Response
{
    public class DishResponse
    {
        public Guid ID { get; set; }
        public required string name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public required string Image { get; set; }
        public GenericResponse? category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
