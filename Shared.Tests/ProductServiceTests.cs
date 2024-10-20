using FluentAssertions;
using Newtonsoft.Json;
using Shared.Models;
using Shared.Services;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Shared.Tests
{
    public class ProductServiceTests
    {
        private static readonly string baseDirectory = AppContext.BaseDirectory;
        private readonly string _filePath = Path.Combine(baseDirectory, "products.json");

        private void RefreshFile()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        } 

        [Fact]
        public void AddToList__Should__AddNewProductToList__ReturnTrue()
        {
            RefreshFile();

            // Arrange  
            Product product = new()
            {
                ProductId = "123",
                Name = "Test Name",
                Price = 1.0m,
                ProductCategory = { CategoryName = "Test Category" }
            };

            FileService fileService = new(_filePath);
            ProductService productService = new(fileService);

            // Act
            var addResult = productService.AddToList(product);
            var productList = productService.GetAllProducts();

            // Assert
            addResult.Should().BeTrue();
            productList.Should().HaveCount(1).And.ContainSingle(p => p.ProductId == product.ProductId);
        }

        [Fact]

        public void LoadProductsFromJson__Should__ReadJsonFile__ReturnProductList()
        {
            RefreshFile();

            // Arrange
            FileService fileService = new(_filePath);
            ProductService productService = new(fileService);
            Product product = new()
            {
                ProductId = "123",
                Name = "Test Name",
                Price = 1.0m,
                ProductCategory = { CategoryName = "Test Category" },
            };

            // Act
            var addProduct = productService.AddToList(product);
            var loadResult = productService.LoadProductsFromJson();
            var productList = productService.GetAllProducts();

            // Assert
            addProduct.Should().BeTrue();
            loadResult.Should().BeTrue();
            productList.Should().HaveCount(1).And.ContainSingle(productList => productList.ProductId == product.ProductId);
            File.Exists(_filePath).Should().BeTrue();

        }

        [Fact]

        public void SaveProductsToJson__Should__SaveProductListToJsonFile__ReturnTrue()
        {
            RefreshFile();

            // Arrange
            FileService fileService = new(_filePath);
            ProductService productService = new(fileService);
            Product product = new()
            {
                ProductId = "123",
                Name = "Test Name",
                Price = 1.0m,
                ProductCategory = { CategoryName = "Test Category" },
            };

            // Act 
            var saveResult = productService.AddToList(product); // AddToList method executes SaveProductsToJson().
            var productList = productService.GetAllProducts();
            var file = File.ReadAllText(_filePath);

            // Assert
            saveResult.Should().BeTrue();
            productList.Should().HaveCount(1).And.ContainSingle(p => p.ProductId == product.ProductId);
            File.Exists(_filePath).Should().BeTrue();
            file.Should().Contain(product.ProductId, product.Price.ToString(), product.Name, product.ProductCategory.CategoryName);
        }

        [Fact]

        public void DeleteProduct__Should__DeleteProductFromList__ReturnTrue()
        {
            RefreshFile();

            // Arrange 
            Product product = new()
            {
                ProductId = "123",
                Name = "Test Name",
                Price = 1.0m,
                ProductCategory = { CategoryName = "Test Name" }
            };

            FileService fileService = new(_filePath);
            ProductService productService = new(fileService);

            // Act
            productService.AddToList(product);
            var result = productService.DeleteProduct(product);
            var productList = productService.GetAllProducts();

            // Assert
            result.Should().BeTrue();
            productList.Should().BeEmpty();
        }

        [Fact]

        public void UpdateProduct__Should__SuccessfullyUpdateProduct()
        {
            RefreshFile();

            // Arrange 
            Product initialproduct = new()
            {
                ProductId = "123",
                Name = "Test Name",
                Price = 1.0m,
                ProductCategory = { CategoryName = "Test Category" }
            };

            string updatedName = "Success";
            decimal updatedPrice = 10.0m;
            string updatedCategory = "Success";

            FileService fileService = new(_filePath);
            ProductService productService = new(fileService);

            // Act
            productService.AddToList(initialproduct);
            productService.UpdateProduct(initialproduct, updatedName, updatedPrice, updatedCategory);
            var productList = productService.GetAllProducts();

            // Assert 
            productList.Should().HaveCount(1).And.ContainSingle(p => p.ProductId == initialproduct.ProductId);
            initialproduct.Name.Should().Be(updatedName);
            initialproduct.Price.Should().Be(updatedPrice);
            initialproduct.ProductCategory.CategoryName.Should().Be(updatedCategory);       
        }

    }
}
