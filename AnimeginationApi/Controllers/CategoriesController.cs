using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace AnimeginationApi.Controllers
{
    public class CategoriesController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Categories
        [SwaggerOperation("GetCategories")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<object> GetCategories()
        {
            var categories =
            db.Categories.Select(cat => new
            {
                CategoryID = cat.CategoryID,
                CategoryName = cat.CategoryName,
                Description = cat.Description,
                ImageFile = cat.ImageFile
            }).AsEnumerable();

            return categories;
        }

        [SwaggerOperation("GetCategory")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetCategory(int id)
        {
            var category =
            db.Categories.Select(cat => new
            {
                CategoryID = cat.CategoryID,
                CategoryName = cat.CategoryName,
                Description = cat.Description,
                ImageFile = cat.ImageFile
            })
            .Where(cat => cat.CategoryID == id);

            return category;
        }

        //// PUT: api/Categories/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutCategory(int id, Category category)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != category.CategoryID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(category).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CategoryExists(id))
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

        //// POST: api/Categories
        //[ResponseType(typeof(Category))]
        //public async Task<IHttpActionResult> PostCategory(Category category)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Categories.Add(category);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = category.CategoryID }, category);
        //}

        //// DELETE: api/Categories/5
        //[ResponseType(typeof(Category))]
        //public async Task<IHttpActionResult> DeleteCategory(int id)
        //{
        //    Category category = await db.Categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Categories.Remove(category);
        //    await db.SaveChangesAsync();

        //    return Ok(category);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(int id)
        {
            return db.Categories.Count(e => e.CategoryID == id) > 0;
        }
    }
}