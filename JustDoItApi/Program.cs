using JustDoItApi.Data;
using JustDoItApi.Entities.Identity;
using JustDoItApi.Hubs;
using JustDoItApi.Interfaces;
using JustDoItApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
        };

        document.Security = [
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer"),
                    []
                }
            }
        ];

        document.SetReferenceHostDocument();

        document.Servers = [
            new OpenApiServer
            {
                Url = builder.Configuration["ServerRunUrl"]
            }
        ];

        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services
    .AddIdentity<UserEntity, RoleEntity>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs/chat"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<IZadachiService, ZadachiService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJWTTokenService, JWTTokenService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddSignalR();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.ConfigureApplicationCookie(options => { options.Events.OnRedirectToLogin = context => { context.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; }; });

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024;
    options.KeyLengthLimit = 10 * 1024;
    options.ValueLengthLimit = int.MaxValue;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSignalR", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true) // для тесту з html
            .AllowCredentials();
    });
});


var app = builder.Build();

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.SwaggerEndpoint("/openapi/v1.json", "JustDoIt API v1");
    options.OAuthUsePkce();
});

var dir = builder.Configuration["ImagesDir"];
var path = Path.Combine(Directory.GetCurrentDirectory(), dir);
Directory.CreateDirectory(path);

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = $"/{dir}"
});

app.UseCors("AllowSignalR");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/hubs/chat");

app.MapControllers();

await app.SeedDataAsync();

app.Run();
