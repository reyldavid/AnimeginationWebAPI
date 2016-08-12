using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace AnimeginationApi.Controllers
{
    public class PublishersController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Publishers
        [SwaggerOperation("GetPublishers")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<object> GetPublishers()
        {
            var publishers =
            db.Publishers.Select(pub => new
            {
                PublisherID = pub.PublisherID,
                PublisherName = pub.PublisherName,
                Description = pub.Description
            }).AsEnumerable();

            return publishers;
        }

        // GET: api/Publishers/5
        [SwaggerOperation("GetPublishers")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetPublishers(int id)
        {
            var publisher =
            db.Publishers.Select(pub => new
            {
                PublisherID = pub.PublisherID,
                PublisherName = pub.PublisherName,
                Description = pub.Description
            })
            .Where(pub => pub.PublisherID == id);

            return publisher;
        }

        //// PUT: api/Publishers/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutPublisher(int id, Publisher publisher)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != publisher.PublisherID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(publisher).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!PublisherExists(id))
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

        //// POST: api/Publishers
        //[ResponseType(typeof(Publisher))]
        //public async Task<IHttpActionResult> PostPublisher(Publisher publisher)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Publishers.Add(publisher);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = publisher.PublisherID }, publisher);
        //}

        //// DELETE: api/Publishers/5
        //[ResponseType(typeof(Publisher))]
        //public async Task<IHttpActionResult> DeletePublisher(int id)
        //{
        //    Publisher publisher = await db.Publishers.FindAsync(id);
        //    if (publisher == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Publishers.Remove(publisher);
        //    await db.SaveChangesAsync();

        //    return Ok(publisher);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PublisherExists(int id)
        {
            return db.Publishers.Count(e => e.PublisherID == id) > 0;
        }
    }
}