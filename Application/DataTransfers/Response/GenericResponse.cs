using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransfers.Response
{
    public class GenericResponse
    {
        public  int id { get; set; }
        public required string name { get; set; }
    }
}
