using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;

namespace AnimeginationApi.Controllers
{
    public class JwtKeys
    {
        public string InPublicKey;
        public string InPrivateKey;
        public string OutPublicKey;
        public string OutPrivateKey;
    }

    public class JwtController : ApiController
    {
        // GET: api/Jwt
        [SwaggerOperation("GetJwt")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public object GetJwt()
        {
            string inPublicKey = string.Empty;
            string inPrivateKey = string.Empty;
            string outPublicKey = string.Empty;
            string outPrivateKey = string.Empty;

            RSACryptoServiceProvider inKey = new RSACryptoServiceProvider();            
            inPublicKey = inKey.ToXmlString(false);
            inPrivateKey = inKey.ToXmlString(true);

            byte[] inPublicBytes = inKey.ExportCspBlob(false);
            byte[] inPrivateBytes = inKey.ExportCspBlob(true);
            inPublicKey = Convert.ToBase64String(inPublicBytes, 0, inPublicBytes.Length);
            inPrivateKey = Convert.ToBase64String(inPrivateBytes, 0, inPrivateBytes.Length);

            RSACryptoServiceProvider outKey = new RSACryptoServiceProvider();
            outPublicKey = outKey.ToXmlString(false);
            outPrivateKey = outKey.ToXmlString(true);

            byte[] outPublicBytes = outKey.ExportCspBlob(false);
            byte[] outPrivateBytes = outKey.ExportCspBlob(true);
            outPublicKey = Convert.ToBase64String(outPublicBytes, 0, outPublicBytes.Length);
            outPrivateKey = Convert.ToBase64String(outPrivateBytes, 0, outPrivateBytes.Length);

            var keys = new JwtKeys() {
                InPublicKey = inPublicKey,
                InPrivateKey = inPrivateKey,
                OutPublicKey = outPublicKey,
                OutPrivateKey = outPrivateKey
            };

            return keys;
        }
    }
}
