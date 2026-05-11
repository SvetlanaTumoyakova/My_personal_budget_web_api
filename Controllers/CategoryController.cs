using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_personal_budget_web_api.DTO.CategoryDto;
using My_personal_budget_web_api.Providers.Interface;
using System.Security.Claims;

namespace My_personal_budget_web_api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryProvider _categoryProvider;

        public CategoryController(ICategoryProvider categoryProvider)
        {
            _categoryProvider = categoryProvider;
        }

        /// <summary>
        /// Создаёт новую категорию для текущего пользователя на основе переданных данных.
        /// </summary>
        /// <param name="createCategoryDto">Данные для создания категории (CreateCategoryDto).</param>
        /// <returns>IActionResult с данными созданной категории (CategoryDto) при успешном выполнении.</returns>
        /// <remarks>
        /// Возможные ошибки:
        /// - 400 Bad Request: некорректные данные в запросе или недопустимый идентификатор пользователя.
        /// - 401 Unauthorized: пользователь не авторизован.
        /// - 500 Internal Server Error: внутренняя ошибка сервера.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Получаем ID пользователя из токена
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Пользователь не авторизован!" });
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return BadRequest(new { message = "Недопустимый идентификатор пользователя" });
            }

            try
            {

                var isUniqueCategoryName = await IsCategoryNameUnique(userId, createCategoryDto.Name);
                if (!isUniqueCategoryName)
                {
                    return BadRequest(new { message = "Категория с таким названием уже существует" });
                }

                var category = await _categoryProvider.CreateCategoryAsync(userId, createCategoryDto);

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    CreatedAt = category.CreatedAt,
                    IsDefault = category.IsDefault
                };

                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получает информацию о категории по её идентификатору (ID).
        /// </summary>
        /// <param name="id">Идентификатор категории (GUID).</param>
        /// <returns>IActionResult с данными категории в формате CategoryDto.</returns>
        /// <remarks>
        /// Возможные коды ошибок:
        /// - 200 OK: категория найдена и возвращены её данные.
        /// - 401 Unauthorized: пользователь не авторизован.
        /// - 404 Not Found: категория с указанным ID не найдена или удалена.
        /// - 500 Internal Server Error: произошла внутренняя ошибка сервера.
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Пользователь не авторизован" });
            }

            try
            {
                var category = await _categoryProvider.GetCategoryByIdAsync(id, userId);
                if (category == null)
                {
                    return NotFound(new { message = "Категория не найдена или удалена" });
                }

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsDefault = category.IsDefault,
                    TransactionTypeId = category.TransactionTypeId,
                    TransactionType = category.TransactionType.Type,
                    CreatedAt = category.CreatedAt
                };

                return Ok(categoryDto);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получает список категорий текущего авторизованного пользователя в формате CategoryDto, включая идентификатор, название, дату создания.
        /// </summary>
        /// <returns>IActionResult с коллекцией CategoryDto при успешном выполнении.</returns>
        /// <remarks>
        /// Возможные коды ошибок:
        /// - 401 Unauthorized: пользователь не авторизован.
        /// - 500 Internal Server Error: произошла внутренняя ошибка сервера.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Пользователь не авторизован" });
            }

            try
            {
                var categories = await _categoryProvider.GetCategoriesAsync(userId);
                var categoriesDto = categories.Select(category => new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsDefault = category.IsDefault,
                    TransactionTypeId = category.TransactionTypeId,
                    TransactionType = category.TransactionType.Type,
                    CreatedAt = category.CreatedAt
                }).ToList();

                return Ok(categoriesDto);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Обновляет название категории для текущего авторизованного пользователя по указанному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор категории (GUID), которую необходимо обновить.</param>
        /// <param name="updateCategoryNameDto">Данные для обновления названия категории (UpdateCategoryNameDto).</param>
        /// <returns>IActionResult с сообщением об успехе, идентификатором категории, новым названием и временем обновления при успешном выполнении.</returns>
        /// <remarks>
        /// Возможные коды ошибок:
        /// - 400 Bad Request: некорректные данные в запросе (например, пустое или слишком длинное название).
        /// - 401 Unauthorized: пользователь не авторизован.
        /// - 404 Not Found: категория с указанным ID не найдена.
        /// - 500 Internal Server Error: произошла внутренняя ошибка сервера.
        /// </remarks>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCategoryName(Guid id, [FromBody] UpdateCategoryNameDto updateCategoryNameDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Пользователь не авторизован" });
            }

            try
            {
                var isUnique = await IsCategoryNameUnique(
                    userId,
                    updateCategoryNameDto.Name,
                    id
                );
                if (!isUnique)
                {
                    return BadRequest(new { message = "Категория с таким названием уже существует" });
                }


                var updatedCategory = await _categoryProvider.UpdateCategoryNameAsync(id, userId, updateCategoryNameDto.Name);

                if (updatedCategory == null)
                {
                    return NotFound(new { message = "Категория не найдена или удалена" });
                }

                return Ok(new
                {
                    message = "Название категории успешно обновлено",
                    categoryId = id,
                    newName = updateCategoryNameDto.Name,
                    updatedAt = updatedCategory.UpdatedAt
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Логически удаляет категорию для текущего авторизованного пользователя по указанному идентификатору.
        /// При логическом удалении категория помечается как удалённая (IsDeleted = true), но остаётся в базе данных.
        /// </summary>
        /// <param name="id">Идентификатор категории (GUID), которую необходимо удалить.</param>
        /// <returns>IActionResult с сообщением об успехе, идентификатором категории и временем обновления при успешном выполнении.</returns>
        /// <remarks>
        /// Возможные коды ошибок:
        /// - 401 Unauthorized: пользователь не авторизован.
        /// - 404 Not Found: категория с указанным ID не найдена или уже удалена.
        /// - 500 Internal Server Error: произошла внутренняя ошибка сервера.
        /// </remarks>

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Пользователь не авторизован" });
            }

            try
            {
                var category = await _categoryProvider.GetCategoryByIdAsync(id, userId);
                if (category == null)
                {
                    return NotFound(new { message = "Категория не найдена или уже удалена" });
                }

                var result = await _categoryProvider.DeleteCategoryAsync(id, userId);

                if (!result)
                {
                    return NotFound(new { message = "Категория не найдена или уже удалена" });
                }

                return Ok(new
                {
                    message = "Категория успешно удалена",
                    categoryId = id,
                    categoryName = category.Name
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера" });
            }
        }

        private async Task<bool> IsCategoryNameUnique(Guid userId, string categoryName, Guid? excludeCategoryId = null)
        {
            try
            {
                return await _categoryProvider.IsCategoryNameUniqueAsync(userId, categoryName, excludeCategoryId);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
