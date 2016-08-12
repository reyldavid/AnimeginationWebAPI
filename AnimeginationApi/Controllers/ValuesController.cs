using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Newtonsoft.Json;

namespace AnimeginationApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [SwaggerOperation("GetAll")]
        public IEnumerable<string> Get()
        {
            return new string[] { "Aya Ueto", "Megumi Hayashibara", "Ryo Hirohashi" };
        }

        //[SwaggerOperation("GetJSON")]
        //[SwaggerResponse(HttpStatusCode.OK)]
        //[SwaggerResponse(HttpStatusCode.NotFound)]
        //public HttpResponseMessage GetJSON()
        //{
        //    var list = new string[] { "Aya Ueto", "Megumi Hayashibara" };
        //    var serializedData = JsonConvert.SerializeObject(list.ToList());

        //    var response = new HttpResponseMessage(HttpStatusCode.OK);
        //    response.Content = new StringContent(serializedData);

        //    response.Content.Headers.ContentType =
        //        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        //    //var content = new StringContent(serializedData);

        //    //var response = Request.CreateResponse(HttpStatusCode.OK, serializedData);

        //    //response.Content.Headers.ContentType =
        //    //    new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        //    return response;
        //}

        // GET api/values/5
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string Get(int id)
        {
            return "Ryo Hirohashi";
        }

        // POST api/values
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Delete(int id)
        {
        }
    }
}
