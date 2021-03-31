using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiApplication1.Models
{
    public class History
    {
        public int BillNo { get; set; }

        public int productid { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }

        public int Total { get; set; }
    }
}