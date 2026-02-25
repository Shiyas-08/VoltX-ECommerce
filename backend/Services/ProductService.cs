using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly PhotoService _photoService;

        public ProductService(AppDbContext context, PhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }
        public async Task<(Product?, string?)> CreateAsync(CreateProductDto dto)
        {
            var normalizedName = dto.Name.Trim().ToLower();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.IsActive);

            if (category == null)
                return (null, "Category not found or inactive");

            if (dto.Price <= 0)
                return (null, "Price must be greater than 0");

            if (dto.Stock < 0)
                return (null, "Stock cannot be negative");

            bool IsGarbage(string text) =>
                string.IsNullOrWhiteSpace(text) ||
                text.All(c => c == '.' || c == '-' || c == '_' || c == ' ');


            if (IsGarbage(dto.Name))
                return (null, "Invalid product name");

            //  DUPLICATE CHECK
            var exists = await _context.Products.AnyAsync(p =>
                p.Name.ToLower() == normalizedName &&
                p.CategoryId == dto.CategoryId &&
                p.IsActive);

            if (exists)
                return (null, "Product already exists in this category");

            var product = new Product
            {
                Name = dto.Name.Trim(),
                Price = dto.Price,
                Description = dto.Description?.Trim(),
                CategoryId = dto.CategoryId,
                Stock = dto.Stock,
                IsFeatured = dto.IsFeatured,
                IsActive = true,
                Images = new List<ProductImage>()
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                if (dto.Images != null && dto.Images.Any())
                {
                    foreach (var image in dto.Images)
                    {
                        var url = await _photoService.UploadProductImageAsync(image, product.Id);
                        product.Images.Add(new ProductImage
                        {
                            ImageUrl = url,
                            IsActive = true
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                return (null, "Product already exists in this category");
            }
            catch
            {
                await transaction.RollbackAsync();
                return (null, "Failed to create product");
            }

            return (product, null);
        }



        public List<Product> AdminGetAll()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .ToList();
        }



        public List<Product> UserGetAll()
    => _context.Products
        .Include(p => p.Category)
        .Include(p => p.Images)
        .Where(p => p.IsActive && p.Category.IsActive) 
        .ToList();


        //get by id
        public Product? GetById(int id)
    => _context.Products
        .Include(p => p.Category)
        .Include(p => p.Images.Where(i => i.IsActive))
        .FirstOrDefault(p => p.Id == id && p.IsActive && p.Category.IsActive);

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return false;


            product.IsActive = false;


            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<(Product?, string?)> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
                return (null, "Product not found");

            // Normalize input and DB name
            var normalizedName = dto.Name.Trim().ToLower();
            var dbName = product.Name.Trim().ToLower();

            // Validate category
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.IsActive);

            if (category == null)
                return (null, "Invalid or inactive category");

            // Business validation
            if (dto.Price <= 0)
                return (null, "Price must be greater than 0");

            if (dto.Stock < 0)
                return (null, "Stock cannot be negative");

            // Garbage validation
            bool IsGarbage(string text) =>
                string.IsNullOrWhiteSpace(text) ||
                text.All(c => c == '.' || c == '-' || c == '_' || c == ' ');

            if (IsGarbage(dto.Name))
                return (null, "Invalid product name");

            if (!string.IsNullOrEmpty(dto.Description) && IsGarbage(dto.Description))
                return (null, "Invalid description");

            //  Duplicate check only if Name or Category changed
            if (normalizedName != dbName || dto.CategoryId != product.CategoryId)
            {
                var duplicate = await _context.Products.AnyAsync(p =>
                    p.Id != id &&
                    p.CategoryId == dto.CategoryId &&
                    p.Name.ToLower() == normalizedName);

                if (duplicate)
                    return (null, "Product already exists in this category");
            }

            // Update fields
            product.Name = dto.Name.Trim();
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.Description = dto.Description?.Trim();
            product.IsFeatured = dto.IsFeatured;
            product.CategoryId = dto.CategoryId;
            product.ModifiedOn = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return (null, "Product already exists in this category (DB constraint)");
            }

            return (product, null);
        }



        public PagedResultDto<ProductCreateResponseDto> GetPagedProducts(ProductQueryDto query)
        {
            var productsQuery = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive && p.Category.IsActive);


            var totalCount = productsQuery.Count();

            var products = productsQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new ProductCreateResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Stock = p.Stock,
                    IsFeatured = p.IsFeatured,

                     ImageUrls = p.Images
                     .Where(i => i.IsActive)
                    .Select(i => i.ImageUrl)
                    .ToList()

                })
                .ToList();

            return new PagedResultDto<ProductCreateResponseDto>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = products
            };
        }


        public PagedResultDto<ProductCreateResponseDto> SearchProducts(ProductSearchQueryDto query)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p=>p.Images)
                .Where(p => p.IsActive && p.Category.IsActive)
                .AsQueryable();

          
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = query.Search.ToLower();

                productsQuery = productsQuery.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    p.Category.Name.ToLower().Contains(keyword)
                );
            }

            var totalCount = productsQuery.Count();

            var products = productsQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new ProductCreateResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Stock = p.Stock,
                    IsFeatured = p.IsFeatured,
                    ImageUrls = p.Images
                    .Where(i => i.IsActive)
                    .Select(i => i.ImageUrl)
                    .ToList()

                })
                     .ToList();

            return new PagedResultDto<ProductCreateResponseDto>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = products
            };
        }


        //filter api
        public PagedResultDto<ProductCreateResponseDto> FilterProducts(ProductFilterDto filter)
        {
            var query = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.Category.IsActive);

            // Category
            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId);

            // Price range
            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice);

            // Sorting 
            if (!string.IsNullOrEmpty(filter.Sort))
            {
                query = filter.Sort switch
                {
                    "priceLow" => query.OrderBy(p => p.Price),
                    "priceHigh" => query.OrderByDescending(p => p.Price),
                    _ => query.OrderByDescending(p => p.Id)
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.Id);
            }

            var totalCount = query.Count();

            var products = query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new ProductCreateResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Stock = p.Stock,
                    IsFeatured = p.IsFeatured,
                    ImageUrls = p.Images
                        .Where(i => i.IsActive)
                        .Select(i => i.ImageUrl)
                        .ToList()
                })
                .ToList();

            return new PagedResultDto<ProductCreateResponseDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                Items = products
            };
        }


        public void UpdateStock(int productId, int quantity)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == productId && p.IsActive && p.Category.IsActive);
            if (product == null)
                throw new Exception("Product not found");

            var newStock = product.Stock + quantity;

            if (newStock < 0)
                throw new Exception("Insufficient stock");

            product.Stock = newStock;
            product.ModifiedOn = DateTime.UtcNow;

            _context.SaveChanges();
        }


        public bool ToggleProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return false;

            product.IsActive = !product.IsActive;
            product.ModifiedOn = DateTime.UtcNow;

            _context.SaveChanges();
            return true;
        }


        public async Task<bool> ActivateAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return false;

            product.IsActive = true;
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<string> AddProductImageAsync(int productId, IFormFile image)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

            if (product == null)
                throw new Exception("Product not found");

            var imageUrl = await _photoService.UploadProductImageAsync(image, productId);

            var productImage = new ProductImage
            {
                ProductId = productId,
                ImageUrl = imageUrl,
                IsActive = true
            };

            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();

            return imageUrl;
        }
        public async Task<bool> RemoveProductImage(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null) return false;

            image.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }


    }

}
