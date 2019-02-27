using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeginationApi.Models
{
    public class OrderItemModel
    {
        public int orderitemid { get; set; }
        public int orderid { get; set; }
        public int productid { get; set; }
        public int quantity { get; set; }
        public decimal unitprice { get; set; }
        public DateTime itemdate { get; set; }
    }
}