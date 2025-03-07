using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Calia.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> CategoryIndex()
        {
            List<CategoryDto>? list = new();

            ResponseDto? response = await _categoryService.GetAllCategoriesAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CategoryDto>>(Convert.ToString(response.Result));

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> CategoryCreate()
        {
            return View();
        }
        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> CategoryCreate(CategoryDto model)
        {
            if (ModelState.IsValid)
            {

                ResponseDto? response = await _categoryService.CreateCategoryAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Category Create Successfully";
                    return RedirectToAction(nameof(CategoryIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(model);
        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> CategoryEdit(int CategoryId)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _categoryService.GetCategoryByIdAsync(CategoryId);

                if (response != null && response.IsSuccess)
                {
                     CategoryDto? model = JsonConvert.DeserializeObject<CategoryDto>(Convert.ToString(response.Result));
                    return View(model);
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return NotFound();


        }
        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> CategoryEdit(CategoryDto model)
        {
            if (ModelState.IsValid)
            {

                ResponseDto? response = await _categoryService.UpdateCategoryAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product Updated Successfully";
                    return RedirectToAction(nameof(CategoryIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(model);
        }





        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> CategoryDelete(int categoryId)
        {
            ResponseDto? response = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (response != null && response.IsSuccess)
            {
                CategoryDto? model = JsonConvert.DeserializeObject<CategoryDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [Authorize(Roles =SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> CategoryDelete(CategoryDto model)
        {
            ResponseDto? response = await _categoryService.DeleteCategoryAsync(model.Id);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product Delete Successfully";
                return RedirectToAction(nameof(CategoryIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(model);
        }
    }
}
