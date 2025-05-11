
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using EasyBlog.Client.Refit;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace EasyBlog.Client.Security
{
    public class JwtTokenMessageHandler :DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _refreshClient;

        public JwtTokenMessageHandler(ILocalStorageService localStorage, HttpClient refreshClient)
        {
            _localStorage = localStorage;
            _refreshClient = refreshClient;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5174/api/Auth/refresh");
                // Set the credentials to include cookies
                refreshRequest.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

                var refreshResponse = await _refreshClient.SendAsync(refreshRequest, cancellationToken);
                if (!refreshResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine(refreshResponse);
                    return response; // Return original 401 if refresh fails
                }

                // Try refreshing the token
                var newToken = await refreshResponse.Content.ReadAsStringAsync(cancellationToken);
                await _localStorage.SetItemAsync("authToken", newToken);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }
    }
}