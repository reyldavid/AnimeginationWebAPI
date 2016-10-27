namespace AnimeginationApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserAccount
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(200)]
        public string StreetAddress { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        public virtual int StateId { get; set; }

        [StringLength(10)]
        public string ZipCode { get; set; }

        [StringLength(256)]
        public string CellPhoneNumber { get; set; }

        [StringLength(256)]
        public string HomePhoneNumber { get; set; }

        [Required]
        [StringLength(256)]
        public string EmailAddress { get; set; }

        public DateTime Created { get; set; }

        [StringLength(20)]
        public string CreditCardType { get; set; }

        [StringLength(20)]
        public string CreditCardNumber { get; set; }

        [StringLength(7)]
        public string CreditCardExpiration { get; set; }

        [ForeignKey("StateId")]
        public virtual State State { get; set; }
    }
}
