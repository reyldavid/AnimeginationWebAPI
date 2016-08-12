namespace AnimeginationApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Rating
    {
        public int RatingID { get; set; }

        [Required]
        [StringLength(100)]
        public string RatingName { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
    }
}
