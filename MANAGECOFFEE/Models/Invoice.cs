using ManagementCoffee.Models;

namespace MANAGECOFFEE.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public virtual Order Order { get; set; }
    }
}
