using Calia.Web.Service.IService;
using Calia.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Calia.Web.Controllers
{
    public class PrintController : Controller
    {
        private readonly IOrderService _orderService;
        public PrintController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public async Task<IActionResult> Index()
        {
            string? printers ="";

            ResponseDto? response = await _orderService.GetPrinters();

            if (response != null && response.IsSuccess)
            {
                printers = JsonConvert.DeserializeObject<string>(Convert.ToString(response.Result));
                ViewBag.Printers = printers;
                //ViewBag.PrinterId = _printerSettings.Value.PrinterId;
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View();

        }

        [HttpPost]
        public IActionResult Print(string printerId)
        {
            if (!string.IsNullOrEmpty(printerId))
            {
                //var filePath = Path.Combine(_environment.ContentRootPath, "appsettings.json");
                //var jsonConfig = System.IO.File.ReadAllText(filePath);
                //dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonConfig);

                //jsonObj["PrinterSettings"]["PrinterId"] = printerId;

                //string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                //System.IO.File.WriteAllText(filePath, output);

                //ViewBag.PrinterId = printerId;
                //return View("Result");
            }
            return BadRequest("Geçersiz yazıcı ID'si");
        }
    }
}
