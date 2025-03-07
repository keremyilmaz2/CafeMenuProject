
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
	public class MaterialController : Controller
	{
		private readonly ICategoryService _categoryService;
        private readonly IStockService _stockService;
		public MaterialController(ICategoryService categoryService , IStockService stockService )
		{
			_categoryService = categoryService;
            _stockService = stockService;
		}

        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> MaterialIndex()
        {
            List<MaterialVM> materialList = new();


            ResponseDto? response = await _categoryService.GetAllCategoriesMaterialAsync();


            if (response != null && response.IsSuccess)
            {

                materialList = JsonConvert.DeserializeObject<List<MaterialVM>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(materialList);
        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> MaterialCreate()
        {

            List<CategoryDto>? list = new();

            ResponseDto? response = await _categoryService.GetAllCategoriesAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CategoryDto>>(Convert.ToString(response.Result));

            }

            CategoryMaterialVM categoryMaterialVM = new()
            {
                CategoryList = list.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                CategoryMaterial = new CategoryMaterialDto()
            };
            return View(categoryMaterialVM);
        }

        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> MaterialCreate(CategoryMaterialVM categoryMaterialVM)
        {
            List<CategoryDto>? list = new();

            ResponseDto? response = await _categoryService.GetAllCategoriesAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CategoryDto>>(Convert.ToString(response.Result));

            }

            if (ModelState.IsValid)
            {
                categoryMaterialVM.CategoryList = list.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

                for (int i = 0; i < categoryMaterialVM.CategoryList.Count(); i++)
                {
                    if (categoryMaterialVM.IsCategorySelected[i])
                    {

                        var selectedCategoryId = int.Parse(categoryMaterialVM.CategoryList.ElementAt(i).Value);
                        CategoryMaterialDto categoryMaterial = new()
                        {
                            MaterialName = categoryMaterialVM.CategoryMaterial.MaterialName,
                            CategoryId = selectedCategoryId
                        };
                        ResponseDto? responsePost = await _categoryService.CreateCategoryMaterialAsync(categoryMaterial);

                        
                    }
                }

                ResponseDto? responseStock = await _stockService.CreateStockAsync(categoryMaterialVM.CategoryMaterial.MaterialName);
                TempData["success"] = "Material created succesfully.";
                return RedirectToAction("MaterialIndex");
            }

            else
            {
                categoryMaterialVM.CategoryList = list.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(categoryMaterialVM);
            }



        }

        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> MaterialEdit(string name)
        {

            CategoryMaterialVM? categoryMaterialVM = new();

            ResponseDto? response = await _categoryService.CategoryMaterialGetForcategoryMaterialVm(name);

            if (response != null && response.IsSuccess)
            {
                categoryMaterialVM = JsonConvert.DeserializeObject<CategoryMaterialVM>(Convert.ToString(response.Result));

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(categoryMaterialVM);
        }

        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MaterialEdit(CategoryMaterialVM categoryMaterialVM)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _categoryService.UpdateCategoryMatiralList(categoryMaterialVM);

                if (response != null && response.IsSuccess)
                {
                    categoryMaterialVM = JsonConvert.DeserializeObject<CategoryMaterialVM>(Convert.ToString(response.Result));

                }
                else
                {
                    TempData["error"] = response?.Message;
                }

                return RedirectToAction("MaterialIndex");
            }

            else
            {
                CategoryMaterialVM? categoryMaterialVMs = new();

                ResponseDto? response = await _categoryService.CategoryMaterialGetForcategoryMaterialVm(categoryMaterialVM.CategoryMaterial.MaterialName);

                if (response != null && response.IsSuccess)
                {
                    categoryMaterialVM = JsonConvert.DeserializeObject<CategoryMaterialVM>(Convert.ToString(response.Result));

                }
                else
                {
                    TempData["error"] = response?.Message;
                }
                return View(categoryMaterialVM);
            }
        }



    }
}
