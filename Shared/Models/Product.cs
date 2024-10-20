using System.Runtime;

namespace Shared.Models
{
    public class Product
    {
        public Category ProductCategory { get; set; } = new();
        public string ProductId { get; set; }  = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
