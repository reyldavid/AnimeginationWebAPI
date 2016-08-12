using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;
using System.Net.Http;
using System.Web;
using AnimeginationApi.Services;

namespace AnimeginationApi.Controllers
{
    public class ProductsController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Products
        [SwaggerOperation("GetProducts")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<object> GetProducts()
        {
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
                ProductAgeRating =prod.ProductAgeRating,
                ProductLength = prod.ProductLength,
                ProductYearCreated = prod.ProductYearCreated,
                MediumName = prod.Medium.MediumName,
                PublisherName = prod.Publisher.PublisherName,
                OnSale = prod.OnSale,
                RatingID = prod.RatingID
            }).AsEnumerable();

            return products;
        }

        // GET: api/Products/5
        [SwaggerOperation("GetProducts")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetProducts(int id)
        {
            var product =
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
            .Where(prod => prod.ProductID == id);

            return product;
        }

        //// PUT: api/Products/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutProduct(int id, Product product)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != product.ProductID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(product).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ProductExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/Products
        //[ResponseType(typeof(Product))]
        //public async Task<IHttpActionResult> PostProduct(Product product)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Products.Add(product);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = product.ProductID }, product);
        //}

        public HttpResponseMessage PostProduct([FromBody] ApiProduct apiProduct)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Unauthorized);

            string apiKey = string.Empty;
            HttpRequestMessage httpRequest = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;

            if (httpRequest.Headers.Contains("ApiKey"))
            {
                IEnumerable<string> apiKeys = httpRequest.Headers.GetValues("ApiKey");

                if (apiKeys != null)
                {
                    string[] apiValues = apiKeys.ToArray<string>();
                    if (apiValues.Length > 0)
                    {
                        apiKey = apiValues[0];
                        if (!string.IsNullOrEmpty(apiKey))
                        {
                            if (apiKey.Equals("AnimeApiClientKey".GetConfigurationValue()))
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK);

                                Product product = new Product
                                {
                                    ProductCode = apiProduct.ProductCode,
                                    ProductTitle = apiProduct.ProductTitle,
                                    ProductDescription = apiProduct.ProductDescription,
                                    UnitPrice = apiProduct.UnitPrice,
                                    YourPrice = apiProduct.YourPrice,
                                    CategoryID = apiProduct.CategoryID,
                                    ProductAgeRating = apiProduct.ProductAgeRating,
                                    ProductLength = apiProduct.ProductLength,
                                    ProductYearCreated = apiProduct.ProductYearCreated,
                                    MediumID = apiProduct.MediumID,
                                    PublisherID = apiProduct.PublisherID,
                                    ProductImageURL = apiProduct.ProductImageURL,
                                    OnSale = apiProduct.OnSale,
                                    RatingID = apiProduct.RatingID
                                };

                                db.Products.Add(product);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            return response;
        }

        //// DELETE: api/Products/5
        //[ResponseType(typeof(Product))]
        //public async Task<IHttpActionResult> DeleteProduct(int id)
        //{
        //    Product product = await db.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Products.Remove(product);
        //    await db.SaveChangesAsync();

        //    return Ok(product);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductID == id) > 0;
        }
    }
}