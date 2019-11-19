using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Data;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;
using AnimeginationApi.Filters;
using System.Threading.Tasks;


namespace AnimeginationApi.Controllers
{
    public class RecommendsController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Recommends
        [HttpGet]
        [Route("api/Recommends")]
        [SwaggerOperation("GetRecommends")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<object> GetRecommends()
        {
            var recommends =
            db.Recommendations.Select(rec => new
            {
                recommendId = rec.recommendId,
                ratingId = rec.ratingId,
                title = rec.title,
                recommendation = rec.recommendation,
                reviewer = rec.reviewer,
                reviewerEmployer = rec.reviewerEmployer,
                employerUrl = rec.employerUrl,
                created = rec.created
            }).OrderByDescending(r => r.created)
            .AsEnumerable();

            return recommends;
        }

        // PUT: api/Recommends
        [Route("api/Recommends")]
        [HttpPut]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("PutRecommend")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PutRecommend ([FromBody] Recommendation recInput)
        {
            string userId = Request.UserId();

            Recommendation rec = new Recommendation
            {
                ratingId = recInput.ratingId,
                title = recInput.title,
                recommendation = recInput.recommendation,
                reviewer = recInput.reviewer,
                reviewerEmployer = recInput.reviewerEmployer,
                employerUrl = recInput.employerUrl,
                created = recInput.created
            };

            db.Recommendations.Add(rec);
            db.SaveChanges();

            var response = new
            {
                recommendId = rec.recommendId,
                ratingId = rec.ratingId,
                title = rec.title,
                recommendation = rec.recommendation,
                reviewer = rec.reviewer,
                reviewerEmployer = rec.reviewerEmployer,
                employerUrl = rec.employerUrl,
                created = rec.created
            };
            return Ok(response);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RecommendExists(int id)
        {
            return db.Recommendations.Count(e => e.recommendId == id) > 0;
        }
    }
}
