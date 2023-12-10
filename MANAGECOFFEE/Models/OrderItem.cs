using System.ComponentModel.DataAnnotations;

namespace ManagementCoffee.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CoffeeId { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }

        public virtual Coffee Coffee { get; set; }
    }
}
