using Application.DataTransfers.Request.Dish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.DishInterfaces
{
    public interface IDishValidator
    {
        Task ValidateCreate(DishBaseRequest request);
        void ValidateCommon(DishBaseRequest request);
        Task ValidateUpdate(Guid idDish,  UpdateDishRequest request);
        Task ValidateCategoryExists(int categoryId);
        Task ValidateDishNameUnique(string name, Guid? id = null);
    }
}
