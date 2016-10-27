using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnimeginationApi.Models;

namespace AnimeginationApi.Services
{
    public static class Helpers
    {
        public static JwtKeys GetJwtKeys()
        {
            JwtKeys keys = new JwtKeys()
            {
                InPrivateKey = "InPrivateKey".GetConfigurationValue(),
                InPublicKey = "InPublicKey".GetConfigurationValue(),
                OutPrivateKey = "OutPrivateKey".GetConfigurationValue(),
                OutPublicKey = "OutPublicKey".GetConfigurationValue()
            };

            return keys;
        }

    }
}