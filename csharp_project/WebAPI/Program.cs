using System.Text;
using Domain;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebAPI;
using UserService = Domain.UserService;

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
builder.Services.AddControllers();
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>(); // Add middleware to handle exceptions.
builder.Services.AddOpenApi();

string grpcUrl = builder.Configuration["gRPC_URL"] ?? "http://localhost:9090";
builder.Services.AddScoped<ChannelBase>(sp => GrpcChannel.ForAddress(grpcUrl));
builder.Services.AddScoped<TradingForumGrpc.PostService.PostServiceClient>();
builder.Services.AddScoped<TradingForumGrpc.UserService.UserServiceClient>();
builder.Services.AddScoped<TradingForumGrpc.ImageService.ImageServiceClient>();
builder.Services.AddScoped<TradingForumGrpc.CategoryService.CategoryServiceClient>();
builder.Services.AddScoped<TradingForumGrpc.ChatService.ChatServiceClient>();
builder.Services.AddScoped<TradingForumGrpc.MessageService.MessageServiceClient>();
builder.Services.AddScoped<TradingForumGrpc.NotificationService.NotificationServiceClient>();

builder.Services.AddScoped<IPostsService, PostsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IImagesService, ImagesService>();
builder.Services.AddScoped<IMessagesService, MessagesService>();
builder.Services.AddScoped<IChatsService, ChatsService>();
builder.Services.AddScoped<INotificationService, NotificationsService>();

//JWT værdier, Issuer/Audience er i appsettings.json, Key er i .env
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
builder.Services.AddAuthorization();
//builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddScoped<AuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()){
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseAuthentication();//vigtigt den er FØR author
app.UseAuthorization();
app.MapControllers();

app.Run();