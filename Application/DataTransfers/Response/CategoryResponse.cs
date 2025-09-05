using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransfers.Response
{
    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public required string CategoryDescription { get; set; }
        public int Order { get; set; }
    }
}