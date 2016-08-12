using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace AnimeginationApi.Controllers
{
    public class RatingsController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Ratings
        [SwaggerOperation("GetRatings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<object> GetRatings()
        {
            var ratings =
            db.Ratings.Select(rate => new
            {
                RatingID = rate.RatingID,
                RatingName = rate.RatingName,
                Description = rate.Description
            }).AsEnumerable();

            return ratings;
        }

        // GET: api/Ratings/5
        [SwaggerOperation("GetRatings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetRatings(int id)
        {
            var rating =
            db.Ratings.Select(rate => new
            {
                RatingID = rate.RatingID,
                RatingName = rate.RatingName,
                Description = rate.Description
            })
            .Where(rate => rate.RatingID == id);

            return rating;
        }

        //// PUT: api/Ratings/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutRating(int id, Rating rating)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != rating.RatingID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(rating).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!RatingExists(id))
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

        //// POST: api/Ratings
        //[ResponseType(typeof(Rating))]
        //public async Task<IHttpActionResult> PostRating(Rating rating)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Ratings.Add(rating);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = rating.RatingID }, rating);
        //}

        //// DELETE: api/Ratings/5
        //[ResponseType(typeof(Rating))]
        //public async Task<IHttpActionResult> DeleteRating(int id)
        //{
        //    Rating rating = await db.Ratings.FindAsync(id);
        //    if (rating == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Ratings.Remove(rating);
        //    await db.SaveChangesAsync();

        //    return Ok(rating);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RatingExists(int id)
        {
            return db.Ratings.Count(e => e.RatingID == id) > 0;
        }
    }
}