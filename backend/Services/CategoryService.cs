using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
            
        // create
        public Category? Create(string name)
        {
            var normalized = name.Trim().ToLower();

            var exists = _context.Categories
                .Any(c => c.Name == normalized);

            if (exists)
                return null;

            var category = new Category
            {
                Name = normalized,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return category;
        }

        // get all
        public List<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        //get by id
        public Category? GetById(int id)
        {
            return _context.Categories
                .FirstOrDefault(c => c.Id == id && c.IsActive);
        }

        // update
        public Category? Update(int id, UpdateCategoryDto dto)
        {
            var category = _context.Categories
                .FirstOrDefault(c => c.Id == id && c.IsActive);

            if (category == null)
                return null;

            category.Name = dto.Name.Trim();
            category.ModifiedOn = DateTime.UtcNow;

            _context.SaveChanges();
            return category;
        }

        //// delete 
        //public bool Delete(int id)
        //{
        //    var category = _context.Categories.Find(id);
        //    if (category == null)
        //        return false;

        //    _context.Categories.Remove(category);
        //    _context.SaveChanges();
        //    return true;
        //}


        // active and deactivate 
        public bool ToggleStatus(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return false;

            // Toggle IsActive
            category.IsActive = !category.IsActive;
            category.ModifiedOn = DateTime.UtcNow;

            _context.SaveChanges();
            return true;
        }

    }
}
