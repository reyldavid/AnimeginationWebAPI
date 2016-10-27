using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeginationApi.Models
{
    public class JwtKeys
    {
        public string InPublicKey;
        public string InPrivateKey;
        public string OutPublicKey;
        public string OutPrivateKey;
    }

    public class TokenModel
    {
        public string token;
    }
}