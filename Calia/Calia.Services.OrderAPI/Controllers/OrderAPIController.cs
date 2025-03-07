using AutoMapper;
using Calia.Services.OrderAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Calia.Services.OrderAPI.Data;
using Calia.Services.OrderAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Calia.Services.OrderAPI.Utility;
using Calia.Services.OrderAPI.Models;
using Stripe;
using Stripe.Checkout;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using System.Drawing.Printing;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System.IdentityModel.Tokens.Jwt;
using Calia.Services.OrderAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using Calia.Services.OrderAPI.Migrations;
namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _db;

        private ResponseDto _response;
        private readonly IHubContext<TableHub> _tableHub;
        private IMapper _mapper;
        public IConfiguration _configuration;
        public IPrintNodeService _printNodeService;
        private IProductService _productService;
        private IAuthService _authService;
		private IStockService _stockService;
		private readonly IWebHostEnvironment _environment;
        private readonly IOptions<Calia.Services.OrderAPI.Utility.PrinterSettings> _printerSettings;
        public OrderAPIController(IOptions<Calia.Services.OrderAPI.Utility.PrinterSettings> printerSettings,IHubContext<TableHub> tableHub ,IAuthService authService, AppDbContext db, IMapper mapper,IPrintNodeService printNodeService, IProductService productService, IStockService stockService, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _db = db;
            _tableHub = tableHub;
            _mapper = mapper;
            _configuration = configuration;
            _productService = productService;
            _printNodeService = printNodeService;
            _stockService = stockService;
            _environment = environment;
            _printerSettings = printerSettings;
            _authService = authService;
            _response = new ResponseDto();
        }


        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDto? Get(string? userId="")
        {
            try
            {
                IEnumerable<OrderHeader> objlist;
                if (User.IsInRole(SD.RoleAdmin)) {

                    objlist = _db.OrderHeaders.Include(u => u.OrderDetails).ThenInclude(u=>u.ProductExtrasList).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                else
                {
                    objlist = _db.OrderHeaders.Include(u => u.OrderDetails).ThenInclude(u => u.ProductExtrasList).Where(u=>u.UserId==userId).OrderByDescending(u => u.OrderHeaderId).ToList();

                }
                _response.Result = _mapper.Map<List<OrderHeaderDto>>(objlist);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                
            }   
            return _response;
        
        }

        [HttpGet("GetTables")]
        public ResponseDto? GetTables()
        {
            try
            {
                // Veritabanındaki tüm masaları liste olarak çekiyoruz
                IEnumerable<TableNo> objlist = _db.TableNos.ToList();

                // Listeyi DTO'ya map ediyoruz ve Response'ta result olarak döndürüyoruz
                _response.Result = _mapper.Map<List<TableNoDto>>(objlist);
                
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }



        [HttpPost("AddTable/{masa}")]
        public ActionResult<ResponseDto> AddTable(string masa)
        {
            if (string.IsNullOrWhiteSpace(masa))
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Masa numarası boş olamaz."
                });
            }

            var response = new ResponseDto();
            try
            {
                TableNo tableNo = new()
                {
                    MasaNo = masa
                };

                _db.TableNos.Add(tableNo);
                _db.SaveChanges();

                response.IsSuccess = true;
                response.Message = "Masa başarıyla eklendi.";
                return Ok(response); // 200 OK
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Hata: {ex.Message}";
                return StatusCode(500, response); // 500 Internal Server Error
            }
        }



        [HttpDelete("DeleteTable/{masaId}")]
        public ActionResult<ResponseDto> DeleteTable(int masaId)
        {
            var response = new ResponseDto();
            try
            {
                // Masa bulunup bulunmadığını kontrol et
                TableNo tableNo = _db.TableNos.FirstOrDefault(u => u.Id == masaId);
                if (tableNo == null)
                {
                    return NotFound(new ResponseDto // 404 Not Found
                    {
                        IsSuccess = false,
                        Message = "Masa bulunamadı."
                    });
                }

                // Masa sil
                _db.TableNos.Remove(tableNo);
                _db.SaveChanges();

                response.IsSuccess = true;
                response.Message = "Masa başarıyla silindi.";
                return Ok(response); // 200 OK
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Hata: {ex.Message}";
                return StatusCode(500, response); // 500 Internal Server Error
            }
        }



        [Authorize]
        [HttpGet("GetRapor")]
        public async Task<ResponseDto>? GetRapor()
        {
            try
            {
                Veriler veriler = _db.Veriler.FirstOrDefault();
                VerilerDto verilerDto = _mapper.Map<VerilerDto>(veriler);
                DateTime today = DateTime.Today;

                List<TableDetails> tableDetails = _db.tableDetails
                 .Include(u => u.tableNo)
                 .Include(u => u.OrderHeaders)
                     .ThenInclude(u => u.OrderDetails) // Include OrderDetails
                 .Include(u => u.OrderHeaders)
                     .ThenInclude(u => u.CancelDetails) // Include cancelDetails separately
                 .Where(t =>
                     (t.CloseTime.HasValue && t.CloseTime.Value.Date == today) ||
                     (t.OpenTime.HasValue && t.OpenTime.Value.Date == today)
                 )
                 .ToList();

                List<TableDetailsDto> tableDetailsDtos = _mapper.Map<List<TableDetailsDto>>(tableDetails);

                // Bugün kapanan masaların toplam kazançlarını hesaplayın
                var toplamKazanc = _db.tableDetails.Include(u => u.tableNo)
                    .Where(t => t.CloseTime.HasValue && t.CloseTime.Value.Date == today)
                    .GroupBy(t => t.tableNo.MasaNo) // Masaların isimlerine göre grupla
                    .ToDictionary(g => g.Key, g => g.Sum(t => t.AlinanFiyat ?? 0));

                List<SatilanUrunlerDto> UrununNeKadarSatildigi = new List<SatilanUrunlerDto>();
                List<VerilenIkramlarDto> UrununNeKadarİkramVerildigi = new List<VerilenIkramlarDto>();
                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();
                List<CancelDetailsDto> cancelOrderDetails = new List<CancelDetailsDto>();
                List<OrderDetailsDto> satilanUrunler = new List<OrderDetailsDto>();
                var GunlukGelirGiderRaporu = new Dictionary<string, double>()
            {
                { "Nakit", 0 },
                { "KrediKarti", 0 },
                { "Iskonto", 0 },
                { "Ikram", 0 },
                { "Giderler", 0 },
            };

                var gunlukGarsonSiparisSayisi = tableDetailsDtos
                .SelectMany(t => t.OrderHeaders)
                .Where(o => o.personTakingOrder != null) // Null olmayanları filtrele
                .GroupBy(o => o.personTakingOrder)
                .ToDictionary(g => g.Key, g => (double)(g.Sum(o => o.OrderDetails.Sum(d => d.OdemesiAlinmisCount) ?? 0)));

                foreach (var tableDetail in tableDetailsDtos)
                {
                    GunlukGelirGiderRaporu["Nakit"] += tableDetail.Nakit ?? 0;
                    GunlukGelirGiderRaporu["KrediKarti"] += tableDetail.KrediKarti ?? 0;
                    GunlukGelirGiderRaporu["Iskonto"] += tableDetail.Ikram ?? 0;
                    if (tableDetail.Iskonto != null)
                    {
                        GunlukGelirGiderRaporu["Ikram"] += tableDetail.Iskonto ?? 0;

                    }
                    foreach (var orderHeader in tableDetail.OrderHeaders)
                    {

                        if (orderHeader.CancelDetails != null)
                        {
                            foreach (var item in orderHeader.CancelDetails)
                            {
                                item.MasaNo = tableDetail.tableNo.MasaNo;
                            }
                            cancelOrderDetails.AddRange(orderHeader.CancelDetails);

                        }



                        foreach (var orderDetail in orderHeader.OrderDetails)
                        {
                            orderDetail.Product = productDtos.FirstOrDefault(u => u.ProductId == orderDetail.ProductId);
                            orderDetail.MasaNo = tableDetail.tableNo.MasaNo;

                            if (orderDetail.PaymentStatus == SD.PaymentStatusApproved)
                            {
                                satilanUrunler.Add(orderDetail);
                                var Urun = UrununNeKadarSatildigi.FirstOrDefault(u => u.ProductName == orderDetail.ProductName);
                                if (Urun == null)
                                {
                                    SatilanUrunlerDto satilanurun = new()
                                    {
                                        ProductName = orderDetail.ProductName,
                                        CategoryName = orderDetail.Product.Category.Name,
                                        Price = (orderDetail.Price * (double)orderDetail.OdemesiAlinmisCount),
                                        SatilmaAdedi = orderDetail.OdemesiAlinmisCount ?? 0
                                    };
                                    UrununNeKadarSatildigi.Add(satilanurun);
                                }
                                else
                                {
                                    Urun.Price += (orderDetail.Price * orderDetail.OdemesiAlinmisCount ?? 0);
                                    Urun.SatilmaAdedi += orderDetail.OdemesiAlinmisCount ?? 0;

                                }
                            }

                            if (orderDetail.PaymentStatus == SD.PaymentStatusIkram)
                            {
                                var Urun = UrununNeKadarİkramVerildigi.FirstOrDefault(u => u.ProductName == orderDetail.ProductName);
                                if (Urun == null)
                                {
                                    VerilenIkramlarDto verilenIkramlarDto = new()
                                    {
                                        ProductName = orderDetail.ProductName,
                                        CategoryName = orderDetail.Product.Category.Name,
                                        Price = (orderDetail.Price * (double)orderDetail.OdemesiAlinmisCount),
                                        IkramAdedi = orderDetail.OdemesiAlinmisCount ?? 0
                                    };
                                    UrununNeKadarİkramVerildigi.Add(verilenIkramlarDto);
                                }
                                else
                                {
                                    Urun.Price += (orderDetail.Price * orderDetail.OdemesiAlinmisCount ?? 0);
                                    Urun.IkramAdedi += orderDetail.OdemesiAlinmisCount ?? 0;

                                }
                            }


                        }
                    }
                }

                List<KisiselGider> kisiselGiders = _db.KisiselGiders.Where(t => t.GiderTarihi.HasValue && t.GiderTarihi.Value.Date == today).ToList();
                foreach (var gider in kisiselGiders)
                {
                    GunlukGelirGiderRaporu["Giderler"] += gider.AlinanPara ?? 0;
                }


                List<OrderDetailsDto> SatilanUrunlerDtos = _mapper.Map<List<OrderDetailsDto>>(satilanUrunler).Take(5).ToList();





                IEnumerable<StockMaterialDto> stockMaterialDtos = await _stockService.GetStockMaterialDtosAsync();

                foreach (var item in SatilanUrunlerDtos)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                }

                foreach (var item in cancelOrderDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                }

                List<ProductDto> KalanTatlilar = new List<ProductDto>();
                foreach (var product in productDtos)
                {
                    if (product.Category.ProductCount == true)
                    {
                        KalanTatlilar.Add(product);
                    }

                }
                KalanTatlilar = KalanTatlilar.Take(5).ToList();
                RaporVM raporVM = new()
                {
                    Veriler = verilerDto,
                    MasalarınGünlükRaporu = tableDetailsDtos.Take(6).ToList(),
                    MasalarınToplamKazancı = toplamKazanc.Take(8).ToDictionary(x => x.Key, x => x.Value),
                    İptaller = cancelOrderDetails.Take(6).ToList(),
                    GünlükGelirGiderRaporu = GunlukGelirGiderRaporu,
                    SatilanUrunler = SatilanUrunlerDtos.Take(6).ToList(),
                    Malzemeler = stockMaterialDtos.Take(6).ToList(),
                    KalanTatlılar = KalanTatlilar,
                    UrunBasinaSatilma = UrununNeKadarSatildigi.Take(6).ToList(),
                    IkramVerilenUrunler = UrununNeKadarİkramVerildigi.Take(6).ToList(),
                    GünlükGarsonRaporu = gunlukGarsonSiparisSayisi ?? new Dictionary<string, double>() // Null kontrolü

                };
                _response.Result = raporVM;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // burasi

        [Authorize]
        [HttpGet("GetGunSonu")]
        public async Task<ResponseDto>? GetGunSonu()    
        {
            try
            {
                DateTime yesterday = DateTime.Now.AddDays(-1).Date;
                Veriler veriler = _db.Veriler.FirstOrDefault();
                VerilerDto verilerDto = _mapper.Map<VerilerDto>(veriler);

                int gunNumarasi = (int)yesterday.DayOfWeek;
                gunNumarasi = (gunNumarasi) % 7;

                List<TableDetails> tableDetails = await _db.tableDetails
                   .Include(u => u.tableNo)
                   .Include(u => u.OrderHeaders)
                       .ThenInclude(u => u.OrderDetails)
                   .Include(u => u.OrderHeaders)
                       .ThenInclude(u => u.CancelDetails)
                   .Where(t => (t.CloseTime.HasValue && t.CloseTime.Value.Date == yesterday) || (t.OpenTime.HasValue && t.OpenTime.Value.Date == yesterday))
                   .ToListAsync();

                List<TableDetailsDto> tableDetailsDtos = _mapper.Map<List<TableDetailsDto>>(tableDetails);

                double SatilanUrunFiyati = 0;
                int ToplamSiparisSayisi = 0;
                double GunlukNakit = 0;
                double GunlukKrediKarti = 0;
                double GunlukIskonto = 0;
                double GunlukIkram = 0;
                double GunlukGider = 0;
                foreach (var tableDetail in tableDetailsDtos)
                {
                    GunlukNakit += tableDetail.Nakit ?? 0;
                    GunlukKrediKarti += tableDetail.KrediKarti ?? 0;
                    GunlukIskonto += tableDetail.Ikram ?? 0;
                    GunlukIkram += tableDetail.Iskonto ?? 0;
                    foreach (var orderHeader in tableDetail.OrderHeaders)
                    {
                        foreach (var orderDetail in orderHeader.OrderDetails)
                        {
                            SatilanUrunFiyati += (double)orderDetail.OdemesiAlinmisCount * orderDetail.Price;
                            ToplamSiparisSayisi += orderDetail.OdemesiAlinmisCount ?? 0;
                        }
                    }
                    
                }

                List<KisiselGider> kisiselGiders = _db.KisiselGiders
                   .Where(t => t.GiderTarihi.HasValue && t.GiderTarihi.Value.Date == yesterday)
                   .ToList();
                foreach (var gider in kisiselGiders)
                {
                    GunlukGider += gider.AlinanPara ?? 0;
                }

                
                GunSonuDto gunSonuDto = new()
                {
                    GununAdi = yesterday.ToString("dddd"),
                    ToplamKazanç = verilerDto.HaftalikNakit[gunNumarasi] + verilerDto.HaftalikKrediKarti[gunNumarasi],
                    SatilanUrunFiyati = SatilanUrunFiyati,
                    ToplamSiparisSayisi = ToplamSiparisSayisi,
                    GunlukNakit = verilerDto.HaftalikNakit[gunNumarasi],
                    GunlukKrediKarti = verilerDto.HaftalikKrediKarti[gunNumarasi],
                    GunlukGider = GunlukGider,
                    GunlukIkram = GunlukIkram,
                    GunlukIskonto = GunlukIskonto,
                    GununTarih = yesterday,
                };
                _response.Result = gunSonuDto;
                _response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("GunSonuPost")]
        public async Task<ResponseDto>? GunSonuPost([FromBody] GunSonuDto gunSonuDto)
        {
            try
            {
                DateTime yesterday = DateTime.Now.AddDays(-1).Date;
                

                GunSonlari gunSonlari = _db.GunSonlaris
                    .FirstOrDefault(g => g.GununTarih.Date == yesterday);

                Veriler veriler = _db.Veriler.FirstOrDefault();

                if (gunSonlari == null)
                {
                    
                    gunSonlari = new GunSonlari
                    {
                        GununTarih = yesterday,
                        GunlukNakit = gunSonuDto.GunlukNakit ?? 0 ,
                        GunlukKrediKarti = gunSonuDto.GunlukKrediKarti ?? 0,
                        GunlukIskonto = gunSonuDto.GunlukIskonto ?? 0,
                        GunlukIkram = gunSonuDto.GunlukIkram ?? 0,
                        GunlukGider = gunSonuDto.GunlukGider ?? 0
                    };

                    await _db.GunSonlaris.AddAsync(gunSonlari);
                }
                else
                {
                    // If the record exists, update its fields
                    gunSonlari.GunlukNakit = gunSonuDto.GunlukNakit ?? 0;
                    gunSonlari.GunlukKrediKarti = gunSonuDto.GunlukKrediKarti ?? 0;
                    gunSonlari.GunlukIskonto = gunSonuDto.GunlukIskonto ?? 0;
                    gunSonlari.GunlukIkram = gunSonuDto.GunlukIkram ?? 0;
                    gunSonlari.GunlukGider = gunSonuDto.GunlukGider ?? 0;

                    _db.GunSonlaris.Update(gunSonlari);
                }

                int gunNumarasi = (int)yesterday.DayOfWeek;
                gunNumarasi = (gunNumarasi) % 7;

                double haftalikNakit = veriler.HaftalikNakit[gunNumarasi];
                double HaftalikKrediKarti = veriler.HaftalikKrediKarti[gunNumarasi];

                veriler.HaftalikNakit[gunNumarasi] -= haftalikNakit;
                veriler.HaftalikNakit[gunNumarasi] += gunSonuDto.GunlukNakit ?? 0;


                veriler.HaftalikKrediKarti[gunNumarasi] -= HaftalikKrediKarti;
                veriler.HaftalikKrediKarti[gunNumarasi] += gunSonuDto.GunlukKrediKarti ?? 0;


                int ayNumarasi = yesterday.Month - 1;
                veriler.AylikNakit[ayNumarasi] -= haftalikNakit;
                veriler.AylikNakit[ayNumarasi] += gunSonuDto.GunlukNakit ?? 0;

                veriler.AylikKrediKarti[ayNumarasi] -= HaftalikKrediKarti;
                veriler.AylikKrediKarti[ayNumarasi] += gunSonuDto.GunlukKrediKarti ?? 0;

                _db.Veriler.Update(veriler);

                // Save changes to the database
                await _db.SaveChangesAsync();

                // Prepare a successful response
                _response.IsSuccess = true;
                _response.Result = gunSonuDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }




        [Authorize]
        [HttpGet("TumunuGor")]
        public async Task<ResponseDto> TumunuGor()
        {
            try
            {
                Veriler veriler = _db.Veriler.FirstOrDefault();
                VerilerDto verilerDto = _mapper.Map<VerilerDto>(veriler);
                DateTime today = DateTime.Today;

                List<TableDetails> tableDetails = _db.tableDetails
                 .Include(u => u.tableNo)
                 .Include(u => u.OrderHeaders)
                     .ThenInclude(u => u.OrderDetails) // Include OrderDetails
                 .Include(u => u.OrderHeaders)
                     .ThenInclude(u => u.CancelDetails) // Include cancelDetails separately
                 .Where(t =>
                     (t.CloseTime.HasValue && t.CloseTime.Value.Date == today) ||
                     (t.OpenTime.HasValue && t.OpenTime.Value.Date == today)
                 )
                 .ToList();

                List<TableDetailsDto> tableDetailsDtos = _mapper.Map<List<TableDetailsDto>>(tableDetails);

                // Bugün kapanan masaların toplam kazançlarını hesaplayın
                var toplamKazanc = _db.tableDetails.Include(u => u.tableNo)
                    .Where(t => t.CloseTime.HasValue && t.CloseTime.Value.Date == today)
                    .GroupBy(t => t.tableNo.MasaNo) // Masaların isimlerine göre grupla
                    .ToDictionary(g => g.Key, g => g.Sum(t => t.AlinanFiyat ?? 0));

                List<SatilanUrunlerDto> UrununNeKadarSatildigi = new List<SatilanUrunlerDto>();
                List<VerilenIkramlarDto> UrununNeKadarİkramVerildigi = new List<VerilenIkramlarDto>();
                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();
                List<CancelDetailsDto> cancelOrderDetails = new List<CancelDetailsDto>();
                List<OrderDetailsDto> satilanUrunler = new List<OrderDetailsDto>();
                var GunlukGelirGiderRaporu = new Dictionary<string, double>()
            {
                { "Nakit", 0 },
                { "KrediKarti", 0 },
                { "Iskonto", 0 },
                { "Ikram", 0 },
                { "Giderler", 0 },
            };

                var gunlukGarsonSiparisSayisi = tableDetailsDtos
                .SelectMany(t => t.OrderHeaders)
                .Where(o => o.personTakingOrder != null) // Null olmayanları filtrele
                .GroupBy(o => o.personTakingOrder)
                .ToDictionary(g => g.Key, g => (double)(g.Sum(o => o.OrderDetails.Sum(d => d.OdemesiAlinmisCount) ?? 0)));

                foreach (var tableDetail in tableDetailsDtos)
                {
                    GunlukGelirGiderRaporu["Nakit"] += tableDetail.Nakit ?? 0;
                    GunlukGelirGiderRaporu["KrediKarti"] += tableDetail.KrediKarti ?? 0;
                    GunlukGelirGiderRaporu["Iskonto"] += tableDetail.Ikram ?? 0;
                    if (tableDetail.Iskonto != null)
                    {
                        GunlukGelirGiderRaporu["Ikram"] += tableDetail.Iskonto ?? 0;

                    }
                    foreach (var orderHeader in tableDetail.OrderHeaders)
                    {

                        if (orderHeader.CancelDetails != null)
                        {
                            foreach (var item in orderHeader.CancelDetails)
                            {
                                item.MasaNo = tableDetail.tableNo.MasaNo;
                            }
                            cancelOrderDetails.AddRange(orderHeader.CancelDetails);

                        }



                        foreach (var orderDetail in orderHeader.OrderDetails)
                        {
                            orderDetail.Product = productDtos.FirstOrDefault(u => u.ProductId == orderDetail.ProductId);
                            orderDetail.MasaNo = tableDetail.tableNo.MasaNo;

                            if (orderDetail.PaymentStatus == SD.PaymentStatusApproved)
                            {
                                satilanUrunler.Add(orderDetail);
                                var Urun = UrununNeKadarSatildigi.FirstOrDefault(u => u.ProductName == orderDetail.ProductName);
                                if (Urun == null)
                                {
                                    SatilanUrunlerDto satilanurun = new()
                                    {
                                        ProductName = orderDetail.ProductName,
                                        CategoryName = orderDetail.Product.Category.Name,
                                        Price = (orderDetail.Price * (double)orderDetail.OdemesiAlinmisCount),
                                        SatilmaAdedi = orderDetail.OdemesiAlinmisCount ?? 0
                                    };
                                    UrununNeKadarSatildigi.Add(satilanurun);
                                }
                                else
                                {
                                    Urun.Price += (orderDetail.Price * orderDetail.OdemesiAlinmisCount ?? 0);
                                    Urun.SatilmaAdedi += orderDetail.OdemesiAlinmisCount ?? 0;

                                }
                            }

                            if (orderDetail.PaymentStatus == SD.PaymentStatusIkram)
                            {
                                var Urun = UrununNeKadarİkramVerildigi.FirstOrDefault(u => u.ProductName == orderDetail.ProductName);
                                if (Urun == null)
                                {
                                    VerilenIkramlarDto verilenIkramlarDto = new()
                                    {
                                        ProductName = orderDetail.ProductName,
                                        CategoryName = orderDetail.Product.Category.Name,
                                        Price = (orderDetail.Price * (double)orderDetail.OdemesiAlinmisCount),
                                        IkramAdedi = orderDetail.OdemesiAlinmisCount ?? 0
                                    };
                                    UrununNeKadarİkramVerildigi.Add(verilenIkramlarDto);
                                }
                                else
                                {
                                    Urun.Price += (orderDetail.Price * orderDetail.OdemesiAlinmisCount ?? 0);
                                    Urun.IkramAdedi += orderDetail.OdemesiAlinmisCount ?? 0;

                                }
                            }


                        }
                    }
                }

                List<KisiselGider> kisiselGiders = _db.KisiselGiders.Where(t => t.GiderTarihi.HasValue && t.GiderTarihi.Value.Date == today).ToList();
                foreach (var gider in kisiselGiders)
                {
                    GunlukGelirGiderRaporu["Giderler"] += gider.AlinanPara ?? 0;
                }


                List<OrderDetailsDto> SatilanUrunlerDtos = _mapper.Map<List<OrderDetailsDto>>(satilanUrunler);





                IEnumerable<StockMaterialDto> stockMaterialDtos = await _stockService.GetStockMaterialDtosAsync();

                foreach (var item in SatilanUrunlerDtos)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                }

                foreach (var item in cancelOrderDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                }

                List<ProductDto> KalanTatlilar = new List<ProductDto>();
                foreach (var product in productDtos)
                {
                    if (product.Category.ProductCount == true)
                    {
                        KalanTatlilar.Add(product);
                    }

                }
                
                RaporVM raporVM = new()
                {
                    Veriler = verilerDto,
                    MasalarınGünlükRaporu = tableDetailsDtos,
                    MasalarınToplamKazancı = toplamKazanc,
                    İptaller = cancelOrderDetails,
                    GünlükGelirGiderRaporu = GunlukGelirGiderRaporu,
                    SatilanUrunler = SatilanUrunlerDtos,
                    Malzemeler = stockMaterialDtos,
                    KalanTatlılar = KalanTatlilar,
                    UrunBasinaSatilma = UrununNeKadarSatildigi,
                    IkramVerilenUrunler = UrununNeKadarİkramVerildigi,
                    GünlükGarsonRaporu = gunlukGarsonSiparisSayisi ?? new Dictionary<string, double>() // Null kontrolü

                };
                _response.Result = raporVM;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }




        //surasi
        [Authorize]
        [HttpGet("GetTableDetails/{id:int}")]
        public async Task<ResponseDto>? GetTableDetails(int id)
        {
            try
            {
                TableDetails tableDetails = _db.tableDetails.Include(u=>u.tableNo).Include(u=>u.OrderHeaders).ThenInclude(u => u.OrderDetails).ThenInclude(u=>u.ProductExtrasList).FirstOrDefault(u=>u.TableId ==id && u.isClosed == false);
                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

                foreach (var item in tableDetails.OrderHeaders)
                {
                    foreach (var detail in item.OrderDetails)
                    {
                        detail.Product = productDtos.FirstOrDefault(u => u.ProductId == detail.ProductId);
                    }
                    

                }
                _response.Result = _mapper.Map<TableDetailsDto>(tableDetails);
                
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }



        [Authorize]
        [HttpGet("GetTablewithId/{id:int}")]
        public async Task<ResponseDto>? GetTablewithId(int id)
        {
            try
            {
                TableDetails tableDetails = _db.tableDetails.Include(u => u.tableNo).Include(u => u.OrderHeaders).ThenInclude(u => u.OrderDetails).ThenInclude(u => u.ProductExtrasList).FirstOrDefault(u => u.Id == id );
                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

                foreach (var item in tableDetails.OrderHeaders)
                {
                    foreach (var detail in item.OrderDetails)
                    {
                        detail.Product = productDtos.FirstOrDefault(u => u.ProductId == detail.ProductId);
                    }


                }
                _response.Result = _mapper.Map<TableDetailsDto>(tableDetails);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }




        [Authorize]
        [HttpGet("GetAdmins")]
        public ResponseDto? GetAdmins()
        {
            try
            {
                
                IEnumerable<AdminNames> objlist = _db.AdminNames.ToList();

                
                _response.Result = _mapper.Map<List<AdminNamesDto>>(objlist);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("CreateKisiselGider")]
        public async Task<ResponseDto>? CreateKisiselGider([FromBody] KisiselGiderDto kisiselGiderDto)
        {
            try
            {
                // DTO'dan KisiselGider modeline dönüştürme
                KisiselGider kisiselGider = new KisiselGider()
                {
                    AdminId = kisiselGiderDto.AdminId,
                    AlinanPara = kisiselGiderDto.AlinanPara,
                    Reason = kisiselGiderDto.Reason,
                    GiderTarihi = DateTime.Now,
                };

                // Veritabanına ekle
                await _db.KisiselGiders.AddAsync(kisiselGider);
                await _db.SaveChangesAsync();

                // Response DTO'ya geri dönüştür
                KisiselGiderDto createdGiderDto = new KisiselGiderDto()
                {
                    Id = kisiselGider.Id,
                    AdminId = kisiselGider.AdminId,
                    AdminName = kisiselGider.AdminName,
                    AlinanPara = kisiselGider.AlinanPara,
                    Reason = kisiselGider.Reason,
                };

                _response.Result = createdGiderDto;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }



        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public async Task<ResponseDto>? Get(int id)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.Include(u => u.OrderDetails).ThenInclude(u => u.ProductExtrasList).First(u=>u.OrderHeaderId == id);
                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();
                foreach (var item in orderHeader.OrderDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                }
                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;

        }

		[Authorize]
		[HttpGet("GetPrinters")]
		public async Task<ResponseDto> GetPrinters()
		{
			try
			{
				var printer = _printNodeService.GetPrintersAsync();
                _response.Result = printer;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}
        [Authorize]
        [HttpPost("ChangePrinter")]
        public async Task<ResponseDto> ChangePrinter([FromBody] string printerId)
        {
            try
            {
				var filePath = Path.Combine(_environment.ContentRootPath, "appsettings.json");
				var jsonConfig = System.IO.File.ReadAllText(filePath);
				dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonConfig);

				jsonObj["PrinterSettings"]["PrinterId"] = printerId;

				string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
				System.IO.File.WriteAllText(filePath, output);
                _response.Result = $"Created {printerId}";
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;

		}


        [Authorize(Roles =SD.RoleAdmin)]
        [HttpPost("Ikram")]
        public async Task<ResponseDto> Ikram([FromBody] CartDto cartDto)
        {
            try
            {
                // Map cart header to order header DTO and set additional properties
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.PaymentStatus = SD.PaymentStatusIkram;
                // Add and save OrderHeader to get its ID
                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                List<OrderDetails> orderDetailsList = new List<OrderDetails>();

                foreach (var item in cartDto.CartDetails)
                {
                    // Create OrderDetails and initialize ProductExtrasList
                    OrderDetails orderDetail = new()
                    {
                        OrderHeaderId = orderCreated.OrderHeaderId,
                        ProductId = item.ProductId,
                        OdemesiAlinmisCount = item.Count,
                        ProductName = item.Product.Name,
                        Price = item.DetailPrice,
                        ProductExtrasList = new List<OrderExtra>(),
                        PaymentStatus = SD.PaymentStatusIkram,
                        
                    };

                    // Add extras associated with the current order detail
                    foreach (var extras in item.ProductExtrasList)
                    {
                        OrderExtra orderExtra = new()
                        {
                            ExtraName = extras.ExtraName,
                            Price = extras.Price
                        };
                        orderDetail.ProductExtrasList.Add(orderExtra); // Add to the OrderDetails' extras list
                    }

                    orderDetailsList.Add(orderDetail); // Add to the overall OrderDetails list
                }

                // Save all OrderDetails and related extras in a single operation
                _db.OrderDetails.AddRange(orderDetailsList);

                // Collect all extras from all order details to save them in bulk
                var allOrderExtras = orderDetailsList.SelectMany(od => od.ProductExtrasList).ToList();
                _db.OrderExtras.AddRange(allOrderExtras);
                double iskonto = 0; 
                foreach (var orderDetails in orderDetailsList)
                {
                    iskonto += orderDetails.Price * (double)orderDetails.OdemesiAlinmisCount;
                }


                // Save everything to the database in a single SaveChangesAsync call
                await _db.SaveChangesAsync();

                // Return the response with the created OrderHeaderId
                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                cartDto.CartHeader.orderId = orderCreated.OrderHeaderId;
                var table = _db.TableNos.FirstOrDefault(x => x.MasaNo == cartDto.CartHeader.MasaNo);
                bool IsTableOccupied = table.IsOccupied;
                if (IsTableOccupied)
                {
                    var tableDetail = _db.tableDetails.FirstOrDefault(u => u.TableId == table.Id && u.isClosed == false);
                    tableDetail.OrderHeaders = new List<OrderHeader>();
                    tableDetail.OrderHeaders.Add(orderCreated);
                    if (tableDetail.Iskonto == null)
                    {
                        tableDetail.Iskonto = iskonto;
                    }
                    else
                    {
                        tableDetail.Iskonto += iskonto;
                    }
                    _db.tableDetails.Update(tableDetail);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    table.IsOccupied = true;
                    _db.TableNos.Update(table);
                    await _db.SaveChangesAsync();
                    TableDetails tableDetails = new()
                    {
                        TableId = table.Id,
                        PaymentStatus = SD.PaymentStatusPending,
                        OpenTime = DateTime.Now,
                        isClosed = false,
                    };
                    if (tableDetails.Iskonto == null)
                    {
                        tableDetails.Iskonto = iskonto;
                    }
                    else
                    {
                        tableDetails.Iskonto += iskonto;
                    }
                    tableDetails.OrderHeaders = new List<OrderHeader>();
                    tableDetails.OrderHeaders.Add(orderCreated);
                    _db.tableDetails.Add(tableDetails);
                    await _db.SaveChangesAsync();
                }

                IEnumerable<TableNo> objlist = _db.TableNos.ToList();
                // Listeyi DTO'ya map ediyoruz ve Response'ta result olarak döndürüyoruz
                var Result = _mapper.Map<List<TableNoDto>>(objlist);

                await _tableHub.Clients.All.SendAsync("ReceiveTableStatusUpdate", Result);

                await PrintOrderAsync(orderCreated.OrderHeaderId);

                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;

            return _response;
        }




        [HttpPost("SiparisVer")]
        public async Task<ResponseDto> SiparisVer([FromBody] CartDto cartDto)
        {
            try
            {
                // Map cart header to order header DTO and set additional properties
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.PaymentStatus = SD.PaymentStatusPending;
                if (User.IsInRole(SD.RoleWaiter))
                {
                    var personTakingOrder = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name)?.Value;
                    orderHeaderDto.personTakingOrder = personTakingOrder;
                }
                else
                {
                    orderHeaderDto.personTakingOrder = "Sistem";
                }
                // Add and save OrderHeader to get its ID
                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                List<OrderDetails> orderDetailsList = new List<OrderDetails>();

                foreach (var item in cartDto.CartDetails)
                {
                    // Create OrderDetails and initialize ProductExtrasList
                    OrderDetails orderDetail = new()
                    {
                        OrderHeaderId = orderCreated.OrderHeaderId,
                        ProductId = item.ProductId,
                        Count = item.Count,
                        ProductName = item.Product.Name,
                        Price = item.DetailPrice,
                        ProductExtrasList = new List<OrderExtra>()
                    };

                    // Add extras associated with the current order detail
                    foreach (var extras in item.ProductExtrasList)
                    {
                        OrderExtra orderExtra = new()
                        {
                            ExtraName = extras.ExtraName,
                            Price = extras.Price
                        };
                        orderDetail.ProductExtrasList.Add(orderExtra); // Add to the OrderDetails' extras list
                    }

                    orderDetailsList.Add(orderDetail); // Add to the overall OrderDetails list
                }

                // Save all OrderDetails and related extras in a single operation
                _db.OrderDetails.AddRange(orderDetailsList);

                // Collect all extras from all order details to save them in bulk
                var allOrderExtras = orderDetailsList.SelectMany(od => od.ProductExtrasList).ToList();
                _db.OrderExtras.AddRange(allOrderExtras);

                // Save everything to the database in a single SaveChangesAsync call
                await _db.SaveChangesAsync();

                // Return the response with the created OrderHeaderId
                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                cartDto.CartHeader.orderId = orderCreated.OrderHeaderId;
                var table = _db.TableNos.FirstOrDefault(x => x.MasaNo == cartDto.CartHeader.MasaNo);
                bool IsTableOccupied = table.IsOccupied;
                if (IsTableOccupied)
                {
                    var tableDetail = _db.tableDetails.FirstOrDefault(u => u.TableId == table.Id && u.isClosed == false);
                    tableDetail.OrderHeaders = new List<OrderHeader>();
                    tableDetail.OrderHeaders.Add(orderCreated);
                    tableDetail.TotalTable += orderCreated.OrderTotal;
                    _db.tableDetails.Update(tableDetail);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    table.IsOccupied = true;
                    _db.TableNos.Update(table);
                    await _db.SaveChangesAsync();
                    TableDetails tableDetails = new()
                    {
                        TableId = table.Id,
                        PaymentStatus = SD.PaymentStatusPending,
                        OpenTime = DateTime.Now,
                        isClosed = false,
                    };
                    tableDetails.OrderHeaders = new List<OrderHeader>();
                    tableDetails.OrderHeaders.Add(orderCreated);
                    tableDetails.TotalTable = orderCreated.OrderTotal;
                    _db.tableDetails.Add(tableDetails);
                    await _db.SaveChangesAsync();

                }

                IEnumerable<TableNo> objlist = _db.TableNos.ToList();

                // Listeyi DTO'ya map ediyoruz ve Response'ta result olarak döndürüyoruz
                var Result = _mapper.Map<List<TableNoDto>>(objlist);

                await _tableHub.Clients.All.SendAsync("ReceiveTableStatusUpdate", Result);


                await PrintOrderAsync(orderCreated.OrderHeaderId);

                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }



        [HttpPut("TakePayment")]
        public async Task<ResponseDto> TakePayment([FromBody] OrderHeaderDto orderHeaderDto)
        {
            try
            {

                OrderHeader? orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.OrderHeaderId == orderHeaderDto.OrderHeaderId);

                if (orderHeaderFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "OrderHeader bulunamadı.";
                    return _response;
                }

                orderHeaderFromDb.Nakit = orderHeaderDto.Nakit;
                orderHeaderFromDb.KrediKarti = orderHeaderDto.KrediKarti;
                orderHeaderFromDb.Ikram = orderHeaderDto.Ikram;
                orderHeaderFromDb.PaymentStatus = SD.PaymentStatusApproved;
                orderHeaderFromDb.CloseTime = DateTime.Now;

                _db.OrderHeaders.Update(orderHeaderFromDb);
                await _db.SaveChangesAsync();

                List<OrderDetails> orderDetails = _db.OrderDetails.Where(u => u.OrderHeaderId == orderHeaderDto.OrderHeaderId).ToList();
                if (!orderDetails.Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "OrderDetails bulunamadı.";
                    return _response;
                }

                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.isPaid = true;
                    orderDetail.PaymentStatus = SD.PaymentStatusApproved;
                }

                _db.OrderDetails.UpdateRange(orderDetails);
                await _db.SaveChangesAsync();

                _response.Result = "Ödeme Başarılı";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }





        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
				OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
				if (stripeRequestDto.PaymentMethod == SD.StatusCash)
                {
                    orderHeader.PaymentStatus = SD.StatusCash;
                    orderHeader.OrderStatus = SD.StatusPending;
                }
                else if(stripeRequestDto.PaymentMethod == SD.StatusWaiterOrder) {
                    orderHeader.PaymentStatus= SD.StatusWaiterOrder;
                    orderHeader.OrderStatus= SD.StatusPending;
                }
                else if(stripeRequestDto.PaymentMethod == SD.StatusCreditCart) {
					var options = new Stripe.Checkout.SessionCreateOptions
					{
						SuccessUrl = stripeRequestDto.ApprovedUrl,
						CancelUrl = stripeRequestDto.CancelUrl,
						LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
						Mode = "payment",

					};
					foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
					{
						var sessionLineItem = new SessionLineItemOptions
						{
							PriceData = new SessionLineItemPriceDataOptions
							{
								UnitAmount = (long)(item.Price * 100),
								Currency = "usd",
								ProductData = new SessionLineItemPriceDataProductDataOptions
								{
									Name = item.Product.Name
								}
							},
							Quantity = item.Count
						};
						options.LineItems.Add(sessionLineItem);
					}


					var service = new Stripe.Checkout.SessionService();
					Session session = service.Create(options);
					stripeRequestDto.StripeSessionUrl = session.Url;
					
					orderHeader.StripeSessionId = session.Id;
					_db.SaveChanges();
					_response.Result = stripeRequestDto;
				}
                
                
            }
            catch (Exception ex)
            {
                _response.IsSuccess=false;
                _response.Message = ex.Message;
            }
            return _response;


            
        }



        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);
                //buraya dokunalacak
                var service = new Stripe.Checkout.SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
                if (paymentIntent.Status == "succeeded") {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    orderHeader.OrderStatus = SD.StatusPending;
                    _db.SaveChanges();
                    
                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);

                }
                var orderDetails = _db.OrderHeaders.Include(u => u.OrderDetails).FirstOrDefault(u=>u.OrderHeaderId==orderHeaderId).OrderDetails;
                foreach (var item in orderDetails)
                {
                    var itemDto = _mapper.Map<OrderDetailsDto>(item);
                    ReduceStock(itemDto);

				}
                await PrintOrderAsync(orderHeaderId);





            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;



        }


        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u=>u.OrderHeaderId == orderId);
                if (orderHeader !=null)
                {
                    if (newStatus == SD.StatusCancelled)
                    {
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId,

                        };

                        var service = new RefundService();
                        Refund refund = service.Create(options);
                        orderHeader.PaymentStatus = newStatus;
                    }
                    orderHeader.PaymentStatus = newStatus;
                    _db.SaveChanges();
                        
                }
                
                

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;



        }


        
        


        private void ReduceStock(OrderDetailsDto orderDetail)
        {
            if (orderDetail.Product != null && orderDetail.Product.ProductMaterials != null)
            {
                // Ürüne ait malzemeleri dolaş ve stoklarını düşür
                var productMaterials = orderDetail.Product.ProductMaterials;
                foreach (var material in productMaterials)
                {
                    string materialName = material.MaterialName;
                    int materialAmount = (int)(material.Amount * orderDetail.OdemesiAlinmisCount);
                    _stockService.DecreaseStockMaterial(materialName, materialAmount);
                }
            }
            if (orderDetail.Product.Category.ProductCount && orderDetail.Product.AvailableProducts > orderDetail.OdemesiAlinmisCount)
            {
                _productService.DropAvailableProduct(orderDetail);
            }
            
        }


        private async Task PrintOrderAsync(int orderHeaderId)
        {
            string wwwRootPath = _environment.WebRootPath;
            string textFilesPath = Path.Combine(wwwRootPath, "textfiles", "order");

            if (!Directory.Exists(textFilesPath))
            {
                Directory.CreateDirectory(textFilesPath);
            }

            var stOrderheaderId = orderHeaderId.ToString();
            OrderHeader orderHeader = await _db.OrderHeaders
                .Include(u => u.OrderDetails)
                .ThenInclude(u => u.ProductExtrasList)
                .FirstAsync(u => u.OrderHeaderId == orderHeaderId);

            string outputFilePath = Path.Combine(textFilesPath, $"{stOrderheaderId}.txt");
            string text = $"{orderHeader.MasaNo}\n---------------------------------\n";
            var orderDetails = orderHeader.OrderDetails;

            foreach (var orderDetail in orderDetails)
            {
                text += $"{orderDetail.ProductName.ToUpper()} X{orderDetail.Count} \n";
                foreach (var extras in orderDetail.ProductExtrasList)
                {
                    text += $"  * {extras.ExtraName}\n";
                }
            }

            // Sipariş metninin altına boşluk eklemek
            for (int i = 0; i < 4; i++)
            {
                text += "\n";
            }

            await System.IO.File.WriteAllTextAsync(outputFilePath, text);
            await _printNodeService.PrintFileAsync(_printerSettings.Value.PrinterId, outputFilePath);
        }
        private void PrintTable(int tableId)
        {
            string wwwRootPath = _environment.WebRootPath;
            string textFilesPath = Path.Combine(wwwRootPath, "textfiles", "table");

            if (!Directory.Exists(textFilesPath))
            {
                Directory.CreateDirectory(textFilesPath);
            }

            var sttableId = tableId.ToString();
            TableDetails tableDetails = _db.tableDetails
                 .Include(u => u.tableNo)
                 .Include(u => u.OrderHeaders)
                     .ThenInclude(u => u.OrderDetails).ThenInclude(od => od.ProductExtrasList) // Include OrderDetails
                 .Include(u => u.OrderHeaders)
                     .ThenInclude(u => u.CancelDetails).ThenInclude(od => od.ProductExtrasList) // Include cancelDetails separately
                 .FirstOrDefault(u => u.TableId == tableId);


            string outputFilePath = Path.Combine(textFilesPath, $"{sttableId}.txt");
            string text = $"{tableDetails.tableNo.MasaNo}\n---------------------------------\n";
            int a = 0;
            bool odemesialinan = false;
            bool ikram = false;
            bool iptal = false;
            foreach (var orderHeader in tableDetails.OrderHeaders)
            {
                if (!odemesialinan)
                {
                    text += $"Odemesi Alinanlar: \n";
                    odemesialinan = true ;
                }

                foreach (var orderDetail in orderHeader.OrderDetails)
                {
                    if (orderDetail.PaymentStatus == SD.PaymentStatusApproved)
                    {
                        text += $"{orderDetail.ProductName.ToUpper()} X{orderDetail.OdemesiAlinmisCount} --- {orderDetail.OdemesiAlinmisCount * orderDetail.Price} \n";
                        foreach (var extras in orderDetail.ProductExtrasList)
                        {
                            text += $"  * {extras.ExtraName}\n";
                        }
                    }
                    else if (orderDetail.PaymentStatus == SD.PaymentStatusIkram)
                    {
                        a++;
                    }

                }
            }
            text += "\n";
            if (a > 0)
            {
                foreach (var orderHeader in tableDetails.OrderHeaders)
                {
                    if (!ikram)
                    {
                        text += $"Ikram Edilenler: \n";
                        ikram = true;
                    }
                    
                    foreach (var orderDetail in orderHeader.OrderDetails)
                    {
                        if (orderDetail.PaymentStatus == SD.PaymentStatusApproved)
                        {
                            text += $"{orderDetail.ProductName.ToUpper()} X{orderDetail.OdemesiAlinmisCount} --- {orderDetail.OdemesiAlinmisCount * orderDetail.Price} \n";
                            foreach (var extras in orderDetail.ProductExtrasList)
                            {
                                text += $"  * {extras.ExtraName}\n";
                            }
                        }

                    }
                }
            }
            text += "\n";
            foreach (var orderHeader in tableDetails.OrderHeaders)
            {
                if (orderHeader.CancelDetails != null)
                {
                    if (!iptal)
                    {
                        text += $"Iptal Edilenler: \n";
                        iptal = true;
                    }
                   
                    foreach (var cancelDetails in orderHeader.CancelDetails)
                    {
                        text += $"{cancelDetails.ProductName.ToUpper()} X{cancelDetails.Count} --- {cancelDetails.Count * cancelDetails.Price} \n";
                        foreach (var extras in cancelDetails.ProductExtrasList)
                        {
                            text += $"  * {extras.ExtraName}\n";
                        }
                    }
                }
            }




            text += "\n";
            text += $"Nakit =>{tableDetails.Nakit}\n\n" +
                $"Kredi Karti =>{tableDetails.KrediKarti}\n\n" +
                $"Iskonto =>{tableDetails.Ikram}\n\n" +
                $"Ikram =>{tableDetails.Iskonto}\n\n";

            // Sipariş metninin altına boşluk eklemek
            for (int i = 0; i < 4; i++)
            {
                text += "\n";
            }
            System.IO.File.WriteAllText(outputFilePath, text);

            _printNodeService.PrintFileAsync(_printerSettings.Value.PrinterId, outputFilePath).Wait();

        }



        [Authorize]
        [HttpPost("ChangeTable")]
        public async Task<ResponseDto> ChangeTable([FromBody] ChangeTableRequest changeTableRequest)
        {
            try
            {
                // Free the current table
                TableNo forcloseTable = _db.TableNos.FirstOrDefault(u => u.Id == changeTableRequest.CurrentTableId);
                if (forcloseTable != null)
                {
                    forcloseTable.IsOccupied = false;
                    _db.TableNos.Update(forcloseTable);
                }

                // Occupy the new table
                TableNo foropenTable = _db.TableNos.FirstOrDefault(u => u.Id == changeTableRequest.NewTableId);
                if (foropenTable != null)
                {
                    foropenTable.IsOccupied = true;
                    _db.TableNos.Update(foropenTable);
                }

                // Update the table details for the order
                TableDetails tableDetails = _db.tableDetails.Include(u => u.tableNo)
                 .Include(u => u.OrderHeaders)
                     .ThenInclude(u => u.OrderDetails) // Include OrderDetails
                 .Include(u => u.OrderHeaders)
                     .ThenInclude(u => u.CancelDetails).FirstOrDefault(u => u.TableId == changeTableRequest.CurrentTableId && u.isClosed == false);
                if (tableDetails != null)
                {
                    TableDetails changeTableDetails = _db.tableDetails.Include(u => u.tableNo)
                 .Include(u => u.OrderHeaders)
                     .ThenInclude(u => u.OrderDetails) // Include OrderDetails
                 .Include(u => u.OrderHeaders)
                     .ThenInclude(u => u.CancelDetails).FirstOrDefault(u => u.TableId == changeTableRequest.NewTableId && u.isClosed == false);
                    if (changeTableDetails != null)
                    {
                        changeTableDetails.OrderHeaders.AddRange(tableDetails.OrderHeaders);
                        changeTableDetails.TotalTable += tableDetails.TotalTable;
                        _db.tableDetails.Remove(tableDetails);

                    }
                    else
                    {
                        tableDetails.TableId = changeTableRequest.NewTableId;   
                        _db.tableDetails.Update(tableDetails);
                    }
                    
                    
                }

                // Save changes to the database
                await _db.SaveChangesAsync();
                _response.Result = "Table change successful";
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }


        [Authorize]
        [HttpPost("ProcessPayment")]
        public async Task<ResponseDto> ProcessPayment([FromBody] PaymentVm paymentVm)
        {
            try
            {
                if (paymentVm.SelectedOrderDetailsIds == null || paymentVm.SelectedOrderDetailCounts == null)
                {
                    var tableDetails = _db.tableDetails
                       .FirstOrDefault(u => u.TableId == paymentVm.TableId && u.isClosed == false);

                    if (tableDetails == null)
                    {
                        // Table bulunamadığında yapılacak işlemler
                        throw new Exception("Table bulunamadı veya zaten kapalı.");
                    }

                    if (tableDetails != null)
                    {
                        // Ikram, Nakit, ve KrediKarti değerlerini güncelle
                        tableDetails.Ikram = (tableDetails.Ikram ?? 0) + (paymentVm.IkramAmount ?? 0);
                        tableDetails.Nakit = (tableDetails.Nakit ?? 0) + (paymentVm.CashAmount ?? 0);
                        tableDetails.KrediKarti = (tableDetails.KrediKarti ?? 0) + (paymentVm.CreditCardAmount ?? 0);

                        // AlinanFiyat değerini güncelle
                        tableDetails.AlinanFiyat = (tableDetails.AlinanFiyat ?? 0) + (paymentVm.IkramAmount ?? 0)
                                                    + (paymentVm.CreditCardAmount ?? 0)
                                                    + (paymentVm.CashAmount ?? 0);

                        _db.tableDetails.Update(tableDetails);
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    List<int> selectedOrderDetailsIds = paymentVm.SelectedOrderDetailsIds
                  .Split(',')
                  .Select(int.Parse)
                  .ToList();
                    List<int> SelectedOrderDetailCounts = paymentVm.SelectedOrderDetailCounts
                  .Split(',')
                  .Select(int.Parse)
                  .ToList();


                    var tableDetails = _db.tableDetails
                        .FirstOrDefault(u => u.TableId == paymentVm.TableId && u.isClosed == false);

                    if (tableDetails == null)
                    {
                        // Table bulunamadığında yapılacak işlemler
                        throw new Exception("Table bulunamadı veya zaten kapalı.");
                    }

                    if (tableDetails != null)
                    {
                        var orderDetails = _db.OrderDetails
                            .Where(o => selectedOrderDetailsIds.Contains(o.OrderDetailsId))
                            .ToList();
                        if (paymentVm.TypeOfPayment == "cash" || paymentVm.TypeOfPayment == "krediKarti")
                        {
                            double odenecekTutar = 0;
                            for (int i = 0; i < orderDetails.Count; i++)
                            {
                                odenecekTutar += orderDetails[i].Count * orderDetails[i].Price;

                            }
                            if (paymentVm.TypeOfPayment == "krediKarti")
                            {
                                tableDetails.KrediKarti = (tableDetails.KrediKarti ?? 0) + odenecekTutar;

                            }
                            else
                            {
                                tableDetails.Nakit = (tableDetails.Nakit ?? 0) + odenecekTutar;
                            }
                            tableDetails.AlinanFiyat = (tableDetails.AlinanFiyat ?? 0) + odenecekTutar;

                        }
                        else
                        {
                            // Ikram, Nakit, ve KrediKarti değerlerini güncelle
                            tableDetails.Ikram = (tableDetails.Ikram ?? 0) + (paymentVm.IkramAmount ?? 0);
                            tableDetails.Nakit = (tableDetails.Nakit ?? 0) + (paymentVm.CashAmount ?? 0);
                            tableDetails.KrediKarti = (tableDetails.KrediKarti ?? 0) + (paymentVm.CreditCardAmount ?? 0);

                            // AlinanFiyat değerini güncelle
                            tableDetails.AlinanFiyat = (tableDetails.AlinanFiyat ?? 0) + (paymentVm.IkramAmount ?? 0)
                                                        + (paymentVm.CreditCardAmount ?? 0)
                                                        + (paymentVm.CashAmount ?? 0);

                        }

                        _db.tableDetails.Update(tableDetails);




                        for (int i = 0; i < orderDetails.Count; i++)
                        {
                            if (orderDetails[i].Count > SelectedOrderDetailCounts[i])
                            {
                                orderDetails[i].Count -= SelectedOrderDetailCounts[i];
                                orderDetails[i].OdemesiAlinmisCount += SelectedOrderDetailCounts[i];
                            }
                            else
                            {
                                orderDetails[i].Count = 0;
                                orderDetails[i].OdemesiAlinmisCount += SelectedOrderDetailCounts[i];
                                orderDetails[i].PaymentStatus = SD.PaymentStatusApproved;
                                orderDetails[i].isPaid = true;

                            }
                            _db.OrderDetails.Update(orderDetails[i]);
                        }




                        await _db.SaveChangesAsync();

                        _response.Result = "Ödeme Alındı";
                        _response.IsSuccess = true;
                    }
                    else
                    {
                        _response.Result = "Masa bulunamadı veya kapalı";
                        _response.IsSuccess = false;
                    }
                }
               

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }






        public class ChangeTableRequest
        {
            public int CurrentTableId { get; set; }
            public int NewTableId { get; set; }
        }


        [Authorize]
        [HttpPost("CloseTable")]
        public async Task<ResponseDto> CloseTable([FromBody] TableCloseRequest request)
        {
            try
            {
                // Kapalı olmayan ve belirtilen TableId'ye sahip masayı bul
                TableDetails tableDetails = await _db.tableDetails
                    .Include(u => u.OrderHeaders)
                    .ThenInclude(x => x.OrderDetails)
                    .FirstOrDefaultAsync(u => u.TableId == request.TableId && u.isClosed == false);

                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();
                foreach (var orderHeasder in tableDetails.OrderHeaders)
                {
                    if (orderHeasder.OrderDetails != null)
                    {
                        foreach (var detail in orderHeasder.OrderDetails)
                        {
                            detail.Product = productDtos.FirstOrDefault(u => u.ProductId == detail.ProductId);
                        }
                    }

                    if (orderHeasder.CancelDetails != null)
                    {
                        foreach (var detail in orderHeasder.CancelDetails)
                        {
                            detail.Product = productDtos.FirstOrDefault(u => u.ProductId == detail.ProductId);
                        }
                    }
                }

                if (tableDetails != null)
                {

                    // Toplam tutar ile alınan tutarı karşılaştır
                    if (tableDetails.TotalTable != tableDetails.AlinanFiyat)
                    {
                        _response.IsSuccess = false;
                        _response.Result = $"Alınacak ücret ile alınan ücret uyuşmuyor.(Alinan Fiyat:{tableDetails.AlinanFiyat},Alinicak Fiyat:{tableDetails.TotalTable}) Lütfen kontrol edip tekrar deneyin. ";
                        return _response;
                    }


                    
                    // Masa kapanışı işlemi
                    tableDetails.PaymentStatus = SD.PaymentStatusApproved;
                    tableDetails.isClosed = true;
                    tableDetails.CloseTime = DateTime.Now;

                    // Stok azaltma işlemi
                    foreach (var orderHeader in tableDetails.OrderHeaders)
                    {
                        orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                        foreach (var orderDetail in orderHeader.OrderDetails)
                        {
                            if (orderDetail != null)
                            {
                                //if (orderDetail.PaymentStatus == SD.PaymentStatusIkram)
                                //{
                                //    tableDetails.Iskonto += orderDetail.Count * orderDetail.Price;
                                //}
                                var itemDto = _mapper.Map<OrderDetailsDto>(orderDetail);
                                ReduceStock(itemDto); 
                            }
                        }

                    }
                    _db.OrderHeaders.UpdateRange(tableDetails.OrderHeaders);
                    PrintTable(tableDetails.TableId);
                    // Verileri güncelle
                    _db.tableDetails.Update(tableDetails);
                    Veriler veriler = await _db.Veriler.FirstOrDefaultAsync(u => u.Id == 1);

                    if (veriler == null)
                    {
                        int a = 0;
                        foreach (var item in tableDetails.OrderHeaders)
                        {
                            foreach (var detail in item.OrderDetails)
                            {
                                a++;
                            }
                        }

                        veriler = new Veriler();
                        veriler.TotalProductNumber = (await _productService.GetProducts()).Count();
                        veriler.ToplamKazanç = (int)tableDetails.AlinanFiyat;
                        veriler.ToplamSiparisSayisi = a;
                        veriler.ToplamKapananMasaSayisi = 1;

                        // Haftanın ilk günü Pazar olacak şekilde gün sırasını bulma
                        DateTime closeTime = DateTime.Now;
                        int gunNumarasi = (int)closeTime.DayOfWeek;
                        gunNumarasi = (gunNumarasi) % 7; // Pazar 0, Pazartesi 1, Salı 2, ...

                        // Haftalık nakit ve kredi kartı değerlerini güncelle
                        veriler.HaftalikNakit[gunNumarasi] += tableDetails.Nakit ?? 0.0;
                        veriler.HaftalikKrediKarti[gunNumarasi] += tableDetails.KrediKarti ?? 0.0;

                        int ayNumarasi = closeTime.Month - 1;
                        veriler.AylikNakit[ayNumarasi] += tableDetails.Nakit ?? 0.0;
                        veriler.AylikKrediKarti[ayNumarasi] += tableDetails.KrediKarti ?? 0.0;

                        veriler.SonGuncellemeTarihi = closeTime.Date;
                        veriler.SonGuncellemeTarihiYil = closeTime.Date;
                        // Verileri kaydet
                        _db.Veriler.Add(veriler);
                    }
                    else
                    {
                        // Haftalık verileri güncellemeden önce kontrol et
                        TimeSpan fark = DateTime.Now - veriler.SonGuncellemeTarihi;

                        if (fark.Days >= 7) // Eğer son güncelleme tarihinden 7 gün geçmişse
                        {
                            veriler.HaftalikNakit = new List<double> { 0, 0, 0, 0, 0, 0, 0 };
                            veriler.HaftalikKrediKarti = new List<double> { 0, 0, 0, 0, 0, 0, 0 };
                            veriler.SonGuncellemeTarihi = DateTime.Now.Date;
                        }

                        var farkyil = DateTime.Now - veriler.SonGuncellemeTarihiYil;

                        if (farkyil.Days >= 365) // Eğer son güncelleme tarihinden 7 gün geçmişse
                        {
                            veriler.AylikKrediKarti = new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                            veriler.AylikNakit = new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                            veriler.SonGuncellemeTarihiYil = DateTime.Now.Date;
                        }


                        // Haftanın gününü yeniden hesapla
                        int gunNumarasi = (int)DateTime.Now.DayOfWeek;
                        gunNumarasi = (gunNumarasi) % 7;

                        // Haftalık nakit ve kredi kartı değerlerini güncelle
                        veriler.HaftalikNakit[gunNumarasi] += tableDetails.Nakit ?? 0.0;
                        veriler.HaftalikKrediKarti[gunNumarasi] += tableDetails.KrediKarti ?? 0.0;

                        int ayNumarasi = DateTime.Now.Month - 1;
                        veriler.AylikNakit[ayNumarasi] += tableDetails.Nakit ?? 0.0;
                        veriler.AylikKrediKarti[ayNumarasi] += tableDetails.KrediKarti ?? 0.0;



                        int b = 0;
                        foreach (var item in tableDetails.OrderHeaders)
                        {
                            foreach (var detail in item.OrderDetails)
                            {
                                b++;
                            }
                        }

                        // Toplam kazanç ve sipariş sayısını güncelle
                        veriler.ToplamKazanç += (int)tableDetails.AlinanFiyat;
                        veriler.ToplamSiparisSayisi += b;
                        veriler.ToplamKapananMasaSayisi += 1;
                    }

                    // İlgili masayı boşalt
                    TableNo forcloseTable = await _db.TableNos.FirstOrDefaultAsync(u => u.Id == request.TableId);
                    if (forcloseTable != null)
                    {
                        forcloseTable.IsOccupied = false;
                        _db.TableNos.Update(forcloseTable);
                    }

                    await _db.SaveChangesAsync();
                    _response.Result = "Masa başarıyla kapatıldı.";
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Masa bulunamadı veya zaten kapatılmış.";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Bir hata oluştu: " + ex.Message;
            }

            return _response;
        }

        public class TableCloseRequest
        {
            public int TableId { get; set; }
        }


        [Authorize]
        [HttpPost("CancelOrder")]
        public async Task<ResponseDto> CancelOrder([FromBody] PaymentVm paymentVm)
        {
            try
            {
                var tableDetails = _db.tableDetails
                    .FirstOrDefault(u => u.TableId == paymentVm.TableId && u.isClosed == false);

                if (tableDetails == null)
                {
                    throw new Exception("Table bulunamadı veya zaten kapalı.");
                }

                // Check if the counts and ids lists are matching in length
                List<int> selectedOrderDetailsIds = paymentVm.SelectedOrderDetailsIds
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();
                List<int> selectedOrderDetailCounts = paymentVm.SelectedOrderDetailCounts
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

                if (selectedOrderDetailsIds.Count != selectedOrderDetailCounts.Count)
                {
                    throw new Exception("Seçilen ürünlerin sayıları ve sipariş detayları uyuşmuyor.");
                }

                var orderDetails = _db.OrderDetails.Include(u => u.ProductExtrasList)
                    .Where(o => selectedOrderDetailsIds.Contains(o.OrderDetailsId))
                    .ToList();

                for (int i = 0; i < orderDetails.Count; i++)
                {
                    // Create cancel detail entry
                    CancelDetails cancelDetails = new()
                    {
                        OrderHeaderId = orderDetails[i].OrderHeaderId,
                        ProductId = orderDetails[i].ProductId,
                        Count = selectedOrderDetailCounts[i],
                        ProductName = orderDetails[i].ProductName,
                        Price = orderDetails[i].Price,
                        ProductExtrasList = orderDetails[i].ProductExtrasList,
                        PaymentStatus = SD.StatusCancelled,
                        CancelTime = DateTime.Now,
                        isPaid = false
                    };

                    // Update order detail count
                    orderDetails[i].Count -= selectedOrderDetailCounts[i];

                    if (orderDetails[i].Count < 0)
                    {
                        throw new Exception("Ürün miktarı sıfırın altına düşemez.");
                    }

                    if (orderDetails[i].Count == 0 && orderDetails[i].OdemesiAlinmisCount == 0)
                    {
                        _db.OrderDetails.Remove(orderDetails[i]);
                    }
                    else
                    {
                        if (orderDetails[i].Count == 0 && orderDetails[i].OdemesiAlinmisCount != 0)
                        {
                            orderDetails[i].isPaid = true;
                            orderDetails[i].PaymentStatus = SD.PaymentStatusApproved;
                        }
                        _db.OrderDetails.Update(orderDetails[i]);
                    }

                    // Add cancel detail and update table total
                    _db.CancelDetails.Add(cancelDetails);
                    tableDetails.TotalTable -= selectedOrderDetailCounts[i] * orderDetails[i].Price;

                    if (tableDetails.TotalTable < 0)
                    {
                        throw new Exception("Toplam masa tutarı sıfırın altına düşemez.");
                    }
                }

                // Update table details
                _db.tableDetails.Update(tableDetails);
                await _db.SaveChangesAsync();

                _response.Result = "Sipariş başarıyla iptal edildi.";
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

       

    }
}
