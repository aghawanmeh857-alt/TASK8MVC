using System.ComponentModel.DataAnnotations;

namespace TASK8MVC.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        [Range(0.01, 1000000)]
        public double Price { get; set; }
        [Range(0, 1000000)]
        public int Quantity { get; set; }
    }
}