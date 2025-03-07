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
		private ResponseDto _response;
		private IMapper _mapper;
		public StockAPIController(AppDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
			_response = new ResponseDto();
		}
		[HttpGet]
		public ResponseDto Get()
		{
			try
			{
				IEnumerable<StockMaterial> objlist = _db.StockMaterials.ToList();

				_response.Result = _mapper.Map<IEnumerable<StockMaterialDto>>(objlist);

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}
		[HttpGet]
		[Route("{id:int}")]
		public ResponseDto Get(int id)
		{
			try
			{
				StockMaterial obj = _db.StockMaterials.First(u => u.MaterialId == id);
				_response.Result = _mapper.Map<StockMaterialDto>(obj);
			}
			catch (Exception ex)
			{
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
                var exists = _db.StockMaterials
                .FirstOrDefault(sm => sm.MaterialName.ToLower() == stockMaterialName.ToLower());


                if (exists != null)
				{
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
				}
			}
			catch (Exception ex)
			{
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
				StockMaterial stockMaterial = _db.StockMaterials.First(u => u.MaterialId == id);
				stockMaterial.MaterialAmount = stockMaterialDto.MaterialAmount;
				_db.StockMaterials.Update(stockMaterial);
				_db.SaveChanges();
				_response.Result = _mapper.Map<StockMaterialDto>(stockMaterial);
			}
			catch (Exception ex)
			{
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

				StockMaterial stockMaterials = _db.StockMaterials.First(u => u.MaterialName == materialName);
				if (stockMaterials.MaterialAmount >= materialAmount)
				{
					stockMaterials.MaterialAmount -= materialAmount;
					_db.StockMaterials.Update(stockMaterials);
					_db.SaveChanges();
					_response.Result = _mapper.Map<StockMaterialDto>(stockMaterials);
				}
				else
				{
					_response.Message = "Yeterli Urun Yok";
					_response.IsSuccess = false;
				}

				
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
