using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

public class CookieHttpClientHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // This enables cookies to be sent along with cross-origin requests
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return await base.SendAsync(request, cancellationToken);
    }
}