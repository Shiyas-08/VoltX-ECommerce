using ECommerce.DTOs;
using ECommerce.Models;

namespace ECommerce.Services
{
    public interface IProductService
    {
        Task<(Product?, string?)> CreateAsync(CreateProductDto dto);

        List<Product> UserGetAll();   
        List<Product> AdminGetAll();   
        Product GetById(int id);
        Task<bool> SoftDeleteAsync(int id);
        bool ToggleProduct(int id);

        Task<(Product?, string?)> UpdateAsync(int id, UpdateProductDto dto);

        PagedResultDto<ProductCreateResponseDto> GetPagedProducts(ProductQueryDto query);
        PagedResultDto<ProductCreateResponseDto> SearchProducts(ProductSearchQueryDto query);
        PagedResultDto<ProductCreateResponseDto> FilterProducts(ProductFilterDto filter);
        void UpdateStock(int productId, int quantity);
        Task<string> AddProductImageAsync(int productId, IFormFile image);
        Task<bool> RemoveProductImage(int imageId);
    }
}
