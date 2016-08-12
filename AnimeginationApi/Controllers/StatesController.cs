using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace AnimeginationApi.Controllers
{
    public class StatesController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/States
        [SwaggerOperation("GetStates")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<object> GetStates()
        {
            var states =
            db.States.Select(st => new
            {
                StateID = st.StateID,
                StateName = st.StateName,
                StateCode = st.StateCode
            }).AsEnumerable();

            return states;
        }

        // GET: api/States/5
        [SwaggerOperation("GetStates")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetStates(int id)
        {
            var state =
            db.States.Select(st => new
            {
                StateID = st.StateID,
                StateName = st.StateName,
                StateCode = st.StateCode
            })
            .Where(st => st.StateID == id);

            return state;
        }

        //// PUT: api/States/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutState(int id, State state)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != state.StateID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(state).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!StateExists(id))
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

        //// POST: api/States
        //[ResponseType(typeof(State))]
        //public async Task<IHttpActionResult> PostState(State state)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.States.Add(state);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = state.StateID }, state);
        //}

        //// DELETE: api/States/5
        //[ResponseType(typeof(State))]
        //public async Task<IHttpActionResult> DeleteState(int id)
        //{
        //    State state = await db.States.FindAsync(id);
        //    if (state == null)
        //    {
        //        return NotFound();
        //    }

        //    db.States.Remove(state);
        //    await db.SaveChangesAsync();

        //    return Ok(state);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StateExists(int id)
        {
            return db.States.Count(e => e.StateID == id) > 0;
        }
    }
}