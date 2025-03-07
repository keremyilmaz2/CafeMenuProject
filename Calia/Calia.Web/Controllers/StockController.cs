using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Callia.Web.Controllers
{
    public class StockController : Controller
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> StockIndex()
        {
            List<StockMaterialDto>? stockList = new();

            ResponseDto? response = await _stockService.GetAllStocksAsync();
            if (response != null && response.IsSuccess)
            {
                stockList = JsonConvert.DeserializeObject<List<StockMaterialDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(stockList);
        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> StockEdit(int stockId)
        {
            ResponseDto? response = await _stockService.GetStockByIdAsync(stockId);

            if (response != null && response.IsSuccess)
            {
                StockMaterialDto? model = JsonConvert.DeserializeObject<StockMaterialDto>(Convert.ToString(response.Result));
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
        public async Task<IActionResult> StockEdit(StockMaterialDto stockMaterialsDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _stockService.UpdateStockAsync(stockMaterialsDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Stock updated successfully";
                    return RedirectToAction(nameof(StockIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(stockMaterialsDto);
        }
    }
}