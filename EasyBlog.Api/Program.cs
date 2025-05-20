
using System.Security.Claims;
using System.Text;
using EasyBlog.Api.Data;
using EasyBlog.Api.Models;
using EasyBlog.Api.Models.Memory;
using EasyBlog.Api.Repositories;
using EasyBlog.Api.Services;
using EasyBlog.Shared.Enums;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<EasyBlogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
    );

//Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//These are the Controller services and repositories.
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

//The JWT settings are setup in the config, then passed through the IOptions and used in code.
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<JwtService>();

// Add JWT authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //Validate the issuer, what's the valid issuer?
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            //Validate the audience, what's the valid audience?
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            //Validate the lifetile too
            ValidateLifetime = true,
            //Give or take 30 seconds
            ClockSkew = TimeSpan.FromSeconds(30),
            //Gotta validate the key, whats's the correct key?
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
        };
        
        //Only for the purposes of testing on http
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, Roles.Admin.ToString()));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy => policy.WithOrigins("https://localhost:5144")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()); //This enables cookies. Don't need anything else.
});

var app = builder.Build();

var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<EasyBlogDbContext>();
context.Database.Migrate();
var adminExists = await context.Users.Where(u => u.Nickname == "admin").FirstOrDefaultAsync();
if (adminExists == null)
{
    try
    {
        string encryptedPassword = Argon2.Hash("Ap@T(^fhM3u*;mE5<:K_e");
        User newAdmin = new User()
        {
            Nickname = "admin",
            PasswordHash = encryptedPassword,
            Email = null,
            DateCreated = DateTime.UtcNow,
            DateDeleted = null
        };
        context.Users.Add(newAdmin);
        context.SaveChanges();
    }
    catch
    {
        throw new Exception("Didnt work");
    }
    
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazorClient");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
