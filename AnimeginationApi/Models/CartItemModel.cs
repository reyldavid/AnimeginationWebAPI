using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeginationApi.Models
{
    public class CartItemModel
    {
        public string ordertype { get; set; }
        public int productid { get; set; }
        public int quantity { get; set; }
        public decimal unitprice { get; set; }
    }
}
