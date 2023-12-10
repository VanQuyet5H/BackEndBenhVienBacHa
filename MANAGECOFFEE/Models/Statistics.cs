namespace MANAGECOFFEE.Models
{
    public class Statistics
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public int NumberOrders { get; set; }
        public decimal ProductSales { get; set; }
        public int ProductOrders { get; set; }
        public decimal CustomerSales { get; set; }
        public int CustomerOrders { get; set; }
    }
}
