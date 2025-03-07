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
		public ProductAPIController(AppDbContext db, IMapper mapper,ICategoryService categoryService)
		{
			_db = db;
			_categoryService = categoryService;
			_mapper = mapper;
			_response = new ResponseDto();
		}

		[HttpGet]
		public ResponseDto Get()
		{
			try
			{
				IEnumerable<Product> objlist = _db.Products.Include(u=>u.ProductMaterials).Include(u=>u.ProductExtras).ToList();

				var objListDto = _mapper.Map<IEnumerable<ProductDto>>(objlist);
                var categories = _categoryService.GetAllCategories().Result;
                foreach (var product in objListDto)
				{
                    var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
                    if (category != null)
                    {
                        product.Category = category; // Eğer CategoryDto kullanıyorsanız.
                    }
                }
				_response.Result = objListDto;

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

        [HttpGet("GetProductsByCategoryId/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategoryId(int categoryId)
        {
            var products = await _db.Products.Include(u=>u.ProductMaterials).Include(u=>u.ProductExtras).Where(p => p.CategoryId == categoryId).ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Bu kategori için ürün bulunamadı."
                });
            }

            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Result = products
            });
        }


        [HttpGet]
		[Route("{id:int}")]
		public ResponseDto Get(int id)
		{
			try
			{
				Product obj = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).First(u => u.ProductId == id);
				var category = _categoryService.GetCategory(obj.CategoryId).Result;
				ProductDto productDto = _mapper.Map<ProductDto>(obj);
				productDto.Category = category;
				_response.Result = productDto;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

        [HttpGet("ProductCreateViewForVm")]
        public ResponseDto ProductCreateViewForVm()
        {
            try
            {
                var category = _categoryService.GetAllCategories();

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

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
		[HttpGet("ProductEditViewForVm/{id:int}")]
		public async Task<ResponseDto> ProductEditViewForVm(int id)
		{
			try
			{
				var category = await _categoryService.GetAllCategories(); // await kullanıldı
				Product product = _db.Products.FirstOrDefault(u => u.ProductId == id); // First yerine FirstOrDefault ile kontrol

				if (product == null)
				{
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
				_response.IsSuccess = true;  // Başarıyla tamamlandığında
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}







		[HttpGet("ProductMaterialAndExtraViewForVm/{id:int}")]
        public async Task<ResponseDto> ProductMaterialAndExtraViewForVm(int id)
        {
            try
            {
                // Retrieve the product with its materials and extras
                var product = await _db.Products
                    .Include(p => p.ProductMaterials)
                    .Include(p => p.ProductExtras)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Product not found.";
                    return _response;
                }

                // Get category details
                var categoryResult = await _categoryService.GetCategory(product.CategoryId);

                if (categoryResult == null)
                {
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
        public async Task<ResponseDto> Post(ProductVM productVM)
        {
            try
            {
                Product product;

                if (productVM.Product.ProductId == 0)
                {
                    // Yeni ürün ekleniyor
                    product = new Product
                    {
                        Name = productVM.Product.Name,
                        Price = productVM.Product.Price,
                        CategoryId = productVM.Product.CategoryId
                    };
                    _db.Products.Add(product);
                    _db.SaveChanges();
                    _response.Result = _mapper.Map<ProductDto>(product);
                }
                else
                {
                    // Var olan ürün güncelleniyor
                    product = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).FirstOrDefault(u => u.ProductId == productVM.Product.ProductId);
                    if (product == null)
                    {
                        throw new Exception("Product not found");
                    }

                    var category = await _categoryService.GetCategory(product.CategoryId);

                    // ProductMaterials güncelleme
                    var materials = productVM.Product.ProductMaterials;
                    if (productVM.Product.ProductMaterials != null && product.ProductMaterials.Count() > 0)
                    {
                        _db.ProductMaterials.RemoveRange(product.ProductMaterials);  // Tüm ProductMaterials siliniyor
                        _db.SaveChanges();
                    }

                    

                    if (materials != null)
                    {
                        if(productVM.Product.ProductMaterials != null)
                        {
                            productVM.Product.ProductMaterials = new List<ProductMaterialDto>();
                        }
                        foreach (var material in materials)
                        {
                            ProductMaterial productMaterial = new()
                            {
                                MaterialName = material.MaterialName,
                                Amount = material.Amount,
                                ProductId = product.ProductId,
                            };
                            _db.ProductMaterials.Add(productMaterial);
                            _db.SaveChanges();

                           
                            ProductMaterialDto productMaterialdto = _mapper.Map<ProductMaterialDto>(productMaterial);
                            productVM.Product.ProductMaterials.Add(productMaterialdto);
                        }
                        product.ProductMaterials = _mapper.Map<List<ProductMaterial>>(productVM.Product.ProductMaterials);
                    }

                    // ProductExtras güncelleme
                    if (product.ProductExtras.Count() > 0)
                    {
                        _db.ProductExtras.RemoveRange(product.ProductExtras);  // Tüm ProductExtras siliniyor
                        _db.SaveChanges();
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
                                _db.SaveChanges();

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
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }






        [HttpPut("AddProductCount")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto AddProductCount(ProductDto productDto)
		{
			try
			{
				Product product = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).First(u => u.ProductId == productDto.ProductId);
				product.AvailableProducts = productDto.AvailableProducts;
				_db.Products.Update(product);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}


        [HttpPut("DropProductCount")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto DropProductCount(OrderDetailsDto orderDetailsDto)
        {
            try
            {
                Product product = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).First(u => u.ProductId == orderDetailsDto.ProductId);
                product.AvailableProducts -= orderDetailsDto.OdemesiAlinmisCount ?? 0;
                _db.Products.Update(product);
                _db.SaveChanges();
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
		[Route("{id:int}")]
		public async Task<ResponseDto> Put(int id, ProductVM productVM)
		{
			try
			{
				
				Product existingProduct = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).First(u => u.ProductId == id);

				if (existingProduct == null)
				{
					_response.IsSuccess = false;
					_response.Message = "Product not found.";
					return _response;
				}
				existingProduct.Name = productVM.Product.Name;
				existingProduct.Price = productVM.Product.Price;
				existingProduct.CategoryId = productVM.Product.CategoryId;

				_db.Products.Update(existingProduct);
				_db.SaveChanges();

				ProductDto productDto = _mapper.Map<ProductDto>(existingProduct);
                var category = await _categoryService.GetCategory(existingProduct.CategoryId);
                productDto.Category = category;
				_response.Result = productDto;
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
				Product obj = _db.Products.Include(u => u.ProductMaterials).Include(u => u.ProductExtras).First(U => U.ProductId == id);
				_db.ProductMaterials.RemoveRange(obj.ProductMaterials);
				_db.ProductExtras.RemoveRange(obj.ProductExtras);
				_db.Products.Remove(obj);
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
