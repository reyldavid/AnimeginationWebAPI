namespace AnimeginationApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            Listings = new HashSet<Listing>();
            OrderItems = new HashSet<OrderItem>();
            UserFeedbacks = new HashSet<UserFeedback>();
        }

        public int ProductID { get; set; }

        [Required]
        [StringLength(10)]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductTitle { get; set; }

        [Required]
        [StringLength(4000)]
        public string ProductDescription { get; set; }

        public double UnitPrice { get; set; }

        public double YourPrice { get; set; }

        public virtual int CategoryID { get; set; }

        [StringLength(20)]
        public string ProductAgeRating { get; set; }

        public int? ProductLength { get; set; }

        public int? ProductYearCreated { get; set; }

        public virtual int? MediumID { get; set; }
        
        public virtual int PublisherID { get; set; }

        public string ProductImageURL { get; set; }

        public bool OnSale { get; set; }

        public int RatingID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Listing> Listings { get; set; }

        [ForeignKey("MediumID")]
        public virtual Medium Medium { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        [ForeignKey("PublisherID")]
        public virtual Publisher Publisher { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserFeedback> UserFeedbacks { get; set; }
    }
}
