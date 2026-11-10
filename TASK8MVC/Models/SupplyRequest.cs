using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace TASK8MVC.Models
{
    public class SupplyRequest
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; } = "";
        public string UserId { get; set; } = "";
        public IdentityUser User { get; set; } = null!;
        public List<RequestItem> RequestItems { get; set; } = new List<RequestItem>();
    }
}