using AutoMapper;
using Calia.Services.CategoryAPI.Data;
using Calia.Services.CategoryAPI.Models;
using Calia.Services.CategoryAPI.Models.Dto;
using Calia.Services.CategoryAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace Calia.Services.CategoryAPI.Controllers
{
	[Route("api/category")]
	[ApiController]
	public class CategoryAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private ResponseDto _response;
		private readonly IStockService _stockService;
		private IMapper _mapper;
		public CategoryAPIController(AppDbContext db, IMapper mapper,IStockService stockService)
		{
			_db = db;
			_mapper = mapper;
			_stockService = stockService;
			_response = new ResponseDto();
		}
		[HttpGet]
		public ResponseDto Get()
		{
			try
			{
				IEnumerable<Category> objlist = _db.Categories.Include(u => u.CategoryMaterials).Include(u=>u.CategoryExtras).ToList();

				_response.Result = _mapper.Map<IEnumerable<CategoryDto>>(objlist);

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}


        [HttpGet("CategoryMaterialGetAll")]
        public ResponseDto CategoryMaterialGetAll()
        {
            try
            {
                var objlist = _db.CategoryMaterials
                    .Include(u => u.Category)
                    .ToList();

                var groupedMaterials = objlist
                    .GroupBy(m => m.MaterialName)
                    .Select(g => new MaterialVM
                    {
                        MaterialName = g.Key,
                        Categories = g.Select(m => m.Category.Name).ToList()
                    })
                    .ToList();

                _response.Result = groupedMaterials;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("CategoryMaterialGetForcategoryMaterialVm/{name}")]
        public ResponseDto CategoryMaterialGetForcategoryMaterialVm(string name)
        {
            try
            {
                // Fetch the material by name
                var material = _db.CategoryMaterials.Include(m => m.Category)
                                  .FirstOrDefault(m => m.MaterialName == name);

				var materialDTO = _mapper.Map<CategoryMaterialDto>(material);

                // Get the list of category IDs associated with the material
                var selectedCategories = _db.CategoryMaterials
                                            .Where(cm => cm.MaterialName == name)
                                            .Select(cm => cm.CategoryId)
                                            .ToList();

                // Create ViewModel without using GetAll()
                CategoryMaterialVM categoryMaterialVM = new()
                {
                    CategoryMaterial = materialDTO,
                    CategoryList = _db.Categories.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }).ToList(),
                    MaterialNameList = _db.CategoryMaterials.Select(u => new SelectListItem
                    {
                        Text = u.MaterialName,
                        Value = u.MaterialId.ToString()
                    }).ToList(),
                    // Determine if each category is selected
                    IsCategorySelected = _db.Categories
                                        .Select(u => selectedCategories.Contains(u.Id))
                                        .ToList()
                };

                // Return a successful response
                _response.Result = categoryMaterialVM;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

		[HttpPost("EditCategoryMaterialList")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto EditCategoryMaterialList([FromBody] CategoryMaterialVM categoryMaterialVM)
		{
			try
			{
                categoryMaterialVM.CategoryList = _db.Categories.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }).ToList();

                var material = _db.CategoryMaterials.Include(u => u.Category).FirstOrDefault(m => m.MaterialName == categoryMaterialVM.CategoryMaterial.MaterialName);

                for (int i = 0; i < categoryMaterialVM.CategoryList.Count(); i++)
                {
                    var category = categoryMaterialVM.CategoryList.ElementAt(i);
                    var selectedCategoryId = int.Parse(category.Value);
                    var existingMaterial = _db.CategoryMaterials.FirstOrDefault(m => m.MaterialName == categoryMaterialVM.CategoryMaterial.MaterialName && m.CategoryId == selectedCategoryId);

                    if (categoryMaterialVM.IsCategorySelected[i])
                    {
                        if (existingMaterial == null)
                        {
                            CategoryMaterial newMaterial = new()
                            {
                                MaterialName = categoryMaterialVM.CategoryMaterial.MaterialName,
                                CategoryId = selectedCategoryId
                            };
                            _db.CategoryMaterials.Add(newMaterial);
                        }
                    }
                    else
                    {
                        if (existingMaterial != null)
                        {
                            _db.CategoryMaterials.Remove(existingMaterial);
                        }
                    }
                }
                _response.Message = "Everything is okeito";
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }


        //EXTRASSS
        [HttpGet("CategoryExtraGetAll")]
        public ResponseDto CategoryExtraGetAll()
        {
            try
            {
                var objlist = _db.CategoryExtras
                    .Include(u => u.Category)
                    .ToList();

                var groupedExtras = objlist
                    .GroupBy(m => m.ExtraName)
                    .Select(g => new ExtraVM
                    {
                        ExtraName = g.Key,
                        ExtraPrice = g.First().ExtraPrice,
                        Categories = g.Select(m => m.Category.Name).ToList()
                    })
                    .ToList();

                _response.Result = groupedExtras;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("CategoryExtraGetForcategoryExtraVm/{name}")]
        public ResponseDto CategoryExtraGetForcategoryExtraVm(string name)
        {
            try
            {
                // Fetch the material by name
                var extra = _db.CategoryExtras.Include(m => m.Category)
                                  .FirstOrDefault(m => m.ExtraName == name);

                var extraDTO = _mapper.Map<CategoryExtraDto>(extra);

                // Get the list of category IDs associated with the material
                var selectedCategories = _db.CategoryExtras
                                            .Where(cm => cm.ExtraName == name)
                                            .Select(cm => cm.CategoryId)
                                            .ToList();

                // Create ViewModel without using GetAll()
                CategoryExtraVM categoryExtraVM = new()
                {
                    CategoryExtra = extraDTO,
                    CategoryList = _db.Categories.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }).ToList(),
                    ExtraNameList = _db.CategoryExtras.Select(u => new SelectListItem
                    {
                        Text = u.ExtraName,
                        Value = u.ExtraId.ToString()
                    }).ToList(),
                    // Determine if each category is selected
                    IsCategorySelected = _db.Categories
                                        .Select(u => selectedCategories.Contains(u.Id))
                                        .ToList()
                };

                // Return a successful response
                _response.Result = categoryExtraVM;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost("EditCategoryExtraList")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto EditCategoryExtraList([FromBody] CategoryExtraVM categoryExtraVM)
        {
            try
            {
                categoryExtraVM.CategoryList = _db.Categories.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }).ToList();

                var extra = _db.CategoryExtras.Include(u => u.Category).FirstOrDefault(m => m.ExtraName == categoryExtraVM.CategoryExtra.ExtraName);

                for (int i = 0; i < categoryExtraVM.CategoryList.Count(); i++)
                {
                    var category = categoryExtraVM.CategoryList.ElementAt(i);
                    var selectedCategoryId = int.Parse(category.Value);
                    var existingExtra = _db.CategoryExtras.FirstOrDefault(m => m.ExtraName == categoryExtraVM.CategoryExtra.ExtraName && m.CategoryId == selectedCategoryId);

                    if (categoryExtraVM.IsCategorySelected[i])
                    {
                        if (existingExtra == null)
                        {
                            CategoryExtra newExtra = new()
                            {
                                ExtraName = categoryExtraVM.CategoryExtra.ExtraName,
                                ExtraPrice = categoryExtraVM.CategoryExtra.ExtraPrice,
                                CategoryId = selectedCategoryId
                            };
                            _db.CategoryExtras.Add(newExtra);
                        }
                    }
                    else
                    {
                        if (existingExtra != null)
                        {
                            _db.CategoryExtras.Remove(existingExtra);
                        }
                    }
                }
                _response.Message = "Everything is okeito";
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }





        //EXTRAS SON 


        [HttpGet]
		[Route("{id:int}")]
		public ResponseDto Get(int id)
		{
			try
			{
				Category obj = _db.Categories.Include(u => u.CategoryMaterials).Include(u => u.CategoryExtras).First(u => u.Id == id);
				_response.Result = _mapper.Map<CategoryDto>(obj);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}


		[HttpPost("PostCategoryMaterial")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Post(CategoryMaterialDto categoryMaterialDto)
		{
			try
			{
				CategoryMaterial categoryMaterial = _mapper.Map<CategoryMaterial>(categoryMaterialDto);
				//_stockService.CreateStockMaterial(categoryMaterialDto.MaterialName);
				_db.CategoryMaterials.Add(categoryMaterial);
				Category obj = _db.Categories.First(u => u.Id == categoryMaterial.CategoryId);
				obj.CategoryMaterials.Add(categoryMaterial);
				_db.Categories.Update(obj);
				_db.SaveChanges();
				
				_response.Result = _mapper.Map<CategoryDto>(obj);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost("PostExtraMaterial")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Post(CategoryExtraDto categoryExtraDto)
		{
			try
			{
				CategoryExtra categoryExtras = _mapper.Map<CategoryExtra>(categoryExtraDto);
				_db.CategoryExtras.Add(categoryExtras);
				Category obj = _db.Categories.First(u => u.Id == categoryExtras.CategoryId);
				obj.CategoryExtras.Add(categoryExtras);
				_db.Categories.Update(obj);
				_db.SaveChanges();

				_response.Result = _mapper.Map<CategoryDto>(obj);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Post(CategoryDto categoryDto)
		{
			try
			{
				Category category = _mapper.Map<Category>(categoryDto);
				_db.Categories.Add(category);
				_db.SaveChanges();
				_response.Result = _mapper.Map<CategoryDto>(category);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}


		[HttpPut]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Put(CategoryDto categoryDto)
		{
			try
			{
				Category category = _mapper.Map<Category>(categoryDto);
				_db.Categories.Update(category);
				_db.SaveChanges();
				_response.Result = _mapper.Map<CategoryDto>(category);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpDelete]
		[Route("{id:int}")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Delete(int id)
		{
			try
			{
				Category obj = _db.Categories.Include(u => u.CategoryMaterials).Include(u => u.CategoryExtras).First(U => U.Id == id);
				_db.CategoryMaterials.RemoveRange(obj.CategoryMaterials);
				_db.CategoryExtras.RemoveRange(obj.CategoryExtras);
				_db.Categories.Remove(obj);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}


        [HttpDelete("DeleteMaterial/{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto DeleteMaterial(int id)
        {
            try
            {
                CategoryMaterial obj = _db.CategoryMaterials.First(U => U.MaterialId == id);
                _db.CategoryMaterials.Remove(obj);
                _db.SaveChanges();
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
