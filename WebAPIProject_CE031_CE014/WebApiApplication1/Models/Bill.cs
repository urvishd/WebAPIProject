using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiApplication1.Models
{
    public class Bill
    {
        public int id { get; set; }

        public string Name { get; set; }

        public DateTime date { get; set; }

        public int total { get; set; }

    }
}