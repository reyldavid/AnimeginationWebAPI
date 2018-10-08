namespace AnimeginationApi.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class OrderType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderType()
        {
            Orders = new HashSet<Order>();
        }

        public int OrderTypeID { get; set; }

        [Required]
        [StringLength(100)]
        public string OrderName { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
