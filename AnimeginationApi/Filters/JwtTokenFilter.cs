using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using AnimeginationApi.Services;
using AnimeginationApi.Models;
using Newtonsoft.Json;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace AnimeginationApi.Filters
{
    public class JwtTokenFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Contains("JWTToken"))
            {
                IEnumerable<string> jwtTokens = actionContext.Request.Headers.GetValues("JWTToken");

                string[] jwtTokenValues = jwtTokens.ToArray<string>();

                string jwtToken = jwtTokenValues[0];

                if (!jwtToken.Equals(string.Empty))
                {
                    TokenModel token = new TokenModel { token = jwtToken };

                    ClaimModel claim = TokenManager.CreateClaim(token);

                    actionContext.Request.UserName(claim.UserName);
                    actionContext.Request.UserId(claim.UserId);
                    actionContext.Request.Email(claim.Email);

                    base.OnActionExecuting(actionContext);
                    return;
                }
            }
            throw new HttpResponseException(actionContext.Request.CreateErrorResponse(
                HttpStatusCode.Unauthorized, "User Token is missing"));
        }
    }
}