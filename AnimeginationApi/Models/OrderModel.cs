using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeginationApi.Models
{
    public class OrderModel
    {
        public int orderid { get; set; }
        public string userid { get; set; }
        public decimal shipping { get; set; }
        public decimal taxes { get; set; }
        public decimal discounts { get; set; }
        public decimal totals { get; set; }
        public DateTime orderdate { get; set; }
        public bool ispurchased { get; set; }
        public string trackingnumber { get; set; }
        public string ordertype { get; set; }
    }
}