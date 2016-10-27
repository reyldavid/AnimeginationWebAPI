using AnimeginationApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace AnimeginationApi.Services
{
    public class TokenManager
    {
        public static string CreateToken(ClaimModel claim)
        {
            JwtKeys keys = Helpers.GetJwtKeys();

            RSACryptoServiceProvider outPrivateKey = new RSACryptoServiceProvider();
            outPrivateKey.ImportCspBlob(Convert.FromBase64String(keys.OutPrivateKey));

            RSACryptoServiceProvider inPublicKey = new RSACryptoServiceProvider();
            inPublicKey.ImportCspBlob(Convert.FromBase64String(keys.InPublicKey));

            var payload = JsonConvert.SerializeObject(claim);

            string encodedPayload = Jose.JWT.Encode(payload,
                outPrivateKey,
                Jose.JwsAlgorithm.RS256);

            var encryptedToken = Jose.JWT.Encode(encodedPayload,
                inPublicKey,
                Jose.JweAlgorithm.RSA1_5,
                Jose.JweEncryption.A256GCM);

            return encryptedToken;
        }

        public static ClaimModel CreateClaim(TokenModel token)
        {
            JwtKeys keys = Helpers.GetJwtKeys();

            RSACryptoServiceProvider outPublicKey = new RSACryptoServiceProvider();
            outPublicKey.ImportCspBlob(Convert.FromBase64String(keys.OutPublicKey));

            RSACryptoServiceProvider inPublicKey = new RSACryptoServiceProvider();
            inPublicKey.ImportCspBlob(Convert.FromBase64String(keys.InPublicKey));

            RSACryptoServiceProvider outPrivateKey = new RSACryptoServiceProvider();
            outPrivateKey.ImportCspBlob(Convert.FromBase64String(keys.OutPrivateKey));

            RSACryptoServiceProvider inPrivateKey = new RSACryptoServiceProvider();
            inPrivateKey.ImportCspBlob(Convert.FromBase64String(keys.InPrivateKey));

            var decryptedToken = Jose.JWT.Decode(token.token, inPrivateKey);

            var decodedPayload = Jose.JWT.Decode(decryptedToken, outPublicKey);

            ClaimModel claim = JsonConvert.DeserializeObject<ClaimModel>(decodedPayload);

            return claim;
        }
    }
}