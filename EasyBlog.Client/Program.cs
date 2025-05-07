using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EasyBlog.Client;
using Refit;
using EasyBlog.Client.Refit;
using EasyBlog.Client.Security;
using Blazored.LocalStorage;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<DialogService>();

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<JwtTokenMessageHandler>();
builder.Services.AddRefitClient<IEasyBlogApi>()
    .ConfigureHttpClient(
        client => 
        {
            client.BaseAddress = new Uri("http://localhost:5174");
        }
    )
    .AddHttpMessageHandler<JwtTokenMessageHandler>();

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
