using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace AnimeginationApi.Controllers
{
    public class MediaController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Media
        [SwaggerOperation("GetMedia")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<object> GetMedia()
        {
            var media =
            db.Media.Select(med => new
            {
                MediumID = med.MediumID,
                MediumName = med.MediumName, 
                Description = med.Description
            }).AsEnumerable();

            return media;
        }

        // GET: api/Media/5
        [SwaggerOperation("GetMedia")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetMedia(int id)
        {
            var medium =
            db.Media.Select(med => new
            {
                MediumID = med.MediumID,
                MediumName = med.MediumName,
                Description = med.Description
            })
            .Where(med => med.MediumID == id);

            return medium;
        }

        //// PUT: api/Media/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutMedium(int id, Medium medium)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != medium.MediumID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(medium).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MediumExists(id))
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

        //// POST: api/Media
        //[ResponseType(typeof(Medium))]
        //public async Task<IHttpActionResult> PostMedium(Medium medium)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Media.Add(medium);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = medium.MediumID }, medium);
        //}

        //// DELETE: api/Media/5
        //[ResponseType(typeof(Medium))]
        //public async Task<IHttpActionResult> DeleteMedium(int id)
        //{
        //    Medium medium = await db.Media.FindAsync(id);
        //    if (medium == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Media.Remove(medium);
        //    await db.SaveChangesAsync();

        //    return Ok(medium);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MediumExists(int id)
        {
            return db.Media.Count(e => e.MediumID == id) > 0;
        }
    }
}