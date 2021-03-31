using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1_client.Models
{
    public class AddProductViewModel
    {
        
        public int ProductId { get; set; }

        [Required,StringLength(50,ErrorMessage ="Product name sholud be short")]
        public string ProductName { get; set; }

        [Required,RegularExpression("^[0-9]*$",ErrorMessage ="Price must be in digits!")]
        public int ProductPrice { get; set; }

        [Required, RegularExpression("^[0-9]*$", ErrorMessage = "Price must be in digits!")]
        public int Quantity { get; set; }

        [Required, StringLength(500, ErrorMessage = "Description should be less than 500 characters")]
        public string Description { get; set; }
    }
}