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
    public class CategoryListController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Search/Haruhi
        [SwaggerOperation("GetCategoryList")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetCategoryList(string id)
        {
            var category = (from cat in db.Categories
                            where cat.CategoryID.ToString() == id
                            select cat)
                            .FirstOrDefault();

            var products =
                db.Products.Select(prod => new
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
                })
                .Where(prod => prod.CategoryName == category.CategoryName)
                .OrderByDescending(prod => prod.ProductYearCreated);

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