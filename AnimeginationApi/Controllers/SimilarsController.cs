using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;
using System.Text.RegularExpressions;
using AnimeginationApi.Services;

namespace AnimeginationApi.Controllers
{
    public class SimilarsController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        //// GET: api/Similars
        //public IQueryable<Product> GetProducts()
        //{
        //    return db.Products;
        //}

        // GET: api/Similars/5
        [SwaggerOperation("GetSimilars")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetSimilars(int id)
        {
            Product product = db.Products.Where(p => p.ProductID == id).FirstOrDefault();

            List<Product> recommendations = new List<Product>();

            //string itemSubCode = new String(product.ProductCode.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
            string itemSubCode = new String(product.ProductCode.Where(c => (c < '0' || c > '9')).ToArray());

            //string title = Regex.Replace(product.ProductTitle.ToLower(), @"[0-9\-]", " ");
            string title = Regex.Replace(product.ProductTitle.ToLower(), @"[0-9]", " ");

            /// Below is the sample test case
            //title = "THE  melancholy , of haruhi  and suzumiya  a movie: Vol 3  Dvd 2: the Complete volume  Blu-Ray Box Set  1".ToLower();
            //title = Regex.Replace(title, @"[0-9\-]", " ");

            List<string> subTitles = title
                .Replace(" a ", " ")
                .Replace(" and ", " ")
                .Replace("the ", "")
                .Replace("of ", "")
                .Replace("complete", "")
                .Replace("box ", "")
                .Replace("set ", "")
                .Replace("vhs", "")
                .Replace("dvd", "")
                .Replace("blu ray", "")
                .Replace("blu-ray", "")
                .Replace("bd ", "")
                .Replace("vol ", "")
                .Replace("volume ", "")
                .Replace("movie ", "")
                .Replace("director", "")
                .Replace("uncut ", "")
                .Replace("premium ", "")
                .Replace("limited ", "")
                .Replace("edition ", "")
                .Replace("ova ", "")
                .Replace("manga ", "")
                .Replace("season ", "")
                .Replace("part ", "")
                .Replace("novel ", "")
                .Replace("hardcover ", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("[", "")
                .Replace("]", "")
                .Split(new char[] { ' ', ',', ':' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            /// First pass for Product Code relationship
            foreach (var prod in db.Products)
            {
                //string prodSubCode = new String(prod.ProductCode.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
                string prodSubCode = new String(prod.ProductCode.Where(c => (c < '0' || c > '9')).ToArray());

                if (itemSubCode.Length > 2 && prodSubCode.Length > 2)
                {
                    if (prodSubCode.Equals(itemSubCode) ||
                        prodSubCode.Contains(itemSubCode) ||
                        itemSubCode.Contains(prodSubCode))
                    {
                        if (prod.CategoryID == product.CategoryID)
                        {
                            if (!recommendations.Contains(prod))
                            {
                                if (prod.ProductID != product.ProductID)
                                {
                                    recommendations.Add(prod);
                                }
                            }
                        }
                    }
                }
            }

            /// Second pass for Product Title relationship
            foreach (var prod in db.Products)
            {
                if (subTitles.Any(s => (s.Length > 2 && prod.ProductTitle.ToLower().Contains(s))) ||
                    (itemSubCode.Length > 2 && prod.ProductTitle.ToLower().Contains(itemSubCode)))
                {
                    if (prod.CategoryID == product.CategoryID)
                    {
                        if (!recommendations.Contains(prod))
                        {
                            if (prod.ProductID != product.ProductID)
                            {
                                recommendations.Add(prod);
                            }
                        }
                    }
                }
            }

            /// Third pass for Product Description relationship
            foreach (var prod in db.Products)
            {
                if (subTitles.Any(s => (s.Length > 2 && prod.ProductDescription.ToLower().Contains(s))) ||
                    (itemSubCode.Length > 2 && prod.ProductDescription.ToLower().Contains(itemSubCode)))
                {
                    if (prod.CategoryID == product.CategoryID)
                    {
                        if (!recommendations.Contains(prod))
                        {
                            if (prod.ProductID != product.ProductID)
                            {
                                recommendations.Add(prod);
                            }
                        }
                    }
                }
            }

            /// Fourth pass for Product Category relationship
            foreach (var prod in db.Products.Where(p => p.CategoryID == product.CategoryID)
                .OrderByDescending(p => p.ProductYearCreated))
            {
                if (!recommendations.Contains(prod))
                {
                    if (prod.ProductID != product.ProductID)
                    {
                        recommendations.Add(prod);
                    }
                }
            }

            int SimilarsMaxNumber = "SimilarsMaxNumber".GetConfigurationNumericValue();

            var products =
            recommendations.Select(prod => new
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
            .Take(SimilarsMaxNumber);

            return products;
        }

        //// GET: api/Similars/5
        //[ResponseType(typeof(Product))]
        //public async Task<IHttpActionResult> GetProduct(int id)
        //{
        //    Product product = await db.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(product);
        //}

        //// PUT: api/Similars/5
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

        //// POST: api/Similars
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

        //// DELETE: api/Similars/5
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

        //private bool ProductExists(int id)
        //{
        //    return db.Products.Count(e => e.ProductID == id) > 0;
        //}
    }
}