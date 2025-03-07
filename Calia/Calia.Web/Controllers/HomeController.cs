using System.IdentityModel;
using Calia.Web.Models;
using Calia.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using Calia.Web.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Calia.Services.ShoppingCartAPI.RabbitMQSender;

namespace Calia.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        
        private readonly IMessageBus _messageBusAzure;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, IMessageBus messageBusAzure , IOrderService orderService, IProductService productService, IShoppingCartService shoppingCartService,ICategoryService categoryService, IConfiguration configuration)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
            _emailSender = emailSender;
            _configuration = configuration;
           
            _messageBusAzure = messageBusAzure;
        }

        public async Task<IActionResult> Iletisim()
        {
            string sessionId = HttpContext.Session.GetString("UserId");
            ViewBag.UserId = sessionId;
            return View();
        }

        public async Task<IActionResult> Index(int? masaNumarasi)
        {
            if (masaNumarasi.HasValue)
            {
                // Masa numarasını Session'a kaydet
                HttpContext.Session.SetInt32("MasaNumarasi", masaNumarasi.Value);

                // İlgili işlemler için masa numarasını ViewBag'e de kaydedebilirsin
                ViewBag.MasaNumarasi = masaNumarasi.Value;
            }
            else
            {
                // Eğer masa numarası Session'da varsa, onu kullan
                if (HttpContext.Session.GetInt32("MasaNumarasi") != null)
                {
                    ViewBag.MasaNumarasi = HttpContext.Session.GetInt32("MasaNumarasi");
                }
                else
                {
                    // Eğer masa numarası yoksa bir hata ya da yönlendirme yapılabilir
                    ViewBag.MasaNumarasi = "Masa numarası bulunamadı.";
                }
            }

            string SessionId = HttpContext.Session.Id;
            HttpContext.Session.SetString("UserId", SessionId);
            ViewBag.UserId = SessionId;

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

        [Authorize]
        public async Task<IActionResult> AdminIndex()
        {
            
            RaporVM? raporVM = new();

            ResponseDto? response = await _orderService.GetRapor();

            if (response != null && response.IsSuccess)
            {
                raporVM = JsonConvert.DeserializeObject<RaporVM>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            for (int i = 0; i < 12; i++)
            {
                if (raporVM.Veriler.AylikKrediKarti[i] != null)
                {
                    raporVM.YillikKrediKarti += raporVM.Veriler.AylikKrediKarti[i];
                }
                if (raporVM.Veriler.AylikNakit[i] != null)
                {
                    raporVM.YillikNakit += raporVM.Veriler.AylikNakit[i];
                }
               
            }

            return View(raporVM);
        }


        [Authorize]
        public async Task<IActionResult> TableDetail(int id)
        {
            TableDetailsDto tableDetail = new();

            ResponseDto? response = await _orderService.GetTablewithId(id);

            if (response != null && response.IsSuccess)
            {
                tableDetail = JsonConvert.DeserializeObject<TableDetailsDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(tableDetail);
        }



        public IActionResult MainIndex(string? masaNumarasi)
        {
            // Eğer 'masaNumarasi' parametresi doluysa, Session'a ve ViewBag'e kaydet
            if (!string.IsNullOrEmpty(masaNumarasi))
            {
                // Masa numarasını Session'a kaydet
                HttpContext.Session.SetString("MasaNumarasi", masaNumarasi);

                // Masa numarasını ViewBag'e aktar
                ViewBag.MasaNumarasi = masaNumarasi;
            }
            else
            {
                // Eğer 'masaNumarasi' parametresi boşsa, Session'daki değeri kontrol et
                string? storedMasaNumarasi = HttpContext.Session.GetString("MasaNumarasi");

                if (!string.IsNullOrEmpty(storedMasaNumarasi))
                {
                    ViewBag.MasaNumarasi = storedMasaNumarasi;
                }
                
            }

            // Kullanıcı oturum ID'sini Session'a kaydet
            string sessionId = HttpContext.Session.Id;
            HttpContext.Session.SetString("UserId", sessionId);
            ViewBag.UserId = sessionId;

            return View();
        }

        [ResponseCache(CacheProfileName = "Default300")]
        public async Task<IActionResult> Menu()
        {
            
            string sessionId = HttpContext.Session.GetString("UserId");
            ViewBag.UserId = sessionId;

            List<CategoryDto>? categories = new();
            
            ResponseDto? responseCategory = await _categoryService.GetAllCategoriesAsync();
           
            if (responseCategory != null && responseCategory.IsSuccess)
            {
                categories = JsonConvert.DeserializeObject<List<CategoryDto>>(Convert.ToString(responseCategory.Result));
            }
            else
            {
                TempData["error"] = responseCategory?.Message;
            }

            


            List<CategoryVM> categoryVMs = new List<CategoryVM>();
            foreach (var category in categories)
            {
                List<ProductDto>? products = new();
                ResponseDto? responseProduct = await _productService.GetProductByCategoryIdAsync(category.Id);
                if (responseProduct != null && responseProduct.IsSuccess)
                {
                    products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responseProduct.Result));
                }
                else
                {
                    TempData["error"] = responseProduct?.Message;
                }
                CategoryVM categoryVM = new()
                {
                    Category = category,
                    Products = products
                };
                categoryVMs.Add(categoryVM);
            }

            List<CartVM> cartVMs = new List<CartVM>();
            foreach (var categoryVM in categoryVMs)
            {
                foreach (var product in categoryVM.Products)
                {
                    CartVM cartVM = new()
                    {
                        ShoppingCart = new ShoppingCart
                        {
                            Product = product,
                            // Diğer ShoppingCart özelliklerini buraya ekleyin
                        },
                        IsExtraSelected = new List<bool>(new bool[product.ProductExtras.Count]),
                        categoryVMs = new List<CategoryVM> { categoryVM }
                    };
                    cartVMs.Add(cartVM);
                }
            }

            return View(cartVMs);
        }


        [HttpPost]
        public async Task<IActionResult> SaveInvoicePdf(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uniqueFileName = GetUniqueFileName(file.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "gunsonu", uniqueFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok();
            }

            return BadRequest("No file received.");
        }

        private string GetUniqueFileName(string fileName)
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "gunsonu");
            var filePath = Path.Combine(directoryPath, fileName);
            var fileExtension = Path.GetExtension(fileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            int count = 1;

            while (System.IO.File.Exists(filePath))
            {
                fileName = $"{fileNameWithoutExtension}_{count}{fileExtension}";
                filePath = Path.Combine(directoryPath, fileName);
                count++;
            }

            return fileName;
        }

        [HttpPost]
        public async Task<IActionResult> SendBasvuruEmail(IsBasvurusu ısBasvurusu)
        {
            string recipientEmail = "keremiremsu14@gmail.com"; // Alıcının e-posta adresi
            var subject = "İş Başvurusu - " + ısBasvurusu.AdSoyad;

            // Başvuru detaylarını e-postanın gövdesine ekle
            var message = $@"
                <h1>İş Başvurusu Detayları</h1>
                <p><strong>Ad Soyad:</strong> {ısBasvurusu.AdSoyad}</p>
                <p><strong>Telefon:</strong> {ısBasvurusu.Telefon}</p>
                <p><strong>E-posta:</strong> {ısBasvurusu.Email}</p>
                <p><strong>Husus:</strong> {ısBasvurusu.Husus}</p>";




            try
            {
                // E-postayı ek dosyayla gönder
                await _emailSender.SendEmailWithAttachmentAsync(recipientEmail, subject, message, "");
                TempData["SuccessMessage"] = "Başvurunuz başarıyla gönderildi!";
            }
            catch (Exception ex)
            {
                // Hata durumunda bilgilendirme yap
                TempData["ErrorMessage"] = $"E-posta gönderilirken bir hata oluştu: {ex.Message}";
                return RedirectToAction("Error"); // Hata sayfasına yönlendir
            }

            return RedirectToAction("MainIndex");
        }




        public async Task<IActionResult> SendInvoiceEmail()
        {
            string recipientEmail = "keremiremsu14@gmail.com"; // Alıcının e-posta adresi
            var subject = "Your Invoice";
            var message = "<h1>Your Invoice Details</h1><p>Invoice content goes here.</p>";

            // Dosya yolunu belirleyin
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "gunsonu");

            // En son dosyayı al
            var latestFilePath = Directory.GetFiles(directoryPath)
                                           .OrderByDescending(f => System.IO.File.GetLastWriteTime(f))
                                           .FirstOrDefault();

            // Dosya yolunun geçerli olup olmadığını kontrol et
            if (latestFilePath == null)
            {
                // Dosya bulunamazsa bir hata mesajı döndür veya kullanıcıyı bilgilendir
                TempData["ErrorMessage"] = "Invoice file not found. Please generate the invoice first.";
                return RedirectToAction("Error"); // Hata sayfasına yönlendirin
            }

            try
            {
                await _emailSender.SendEmailWithAttachmentAsync(recipientEmail, subject, message, latestFilePath);
                TempData["SuccessMessage"] = "Invoice sent successfully!";
            }
            catch (Exception ex)
            {
                // Hata oluşursa kullanıcıyı bilgilendirin
                TempData["ErrorMessage"] = $"An error occurred while sending the email: {ex.Message}";
                return RedirectToAction("Error"); // Hata sayfasına yönlendirin
            }

            return RedirectToAction("InvoiceSentConfirmation");
        }




        public async Task<IActionResult> GunSonu()
        {
            GunSonuDto? gunSonuDto = new();

            ResponseDto? response = await _orderService.GetGunSonu();
            if (response != null && response.IsSuccess)
            {
                gunSonuDto = JsonConvert.DeserializeObject<GunSonuDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(gunSonuDto);
        }

        [HttpPost]
        public async Task<IActionResult> GunSonu(GunSonuDto gunSonuDto)
        {
            
            var response = await _orderService.GunSonuPost(gunSonuDto);

            // Yanıtın başarı durumunu kontrol edin
            if (response != null && response.IsSuccess)
            {
                gunSonuDto = JsonConvert.DeserializeObject<GunSonuDto>(Convert.ToString(response.Result));
                return View(gunSonuDto);
            }
            return View();
        }




        public async Task<IActionResult> MasaGunlukRapor()
        {
            RaporVM? raporVM = new();

            ResponseDto? response = await _orderService.TumunuGor();

            if (response != null && response.IsSuccess)
            {
                raporVM = JsonConvert.DeserializeObject<RaporVM>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }


            return View(raporVM);
        }

        public async Task<IActionResult> Iptaller()
        {
            RaporVM? raporVM = new();

            ResponseDto? response = await _orderService.TumunuGor();

            if (response != null && response.IsSuccess)
            {
                raporVM = JsonConvert.DeserializeObject<RaporVM>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }


            return View(raporVM);
        }

        public async Task<IActionResult> MasaToplamKazanc()
        {
            RaporVM? raporVM = new();

            ResponseDto? response = await _orderService.TumunuGor();

            if (response != null && response.IsSuccess)
            {
                raporVM = JsonConvert.DeserializeObject<RaporVM>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }


            return View(raporVM);
        }

        public async Task<IActionResult> Malzemeler()
        {
            RaporVM? raporVM = new();

            ResponseDto? response = await _orderService.TumunuGor();

            if (response != null && response.IsSuccess)
            {
                raporVM = JsonConvert.DeserializeObject<RaporVM>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }


            return View(raporVM);
        }

        public async Task<IActionResult> SatilanUrunler()
        {
            RaporVM? raporVM = new();

            ResponseDto? response = await _orderService.TumunuGor();

            if (response != null && response.IsSuccess)
            {
                raporVM = JsonConvert.DeserializeObject<RaporVM>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }


            return View(raporVM);
        }

        public async Task<IActionResult> IkramEdilenUrunler()
        {
            RaporVM? raporVM = new();

            ResponseDto? response = await _orderService.TumunuGor();

            if (response != null && response.IsSuccess)
            {
                raporVM = JsonConvert.DeserializeObject<RaporVM>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }


            return View(raporVM);
        }


        public async Task<IActionResult> KalanTatlilar()
        {
            RaporVM? raporVM = new();

            ResponseDto? response = await _orderService.TumunuGor();

            if (response != null && response.IsSuccess)
            {
                raporVM = JsonConvert.DeserializeObject<RaporVM>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }


            return View(raporVM);
        }








        [HttpGet]
        public async Task<IActionResult> Products(int categoryId)
        {
            // Ürün listesi oluşturuyoruz
            List<ProductDto> productList = new List<ProductDto>();

            // Kategoriye göre ürünleri getiren servis çağrısı
            ResponseDto? response = await _productService.GetProductByCategoryIdAsync(categoryId);

            if (response != null && response.IsSuccess)
            {
                // Sonuçları ProductDto listesine deserialize ediyoruz
                productList = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
                productList = new List<ProductDto>(); // Boş liste döndürüyoruz
            }

            // Kategori listesi ve diğer seçenekler için servis çağrıları yapabilirsiniz
            // ViewBag.CategoryList = ...;
            // ViewBag.CategoryMaterials = ...;
            // ViewBag.CategoryExtras = ...;

            // Listeyi View'a gönderiyoruz
            return View(productList);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            CartVM cartVM = new CartVM
            {
                ShoppingCart = new ShoppingCart()
            };

            ResponseDto response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
            {
                cartVM.ShoppingCart.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(cartVM);
        }



        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails([FromBody] CartVM cartVM)
        {

            string? MasaNumarasi = HttpContext.Session.GetString("MasaNumarasi");
            if (!User.IsInRole(SD.RoleAdmin) && !User.IsInRole(SD.RoleWaiter))
            {
                // Eğer masa numarası yoksa, kullanıcıyı yönlendir
                if (string.IsNullOrEmpty(MasaNumarasi))
                {

                    return Json(new { success = false, message = "Lütfen QR' dan giriş yapınız!" });
                }
            }
            

            var sessionId = HttpContext.Session.GetString("UserId");

            cartVM.ShoppingCart.SessionId = sessionId;
            //string topicName = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            //await _messageBusAzure.PublishMessage(cartVM, topicName);

            //_messageBus.SendMessage(cartVM, _configuration.GetValue<string>("TopicAndQueueNames:AddShoppingCartQueue"));

            ResponseDto response = await _shoppingCartService.UpsertCartAsync(cartVM);

            if (response != null && response.IsSuccess)
            {
                // Optionally deserialize the product information
                // cartVM.ShoppingCart.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));

                return Json(new { success = true, message = "Ürün başarıyla eklendi!" });
            }
            else
            {
                return Json(new { success = false, message = response?.Message ?? "Bir hata oluştu." });
            }
        }





        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
