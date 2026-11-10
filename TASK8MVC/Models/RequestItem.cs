namespace TASK8MVC.Models
{
    public class RequestItem
    {
        public int Id { get; set; }
        public int SupplyRequestId { get; set; }
        public SupplyRequest SupplyRequest { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}