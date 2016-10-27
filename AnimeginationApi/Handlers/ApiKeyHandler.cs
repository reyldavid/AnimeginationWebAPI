using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AnimeginationApi.Services;

namespace AnimeginationApi.Handlers
{
    public class ApiKeyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            TaskCompletionSource<HttpResponseMessage> tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);

            if (!request.Headers.Contains("AnimeApiClientKey"))
            {
                return tsc.Task;
            }

            IEnumerable<string> apiKeys = request.Headers.GetValues("AnimeApiClientKey");
            if (apiKeys == null)
            {
                return tsc.Task;
            }

            string[] apiValues = apiKeys.ToArray<string>();
            if (apiValues.Length == 0)
            {
                return tsc.Task;
            }

            string apiKey = apiValues[0];
            if (apiKey.Equals(string.Empty))
            {
                return tsc.Task;
            }

            if (!apiKey.Equals("AnimeApiClientKey".GetConfigurationValue()))
            {
                return tsc.Task;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}