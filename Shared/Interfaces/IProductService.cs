using Shared.Models;

namespace Shared.Services
{
    public interface IProductService
    {
        bool AddToList(Product product);
        IEnumerable<Product> GetAllProducts();
        public bool SaveProductsToJson();
        public bool LoadProductsFromJson();
        public bool DeleteProduct(Product product);
        public void UpdateProduct(Product product, string newProductName, decimal newProductPrice, string newCategoryName);
    }
}