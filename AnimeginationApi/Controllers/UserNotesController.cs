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
    public class UserNotesController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Notes
        [SwaggerOperation("GetUserNotes")]
        [JwtTokenFilter]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetUserNotes()
        {
            var notes =
            db.UserNotes.Select(note => new
            {
                noteId = note.UserNoteId,
                userId = note.UserId,
                correspondenceType = note.CorrespondenceType,
                title = note.Title, 
                note = note.Note, 
                created = note.Created
            }).AsEnumerable();

            return Ok(notes);
        }

        // GET: api/Notes/5
        [Route("api/UserNotes/{id}", Name = "GetNotesByID")]
        [JwtTokenFilter]
        [SwaggerOperation("GetNotesById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetUserNotesById(int id)
        {
            string userId = Request.UserId();

            var notes =
            db.UserNotes.Select(note => new
            {
                noteId = note.UserNoteId,
                userId = note.UserId,
                correspondenceType = note.CorrespondenceType,
                title = note.Title, 
                note = note.Note, 
                created = note.Created
            })
            .Where(note => note.noteId == id);

            return Ok(notes);
        }

        [HttpPut]
        [JwtTokenFilter]
        [SwaggerOperation("PutNote")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PutUserNote([FromBody] UserNoteModel noteInput)
        {
            string userId = Request.UserId();

            // if (!userId.Equals(noteInput.userid))
            // {
            //     return Unauthorized();
            // }

            UserNote note = new UserNote 
            {
                UserId = userId,
                CorrespondenceType = noteInput.correspondenceType,
                Title = noteInput.title,
                Note = noteInput.note, 
                Created = DateTime.Now
            };

            db.UserNotes.Add(note);
            db.SaveChanges();

            var response = new
            {
                userNoteId = noteInput.userNoteId,
                userId = noteInput.userid,
                correspondenceType = noteInput.correspondenceType,
                title = noteInput.title,
                note = noteInput.note, 
                created = noteInput.created
            };
            return Ok(response);
        }

        // DELETE: api/Notes/guid
        [HttpDelete]
        [AdminRoleFilter]
        [SwaggerOperation("DeleteUserNote")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> DeleteUserNote(int id)
        {
            UserNote note = db.UserNotes.SingleOrDefault(nt => nt.UserNoteId == id);

            if (note == null)
            {
                return NotFound();
            }

            db.UserNotes.Remove(note);
            await db.SaveChangesAsync();

            return Ok(note);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserNoteExists(int id)
        {
            return db.UserNotes.Count(e => e.UserNoteId == id) > 0;
        }
    }
}