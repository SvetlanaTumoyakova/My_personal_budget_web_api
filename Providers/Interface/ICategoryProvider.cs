using Microsoft.AspNetCore.Mvc;
using My_personal_budget_web_api.DTO.CategoryDto;
using My_personal_budget_web_api.Models;

namespace My_personal_budget_web_api.Providers.Interface
{
    public interface ICategoryProvider
    {
        Task<Category> CreateCategoryAsync(Guid userId, CreateCategoryDto createCategoryDto);
        Task<Category?> GetCategoryByIdAsync(Guid categoryId, Guid userId);
        Task<List<Category>> GetCategoriesAsync(Guid userId);
        Task<Category?> UpdateCategoryNameAsync(Guid categoryId, Guid userId, string newName);
        Task<bool> DeleteCategoryAsync(Guid categoryId, Guid userId);
        Task<bool> IsCategoryNameUniqueAsync(Guid userId, string categoryName, Guid? excludeCategoryId = null);
    }
}
