namespace AnimeginationApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Listing
    {
        public int ListingID { get; set; }

        public int ListTypeID { get; set; }

        public int Rank { get; set; }

        public int ProductID { get; set; }

        public DateTime Effective { get; set; }

        public DateTime Expiration { get; set; }

        public DateTime Created { get; set; }

        public virtual ListType ListType { get; set; }

        public virtual Product Product { get; set; }
    }
}
