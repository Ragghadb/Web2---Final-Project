using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalPtoject.Models
{
    public class my_purchase
    {
        public int Id { get; set; }
        public string name { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public DateTime buydate { get; set; } = DateTime.Now;
        public int price { get; set; }
        public int quantity { get; set; }

    }
}
