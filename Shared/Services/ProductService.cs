using Newtonsoft.Json;
using Shared.Models;

namespace Shared.Services
{
    public class ProductService(FileService fileService) : IProductService
    {
        private readonly FileService _fileService = fileService;
        private List<Product> _products = [];

        #region CRUD
        public bool AddToList(Product product)
        {
            try
            {
                LoadProductsFromJson();

                if (!_products.Any(p => p.Name == product.Name))
                {
                    _products.Add(product);
                    if (SaveProductsToJson())
                        return true;
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            LoadProductsFromJson();
            return _products;
        }

        public void UpdateProduct(Product product, string newProductName, decimal newProductPrice, string newCategoryName)
        {
            var existingProduct = _products.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existingProduct != null)
            {
                existingProduct.Name = newProductName;
                existingProduct.Price = newProductPrice;
                existingProduct.ProductCategory.CategoryName = newCategoryName;

                SaveProductsToJson();
            }
        }

        public bool DeleteProduct(Product product)
        {
            LoadProductsFromJson();

            try
            {
                if (_products.Any(p => p.ProductId == product.ProductId))
                {
                    _products.RemoveAll(p => p.ProductId == product.ProductId);

                    if (SaveProductsToJson())
                        return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public bool SaveProductsToJson()
        {
            return _fileService.SaveToFile(JsonConvert.SerializeObject(_products, Formatting.Indented));
        }

        public bool LoadProductsFromJson()
        {
            var json = _fileService.LoadFromFile();
            if (!string.IsNullOrEmpty(json))
            {
                _products = JsonConvert.DeserializeObject<List<Product>>(json)!;
                return true;
            }
            return false;
        }

        #endregion

    }
}
