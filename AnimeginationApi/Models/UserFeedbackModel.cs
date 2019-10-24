using System;
using System.Collections.Generic;

namespace AnimeginationApi.Models
{
    public class UserFeedbackModel
    {
        public int feedbackId { get; set; }
        public string userid { get; set; }
        public string feedbackType { get; set; }
        public int productId { get; set; }
        public int ratingId { get; set; }
        public string title { get; set; }
        public string feedback { get; set; }
        public DateTime created { get; set; }
    }
}
