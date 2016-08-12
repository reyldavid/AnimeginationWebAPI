namespace AnimeginationApi.Models
{
    public class ApiProduct
    {
        public int ProductID { get; set; }

        public string ProductCode { get; set; }

        public string ProductTitle { get; set; }

        public string ProductDescription { get; set; }

        public double UnitPrice { get; set; }

        public double YourPrice { get; set; }

        public int CategoryID { get; set; }

        public string ProductAgeRating { get; set; }

        public int? ProductLength { get; set; }

        public int? ProductYearCreated { get; set; }

        public int? MediumID { get; set; }

        public int PublisherID { get; set; }

        public string ProductImageURL { get; set; }

        public bool OnSale { get; set; }

        public int RatingID { get; set; }
    }
}