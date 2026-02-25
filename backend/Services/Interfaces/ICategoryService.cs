using ECommerce.DTOs;
using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface ICategoryService
    {
        Category Create(string name);
        List<Category> GetAll();
        Category? GetById(int id);
        Category? Update(int id, UpdateCategoryDto dto);
        //bool Delete(int id);
        bool ToggleStatus(int id);
    }
}
