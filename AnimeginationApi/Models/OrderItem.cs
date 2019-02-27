namespace AnimeginationApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderItem
    {
        public int OrderItemID { get; set; }

        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public double FinalUnitPrice { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }

        public DateTime ItemDate { get; set; }
    }
}
