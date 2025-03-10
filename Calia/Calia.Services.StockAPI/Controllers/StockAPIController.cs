using AutoMapper;
using Calia.Services.CategoryAPI.Models.Dto;
using Calia.Services.StockAPI.Data;
using Calia.Services.StockAPI.Models;
using Calia.Services.StockAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Calia.Services.StockAPI.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly ILogger<StockAPIController> _logger;

        public StockAPIController(AppDbContext db, IMapper mapper, ILogger<StockAPIController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                _logger.LogInformation("Fetching all stock materials.");

                IEnumerable<StockMaterial> objlist = _db.StockMaterials.ToList();
                _response.Result = _mapper.Map<IEnumerable<StockMaterialDto>>(objlist);

                _logger.LogInformation("Successfully fetched {Count} stock materials.", objlist.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching stock materials.");
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                _logger.LogInformation("Fetching stock material with ID: {MaterialId}", id);

                StockMaterial obj = _db.StockMaterials.First(u => u.MaterialId == id);
                _response.Result = _mapper.Map<StockMaterialDto>(obj);

                _logger.LogInformation("Successfully fetched stock material: {MaterialName}", obj.MaterialName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching stock material with ID: {MaterialId}", id);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("Creatematerial")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] string stockMaterialName)
        {
            try
            {
                _logger.LogInformation("Creating stock material: {MaterialName}", stockMaterialName);

                var exists = _db.StockMaterials
                    .FirstOrDefault(sm => sm.MaterialName.ToLower() == stockMaterialName.ToLower());

                if (exists != null)
                {
                    _logger.LogWarning("Stock material already exists: {MaterialName}", stockMaterialName);
                    _response.IsSuccess = false;
                    _response.Message = "Bu isimde bir materyal zaten mevcut.";
                }
                else
                {
                    StockMaterial stockMaterial = new()
                    {
                        MaterialName = stockMaterialName,
                    };
                    _db.StockMaterials.Add(stockMaterial);
                    _db.SaveChanges();
                    _response.Result = _mapper.Map<StockMaterialDto>(stockMaterial);

                    _logger.LogInformation("Stock material created successfully: {MaterialName}", stockMaterialName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating stock material: {MaterialName}", stockMaterialName);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut("ChangeMaterialAmount/{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put(int id, StockMaterialDto stockMaterialDto)
        {
            try
            {
                _logger.LogInformation("Updating stock material amount. ID: {MaterialId}, New Amount: {Amount}",
                    id, stockMaterialDto.MaterialAmount);

                StockMaterial stockMaterial = _db.StockMaterials.First(u => u.MaterialId == id);
                stockMaterial.MaterialAmount = stockMaterialDto.MaterialAmount;
                _db.StockMaterials.Update(stockMaterial);
                _db.SaveChanges();

                _response.Result = _mapper.Map<StockMaterialDto>(stockMaterial);

                _logger.LogInformation("Stock material amount updated successfully. ID: {MaterialId}, New Amount: {Amount}",
                    id, stockMaterialDto.MaterialAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating stock material amount. ID: {MaterialId}", id);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut("DecreaseStockMaterial/{materialName}")]
        public ResponseDto Put(string materialName, [FromBody] int materialAmount)
        {
            try
            {
                _logger.LogInformation("Decreasing stock material: {MaterialName}, Amount: {Amount}",
                    materialName, materialAmount);

                StockMaterial stockMaterial = _db.StockMaterials.First(u => u.MaterialName == materialName);

                if (stockMaterial.MaterialAmount >= materialAmount)
                {
                    stockMaterial.MaterialAmount -= materialAmount;
                    _db.StockMaterials.Update(stockMaterial);
                    _db.SaveChanges();
                    _response.Result = _mapper.Map<StockMaterialDto>(stockMaterial);

                    _logger.LogInformation("Stock material decreased successfully: {MaterialName}, New Amount: {Amount}",
                        materialName, stockMaterial.MaterialAmount);
                }
                else
                {
                    _logger.LogWarning("Not enough stock for material: {MaterialName}. Requested: {RequestedAmount}, Available: {AvailableAmount}",
                        materialName, materialAmount, stockMaterial.MaterialAmount);

                    _response.Message = "Yeterli Urun Yok";
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while decreasing stock material: {MaterialName}, Amount: {Amount}",
                    materialName, materialAmount);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
