using System.Text;
using BlazorApp.Auth;
using BlazorApp.Components;
using BlazorApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

// Load the repo-root .env (searched upwards from the working dir) as environment variables; real env vars win.
for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir != null; dir = dir.Parent)
{
    string envFile = Path.Combine(dir.FullName, ".env");
    if (!File.Exists(envFile)) continue;
    foreach (string line in File.ReadAllLines(envFile))
    {
        int i = line.IndexOf('=');
        if (i <= 0 || line.TrimStart().StartsWith('#')) continue;
        string name = line[..i].Trim();
        Environment.SetEnvironmentVariable(name, Environment.GetEnvironmentVariable(name) ?? line[(i + 1)..].Trim());
    }
    break;
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorBootstrap();

string webApiUrl = builder.Configuration["WebAPI_URL"] ?? "http://localhost:5012";
builder.Services.AddScoped(sp => new HttpClient{
    BaseAddress = new Uri(webApiUrl)
});

builder.Services.AddScoped<IHttpPostsService, HttpPostsService>();
builder.Services.AddScoped<IHttpUsersService, HttpUsersService>();
builder.Services.AddScoped<IHttpCategoriesService, HttpCategoriesService>();
builder.Services.AddScoped<IHttpImagesService, HttpImagesService>();
builder.Services.AddScoped<IHttpChatsService, HttpChatsService>();
builder.Services.AddScoped<IHttpMessagesService, HttpMessagesService>();
builder.Services.AddScoped<IHttpNotificationService, HttpNotificationService>();

var issuer = builder.Configuration["JWT:Issuer"];
var audience = builder.Configuration["JWT:Audience"];
var key = builder.Configuration["JWT:Key"];
if (string.IsNullOrWhiteSpace(key))
{
    throw new InvalidOperationException("JWT:Key is not configured. Copy .env.example to .env in the repo root and set Jwt__Key.");
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key))
        };
    });
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()){
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();