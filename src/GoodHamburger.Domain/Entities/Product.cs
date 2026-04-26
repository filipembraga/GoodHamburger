using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;  
        public decimal Price { get; set; }
        public ProductCategory Category { get; set; }
    }
}