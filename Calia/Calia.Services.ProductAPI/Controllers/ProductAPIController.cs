using AutoMapper;
using Calia.Services.ProductAPI.Data;
using Calia.Services.ProductAPI.Models;
using Calia.Services.ProductAPI.Models.Dto;
using Calia.Services.ProductAPI.Models.ViewModel;
using Calia.Services.ProductAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Calia.Services.ProductAPI.Controllers
{
	[Route("api/product")]
	[ApiController]
	public class ProductAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly ICategoryService _categoryService;
		private ResponseDto _response;
		private IMapper _mapper;
        private readonly ILogger<ProductAPIController> _logger;

        public ProductAPIController(AppDbContext db, IMapper mapper,ICategoryService categoryService, ILogger<ProductAPIController> logger)
		{
			_db = db;
			_categoryService = categoryService;
			_mapper = mapper;
			_response = new ResponseDto();
            _logger = logger;
		}

        [HttpGet]
        public ResponseDto Get()
        {
            _logger.LogInformation("Get all products called.");

            try
            {
                IEnumerable<Product> objlist = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).ToList();

                _logger.LogInformation("Fetched {ProductCount} products from the database.", objlist.Count());

                var objListDto = _mapper.Map<IEnumerable<ProductDto>>(objlist);
                var categories = _categoryService.GetAllCategories().Result;

                _logger.LogInformation("Fetched {CategoryCount} categories.", categories.Count());

                foreach (var product in objListDto)
                {
                    var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
                    if (category != null)
                    {
                        product.Category = category; // If using CategoryDto
                    }
                }

                _response.Result = objListDto;
                _logger.LogInformation("Successfully mapped products to DTOs and set category data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products.");
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet("GetProductsByCategoryId/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategoryId(int categoryId)
        {
            _logger.LogInformation("Get products by CategoryId: {CategoryId} called.", categoryId);

            try
            {
                var products = await _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras)
                    .Where(p => p.CategoryId == categoryId).ToListAsync();

                if (products == null || !products.Any())
                {
                    _logger.LogWarning("No products found for CategoryId: {CategoryId}.", categoryId);
                    return NotFound(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "No products found for this category."
                    });
                }

                _logger.LogInformation("{ProductCount} products found for CategoryId: {CategoryId}.", products.Count(), categoryId);

                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Result = products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products for CategoryId: {CategoryId}.", categoryId);
                return StatusCode(500, new ResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            _logger.LogInformation("Get product by ProductId: {ProductId} called.", id);

            try
            {
                Product obj = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).First(u => u.ProductId == id);

                _logger.LogInformation("Fetched product with ProductId: {ProductId}.", id);

                var category = _categoryService.GetCategory(obj.CategoryId).Result;
                ProductDto productDto = _mapper.Map<ProductDto>(obj);
                productDto.Category = category;

                _response.Result = productDto;
                _logger.LogInformation("Successfully mapped product to DTO and set category data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching product with ProductId: {ProductId}.", id);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }





        [HttpGet("ProductCreateViewForVm")]
        public ResponseDto ProductCreateViewForVm()
        {
            _logger.LogInformation("ProductCreateViewForVm called.");

            try
            {
                var category = _categoryService.GetAllCategories();

                _logger.LogInformation("Fetched {CategoryCount} categories.", category.Result.Count());

                ProductVM productVM = new()
                {
                    CategoryList = category.Result.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                    Product = new ProductDto()
                };
                _response.Result = productVM;
                _logger.LogInformation("Successfully created product view model for create.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating product creation view.");
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("ProductEditViewForVm/{id:int}")]
        public async Task<ResponseDto> ProductEditViewForVm(int id)
        {
            _logger.LogInformation("ProductEditViewForVm called for ProductId: {ProductId}.", id);

            try
            {
                var category = await _categoryService.GetAllCategories(); // await used here
                Product product = await _db.Products.FirstOrDefaultAsync(u => u.ProductId == id); // Used FirstOrDefaultAsync

                if (product == null)
                {
                    _logger.LogWarning("Product with ProductId: {ProductId} not found.", id);
                    throw new Exception("Product not found.");
                }

                ProductDto productDto = _mapper.Map<ProductDto>(product);
                ProductVM productVM = new()
                {
                    CategoryList = category.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                    Product = productDto
                };

                _response.Result = productVM;
                _response.IsSuccess = true;
                _logger.LogInformation("Successfully created product view model for edit.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating product edit view for ProductId: {ProductId}.", id);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("ProductMaterialAndExtraViewForVm/{id:int}")]
        public async Task<ResponseDto> ProductMaterialAndExtraViewForVm(int id)
        {
            _logger.LogInformation("ProductMaterialAndExtraViewForVm called for ProductId: {ProductId}.", id);

            try
            {
                // Retrieve the product with its materials and extras
                var product = await _db.Products
                    .Include(p => p.ProductMaterials)
                    .Include(p => p.ProductExtras)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ProductId: {ProductId} not found.", id);
                    _response.IsSuccess = false;
                    _response.Message = "Product not found.";
                    return _response;
                }

                // Get category details
                var categoryResult = await _categoryService.GetCategory(product.CategoryId);

                if (categoryResult == null)
                {
                    _logger.LogWarning("Category for ProductId: {ProductId} not found.", id);
                    _response.IsSuccess = false;
                    _response.Message = "Category not found.";
                    return _response;
                }

                // Map product to DTO
                var productDto = _mapper.Map<ProductDto>(product);

                // Construct the view model
                var productVM = new ProductVM
                {
                    CategoryMaterials = categoryResult.CategoryMaterials.Select(cm => new SelectListItem
                    {
                        Text = cm.MaterialName,
                        Value = cm.MaterialId.ToString()
                    }),

                    CategoryExtras = categoryResult.CategoryExtras.Select(ce => new SelectListItem
                    {
                        Text = ce.ExtraName,
                        Value = ce.ExtraId.ToString()
                    }),

                    Product = productDto
                };

                // Initialize ProductMaterials list
                productVM.Product.ProductMaterials = categoryResult.CategoryMaterials.Select(cm => new ProductMaterialDto
                {
                    MaterialName = cm.MaterialName,
                    Amount = 0,
                    ProductId = product.ProductId
                }).ToList();

                productVM.Product.ProductExtras = categoryResult.CategoryExtras.Select(cm => new ProductExtraDto
                {
                    ExtraName = cm.ExtraName,
                    Price = cm.ExtraPrice,
                    ProductId = product.ProductId,
                }).ToList();

                _response.Result = productVM;
                _response.IsSuccess = true;
                _logger.LogInformation("Successfully created product view model with materials and extras.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating product material and extra view for ProductId: {ProductId}.", id);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }




        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Post(ProductVM productVM)
        {
            _logger.LogInformation("Post method called for creating/updating product.");

            try
            {
                Product product;

                if (productVM.Product.ProductId == 0)
                {
                    // Creating a new product
                    product = new Product
                    {
                        Name = productVM.Product.Name,
                        Price = productVM.Product.Price,
                        CategoryId = productVM.Product.CategoryId
                    };
                    _db.Products.Add(product);
                    _db.SaveChanges();

                    _response.Result = _mapper.Map<ProductDto>(product);
                    _logger.LogInformation("New product created with ProductId: {ProductId}.", product.ProductId);
                }
                else
                {
                    // Updating an existing product
                    product = await _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).FirstOrDefaultAsync(u => u.ProductId == productVM.Product.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning("Product with ProductId: {ProductId} not found.", productVM.Product.ProductId);
                        throw new Exception("Product not found");
                    }

                    var category = await _categoryService.GetCategory(product.CategoryId);

                    // Update ProductMaterials
                    if (productVM.Product.ProductMaterials != null && product.ProductMaterials.Any())
                    {
                        _db.ProductMaterials.RemoveRange(product.ProductMaterials);  // Remove all ProductMaterials
                        await _db.SaveChangesAsync();
                    }

                    // Adding new ProductMaterials
                    if (productVM.Product.ProductMaterials != null)
                    {
                        foreach (var material in productVM.Product.ProductMaterials)
                        {
                            var productMaterial = new ProductMaterial
                            {
                                MaterialName = material.MaterialName,
                                Amount = material.Amount,
                                ProductId = product.ProductId
                            };
                            _db.ProductMaterials.Add(productMaterial);
                            await _db.SaveChangesAsync();

                            ProductMaterialDto productMaterialDto = _mapper.Map<ProductMaterialDto>(productMaterial);
                            productVM.Product.ProductMaterials.Add(productMaterialDto);
                        }
                        product.ProductMaterials = _mapper.Map<List<ProductMaterial>>(productVM.Product.ProductMaterials);
                    }

                    // Update ProductExtras
                    if (product.ProductExtras.Any())
                    {
                        _db.ProductExtras.RemoveRange(product.ProductExtras);  // Remove all ProductExtras
                        await _db.SaveChangesAsync();
                    }

                    if (productVM.IsExtraSelected != null)
                    {
                        var categoryExtras = category.CategoryExtras;
                        for (int i = 0; i < categoryExtras.Count(); i++)
                        {
                            if (productVM.IsExtraSelected[i])
                            {
                                var selectedExtra = categoryExtras[i];
                                var productExtra = new ProductExtra
                                {
                                    ExtraName = selectedExtra.ExtraName,
                                    Price = selectedExtra.ExtraPrice,
                                    ProductId = product.ProductId
                                };
                                _db.ProductExtras.Add(productExtra);
                                await _db.SaveChangesAsync();

                                if (productVM.Product.ProductExtras == null)
                                {
                                    productVM.Product.ProductExtras = new List<ProductExtraDto>();
                                }

                                ProductExtraDto productExtraDto = _mapper.Map<ProductExtraDto>(productExtra);
                                productVM.Product.ProductExtras.Add(productExtraDto);
                            }
                        }
                        product.ProductExtras = _mapper.Map<List<ProductExtra>>(productVM.Product.ProductExtras);
                    }

                    _db.Products.Update(product);
                    await _db.SaveChangesAsync();

                    ProductDto productDto = _mapper.Map<ProductDto>(product);
                    productDto.Category = category;
                    _response.Result = productDto;
                    _response.IsSuccess = true;

                    _logger.LogInformation("Product with ProductId: {ProductId} updated successfully.", product.ProductId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing product creation/update.");
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut("AddProductCount")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto AddProductCount(ProductDto productDto)
        {
            _logger.LogInformation("AddProductCount called for ProductId: {ProductId}.", productDto.ProductId);

            try
            {
                Product product = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).First(u => u.ProductId == productDto.ProductId);
                product.AvailableProducts = productDto.AvailableProducts;
                _db.Products.Update(product);
                _db.SaveChanges();

                _logger.LogInformation("Product count for ProductId: {ProductId} updated to {AvailableProducts}.", productDto.ProductId, product.AvailableProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product count for ProductId: {ProductId}.", productDto.ProductId);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut("DropProductCount")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto DropProductCount(OrderDetailsDto orderDetailsDto)
        {
            _logger.LogInformation("DropProductCount called for ProductId: {ProductId}.", orderDetailsDto.ProductId);

            try
            {
                Product product = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).First(u => u.ProductId == orderDetailsDto.ProductId);
                product.AvailableProducts -= orderDetailsDto.OdemesiAlinmisCount ?? 0;
                _db.Products.Update(product);
                _db.SaveChanges();

                _logger.LogInformation("Product count for ProductId: {ProductId} decreased by {Quantity}.", orderDetailsDto.ProductId, orderDetailsDto.OdemesiAlinmisCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while decreasing product count for ProductId: {ProductId}.", orderDetailsDto.ProductId);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        [Route("{id:int}")]
        public async Task<ResponseDto> Put(int id, ProductVM productVM)
        {
            _logger.LogInformation("Put method called for updating product with ProductId: {ProductId}.", id);

            try
            {
                Product existingProduct = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).FirstOrDefault(u => u.ProductId == id);

                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ProductId: {ProductId} not found.", id);
                    _response.IsSuccess = false;
                    _response.Message = "Product not found.";
                    return _response;
                }

                // Updating product details
                existingProduct.Name = productVM.Product.Name;
                existingProduct.Price = productVM.Product.Price;
                existingProduct.CategoryId = productVM.Product.CategoryId;

                _db.Products.Update(existingProduct);
                await _db.SaveChangesAsync();

                ProductDto productDto = _mapper.Map<ProductDto>(existingProduct);
                var category = await _categoryService.GetCategory(existingProduct.CategoryId);
                productDto.Category = category;

                _response.Result = productDto;
                _response.IsSuccess = true;

                _logger.LogInformation("Product with ProductId: {ProductId} updated successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product with ProductId: {ProductId}.", id);
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
            _logger.LogInformation("Delete method called for product with ProductId: {ProductId}.", id);

            try
            {
                Product obj = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).FirstOrDefault(U => U.ProductId == id);

                if (obj == null)
                {
                    _logger.LogWarning("Product with ProductId: {ProductId} not found.", id);
                    _response.IsSuccess = false;
                    _response.Message = "Product not found.";
                    return _response;
                }

                // Deleting product materials, extras and product
                _db.ProductMaterials.RemoveRange(obj.ProductMaterials);
                _db.ProductExtras.RemoveRange(obj.ProductExtras);
                _db.Products.Remove(obj);
                _db.SaveChanges();

                _response.IsSuccess = true;
                _logger.LogInformation("Product with ProductId: {ProductId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product with ProductId: {ProductId}.", id);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }



    }
}
