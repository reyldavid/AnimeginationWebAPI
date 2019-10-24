using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;
using AnimeginationApi.Filters;
using System.Threading.Tasks;
using AnimeginationApi.Services;

namespace AnimeginationApi.Controllers
{
    public class UserFeedbackController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/UserFeedbacks
        [SwaggerOperation("GetUserFeedbacks")]
        [JwtTokenFilter]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetUserFeedbacks()
        {
            string userId = Request.UserId();

            var feedbacks =
            db.UserFeedbacks.Select(feedback => new
            {
                feedbackId = feedback.FeedbackId,
                userId = feedback.UserId,
                feedbackType = feedback.FeedbackType,
                productId = feedback.ProductId, 
                rating = feedback.RatingID, 
                title = feedback.Title, 
                feedback = feedback.Feedback, 
                created = feedback.Created
            }).Where(review => review.userId == userId);

            return Ok(feedbacks);
        }

        // GET: api/UserFeedbacks/5
        [Route("api/UserFeedback/{id}", Name = "GetFeedbacksByProductID")]
        [JwtTokenFilter]
        [SwaggerOperation("GetFeedbacksByProductID")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetFeedbacksByProductId(int id)
        {
            var feedbacks =
            db.UserFeedbacks.Select(feedback => new
            {
                feedbackId = feedback.FeedbackId,
                userId = feedback.UserId,
                feedbackType = feedback.FeedbackType,
                productId = feedback.ProductId, 
                rating = feedback.RatingID, 
                title = feedback.Title, 
                feedback = feedback.Feedback, 
                created = feedback.Created
            }).Where(review => review.productId == id);

            return Ok(feedbacks);
        }

        [HttpPut]
        [JwtTokenFilter]
        [SwaggerOperation("PutFeedback")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PutUserFeedback([FromBody] UserFeedbackModel feedbackInput)
        {
            string userId = Request.UserId();

            UserFeedback feedback = new UserFeedback 
            {
                UserId = userId,
                FeedbackType = feedbackInput.feedbackType,
                ProductId = feedbackInput.productId, 
                RatingID = feedbackInput.ratingId, 
                Title = feedbackInput.title, 
                Feedback = feedbackInput.feedback, 
                Created = DateTime.Now
            };

            db.UserFeedbacks.Add(feedback);
            db.SaveChanges();

            var response = new
            {
                feedbackId = feedback.FeedbackId,
                userId = feedback.UserId,
                feedbackType = feedback.FeedbackType,
                productId = feedback.ProductId, 
                rating = feedback.RatingID, 
                title = feedback.Title, 
                feedback = feedback.Feedback, 
                created = feedback.Created
            };
            return Ok(response);
        }

        // DELETE: api/UserFeedbacks/guid
        [HttpDelete]
        [AdminRoleFilter]
        [SwaggerOperation("DeleteUserFeedback")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> DeleteUserFeedback(int id)
        {
            UserFeedback feedback = db.UserFeedbacks.SingleOrDefault(fb => fb.FeedbackId == id);

            if (feedback == null)
            {
                return NotFound();
            }

            db.UserFeedbacks.Remove(feedback);
            await db.SaveChangesAsync();

            return Ok(feedback);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserFeedbackExists(int id)
        {
            return db.UserFeedbacks.Count(e => e.FeedbackId == id) > 0;
        }
    }
}