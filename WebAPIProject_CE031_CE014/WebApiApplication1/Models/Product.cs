using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiApplication1.Models
{
    public class Product
    {
        [DataMember(Order=1)]
        public int ProductId { get; set; }

        [DataMember(Order = 2)]
        public string ProductName { get; set; }

        [DataMember(Order = 3)]
        public int ProductPrice { get; set; }

        [DataMember(Order = 4)]
        public int Quantity { get; set; }

        [DataMember(Order = 5)]
        public string Description { get; set; }
    }
}