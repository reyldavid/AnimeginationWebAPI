namespace AnimeginationApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserFeedback
    {
        [Key]
        public int FeedbackId { get; set; }

        public int UserId { get; set; }

        public string FeedbackType { get; set; }

        public int ProductId { get; set; }

        public int RatingID { get; set; }

        public string Title { get; set; }

        public string Feedback { get; set; }

        public DateTime Created { get; set; }

        public virtual Product Product { get; set; }

        public virtual UserProfile UserProfile { get; set; }
    }
}
