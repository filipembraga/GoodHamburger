namespace GoodHamburger.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new();


        public decimal Subtotal => Items.Sum(i => i.Product.Price);
        public decimal Discount {get; set; }
        public decimal Total => Subtotal - (Subtotal * Discount);

    }
}