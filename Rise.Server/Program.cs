using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Persistence.Triggers;
using Rise.Services.Products;
using Rise.Shared.Products;
using Rise.Services.Leveranciers;
using Rise.Shared.Leveranciers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Auth0Net.DependencyInjection;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Rise.Server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri($"{builder.Configuration["Auth0:Authority"]}/oauth/token"),
                AuthorizationUrl = new Uri($"{builder.Configuration["Auth0:Authority"]}/authorize?audience={builder.Configuration["Auth0:Audience"]}"),
            }
        }
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new string[] { "openid" }
        }
    });
});

// Fluentvalidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDto.Validator>();
builder.Services.AddFluentValidationAutoValidation();



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Auth0:Authority"];
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddAuth0AuthenticationClient(config =>
{
    config.Domain = builder.Configuration["Auth0:Authority"]!;
    config.ClientId = builder.Configuration["Auth0:M2MClientId"];
    config.ClientSecret = builder.Configuration["Auth0:M2MClientSecret"];
});
builder.Services.AddAuth0ManagementClient().AddManagementAccessToken();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
    options.UseTriggers(options => options.AddTrigger<EntityBeforeSaveTrigger>());
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ILeverancierService, LeverancierService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1.0");
        options.OAuthClientId(builder.Configuration["Auth0:BlazorClientId"]);
        options.OAuthClientSecret(builder.Configuration["Auth0:BlazorClientSecret"]);
    });
}

// Add logging middleware at the beginning of the request pipeline
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// beveiligt alle controllers
app.MapControllers().RequireAuthorization();
app.MapFallbackToFile("index.html");


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //dbContext.Database.Migrate();
    Seeder seeder = new(dbContext);
    seeder.Seed();
}

app.Run();

public partial class Program {}