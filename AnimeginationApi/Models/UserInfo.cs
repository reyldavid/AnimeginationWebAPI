namespace AnimeginationApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(200)]
        public string StreetAddress { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        public int StateId { get; set; }

        [StringLength(10)]
        public string ZipCode { get; set; }

        public string CellPhoneNumber { get; set; }

        public string HomePhoneNumber { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        public DateTime Created { get; set; }

        public string CreditCardType { get; set; }

        [StringLength(20)]
        public string CreditCardNumber { get; set; }

        [StringLength(7)]
        public string CreditCardExpiration { get; set; }

        public virtual State State { get; set; }

        //public virtual UserProfile UserProfile { get; set; }
    }
}
