using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;
using AnimeginationApi.Services;

namespace AnimeginationApi.Controllers
{
    public class SearchController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Search/Haruhi
        [SwaggerOperation("GetSearch")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetSearch(string id)
        {
            List<Product> candidates = new List<Product>();

            string searchText = id.ToLower();

            /// First pass for Product Code relationship
            foreach (var prod in db.Products)
            {
                if (prod.ProductCode.ToLower().Contains(searchText) || 
                    searchText.Contains(prod.ProductCode.ToLower()))
                {
                    if (!candidates.Contains(prod))
                    {
                        candidates.Add(prod);
                    }
                }
            }

            /// Second pass for Product Title relationship
            foreach (var prod in db.Products)
            {
                if (prod.ProductTitle.ToLower().Contains(searchText))
                {
                    if (!candidates.Contains(prod))
                    {
                        candidates.Add(prod);
                    }
                }
            }

            /// Third pass for Product Description relationship
            foreach (var prod in db.Products)
            {
                if (prod.ProductDescription.ToLower().Contains(searchText))
                {
                    if (!candidates.Contains(prod))
                    {
                        candidates.Add(prod);
                    }
                }
            }

            int SearchMaxNumber = "SearchMaxNumber".GetConfigurationNumericValue();

            var products =
                candidates.Select(prod => new
                {
                    ProductID = prod.ProductID,
                    ProductCode = prod.ProductCode,
                    ProductTitle = prod.ProductTitle,
                    ProductDescription = prod.ProductDescription,
                    UnitPrice = prod.UnitPrice,
                    YourPrice = prod.YourPrice,
                    CategoryName = prod.Category.CategoryName,
                    ProductAgeRating = prod.ProductAgeRating,
                    ProductLength = prod.ProductLength,
                    ProductYearCreated = prod.ProductYearCreated,
                    MediumName = prod.Medium.MediumName,
                    PublisherName = prod.Publisher.PublisherName,
                    OnSale = prod.OnSale,
                    RatingID = prod.RatingID
                }).AsEnumerable()
                .Take(SearchMaxNumber);

            return products;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}