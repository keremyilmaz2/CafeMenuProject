
using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Reflection;
using System.Xml.Linq;

namespace Calia.Web.Controllers
{
	public class ExtraController : Controller
	{
		private readonly ICategoryService _categoryService;
        private readonly IStockService _stockService;
		public ExtraController(ICategoryService categoryService , IStockService stockService )
		{
			_categoryService = categoryService;
            _stockService = stockService;
		}

        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> ExtraIndex()
        {
            List<ExtraVM> extraList = new();


            ResponseDto? response = await _categoryService.GetAllCategoriesExtraAsync();


            if (response != null && response.IsSuccess)
            {

                extraList = JsonConvert.DeserializeObject<List<ExtraVM>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(extraList);
        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> ExtraCreate()
        {

            List<CategoryDto>? list = new();

            ResponseDto? response = await _categoryService.GetAllCategoriesAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CategoryDto>>(Convert.ToString(response.Result));

            }

            CategoryExtraVM categoryExtraVM = new()
            {
                CategoryList = list.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                CategoryExtra = new CategoryExtraDto()
            };
            return View(categoryExtraVM);
        }

        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> ExtraCreate(CategoryExtraVM categoryExtraVM)
        {
            List<CategoryDto>? list = new();

            ResponseDto? response = await _categoryService.GetAllCategoriesAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CategoryDto>>(Convert.ToString(response.Result));

            }

            if (ModelState.IsValid)
            {
                categoryExtraVM.CategoryList = list.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

                for (int i = 0; i < categoryExtraVM.CategoryList.Count(); i++)
                {
                    if (categoryExtraVM.IsCategorySelected[i])
                    {

                        var selectedCategoryId = int.Parse(categoryExtraVM.CategoryList.ElementAt(i).Value);
                        CategoryExtraDto categoryExtraDto = new()
                        {
                            ExtraName = categoryExtraVM.CategoryExtra.ExtraName,
                            ExtraPrice = categoryExtraVM.CategoryExtra.ExtraPrice,
                            CategoryId = selectedCategoryId
                        };
                        ResponseDto? responsePost = await _categoryService.CreateCategoryExtraAsync(categoryExtraDto);

                        
                    }
                }

                
                TempData["success"] = "Material created succesfully.";
                return RedirectToAction("ExtraIndex");
            }

            else
            {
                categoryExtraVM.CategoryList = list.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(categoryExtraVM);
            }



        }

        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> ExtraEdit(string name)
        {

            CategoryExtraVM? categoryExtraVM = new();

            ResponseDto? response = await _categoryService.CategoryExtraGetForcategoryExtraVm(name);

            if (response != null && response.IsSuccess)
            {
                categoryExtraVM = JsonConvert.DeserializeObject<CategoryExtraVM>(Convert.ToString(response.Result));

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(categoryExtraVM);
        }

        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExtraEdit(CategoryExtraVM categoryExtraVM)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _categoryService.UpdateCategoryExtraList(categoryExtraVM);

                if (response != null && response.IsSuccess)
                {
                    categoryExtraVM = JsonConvert.DeserializeObject<CategoryExtraVM>(Convert.ToString(response.Result));

                }
                else
                {
                    TempData["error"] = response?.Message;
                }

                return RedirectToAction("ExtraIndex");
            }

            else
            {
                CategoryExtraVM? categoryExtraVMs = new();

                ResponseDto? response = await _categoryService.CategoryExtraGetForcategoryExtraVm(categoryExtraVM.CategoryExtra.ExtraName);

                if (response != null && response.IsSuccess)
                {
                    categoryExtraVM = JsonConvert.DeserializeObject<CategoryExtraVM>(Convert.ToString(response.Result));

                }
                else
                {
                    TempData["error"] = response?.Message;
                }
                return View(categoryExtraVM);
            }
        }



    }
}
