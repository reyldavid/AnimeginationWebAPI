﻿using AnimeginationApi.Models;
using AnimeginationApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AnimeginationApi.Handlers
{
    public class AdminRoleHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            TaskCompletionSource<HttpResponseMessage> tsc = new TaskCompletionSource<HttpResponseMessage>();

            if (!request.Headers.Contains("JWTToken"))
            {
                return tsc.Task;
            }

            IEnumerable<string> jwtTokens = request.Headers.GetValues("JWTToken");
            if (jwtTokens == null)
            {
                return tsc.Task;
            }

            string[] jwtTokenValues = jwtTokens.ToArray<string>();
            if (jwtTokenValues.Length == 0)
            {
                return tsc.Task;
            }

            string jwtToken = jwtTokenValues[0];
            if (jwtToken.Equals(string.Empty))
            {
                return tsc.Task;
            }

            TokenModel token = new TokenModel() { token = jwtToken };
            ClaimModel claim = TokenManager.CreateClaim(token);

            if (!claim.Roles.Contains("Admin"))
            {
                return tsc.Task;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
