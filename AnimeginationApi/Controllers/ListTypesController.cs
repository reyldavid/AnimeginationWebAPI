using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace AnimeginationApi.Controllers
{
    public class ListTypesController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/ListTypes
        [SwaggerOperation("GetListTypes")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<object> GetListTypes()
        {
            var listTypes =
            db.ListTypes.Select(lt => new
            {
                ListTypeID = lt.ListTypeID,
                ListTypeName = lt.ListTypeName,
                Description = lt.Description
            }).AsEnumerable();

            return listTypes;
        }
        
        // GET: api/ListTypes/5
        [SwaggerOperation("GetListType")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetListType(int id)
        {
            var listType =
            db.ListTypes.Select(lt => new
            {
                ListTypeID = lt.ListTypeID,
                ListTypeName = lt.ListTypeName,
                Description = lt.Description
            })
            .Where(lt => lt.ListTypeID == id)
            .FirstOrDefault();

            return listType;
        }

        //// PUT: api/ListTypes/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutListType(int id, ListType listType)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != listType.ListTypeID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(listType).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ListTypeExists(id))
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

        //// POST: api/ListTypes
        //[ResponseType(typeof(ListType))]
        //public async Task<IHttpActionResult> PostListType(ListType listType)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.ListTypes.Add(listType);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = listType.ListTypeID }, listType);
        //}

        //// DELETE: api/ListTypes/5
        //[ResponseType(typeof(ListType))]
        //public async Task<IHttpActionResult> DeleteListType(int id)
        //{
        //    ListType listType = await db.ListTypes.FindAsync(id);
        //    if (listType == null)
        //    {
        //        return NotFound();
        //    }

        //    db.ListTypes.Remove(listType);
        //    await db.SaveChangesAsync();

        //    return Ok(listType);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ListTypeExists(int id)
        {
            return db.ListTypes.Count(e => e.ListTypeID == id) > 0;
        }
    }
}