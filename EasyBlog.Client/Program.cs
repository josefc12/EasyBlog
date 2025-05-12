using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EasyBlog.Client;
using Refit;
using EasyBlog.Client.Refit;
using EasyBlog.Client.Security;
using Blazored.LocalStorage;
using Radzen;
using EasyBlog.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<LoginStateService>();

builder.Services.AddScoped<DialogService>();

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<JwtTokenMessageHandler>();

builder.Services.AddRefitClient<IEasyBlogApi>()
    .ConfigureHttpClient(
        client => 
        {
            client.BaseAddress = new Uri("https://localhost:5174");
        }
    )
    .ConfigurePrimaryHttpMessageHandler(() => new CookieHttpClientHandler()) 
    .AddHttpMessageHandler<JwtTokenMessageHandler>();

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
