using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;

namespace AnimeginationApi
{
    public static class RequestManager: Object
    {
        public static string UserId (this HttpRequestMessage request)
        {
            if (!request.Properties.Keys.Contains("userid"))
            {
                return string.Empty;
            }
            else
            {
                return request.Properties["userid"].ToString();
            };
        }

        public static string UserName (this HttpRequestMessage request)
        {
            if (!request.Properties.Keys.Contains("username"))
            {
                return string.Empty;
            }
            else
            {
                return request.Properties["username"].ToString();
            };
        }

        public static string Email(this HttpRequestMessage request)
        {
            if (!request.Properties.Keys.Contains("email"))
            {
                return string.Empty;
            }
            else
            {
                return request.Properties["email"].ToString();
            };
        }

        public static void UserId (this HttpRequestMessage request, string val)
        {
            if (request.Properties.Keys.Contains("userid"))
            {
                request.Properties["userid"] = val;
            }
            else
            {
                request.Properties.Add("userid", val);
            }
        }

        public static void UserName (this HttpRequestMessage request, string val)
        {
            if (request.Properties.Keys.Contains("username"))
            {
                request.Properties["username"] = val;
            }
            else
            {
                request.Properties.Add("username", val);
            }
        }

        public static void Email (this HttpRequestMessage request, string val)
        {
            if (request.Properties.Keys.Contains("email"))
            {
                request.Properties["email"] = val;
            }
            else
            {
                request.Properties.Add("email", val);
            }
        }
    }
}