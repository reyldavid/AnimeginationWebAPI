using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AnimeginationApi.Services;
using AnimeginationApi.Filters;

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
            }).OrderBy(pr => pr.ProductTitle)
            .AsEnumerable();

            return products;
        }

        // GET: api/Products/5
        // [JwtTokenFilter]
        [SwaggerOperation("GetProducts")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetProducts(int id)
        {
            //string username = Request.UserName();

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

        [SwaggerOperation("PostProduct")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.RequestTimeout)]
        [SwaggerResponse(HttpStatusCode.ServiceUnavailable)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public HttpResponseMessage PostProduct([FromBody] ApiProduct apiProduct)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Unauthorized);

            //string apiKey = string.Empty;
            //HttpRequestMessage httpRequest = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;

            //if (httpRequest.Headers.Contains("ApiKey"))
            //{
            //    IEnumerable<string> apiKeys = httpRequest.Headers.GetValues("ApiKey");

            //    if (apiKeys != null)
            //    {
            //        string[] apiValues = apiKeys.ToArray<string>();
            //        if (apiValues.Length > 0)
            //        {
            //            apiKey = apiValues[0];
            //            if (!string.IsNullOrEmpty(apiKey))
            //            {
            //                if (apiKey.Equals("AnimeApiClientKey".GetConfigurationValue()))
            //                {
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
            //                }
            //            }
            //        }
            //    }
            //}
            return response;
        }

        // [Route("api/products")]
        [HttpPut]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("PutProduct")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public HttpResponseMessage PutProduct([FromBody] ApiProduct productInput)
        {
            Product product = db.Products.SingleOrDefault(pd => pd.ProductID == productInput.ProductID); 

            if (product == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            product.ProductCode = string.IsNullOrEmpty(productInput.ProductCode) ? product.ProductCode : productInput.ProductCode;
            product.ProductTitle = string.IsNullOrEmpty(productInput.ProductTitle) ? product.ProductTitle : productInput.ProductTitle;
            product.ProductDescription = string.IsNullOrEmpty(productInput.ProductDescription) ? product.ProductDescription : productInput.ProductDescription;
            product.CategoryID = productInput.CategoryID == 0 ? product.CategoryID : productInput.CategoryID;
            product.MediumID = productInput.MediumID == 0 ? product.MediumID : productInput.MediumID;
            product.OnSale =  productInput.OnSale;
            product.ProductAgeRating = string.IsNullOrEmpty(productInput.ProductAgeRating) ? product.ProductAgeRating : productInput.ProductAgeRating;
            product.ProductImageURL = string.IsNullOrEmpty(productInput.ProductImageURL) ? product.ProductImageURL : productInput.ProductImageURL;
            product.ProductLength = productInput.ProductLength == 0 ? product.ProductLength : productInput.ProductLength;
            product.ProductYearCreated = productInput.ProductYearCreated == 0 ? product.ProductYearCreated : productInput.ProductYearCreated;
            product.PublisherID = productInput.PublisherID == 0 ? product.PublisherID : productInput.PublisherID;
            product.RatingID = productInput.RatingID == 0 ? product.RatingID : productInput.RatingID;
            product.UnitPrice = productInput.UnitPrice == 0 ? product.UnitPrice : productInput.UnitPrice;
            product.YourPrice = productInput.YourPrice == 0 ? product.YourPrice : productInput.YourPrice;

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
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

        // DELETE: api/products/5
        [HttpDelete]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("DeleteProduct")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            // Delete Product Record
            Product product = db.Products.SingleOrDefault(item => item.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            //await db.SaveChangesAsync();
            db.SaveChanges();

            return Ok(product);
        }

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