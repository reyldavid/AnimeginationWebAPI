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

        // PUT: api/Categories
        [HttpPut]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("PutCategory")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PutCategory([FromBody] Category categoryInput)
        {
            string userId = Request.UserId();

            Category category = new Category {
                CategoryID = categoryInput.CategoryID, 
                CategoryName = categoryInput.CategoryName, 
                Description = categoryInput.Description, 
                ImageFile = categoryInput.ImageFile
            };

            db.Categories.Add(category);
            db.SaveChanges();

            var response = new
            {
                CategoryID = category.CategoryID, 
                CategoryName = category.CategoryName,
                Description = category.Description, 
                ImageFile = category.ImageFile 
            };
            return Ok(response);
        }

        // POST: api/Categories
        [HttpPost]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("PostCategory")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PostCategory([FromBody] Category categoryInput)
        {
            if (!this.CategoryExists(categoryInput.CategoryID)) {
                return NotFound();
            }
            Category category = db.Categories.SingleOrDefault(item => 
                item.CategoryID == categoryInput.CategoryID);
            if (category == null) {
                return NotFound();
            }
            category.CategoryName = categoryInput.CategoryName;
            category.Description = categoryInput.Description;
            category.ImageFile = categoryInput.ImageFile;
            db.SaveChanges();

            var response = new {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName, 
                Description = category.Description,
                ImageFile = category.ImageFile 
            };
            return Ok(response);
        }

        // DELETE: api/categories/5
        [HttpDelete]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("DeleteCategory")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            // Delete Categories Record
            Category category = db.Categories.SingleOrDefault(item => item.CategoryID == id);

            if (category == null)
            {
                return NotFound();
            }

            db.Categories.Remove(category);
            //await db.SaveChangesAsync();
            db.SaveChanges();

            return Ok(category);
        }
        
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