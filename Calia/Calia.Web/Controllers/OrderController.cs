using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Calia.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
                _orderService = orderService;
        }
        [Authorize]
        public IActionResult OrderIndex()
        {
            return View();
        }

        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> GetTables()
        {
            var response = await _orderService.GetTables();
            if (response != null && response.IsSuccess)
            {
                var tables = JsonConvert.DeserializeObject<List<TableNoDto>>(Convert.ToString(response.Result));
                TempData["Success"] = "Masa Eklendi";

                return View(tables);

            }
            return View();


        }
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> AddTable()
        {
            TableNoDto TableNoDto = new TableNoDto();
            return View(TableNoDto);


        }
        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> AddTable(TableNoDto tableNoDto)
        {
            var response = await _orderService.AddTables(tableNoDto.MasaNo);
            if (response != null && response.IsSuccess)
            {

                TempData["Success"] = "Masa Eklendi";

                return RedirectToAction(nameof(GetTables));

            }
            return View();


        }
        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> MasaDelete(int masaId)
        {
            var response = await _orderService.DeleteTable(masaId);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Masa başarıyla silindi."; // Başarı mesajı güncellendi
                return RedirectToAction(nameof(GetTables));
            }
            TempData["Error"] = "Masa silinirken bir hata oluştu."; // Hata mesajı
            return RedirectToAction(nameof(GetTables)); // Hata durumunda da tabloya geri döner
        }

        [Authorize(Roles = SD.RoleAdmin)]
        [Authorize]
        public async Task<IActionResult> Gider()
        {
            // Admin adları listesini tutmak için kullanıyoruz
            IEnumerable<SelectListItem> adminNames = new List<SelectListItem>();

            // API veya veritabanı üzerinden admin adlarını çekiyoruz
            ResponseDto? response = await _orderService.GetAdmins();

            if (response != null && response.IsSuccess)
            {
                var admins = JsonConvert.DeserializeObject<List<AdminNamesDto>>(Convert.ToString(response.Result));

                // Admin adlarını SelectListItem yapısına çeviriyoruz
                adminNames = admins.Select(a => new SelectListItem
                {
                    Text = a.AdminName,  // Görüntülenecek isim
                    Value = a.Id.ToString()  // Seçildiğinde gönderilecek değer
                });
            }

            // ViewModel'i dolduruyoruz
            var model = new KisiselGiderVm
            {
                Admins = adminNames // Admin adlarını dolduruyoruz
            };

            return View(model); // View'e model ile birlikte geri dönüyoruz
        }



        [Authorize(Roles = SD.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> CreateGider(KisiselGiderVm model)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<AdminNamesDto> admins = new List<AdminNamesDto>();

                // Örnek veri olarak ResponseDto'dan Admin adlarını dolduruyoruz.
                ResponseDto? response1 = await _orderService.GetAdmins();

                if (response1 != null && response1.IsSuccess)
                {
                    admins = JsonConvert.DeserializeObject<List<AdminNamesDto>>(Convert.ToString(response1.Result));
                }



                // DTO oluşturma ve doldurma
                KisiselGiderDto kisiselGiderDto = new KisiselGiderDto
                {
                    AdminId = model.AdminId,
                    AlinanPara = model.CashAmount,
                    Reason = model.Reason
                };
                foreach (var admin in admins)
                {
                    if (admin.Id == model.AdminId)
                    {
                        kisiselGiderDto.AdminName = admin;
                    }
                }
                // POST isteği ile veriyi API'ye gönderiyoruz
                ResponseDto? response = await _orderService.CreateKisiselGider(kisiselGiderDto);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("Gider");
                }

                ModelState.AddModelError(string.Empty, "An error occurred while creating the personal expense.");
            }

            // Admin listesi yeniden doldurulmalı
            IEnumerable<SelectListItem> adminNames = new List<SelectListItem>();
            ResponseDto? adminResponse = await _orderService.GetAdmins();

            if (adminResponse != null && adminResponse.IsSuccess)
            {
                var adminList = JsonConvert.DeserializeObject<List<AdminNamesDto>>(Convert.ToString(adminResponse.Result));

                // Admin isimlerini SelectListItem formatına dönüştürme
                adminNames = adminList.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.AdminName
                }).ToList();
            }

            model.Admins = adminNames;

            return View(model); // Hatalı ise formu yeniden doldur ve göster
        }





        [Authorize]
        public async Task<IActionResult> TableDetails(int id)
        {
            TableDetailsDto? tableDetails = new();
            PaymentVm paymentVm = new()
            {
                UnpaidOrderDetails = new List<OrderDetailsDto>(),
                PaidOrderDetails = new List<OrderDetailsDto>(),
                IkramOrderDetails = new List<OrderDetailsDto>(),
            };

            ResponseDto? response = await _orderService.GetTableDetails(id);
            ResponseDto? responsee = await _orderService.GetTables();
            if (response != null && response.IsSuccess)
            {
                tableDetails = JsonConvert.DeserializeObject<TableDetailsDto>(Convert.ToString(response.Result));

                paymentVm.TableDetails = tableDetails;

                foreach (var item in tableDetails.OrderHeaders)
                {
                    foreach (var orderDetails in item.OrderDetails)
                    {
                        if (orderDetails.PaymentStatus != SD.StatusCancelled)
                        {
                            if (orderDetails.PaymentStatus == SD.PaymentStatusIkram)
                            {
                                paymentVm.IkramOrderDetails.Add(orderDetails);
                            }
                            else if (orderDetails.isPaid == true )
                            {
                                paymentVm.PaidOrderDetails.Add(orderDetails);
                            }
                            else if (orderDetails.OdemesiAlinmisCount != 0 && orderDetails.Count != 0)
                            {
                                paymentVm.PaidOrderDetails.Add(orderDetails);
                                paymentVm.UnpaidOrderDetails.Add(orderDetails);
                            }
                            else
                            {
                                paymentVm.UnpaidOrderDetails.Add(orderDetails);
                            }
                        }
                        
                    }
                }
                var Tables = JsonConvert.DeserializeObject<List<TableNoDto>>(Convert.ToString(responsee.Result));

                paymentVm.Tables = Tables.Select(u => new SelectListItem
                {
                    Text = u.MasaNo,
                    Value = u.Id.ToString()
                });
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(paymentVm);
        }



        [Authorize]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

            }
            if (!User.IsInRole(SD.RoleAdmin) && userId != orderHeaderDto.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDto);
        }

        [Authorize]
        public async Task<IActionResult> Masalar()
        {
            IEnumerable<TableNoDto> Tables = new List<TableNoDto>();
            ResponseDto? response = await _orderService.GetTables();

            if (response != null && response.IsSuccess)
            {
                // Tables'a atama yapıyoruz
                Tables = JsonConvert.DeserializeObject<List<TableNoDto>>(Convert.ToString(response.Result));
            }

            return View(Tables);
        }


        [Authorize]
        public async Task<IActionResult> Masa(int MasaId)
        {
            IEnumerable<TableNoDto> Tables = new List<TableNoDto>();
            ResponseDto? response = await _orderService.GetTables();

            if (response != null && response.IsSuccess)
            {
                // Tables'a atama yapıyoruz
                Tables = JsonConvert.DeserializeObject<List<TableNoDto>>(Convert.ToString(response.Result));
            }

            return View(Tables);
        }

        [HttpPost]
        public async Task<IActionResult> OdemeAl(OrderHeaderDto OrderHeaderDto)
        {
            var response = await _orderService.OdemeAlForUser(OrderHeaderDto);
            if (response != null && response.IsSuccess)
            {
                var orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
                TempData["Success"] = "Odeme Alindi";

                return RedirectToAction(nameof(OrderDetail), new { orderId = OrderHeaderDto.OrderHeaderId });

            }
            return View();

            
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentVm paymentVm)
        {
            paymentVm.TypeOfPayment = "normal";
            // Ödeme işlemini gerçekleştirin ve yanıtı alın
            var response = await _orderService.ProcessPayment(paymentVm);

            // Yanıtın başarı durumunu kontrol edin
            if (response != null && response.IsSuccess)
            {
                // Başarılı durum yanıtı döndürün
                return RedirectToAction(nameof(TableDetails), new { id = paymentVm.TableId });
            }

            // Başarısız durum yanıtı döndürün
            return Json(new
            {
                success = false,
                message = response?.Message ?? "Ödeme alınırken bir hata oluştu."
            });
        }


        [HttpPost]
        public async Task<IActionResult> ProcessPaymentCredit(PaymentVm paymentVm)
        {
            paymentVm.TypeOfPayment = "krediKarti";
            // Ödeme işlemini gerçekleştirin ve yanıtı alın
            var response = await _orderService.ProcessPayment(paymentVm);

            // Yanıtın başarı durumunu kontrol edin
            if (response != null && response.IsSuccess)
            {
                // Başarılı durum yanıtı döndürün
                return RedirectToAction(nameof(TableDetails), new { id = paymentVm.TableId });
            }

            // Başarısız durum yanıtı döndürün
            return Json(new
            {
                success = false,
                message = response?.Message ?? "Ödeme alınırken bir hata oluştu."
            });
        }



        [HttpPost]
        public async Task<IActionResult> ProcessPaymentCash(PaymentVm paymentVm)
        {
            paymentVm.TypeOfPayment = "cash";
            // Ödeme işlemini gerçekleştirin ve yanıtı alın
            var response = await _orderService.ProcessPayment(paymentVm);

            // Yanıtın başarı durumunu kontrol edin
            if (response != null && response.IsSuccess)
            {
                // Başarılı durum yanıtı döndürün
                return RedirectToAction(nameof(TableDetails), new { id = paymentVm.TableId });
            }

            // Başarısız durum yanıtı döndürün
            return Json(new
            {
                success = false,
                message = response?.Message ?? "Ödeme alınırken bir hata oluştu."
            });
        }


        [HttpPost]
        public async Task<IActionResult> CloseTable([FromBody] TableCloseRequest request)
        {
            var response = await _orderService.CloseTable(request);
            if (response != null && response.IsSuccess)
            {
                return Json(new
                {
                    success = true,
                    message = "Masa başarıyla değiştirildi"
                });
            }

            return Json(new { success = false, message = response.Result });
        }

        

        [HttpPost]
        public async Task<IActionResult> ChangeTable([FromBody] ChangeTableRequest request)
        {
            var response = await _orderService.ChangeTable(request);
            if (response != null && response.IsSuccess)
            {
                return Json(new
                {
                    success = true,
                    message = "Masa başarıyla değiştirildi",
                    newTableId = request.NewTableId
                });
            }

            return Json(new { success = false, message = "Masa değiştirilemedi" });
        }


        
        //degistirilcek
        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            
            var response = await _orderService.UpdateOrderStatus(orderId,SD.PaymentStatusApproved);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status Updated successfully";
                return RedirectToAction(nameof(OrderDetail) , new {orderId = orderId});
            }
            return View();
        }


        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {

            var response = await _orderService.UpdateOrderStatus(orderId, SD.StatusShipped);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status Updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(PaymentVm paymentVm)
        {
            // Ödeme işlemini gerçekleştirin ve yanıtı alın
            var response = await _orderService.CancelOrder(paymentVm);
            if (response != null && response.IsSuccess)
            {
                // Başarılı durum yanıtı döndürün
                return RedirectToAction(nameof(TableDetails), new { id = paymentVm.TableId });
            }

            // Başarısız durum yanıtı döndürün
            return Json(new
            {
                success = false,
                message = response?.Message ?? "Ödeme alınırken bir hata oluştu."
            });
        }



        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeaderDto> list;
            var userId = "";
            if (!User.IsInRole(SD.RoleAdmin))
            {
                userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            
            ResponseDto response = _orderService.GetAllOrder(userId).GetAwaiter().GetResult();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(response.Result));
                switch (status)
                {
                    case "approved":
                        list = list.Where(u=>u.PaymentStatus==SD.PaymentStatusApproved);
                        break;
                    case "creditcart":
                        list = list.Where(u => u.PaymentStatus == SD.StatusCreditCart);
                        break;
                    case "cash":
                        list = list.Where(u => u.PaymentStatus == SD.StatusCash);
                        break;
                    case "pending":
                        list = list.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
                        break;

                    case "cancelled":
                        list = list.Where(u => u.PaymentStatus == SD.StatusCancelled);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                list = new List<OrderHeaderDto>(); 
            }
            return Json(new {data = list});  

        }
    }
}
