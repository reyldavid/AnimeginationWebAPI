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

        // PUT: api/Media
        [HttpPut]
        [AdminRoleFilter]
        [SwaggerOperation("PutMedium")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PutMedium([FromBody] Medium mediumInput)
        {
            string userId = Request.UserId();

            Medium medium = new Medium {
                MediumID = mediumInput.MediumID, 
                MediumName = mediumInput.MediumName, 
                Description = mediumInput.Description
            };

            db.Media.Add(medium);
            db.SaveChanges();

            var response = new
            {
                MediumID = medium.MediumID, 
                MediumName = medium.MediumName,
                Description = medium.Description
            };
            return Ok(response);
        }

        // POST: api/Media
        [HttpPost]
        [AdminRoleFilter]
        [SwaggerOperation("PostMedium")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PostMedium([FromBody] Medium mediumInput)
        {
            if (!this.MediumExists(mediumInput.MediumID)) {
                return NotFound();
            }
            Medium medium = db.Media.SingleOrDefault(item => 
                item.MediumID == mediumInput.MediumID);
            if (medium == null) {
                return NotFound();
            }
            medium.MediumName = mediumInput.MediumName;
            medium.Description = mediumInput.Description;
            db.SaveChanges();

            var response = new {
                MediumID = medium.MediumID,
                MediumName = medium.MediumName, 
                Description = medium.Description
            };
            return Ok(response);
        }

        // // DELETE: api/Media/5
        // [ResponseType(typeof(Medium))]
        // public async Task<IHttpActionResult> DeleteMedium(int id)
        // {
        //    Medium medium = await db.Media.FindAsync(id);
        //    if (medium == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Media.Remove(medium);
        //    await db.SaveChangesAsync();

        //    return Ok(medium);
        // }

        // DELETE: api/media/5
        [HttpDelete]
        [AdminRoleFilter]
        [SwaggerOperation("DeleteMedium")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> DeleteMedium(int id)
        {
            // Delete Media Record
            Medium medium = db.Media.SingleOrDefault(item => item.MediumID == id);

            if (medium == null)
            {
                return NotFound();
            }

            db.Media.Remove(medium);
            //await db.SaveChangesAsync();
            db.SaveChanges();

            return Ok(medium);
        }

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