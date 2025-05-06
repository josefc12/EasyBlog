
using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace EasyBlog.Client.Security
{
    public class JwtTokenMessageHandler :DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public JwtTokenMessageHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}