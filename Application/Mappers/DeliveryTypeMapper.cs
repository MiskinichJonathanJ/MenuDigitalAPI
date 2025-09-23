using Application.DataTransfers.Response;
using Application.Interfaces.IDeliveryType;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class DeliveryTypeMapper : IDeliveryTypeMapper
    {
        public GenericResponse ToResponse(DeliveryType entity)
        {
            return new GenericResponse
            {
                id = entity.Id,
                name = entity.Name
            };
        }
    }
}
