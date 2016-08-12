using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace AnimeginationApi.Controllers
{
    public class ListingsController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Listings
        [SwaggerOperation("GetListings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<object> GetListings()
        {
            var listings =
            db.Listings.Select(lists => new
            {
                ListingID = lists.ListingID,
                ListingTypeID = lists.ListTypeID,
                ListingTypeDescription = lists.ListType.Description,
                Rank = lists.Rank,
                EffectiveDate = lists.Effective,
                Expiration = lists.Expiration,
                ProductID = lists.Product.ProductID,
                ProductCode = lists.Product.ProductCode,
                ProductTitle = lists.Product.ProductTitle,
                ProductDescription = lists.Product.ProductDescription,
                UnitPrice = lists.Product.UnitPrice,
                YourPrice = lists.Product.YourPrice,
                CategoryName = lists.Product.Category.CategoryName,
                ProductAgeRating = lists.Product.ProductAgeRating,
                ProductLength = lists.Product.ProductLength,
                ProductYearCreated = lists.Product.ProductYearCreated,
                MediumName = lists.Product.Medium.MediumName,
                PublisherName = lists.Product.Publisher.PublisherName,
                OnSale = lists.Product.OnSale,
                RatingID = lists.Product.RatingID
            })
            .Where(list => list.EffectiveDate < DateTime.Now && list.Expiration > DateTime.Now)
            .AsEnumerable();

            return listings;

        }

        // GET: api/Listings
        [SwaggerOperation("GetListings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<object> GetListings(int id)
        {
            var listings =
            db.Listings.Select(lists => new
            {
                ListingID = lists.ListingID,
                ListingTypeID = lists.ListTypeID,
                ListingTypeDescription = lists.ListType.Description,
                Rank = lists.Rank,
                EffectiveDate = lists.Effective,
                Expiration = lists.Expiration,
                ProductID = lists.Product.ProductID,
                ProductCode = lists.Product.ProductCode,
                ProductTitle = lists.Product.ProductTitle,
                ProductDescription = lists.Product.ProductDescription,
                UnitPrice = lists.Product.UnitPrice,
                YourPrice = lists.Product.YourPrice,
                CategoryName = lists.Product.Category.CategoryName,
                ProductAgeRating = lists.Product.ProductAgeRating,
                ProductLength = lists.Product.ProductLength,
                ProductYearCreated = lists.Product.ProductYearCreated,
                MediumName = lists.Product.Medium.MediumName,
                PublisherName = lists.Product.Publisher.PublisherName,
                OnSale = lists.Product.OnSale,
                RatingID = lists.Product.RatingID
            })
            .Where(list => list.EffectiveDate < DateTime.Now
                    && list.Expiration > DateTime.Now
                    && list.ListingTypeID == id )
            .AsEnumerable();

            return listings;
        }

        //// PUT: api/Listings/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutListing(int id, Listing listing)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != listing.ListingID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(listing).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ListingExists(id))
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

        //// POST: api/Listings
        //[ResponseType(typeof(Listing))]
        //public async Task<IHttpActionResult> PostListing(Listing listing)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Listings.Add(listing);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = listing.ListingID }, listing);
        //}

        //// DELETE: api/Listings/5
        //[ResponseType(typeof(Listing))]
        //public async Task<IHttpActionResult> DeleteListing(int id)
        //{
        //    Listing listing = await db.Listings.FindAsync(id);
        //    if (listing == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Listings.Remove(listing);
        //    await db.SaveChangesAsync();

        //    return Ok(listing);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ListingExists(int id)
        {
            return db.Listings.Count(e => e.ListingID == id) > 0;
        }

        private bool ListingTypeExists(int id)
        {
            return db.Listings.Count(e => e.ListTypeID == id) > 0;
        }
    }
}