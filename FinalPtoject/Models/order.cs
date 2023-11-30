using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalPtoject.Models
{
    public class order
    {
        public int Id { get; set; }
        public int itemid { get; set; }
        public int userid { get; set; }
        public int quantity { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime buydate { get; set; }
    }
}
