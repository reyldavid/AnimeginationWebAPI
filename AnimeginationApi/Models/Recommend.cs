using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace AnimeginationApi.Models
{
    public partial class Recommendation
    {
        [Key]
        public int recommendId { get; set; }

        [Required]
        public int ratingId { get; set; }

        [Required]
        [StringLength(200)]
        public string title { get; set; }

        [Required]
        [StringLength(1000)]
        public string recommendation { get; set; }

        [Required]
        [StringLength(50)]
        public string reviewer { get; set; }

        [StringLength(200)]
        public string reviewerEmployer { get; set; }

        [StringLength(50)]
        public string employerUrl { get; set; }

        public DateTime created { get; set; }
    }
}
