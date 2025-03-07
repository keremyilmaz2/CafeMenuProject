using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calia.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? list = new();

            ResponseDto? response = await _productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess )
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> ProductCreate()
        {
            ProductVM? productVM = new();

            ResponseDto? response = await _productService.ProductCreateViewForVm();

            if (response != null && response.IsSuccess)
            {
                productVM = JsonConvert.DeserializeObject<ProductVM>(Convert.ToString(response.Result));

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productVM);
        }
        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductVM model)
        {
            if (ModelState.IsValid)
            {

                ResponseDto? response = await _productService.CreateProductAsync(model);

                if (response != null && response.IsSuccess)
                {

                    ProductDto productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                    TempData["success"] = "Product Create Successfully";
                    return RedirectToAction(nameof(ProductSettingMaterial),new { id = productDto.ProductId });
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(model);
        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> ProductSettingMaterial(int id)
        {
            ProductVM? productVM = new();

            ResponseDto? response = await _productService.ProductMaterialAndExtraViewForVm(id);

            if (response != null && response.IsSuccess)
            {
                productVM = JsonConvert.DeserializeObject<ProductVM>(Convert.ToString(response.Result));

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productVM);
        }
        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> ProductSettingMaterial(ProductVM model)
        {
            if (ModelState.IsValid)
            {

                ResponseDto? response = await _productService.CreateProductAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product Create Successfully";
                    return RedirectToAction(nameof(ProductSettingExtra), new { id = model.Product.ProductId });
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(model);
        }

        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> ProductSettingExtra(int id)
        {
            ProductVM? productVM = new();

            ResponseDto? response = await _productService.ProductMaterialAndExtraViewForVm(id);

            if (response != null && response.IsSuccess)
            {
                productVM = JsonConvert.DeserializeObject<ProductVM>(Convert.ToString(response.Result));

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productVM);
        }
        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> ProductSettingExtra(ProductVM model)
        {
            if (ModelState.IsValid)
            {

                ResponseDto? response = await _productService.CreateProductAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product Create Successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(model);
        }




        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> ProductEdit(int ProductId)
		{

			ProductVM? productVM = new();

			ResponseDto? response = await _productService.ProductEditViewForVm(ProductId);

			if (response != null && response.IsSuccess)
			{
				productVM = JsonConvert.DeserializeObject<ProductVM>(Convert.ToString(response.Result));

			}
			else
			{
				TempData["error"] = response?.Message;
			}
			return View(productVM);


		}
        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
		public async Task<IActionResult> ProductEdit(ProductVM model)
		{
			if (ModelState.IsValid)
			{

				ResponseDto? response = await _productService.UpdateProductAsync(model);

				if (response != null && response.IsSuccess)
				{
					TempData["success"] = "Product Updated Successfully";
					return RedirectToAction(nameof(ProductIndex));
				}
				else
				{
					TempData["error"] = response?.Message;
				}
			}

			return View(model);
		}

        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
            {
                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();  
        }
        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            ResponseDto? response = await _productService.DeleteProductAsync(model.ProductId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product Delete Successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(model);
        }

        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> ProductCount(int productId)
		{
			ResponseDto? response = await _productService.GetProductByIdAsync(productId);

			if (response != null && response.IsSuccess)
			{
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				return View(model);
			}
			else
			{
				TempData["error"] = response?.Message;
			}
			return NotFound();
		}
        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
		public async Task<IActionResult> ProductCount(ProductDto model)
		{
			ResponseDto? response = await _productService.AddProductCount(model);

			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Product updated Successfully";
				return RedirectToAction(nameof(ProductIndex));
			}
			else
			{
				TempData["error"] = response?.Message;
			}
			return View(model);
		}




	}
}
