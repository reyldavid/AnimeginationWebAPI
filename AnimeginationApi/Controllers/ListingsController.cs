using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;
using AnimeginationApi.Filters;
using System.Threading.Tasks;

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
            .OrderBy(lt => lt.ListingTypeID)
            .ThenByDescending(ld => ld.Expiration).ThenBy(lr => lr.Rank)
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

        [HttpPut]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("PutListing")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PutListing([FromBody] Listing listingInput)
        {
            string userId = Request.UserId();

            Listing listing = new Listing {
                ListTypeID = listingInput.ListTypeID, 
                Rank = listingInput.Rank, 
                ProductID = listingInput.ProductID,
                Effective = listingInput.Effective,
                Expiration = listingInput.Expiration,
                Created = DateTime.Now
            };

            db.Listings.Add(listing);
            db.SaveChanges();

            var response = new
            {
                ListTypeID = listing.ListTypeID, 
                Rank = listing.Rank, 
                ProductID = listing.ProductID,
                Effective = listing.Effective,
                Expiration = listing.Expiration,
                Created = listing.Created
            };
            return Ok(response);
        }

        [HttpPost]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("PostListing")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PostListing([FromBody] Listing listingInput)
        {
            if (!this.ListingExists(listingInput.ListingID)) {
                return NotFound();
            }
            Listing listing = db.Listings.SingleOrDefault(item => 
                item.ListingID == listingInput.ListingID);
            if (listing == null) {
                return NotFound();
            }
            listing.ListTypeID = listingInput.ListTypeID;
            listing.Rank = listingInput.Rank;
            listing.ProductID = listingInput.ProductID;
            listing.Effective = listingInput.Effective;
            listing.Expiration = listingInput.Expiration;
            db.SaveChanges();

            var response = new {
                ListingID = listing.ListingID,
                ListTypeID = listing.ListTypeID,
                Rank = listing.Rank,
                ProductID = listing.ProductID,
                Effective = listing.Effective,
                Expiration = listing.Expiration
            };
            return Ok(response);
        }

        //// DELETE: api/Listings/5
        [HttpDelete]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("DeleteListing")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> DeleteListing(int id)
        {
            // Delete Media Record
            Listing listing = db.Listings.SingleOrDefault(item => item.ListingID == id);

            if (listing == null)
            {
                return NotFound();
            }

            db.Listings.Remove(listing);
            //await db.SaveChangesAsync();
            db.SaveChanges();

            return Ok(listing);
        }

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