using System.Net.Http.Headers;
using EasyBlog.Web.Components;
using EasyBlog.Web.Refit;
using Radzen;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureHostOptions(opts =>
{
    opts.ShutdownTimeout = TimeSpan.FromSeconds(1); // default is 30
});

builder.Services.AddRadzenComponents();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRefitClient<IEasyBlogApi>().ConfigureHttpClient(
        c => 
            {
                c.BaseAddress = new Uri("http://localhost:5174");
            }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
