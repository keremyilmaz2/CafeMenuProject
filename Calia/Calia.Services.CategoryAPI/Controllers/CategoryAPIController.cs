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
        private readonly ILogger<CategoryAPIController> _logger;
        public CategoryAPIController(AppDbContext db, IMapper mapper,IStockService stockService, ILogger<CategoryAPIController> logger)
		{
			_db = db;
			_mapper = mapper;
			_stockService = stockService;
			_response = new ResponseDto();
            _logger = logger;

        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                _logger.LogInformation("Get method called to fetch all categories.");

                IEnumerable<Category> objlist = _db.Categories
                    .Include(u => u.CategoryMaterials)
                    .Include(u => u.CategoryExtras)
                    .ToList();

                _logger.LogInformation("Successfully fetched {Count} categories.", objlist.Count());

                _response.Result = _mapper.Map<IEnumerable<CategoryDto>>(objlist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching categories.");
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
                _logger.LogInformation("CategoryMaterialGetAll method called to fetch all category materials.");

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

                _logger.LogInformation("Successfully fetched {Count} unique category materials.", groupedMaterials.Count);

                _response.Result = groupedMaterials;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching category materials.");
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
                _logger.LogInformation("CategoryMaterialGetForcategoryMaterialVm method called with name: {MaterialName}", name);

                // Fetch the material by name
                var material = _db.CategoryMaterials.Include(m => m.Category)
                                  .FirstOrDefault(m => m.MaterialName == name);

                if (material == null)
                {
                    _logger.LogWarning("No material found with name: {MaterialName}", name);
                    _response.IsSuccess = false;
                    _response.Message = "Material not found.";
                    return _response;
                }

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

                _logger.LogInformation("Successfully fetched category material details for: {MaterialName}", name);

                // Return a successful response
                _response.Result = categoryMaterialVM;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching category material details for: {MaterialName}", name);
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
                _logger.LogInformation("EditCategoryMaterialList called for Material: {MaterialName}", categoryMaterialVM.CategoryMaterial.MaterialName);

                categoryMaterialVM.CategoryList = _db.Categories.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }).ToList();

                var material = _db.CategoryMaterials.Include(u => u.Category)
                                .FirstOrDefault(m => m.MaterialName == categoryMaterialVM.CategoryMaterial.MaterialName);

                if (material == null)
                {
                    _logger.LogWarning("No existing material found for: {MaterialName}", categoryMaterialVM.CategoryMaterial.MaterialName);
                }

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
                            _logger.LogInformation("Added new CategoryMaterial: {MaterialName} - CategoryId: {CategoryId}", categoryMaterialVM.CategoryMaterial.MaterialName, selectedCategoryId);
                        }
                    }
                    else
                    {
                        if (existingMaterial != null)
                        {
                            _db.CategoryMaterials.Remove(existingMaterial);
                            _logger.LogInformation("Removed CategoryMaterial: {MaterialName} - CategoryId: {CategoryId}", categoryMaterialVM.CategoryMaterial.MaterialName, selectedCategoryId);
                        }
                    }
                }

                _db.SaveChanges();
                _response.Message = "Category material list updated successfully.";
                _logger.LogInformation("Successfully updated CategoryMaterial list for: {MaterialName}", categoryMaterialVM.CategoryMaterial.MaterialName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating CategoryMaterial list for: {MaterialName}", categoryMaterialVM.CategoryMaterial.MaterialName);
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
                _logger.LogInformation("CategoryExtraGetAll endpoint called.");

                var objlist = _db.CategoryExtras
                    .Include(u => u.Category)
                    .ToList();

                _logger.LogInformation("Fetched {Count} category extras from database.", objlist.Count);

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
                _logger.LogInformation("Successfully processed {Count} grouped extras.", groupedExtras.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching category extras.");
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
                _logger.LogInformation("CategoryExtraGetForcategoryExtraVm endpoint called with name: {Name}", name);

                // Fetch the extra by name
                var extra = _db.CategoryExtras.Include(m => m.Category)
                                  .FirstOrDefault(m => m.ExtraName == name);

                if (extra == null)
                {
                    _logger.LogWarning("No extra found with name: {Name}", name);
                    _response.IsSuccess = false;
                    _response.Message = "Extra not found";
                    return _response;
                }

                var extraDTO = _mapper.Map<CategoryExtraDto>(extra);

                // Get the list of category IDs associated with the extra
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

                _logger.LogInformation("Successfully retrieved extra details for {Name}", name);

                // Return a successful response
                _response.Result = categoryExtraVM;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching extra details for {Name}", name);
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
                _logger.LogInformation("EditCategoryExtraList endpoint called for ExtraName: {ExtraName}", categoryExtraVM.CategoryExtra.ExtraName);

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
                            _logger.LogInformation("Added new extra: {ExtraName} for CategoryId: {CategoryId}", categoryExtraVM.CategoryExtra.ExtraName, selectedCategoryId);
                        }
                    }
                    else
                    {
                        if (existingExtra != null)
                        {
                            _db.CategoryExtras.Remove(existingExtra);
                            _logger.LogInformation("Removed extra: {ExtraName} for CategoryId: {CategoryId}", categoryExtraVM.CategoryExtra.ExtraName, selectedCategoryId);
                        }
                    }
                }

                _db.SaveChanges();
                _response.Message = "Everything is okeito";
                _logger.LogInformation("EditCategoryExtraList successfully processed for ExtraName: {ExtraName}", categoryExtraVM.CategoryExtra.ExtraName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing extra list for ExtraName: {ExtraName}", categoryExtraVM.CategoryExtra.ExtraName);
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
                _logger.LogInformation("Get Category endpoint called with Id: {CategoryId}", id);

                Category obj = _db.Categories
                    .Include(u => u.CategoryMaterials)
                    .Include(u => u.CategoryExtras)
                    .First(u => u.Id == id);

                _response.Result = _mapper.Map<CategoryDto>(obj);
                _logger.LogInformation("Category retrieved successfully for Id: {CategoryId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with Id: {CategoryId}", id);
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
                _logger.LogInformation("PostCategoryMaterial called with MaterialName: {MaterialName}, CategoryId: {CategoryId}",
                    categoryMaterialDto.MaterialName, categoryMaterialDto.CategoryId);

                CategoryMaterial categoryMaterial = _mapper.Map<CategoryMaterial>(categoryMaterialDto);
                _db.CategoryMaterials.Add(categoryMaterial);

                Category obj = _db.Categories.First(u => u.Id == categoryMaterial.CategoryId);
                obj.CategoryMaterials.Add(categoryMaterial);
                _db.Categories.Update(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CategoryDto>(obj);
                _logger.LogInformation("CategoryMaterial added successfully: {MaterialName} to CategoryId: {CategoryId}",
                    categoryMaterialDto.MaterialName, categoryMaterialDto.CategoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding CategoryMaterial: {MaterialName} to CategoryId: {CategoryId}",
                    categoryMaterialDto.MaterialName, categoryMaterialDto.CategoryId);
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
                _logger.LogInformation("PostExtraMaterial called with ExtraName: {ExtraName}, CategoryId: {CategoryId}",
                    categoryExtraDto.ExtraName, categoryExtraDto.CategoryId);

                CategoryExtra categoryExtras = _mapper.Map<CategoryExtra>(categoryExtraDto);
                _db.CategoryExtras.Add(categoryExtras);

                Category obj = _db.Categories.First(u => u.Id == categoryExtras.CategoryId);
                obj.CategoryExtras.Add(categoryExtras);
                _db.Categories.Update(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CategoryDto>(obj);
                _logger.LogInformation("CategoryExtra added successfully: {ExtraName} to CategoryId: {CategoryId}",
                    categoryExtraDto.ExtraName, categoryExtraDto.CategoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding CategoryExtra: {ExtraName} to CategoryId: {CategoryId}",
                    categoryExtraDto.ExtraName, categoryExtraDto.CategoryId);
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
                _logger.LogInformation("Post Category called with Name: {CategoryName}", categoryDto.Name);

                Category category = _mapper.Map<Category>(categoryDto);
                _db.Categories.Add(category);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CategoryDto>(category);
                _logger.LogInformation("Category added successfully: {CategoryName}", categoryDto.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding Category: {CategoryName}", categoryDto.Name);
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
                _logger.LogInformation("Updating Category with Id: {CategoryId}, Name: {CategoryName}",
                    categoryDto.Id, categoryDto.Name);

                Category category = _mapper.Map<Category>(categoryDto);
                _db.Categories.Update(category);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CategoryDto>(category);
                _logger.LogInformation("Category updated successfully: Id: {CategoryId}, Name: {CategoryName}",
                    categoryDto.Id, categoryDto.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Category: Id: {CategoryId}, Name: {CategoryName}",
                    categoryDto.Id, categoryDto.Name);
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
                _logger.LogInformation("Deleting Category with Id: {CategoryId}", id);

                Category obj = _db.Categories
                    .Include(u => u.CategoryMaterials)
                    .Include(u => u.CategoryExtras)
                    .First(u => u.Id == id);

                _db.CategoryMaterials.RemoveRange(obj.CategoryMaterials);
                _db.CategoryExtras.RemoveRange(obj.CategoryExtras);
                _db.Categories.Remove(obj);
                _db.SaveChanges();

                _logger.LogInformation("Category deleted successfully: Id: {CategoryId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Category: Id: {CategoryId}", id);
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
                _logger.LogInformation("Deleting CategoryMaterial with Id: {MaterialId}", id);

                CategoryMaterial obj = _db.CategoryMaterials.First(u => u.MaterialId == id);
                _db.CategoryMaterials.Remove(obj);
                _db.SaveChanges();

                _logger.LogInformation("CategoryMaterial deleted successfully: Id: {MaterialId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting CategoryMaterial: Id: {MaterialId}", id);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


    }
}
